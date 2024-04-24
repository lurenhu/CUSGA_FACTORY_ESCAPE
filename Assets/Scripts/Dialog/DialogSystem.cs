using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Net.Http.Headers;

public class DialogSystem : SingletonMonobehaviour<DialogSystem>
{
    [Header("UI组件")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public Image character_1;
    public Image character_2;
    public GameObject mouse;
    [Header("对话参数")]
    public TextAsset textFile;  //对话文件    
    [Header("文字显示速度，值越小显示越快")]
    public float playingTimeInterval = 0.05f;
    [Header("立绘移动参数")]
    public float move_time = 0.5f;
    private float left = -4300;
    private float right = 500;
    private float middle = -1500;    

    [Tooltip("对话参数")]
    List<string> character1List = new List<string>();
    List<string> character2List = new List<string>();
    List<string> nameList = new List<string>();
    [SerializeField] List<string> textList = new List<string>();
    List<int> speakingCharacterList = new List<int>();
    List<string[]> imageList = new List<string[]>();
    [Header("计时器参数")]
    private float lockTime = 0f;
    private bool isTimerRunning = false;
    [Header("其他变量")]
    public bool textFinished = true;
    public bool cancelTyping = false;
    public int textIndex = 0;

    /*
     * is_playing_text用于开始检测点击与渲染文字，
     * text_finished用于检测文字是否渲染完成
     */


    void Start()
    {
        dialogPanel.SetActive(false);
        if (textFile!=null)
        { 
            GetText(textFile);
        }
    }

    private void Update()
    {
        if (!dialogPanel.activeSelf) return;

        // AI思考
        if (isTimerRunning)
        {
            CountDown();
        }
        else
        {
            UpdateText();
        }

        mouse.SetActive(textFinished);
    }   

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
    /// AI思考时间
    /// </summary>
    private void CountDown()
    {
        // 更新计时器时间
        lockTime -= Time.deltaTime;

        // 检查计时器是否达到持续时间
        if (lockTime < 0f)
        {
            // 计时器达到持续时间，执行相应操作
            isTimerRunning = false;
            SetInitialValue();
        }        
    }

    /// <summary>
    /// 更新对话框内文本
    /// </summary>
    public void UpdateText()   
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

        dialogPanel.SetActive(true);
        UIManager.Instance.UIShow = true;
        SetInitialValue();
    }

    //从ai处获取文本
    public void get_text_in_other_ways(string name, string text, string[] image_display)
    {
        ClearReference();

        nameList.Add(name);
        textList.Add(text);
        imageList.Add(image_display);
    }
    
    /// <summary>
    /// 开始计时并加lock
    /// </summary>
    public void lockUI_and_setText(float lockTime,string text) 
    {
        ClearReference();

        // 计时器完成后的操作
        isTimerRunning = true;
        dialogPanel.SetActive(true);
        UIManager.Instance.UIShow = true;
        this.lockTime = lockTime;
        nameText.text = "823";
        dialogText.text= text;
        LoadImageForCharacter();
    }    

    /// <summary>
    /// 清理数据列表
    /// </summary>
    public void ClearReference()
    {        
        textList.Clear();
        nameList.Clear();
        imageList.Clear();
        character1List.Clear();
        character2List.Clear();
        speakingCharacterList.Clear();

        textIndex = 0;    
    }
}
