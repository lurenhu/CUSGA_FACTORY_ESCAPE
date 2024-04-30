using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogSystem : SingletonMonobehaviour<DialogSystem>
{
    [Header("普通对话UI组件")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public Image character_1;
    public Image character_2;
    [Space(5)]
    [Header("AI对话UI组件")]
    public Transform AIDialogPanel;
    public TMP_Text AIDialogText;
    public TMP_Text AINameText;
    public Image AICharacter_1;
    public Image AICharacter_2;
    public GameObject dialogCellPrefab;
    public Transform content;
    public TMP_Text value;
    public RectTransform anxietyValue;
    public List<Image> SubmitTimer;
    [Space(5)]
    [Header("参数")]
    [Tooltip("文本显示间隔时间")]
    public float playingTimeInterval = 0.05f;
    List<string> character1List = new List<string>();// 存储角色1的图片数据
    List<string> character2List = new List<string>();// 存储角色2的图片数据
    List<string> nameList = new List<string>();// 存储每个对话的发出者的姓名
    List<string> textList = new List<string>();// 存储每个对话的文本内容
    List<int> speakingCharacterList = new List<int>();// 存储对话者的属于左侧还是右侧（用于控制角色头像动画）
    List<string[]> imageList = new List<string[]>();
    private float lockTime = 0f;// AI思考时间
    private bool isTimerRunning = false;// 是否进行AI思考
    public bool textFinished = true;// 文本动画是否播放结束
    private bool cancelTyping = false;// 是否取消打字
    private int textIndex = 0;// 文本索引

    void Start()
    {
    }

    private void Update()
    {
        if (VideoManager.Instance.isPlayingCutScene) return;

        if (!dialogPanel.activeSelf && textIndex < textList.Count)
        {
            dialogPanel.SetActive(true);
            UIManager.Instance.UIShow = true;
            PopUpDialogPanel();
            SetInitialValue();
        }

        if (AIDialogPanel.gameObject.activeSelf)
        {
            UpdateTextForAI();
        }

        if (dialogPanel.activeSelf)
        {
            UpdateText();
        }
    }   

    /// <summary>
    /// 清理数据列表
    /// </summary>
    private void ClearReference()
    {        
        textList.Clear();
        nameList.Clear();
        imageList.Clear();
        character1List.Clear();
        character2List.Clear();
        speakingCharacterList.Clear();

        textIndex = 0;    
    }

#region 普通文本对话脚本
    /// <summary>
    /// 设置初始状态
    /// </summary>
    private void SetInitialValue()
    {
        nameText.text = nameList[textIndex];
        StartCoroutine(PlayingRowText(textList[textIndex]));

        LoadImageForCharacter();
    }

    /// <summary>
    /// 更新对话框内文本
    /// </summary>
    private void UpdateText()   
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (textFinished && textIndex < textList.Count)
            {
                LoadImageForCharacter();

                nameText.text = nameList[textIndex];
                StartCoroutine(PlayingRowText(textList[textIndex]));
            }
            else if (!textFinished && !cancelTyping)
            {
                cancelTyping = true;
            }
            else if (textFinished && textIndex >= textList.Count)
            {
                dialogPanel.SetActive(false);
                UIManager.Instance.UIShow = false;
            }
        }
    }

    /// <summary>
    /// 导入对话角色图片差分以及对话角色动画
    /// </summary>
    private void LoadImageForCharacter()
    {
        // 导入左侧角色差分
        if (character1List.Count > 0)
        {
            character_1.gameObject.SetActive(true);
            character_1.sprite = GameResources.Instance.characters.Find(x => x.name == character1List[textIndex]).sprite;
        }
        else
        {
            character_1.gameObject.SetActive(false);
            Debug.Log("Character_1 No Image");
        }

        // 导入右侧角色差分
        if (character2List.Count > 0)
        {
            character_2.gameObject.SetActive(true);
            character_2.sprite = GameResources.Instance.characters.Find(x => x.name == character2List[textIndex]).sprite;
        }
        else
        {
            character_2.gameObject.SetActive(false);
            Debug.Log("Character_2 No Image");
        }

        // 根据谈话者设置差分明暗
        if (speakingCharacterList.Count > 0)
        {
            Color dark = new Color(0.5f, 0.5f, 0.5f, 1);
            switch (speakingCharacterList[textIndex])
            {
                case 1:
                    character_1.material.DOColor(Color.white, 0.5f);
                    character_2.material.DOColor(dark, 0.5f);
                    break;
                case 2:
                    character_1.material.DOColor(dark, 0.5f);
                    character_2.material.DOColor(Color.white, 0.5f);
                    break;
                case 3:
                    character_1.material.DOColor(dark, 0.5f);
                    character_2.material.DOColor(dark, 0.5f);
                    break;
            }
        }
        else
        {
            Debug.Log("No Speaker");
        }
    }

    /// <summary>
    /// 逐字渲染文字
    /// </summary>
    IEnumerator PlayingRowText(string textToPlay)
    {
        textFinished = false;
        dialogText.text = Setting.stringDefaultValue;
        int index = 0;
        while (!cancelTyping && index < textToPlay.Length-1)
        {
            soundManager.Instance.PlaySFX("Text");
            dialogText.text += textToPlay[index];
            index++;
            yield return new WaitForSeconds(playingTimeInterval);
        }

        dialogText.text = textToPlay;
        cancelTyping = false;
        textFinished = true;

        textIndex++;
    }

    /// <summary>
    /// 获取固定文本的文字
    /// </summary>
    public void GetText(TextAsset textFile)
    {
        ClearReference();
        
        var rows = textFile.text.Split('\n');
        foreach (var row in rows)
        {
            string[] row_list = row.Split(':');
            character1List.Add(row_list[0]);
            character2List.Add(row_list[1]);
            nameList.Add(row_list[2]);
            textList.Add(row_list[3]);
            speakingCharacterList.Add(int.Parse(row_list[4]));
        }

        UIManager.Instance.UIShow = true;
    }
#endregion

#region AI对话脚本

    /// <summary>
    /// 更新对话框内文本
    /// </summary>
    private void UpdateTextForAI()   
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!textFinished && !cancelTyping)
            {
                cancelTyping = true;
            }
        }
    }

    /// <summary>
    /// 逐字渲染文字
    /// </summary>
    IEnumerator PlayingRowTextForAI(string textToPlay)
    {
        textFinished = false;
        AIDialogText.text = Setting.stringDefaultValue;
        int index = 0;
        while (!cancelTyping && index < textToPlay.Length-1)
        {
            soundManager.Instance.PlaySFX("Text");
            AIDialogText.text += textToPlay[index];
            index++;
            yield return new WaitForSeconds(playingTimeInterval);
        }

        AIDialogText.text = textToPlay;

        cancelTyping = false;
        textFinished = true;

        textIndex++;

        AddAIDialogLogCell(textToPlay);
    }

    /// <summary>
    /// 添加对话历史记录
    /// </summary>
    public void AddAIDialogLogCell(string dialogText)
    {
        GameObject dialogCell = Instantiate(dialogCellPrefab,content);
        dialogCell.GetComponentInChildren<TMP_Text>().text = dialogText;
    }

    //从ai处获取文本
    public void get_text_in_other_ways(string name, string text, string[] image_display)
    {
        ClearReference();

        // nameList.Add(name);
        // textList.Add(text);
        // imageList.Add(image_display);

        AINameText.text = name;
        StartCoroutine(PlayingRowTextForAI(text));

    }
    
    /// <summary>
    /// 开始计时并加lock
    /// </summary>
    public void lockUI_and_setText(float lockTime,string text) 
    {
        ClearReference();

        // 计时器完成后的操作
        isTimerRunning = true;
        UIManager.Instance.UIShow = true;
        this.lockTime = lockTime;
        AINameText.text = "823";
        AIDialogText.text= text;
    }    
#endregion

    public void PopUpDialogPanel()
    {
        soundManager.Instance.PlaySFX("NodeBorn");

        dialogPanel.transform.localScale = Vector3.one * 0.3f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(dialogPanel.transform.DOScale(new Vector3(1f,0.3f,1),0.1f));
        sequence.Append(dialogPanel.transform.DOScale(new Vector3(1f,1f,1),0.1f));
    }

    public void PopUpAIDialogPanel()
    {
        soundManager.Instance.PlaySFX("NodeBorn");

        AIDialogPanel.transform.localScale = Vector3.one * 0.3f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(AIDialogPanel.transform.DOScale(new Vector3(1f,0.3f,1),0.1f));
        sequence.Append(AIDialogPanel.transform.DOScale(new Vector3(1f,1f,1),0.1f));
    }
}