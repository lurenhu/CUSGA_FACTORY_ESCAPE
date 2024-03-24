using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : SingletonMonobehaviour<DialogManager>
{
    [Header("对话UI界面")]
    [Tooltip("对话界面")]
    public Transform DialogPanel;
    [Tooltip("对话角色1(包含角色名称文本,角色头像)")]
    public Transform speaker_1_Transform;
    [Tooltip("对话角色2(包含角色名称文本,角色头像")]
    public Transform speaker_2_Transform;
    [Tooltip("对话文本")]
    public Text dialogText;

    private List<string> speakers = new  List<string>();
    private List<string> dialogs = new List<string>();
    private int index = 0;

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

        ShowDialogPanel();
    }

    /// <summary>
    /// 初始化对话数据
    /// </summary>
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
                speaker_1_Transform.GetComponentInChildren<Image>().SetNativeSize();
            }
            else if (speaker_2_Transform.GetComponentInChildren<TMP_Text>().text == character.name)
            {
                speaker_2_Transform.GetComponentInChildren<Image>().sprite = character.sprite;
                speaker_2_Transform.GetComponentInChildren<Image>().SetNativeSize();
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
        if (DialogPanel.gameObject.activeSelf)
        {
            DisplayNextText();
        }
    }

    /// <summary>
    /// 展现下一个对话文本
    /// </summary>
    private void DisplayNextText()
    {
        if (dialogs.Count == 0) return;

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
    private void ShowDialogPanel()
    {
        DialogPanel.gameObject.SetActive(true);
    }
}
