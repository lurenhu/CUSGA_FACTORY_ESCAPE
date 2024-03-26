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
    [Header("UI组件")]
    public GameObject talk_ui;
    public Text textLabel;
    public Text name_text;
    public GameObject mouse;
    [Header("对话参数")]
    public TextAsset textFile;  //对话文件    
    [Header("文字显示速度，值越小显示越快")]
    public float textSpeed = 0.05f;
    [Header("立绘移动参数")]
    public float move_time = 0.5f;
    private float left = -4300;
    private float right = 500;
    private float middle = -1500;    

    [Tooltip("对话参数")]
    List<string> name_list = new List<string>();
    List<string> text_list = new List<string>();
    List<string[]> image_list = new List<string[]>();
    [Header("计时器参数")]
    private float locktime = 0f;
    private bool isTimerRunning = false;
    [Header("其他变量")]
    static public DialogSystem instance;
    public bool text_finished = true;
    private Coroutine text_display;
    private bool is_blitting_text = false;
    private int index = 0;
    private int max_index = 0;

    /*
     * is_blitting_text用于开始检测点击与渲染文字，
     * text_finished用于检测文字是否渲染完成
     */


    private void Awake()   //单例的默认写法
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        talk_ui.SetActive(false);
        if (textFile!=null)
        { 
            GetText(textFile);
        }
        
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            countDown();
        }
        if (is_blitting_text)
        {
            //设置可点击图标
            instance.mouse.SetActive(instance.text_finished);
            is_blitting_text = blit_text();
        }
        else 
        {
            instance.mouse.SetActive(false);
        }
        
    }    
    private void countDown()
    {
        // 更新计时器时间
        instance.locktime -= Time.deltaTime;

        // 检查计时器是否达到持续时间
        if (instance.locktime < 0f)
        {
            // 计时器达到持续时间，执行相应操作
            //instance.talk_ui.SetActive(false);
            instance.isTimerRunning = false;
            //instance.text_finished = true;
            updateText();
            instance.is_blitting_text = true;
            
        }        
    }
    //开始计时并加lock
    static public void lockUI_and_setText(float locktime,string text) 
    {
        // 计时器完成后的操作
        //Debug.Log("计时器开始");
        instance.isTimerRunning = true;
        instance.locktime = locktime;
        instance.talk_ui.SetActive(true);
        instance.is_blitting_text = false;
        instance.name = "823";
        instance.textLabel.text= text;
    }    
    //static public void image_update(string[] image_pos)
    //{
    //    string sign = image_pos[0];
    //    char pos = image_pos[1][0];
    //    Debug.Log(sign);
    //    if (sign == "取消") 
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
        instance.text_list.Clear();
        instance.name_list.Clear();
        instance.image_list.Clear();
        //Debug.Log("closeUi");
        instance.talk_ui.SetActive(false);
    }
    //检测点击以及文本是否播放完毕,使用前记得开is_bliting_text
    static public bool blit_text()
    {
        if (instance.index <= instance.max_index)   //文字内容没有播完
        {
            if (!instance.talk_ui.activeSelf)
            {
                //设置立绘初始位置
                //string[] image_pos = instance.image_list[0];

                instance.talk_ui.SetActive(true);
                //image_update(image_pos);

                //自动拉取第一行
                updateText();
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))//鼠标点击或F键
            {
                //检测文字需要渲染
                updateText();
            }
            return true;      //不按按键保持talk状态
        }
        else
        {   //文字渲染完成
            closeUi();
            return false;
        }
        
    }
    //按照index排文字
    static public void updateText()   
    {
        if (instance.text_finished)
        {

            string content = instance.text_list[instance.index];

            instance.name_text.text = instance.name_list[instance.index];
            //string[] image_pos = instance.image_list[instance.index];                
            //image_update(image_pos);

            //将文本渲染设置为唯一携程
            instance.text_display = instance.StartCoroutine(instance.setTextUI(content));
            instance.text_finished = false;
        }
        else //文本没结束的时候再按R，停止携程并直接输出文字
        {
            instance.name_text.text = instance.name_list[instance.index];
            instance.StopCoroutine(instance.text_display);
            string content = instance.text_list[instance.index];
            instance.textLabel.text = content;
            instance.text_finished = true;
            instance.index++;

        }
        
        
    }
    //逐字渲染文字
    public IEnumerator setTextUI(string content)
    {
        textLabel.text = string.Empty;
        for (int i = 0; i < content.Length; i++)
        {
            textLabel.text += content[i];
            yield return new WaitForSeconds(textSpeed);
        }
        text_finished = true;
        instance.index++;
        //mouse.SetActive(true);
    }
    //获取固定文本的文字
    static public void GetText(TextAsset textFile)
    {
        instance.name_list.Clear();
        instance.text_list.Clear();
        instance.image_list.Clear();
        var rows = textFile.text.Split('\n');
        foreach (var row in rows)
        {
            string text = row.ToString();
            string[] row_list = text.Split(':');
            string name = row_list[0];
            string content = row_list[1];                   
            instance.name_list.Add(name);
            instance.text_list.Add(content);
                     
        }
        set_index();
    }
    static void set_index()
    {
        instance.index = 0;
        instance.max_index = instance.text_list.Count - 1;
        instance.is_blitting_text = true;
        
    }
    //从ai处获取文本
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
        
    }
    
    
    

}
