using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;

public class DialogSystem : MonoBehaviour
{
    [Header("UI���")]
    public GameObject talk_ui;
    public Text textLabel;
    public Text name_text;
    public GameObject mouse;

    [Header("�Ի�����")]
    public TextAsset textFile;  //�Ի��ļ�
    public int index = 0;
    public int max_index = 0;
    public float textSpeed = 0.1f;

    //[Header("ͼƬ��Դ")]
    //public GameObject heroine;
    //public GameObject female_2;
    //
    //Dictionary<string,GameObject> GameObject_dic = new Dictionary<string, GameObject>();

    [Header("�����ƶ�����")]
    private float left = -4300;
    private float right = 500;
    private float middle = -1500;
    public float move_time = 0.5f;

    [Header("�ƶ����ĸ�����")]
    public int sceneNum;

    List<string> name_list = new List<string>();
    List<string> text_list = new List<string>();
    List<string[]> image_list = new List<string[]>();
    static public DialogSystem instance;
    bool text_finished = true;
    Coroutine text_display;
    bool is_blitting_text = true;
    private void Awake()   //������Ĭ��д��
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    void Start()
    {
        talk_ui.SetActive(false);
        //max_index = GetText(textFile)-1;
    }
    static public void awake_talk_ui(TextAsset textFile)
    {

        instance.talk_ui.SetActive(false);
        //instance.max_index = GetText(textFile) - 1;

    }

    void Update()
    {
        if (is_blitting_text)
        {
            //Debug.Log("blit_text");
            is_blitting_text = blit_text();
        }
    }
    static public bool blit_text()
    {

        if (!instance.talk_ui.activeSelf)
        {
            //���������ʼλ��
            //string[] image_pos = instance.image_list[0];

            instance.talk_ui.SetActive(true);
            //Debug.Log(instance.talk_ui.activeSelf);
            //image_update(image_pos);

            return updateText();
        }
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))//�������F��
        {
            return updateText();
        }
        return true;      //������������talk״̬
    }
    //static public void image_update(string[] image_pos)
    //{
    //    string sign = image_pos[0];
    //    char pos = image_pos[1][0];
    //    Debug.Log(sign);
    //    if (sign == "ȡ��") 
    //    {
    //        return;
    //    }
    //    GameObject ga = instance.GameObject_dic[sign];
    //    RectTransform rectTransform = ga.GetComponent<RectTransform>();
    //    float x_coordinate=instance.middle;

    //    if (pos == 'm')
    //    {
    //        x_coordinate = instance.middle;
    //    }
    //    else if (pos == 'l')
    //    {
    //        x_coordinate = instance.left;
    //    }
    //    else 
    //    {
    //        x_coordinate = instance.right;
    //    }
    //    Debug.Log(x_coordinate);
    //    if (!ga.activeSelf)
    //    {
    //        ga.SetActive(true);
    //        rectTransform.localPosition = new Vector3(x_coordinate, rectTransform.localPosition.y, rectTransform.localPosition.z);

    //    }
    //    if (rectTransform.localPosition.x != x_coordinate)
    //        rectTransform.DOLocalMoveX(x_coordinate,instance.move_time);
    //    //tran.DOLocalMoveX(x_coordinate, instance.move_time);

    //}
    static public void closeUi()
    {
        //foreach (GameObject value in instance.GameObject_dic.Values)
        //{
        //    value.SetActive(false);
        //}
        instance.text_list.Clear();
        instance.name_list.Clear();
        instance.image_list.Clear();
        Debug.Log("closeUi");
        instance.talk_ui.SetActive(false);
    }
    static public bool updateText()   //�����������
    {
        
        if (instance.index < instance.max_index)   //��������û�в���
        {
            if (instance.text_finished)
            {
                instance.index++;
                Debug.Log($"instance.index:{instance.index}");
                Debug.Log($"instance.max_index:{instance.max_index}");
                string content = instance.text_list[instance.index];
                if (content == "��ת")
                {
                    closeUi();
                }

                instance.name_text.text = instance.name_list[instance.index];
                string[] image_pos = instance.image_list[instance.index];
                //Debug.Log($"index:{instance.index}");
                //Debug.Log($"sign:{image_pos[0]}");
                //image_update(image_pos);
                instance.text_display = instance.StartCoroutine(instance.setTextUI(content));
                instance.text_finished = false;
                instance.mouse.SetActive(false);
            }
            else //�ı�û������ʱ���ٰ�R
            {
                instance.name_text.text = instance.name_list[instance.index];
                instance.StopCoroutine(instance.text_display);
                string content = instance.text_list[instance.index];
                instance.textLabel.text = content;
                instance.text_finished = true;
                instance.mouse.SetActive(true);
            }
            return true;
        }
        else
        {
            closeUi();
            return false;
        }
    }
    //��ȡ�̶��ı�������
    static public int GetText(TextAsset textFile)
    {
        instance.text_list.Clear();
        instance.name_list.Clear();
        instance.image_list.Clear();
        var rows = textFile.text.Split('\n');
        foreach (var row in rows)
        {
            string text = row.ToString();
            string[] row_list = text.Split(',');
            string sign = row_list[0];
            if (sign == "�����־")
            {
                continue;
            }

            string position = row_list[4];
            string name = row_list[2];
            string content = row_list[3];
            instance.name_list.Add(name);
            instance.text_list.Add(content);
            instance.image_list.Add(new string[2] { sign, position });
            //Debug.Log($"image_list:{sign + position}");
        }
        Debug.Log($"image_list:{instance.image_list[4][0]}");
        return instance.text_list.Count;
    }
    static public void get_text_in_other_ways(string name, string text, string[] image_display)
    {
        instance.text_list.Clear();
        instance.name_list.Clear();
        instance.image_list.Clear();
        instance.name_list.Add(name);
        instance.text_list.Add(text);
        instance.image_list.Add(image_display);
        //Debug.Log("get_text_in_other_ways");
        set_index();
        instance.is_blitting_text = true;
    }
    //������Ⱦ����
    public IEnumerator setTextUI(string content)
    {
        textLabel.text = string.Empty;

        for (int i = 0; i < content.Length; i++)
        {
            textLabel.text += content[i];
            yield return new WaitForSeconds(textSpeed);
        }
        text_finished = true;
        mouse.SetActive(true);
    }
    static void set_index()
    {
        instance.index = -1;
        instance.max_index = instance.text_list.Count-1;
    }
}