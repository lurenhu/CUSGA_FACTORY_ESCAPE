using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : SingletonMonobehaviour<DialogManager>
{
    public Transform DialogPanel;
    public Transform speaker_1_Transform;
    public Transform speaker_2_Transform;
    public Text dialogText;
    public TextAsset textAsset1;

    public List<string> speakers = new  List<string>();
    public List<string> dialogs = new List<string>();

    public int index = 0;

    /// <summary>
    /// 导入对话数据并初始化
    /// </summary>
    public void LoadDialogData(TextAsset textAsset)
    {
        var text = textAsset.text.Split('\n');

        foreach (var line in text)
        {
            speakers.Add(line.Split(':')[0]);
            dialogs.Add(line.Split(':')[1]);
        }

        InitializeDialogData();
    }

    private void InitializeDialogData()
    {
        // 角色名称初始化
        speaker_1_Transform.GetComponentInChildren<TMP_Text>().text = speakers[index];
        foreach (string speaker in speakers)
        {
            if (speaker != speakers[index])
            {
                speaker_2_Transform.GetComponentInChildren<TMP_Text>().text = speaker;
                break;
            }
        }

        // 角色图片初始化
        foreach (Character character in GameResources.Instance.characters)
        {
            if (speaker_1_Transform.GetComponentInChildren<TMP_Text>().text == character.name)
            {
                speaker_1_Transform.GetComponentInChildren<Image>().sprite = character.sprite;
            }
            else if (speaker_2_Transform.GetComponentInChildren<TMP_Text>().text == character.name)
            {
                speaker_2_Transform.GetComponentInChildren<Image>().sprite = character.sprite;
            }
            else
            {
                Debug.LogError("Character not found");
            }
        }

        // 对话内容初始化
        dialogText.text = dialogs[index];
    }

    private void Update() {
        DisplayNextText();
    }

    /// <summary>
    /// 展现下一个对话文本
    /// </summary>
    private void DisplayNextText()
    {
        dialogText.text = dialogs[index];

        if (Input.GetMouseButtonDown(0) && index < dialogs.Count - 1)
        {
            index++;
        }
        else if (Input.GetMouseButtonDown(0) && index == dialogs.Count - 1)
        {
            index = 0;
            DialogPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 展现对话框
    /// </summary>
    public void ShowDialogPanel()
    {
        DialogPanel.gameObject.SetActive(true);
    }
}
