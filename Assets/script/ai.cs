using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ai : MonoBehaviour
{
    [Header("Ui组件")]
    [SerializeField] public InputField chat_input_field;
    [SerializeField] public Button send_button;
    public GameObject input_field;
    [Header("Ai设置")]
    [Header("机器人ID")]
    [SerializeField] public long bot_id;
    [Header("用户cookie")]
    public  string CookieValue = "b-user-id=d968b1f3-c59c-5545-1f08-8abe63838109; app-satoken=6a905cdc-ca23-4b98-8ddb-71431b0729fc; Hm_lvt_11ee634290fd232d05132bc7c7c9ad3b=1708756486,1708909854; Hm_lpvt_11ee634290fd232d05132bc7c7c9ad3b=1708911528";
    
    /// <summary>
    /// 获取的文字在content里
    /// 
    /// </summary>

    

    // Start is called before the first frame update
    void Start()
    {
        reflesh(bot_id);
        setChatUIActive(true);
        send_button.onClick.AddListener(delegate {sendMessage(bot_id);});
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    public void setChatUIActive(bool activeSelf)
    {   
        
        input_field.SetActive(activeSelf);
    }

    public async void  sendMessage(long bot_id)
    {
        
        if (chat_input_field.text.Equals(""))
            return;
        string content = chat_input_field.text;     //在这里获取文本的信息
        Debug.Log(content);
        chat_input_field.text = "";
        await PostMessage(bot_id,content);
        //post(bot_id,content);

    }
    public async void reflesh(long botid)
    {
        Debug.Log("reflesh");
        StartCoroutine(RefleshRequest(botid));
    }
    public async Task PostMessage(long botid, string message)
    {
        Debug.Log("post");
        StartCoroutine(SendRequest(botid, message));
    }

    private IEnumerator SendRequest(long botid, string message)
    {
        string Url = "https://xuanheai.com/chat/sendStream.do";
        // 创建一个UnityWebRequest对象，指定请求方法为POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // 构建请求的内容
        var requestBody = new
        {
            botId = botid,      // 角色id
            input = message,                   // 输入内容
            channel = 123,
            width = 659,
            height = 704
        };

        // 将请求内容序列化为JSON字符串
        string json = JsonConvert.SerializeObject(requestBody);

        // 将JSON字符串作为请求体内容
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Cookie", CookieValue);

        // 发送请求并等待返回
        yield return request.SendWebRequest();

        // 处理返回结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("成功发送信息！");
            Debug.Log("响应内容：" + responseContent);

            string line = responseContent.Trim();
            string[] lines = line.Split('\n');

            string finalContent = "";
            foreach (string text in lines)
            {
                if (text == "")
                {
                    //Debug.Log("none");
                }
                else
                {
                    try
                    {
                        string text1 = text.Replace("data:", "");
                        //Debug.Log($"text1:{text1}");
                        var jsonObject = JsonConvert.DeserializeObject<MyData>(text1);
                        var partContent = jsonObject.choices[0].delta.content;
                        finalContent += partContent;
                        //Debug.Log(partContent);
                    }
                    catch (Exception ex)
                    {
                        //Debug.Log("吃掉第一行返回的用户对话信息");
                    }
                }
            }

            Debug.Log(requestBody.input);
            Debug.Log(finalContent);
        }
        else
        {
            Debug.Log("请求失败，状态码：" + request.responseCode);
        }
    }
    private IEnumerator RefleshRequest(long botid)
    {
        string Url = "https://xuanheai.com/chat/clear.do";
        // 创建一个UnityWebRequest对象，指定请求方法为POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // 构建请求的内容
        var requestBody = new
        {
            botId = botid,      // 角色id
        };

        // 将请求内容序列化为JSON字符串
        string json = JsonConvert.SerializeObject(requestBody);

        // 将JSON字符串作为请求体内容
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Cookie", CookieValue);

        // 发送请求并等待返回
        yield return request.SendWebRequest();

        // 处理返回结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("成功发送信息！");
            Debug.Log("响应内容：" + responseContent);
        }
        else
        {
            Debug.Log("请求失败，状态码：" + request.responseCode);
        }
    }
    public class MyData    //提取数据的方法
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Delta delta { get; set; }
    }

    public class Delta
    {
        public string content { get; set; }
    }
}
