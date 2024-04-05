using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public Transform cutSceneUIPanel;
    public Image graphic;
    public TMP_Text tmpText;
    public Queue<string> textForShow = new Queue<string>();

    /// <summary>
    /// 展示过场演出图片文本
    /// </summary>
    public void ShowCutScene(GraphicsAndText graphicsAndText)
    {
        var rows = graphicsAndText.text.text.Split("\n");
        foreach (var row in rows)
        {
            textForShow.Enqueue(row);
        }

        graphic.sprite = graphicsAndText.graphic;
        tmpText.text = textForShow.Dequeue();

        cutSceneUIPanel.gameObject.SetActive(true);
    }

    private void Update() {
        if (!cutSceneUIPanel.gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textForShow.Count > 0)
            {
                tmpText.text = textForShow.Dequeue();
            }
            else
            {
                cutSceneUIPanel.gameObject.SetActive(false);
            }
        }
    }
}
