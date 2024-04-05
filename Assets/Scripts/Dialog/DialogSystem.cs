using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogSystem : SingletonMonobehaviour<DialogSystem>
{
    [Header("UI组件")]
    public GameObject talk_ui;
    public Text textLabel;
    public Text name_text;
    public Image image;
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
    List<string> imageList = new List<string>();
    List<string> name_list = new List<string>();
    List<string> text_list = new List<string>();
    List<string[]> image_list = new List<string[]>();
    [Header("计时器参数")]
    private float locktime = 0f;
    private bool isTimerRunning = false;
    [Header("其他变量")]
    private Coroutine text_display;
    public bool is_blitting_text = false;
    public bool text_finished = true;
    private int index = 0;
    private int max_index = 0;

    /*
     * is_blitting_text用于开始检测点击与渲染文字，
     * text_finished用于检测文字是否渲染完成
     */


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
        // AI思考
        if (isTimerRunning)
        {
            countDown();
        }

        // 对话文本展示
        if (is_blitting_text)
        {
            //设置可点击图标
            mouse.SetActive(text_finished);
            is_blitting_text = blit_text();
        }
        else 
        {
            mouse.SetActive(false);
        }
        
    }   

    /// <summary>
    /// AI思考时间
    /// </summary>
    private void countDown()
    {
        // 更新计时器时间
        locktime -= Time.deltaTime;

        // 检查计时器是否达到持续时间
        if (locktime < 0f)
        {
            // 计时器达到持续时间，执行相应操作
            //talk_ui.SetActive(false);
            isTimerRunning = false;
            //text_finished = true;
            is_blitting_text = true;
            updateText();
            
        }        
    }

    /// <summary>
    /// 开始计时并加lock
    /// </summary>
    public void lockUI_and_setText(float locktime,string text) 
    {
        // 计时器完成后的操作
        //Debug.Log("计时器开始");
        isTimerRunning = true;
        this.locktime = locktime;
        talk_ui.SetActive(true);
        is_blitting_text = false;
        name = "823";
        textLabel.text= text;
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
    //    GameObject ga = GameObject_dic[sign];
    //    RectTransform rectTransform = ga.GetComponent<RectTransform>();
    //    float x_coordinate=middle;

    //    if (pos == 'm')
    //    {
    //        x_coordinate = middle;
    //    }
    //    else if (pos == 'l')
    //    {
    //        x_coordinate = left;
    //    }
    //    else 
    //    {
    //        x_coordinate = right;
    //    }
    //    Debug.Log(x_coordinate);
    //    if (!ga.activeSelf)
    //    {
    //        ga.SetActive(true);
    //        rectTransform.localPosition = new Vector3(x_coordinate, rectTransform.localPosition.y, rectTransform.localPosition.z);

    //    }
    //    if (rectTransform.localPosition.x != x_coordinate)
    //        rectTransform.DOLocalMoveX(x_coordinate,move_time);
    //    //tran.DOLocalMoveX(x_coordinate, move_time);
    //}
    
    /// <summary>
    /// 关闭对话框并清理数据列表
    /// </summary>
    public void closeUi()
    {        
        text_list.Clear();
        name_list.Clear();
        image_list.Clear();
        imageList.Clear();
        talk_ui.SetActive(false);
    }

    /// <summary>
    /// 检测点击以及文本是否播放完毕,使用前记得开is_bliting_text
    /// </summary>
    public bool blit_text()
    {
        if (index <= max_index)   //文字内容没有播完
        {
            if (!talk_ui.activeSelf)
            {
                //设置立绘初始位置
                //string[] image_pos = image_list[0];

                talk_ui.SetActive(true);
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
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))//鼠标点击或F键
            {
                //文字渲染完成
                closeUi();
                return false;
            }
            
        }
        return true;

    }
 
    /// <summary>
    /// 更新对话框内文本
    /// </summary>
    public void updateText()   
    {
        if (text_finished)
        {
            image.sprite = GameResources.Instance.characters.Find(x => x.name == name_list[index] && x.differ == imageList[index]).sprite;
            image.SetNativeSize();
            name_text.text = name_list[index];
            //string[] image_pos = image_list[index];                
            //image_update(image_pos);

            //将文本渲染设置为唯一携程
            string content = text_list[index];
            text_display = StartCoroutine(setTextUI(content));
            text_finished = false;
        }
        else //文本没结束的时候再按R，停止携程并直接输出文字
        {
            name_text.text = name_list[index];
            StopCoroutine(text_display);
            string content = text_list[index];
            textLabel.text = content;
            text_finished = true;
            index++;
        }
    }

    /// <summary>
    /// 逐字渲染文字
    /// </summary>
    public IEnumerator setTextUI(string content)
    {
        textLabel.text = string.Empty;
        for (int i = 0; i < content.Length; i++)
        {
            textLabel.text += content[i];
            yield return new WaitForSeconds(textSpeed);
        }
        text_finished = true;
        index++;
        //mouse.SetActive(true);
    }

    /// <summary>
    /// 获取固定文本的文字
    /// </summary>
    public void GetText(TextAsset textFile)
    {
        name_list.Clear();
        text_list.Clear();
        image_list.Clear();
        var rows = textFile.text.Split('\n');
        foreach (var row in rows)
        {
            string text = row.ToString();
            string[] row_list = text.Split(':');
            string name = row_list[1];
            string content = row_list[2];                   
            name_list.Add(name);
            text_list.Add(content);
            imageList.Add(row_list[0]);
        }
        set_index();
    }
    void set_index()
    {
        index = 0;
        max_index = text_list.Count - 1;
        is_blitting_text = true;
        
    }
    //从ai处获取文本
    public void get_text_in_other_ways(string name, string text, string[] image_display)
    {
        text_list.Clear();
        name_list.Clear();
        image_list.Clear();
        name_list.Add(name);
        text_list.Add(text);
        image_list.Add(image_display);
        //Debug.Log("get_text_in_other_ways");
        set_index();
        
    }
    
    
    

}
