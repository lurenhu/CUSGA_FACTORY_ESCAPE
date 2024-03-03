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
    //public  string CookieValue = "b-user-id=d968b1f3-c59c-5545-1f08-8abe63838109; app-satoken=6a905cdc-ca23-4b98-8ddb-71431b0729fc; Hm_lvt_11ee634290fd232d05132bc7c7c9ad3b=1708756486,1708909854; Hm_lpvt_11ee634290fd232d05132bc7c7c9ad3b=1708911528";
    public string Apikey = "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";
    /// <summary>
    /// 获取的文字在content里
    /// 
    /// </summary>



    // Start is called before the first frame update
    void Start()
    {
        //reflesh(bot_id);
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
    public async Task PostMessage(long botid, string message)
    {
        Debug.Log("post");
        StartCoroutine(SendRequest(botid, message));
    }

    private IEnumerator SendRequest(long botid, string message)
    {
        string Url = "https://api.openai.com/v1/chat/completions";
        // 创建一个UnityWebRequest对象，指定请求方法为POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // 构建请求消息
        var requestBody = new
        {
            messages = new[]//chatgpt的messages配置
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            },
            max_tokens = 50,
            model = "gpt-3.5-turbo"
        };

        // 将请求内容序列化为JSON字符串
        string json = JsonConvert.SerializeObject(requestBody);

        // 将JSON字符串作为请求体内容
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {Apikey}");

        // 发送请求并等待返回
        yield return request.SendWebRequest();

        // 处理返回结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("成功发送信息！");
            Debug.Log("响应内容：" + responseContent);

            //string jsonString = "{ \"id\": \"chatcmpl-8yMXg6UFUecHISzgTaCijmCmCPnFV\", \"object\": \"chat.completion\", \"created\": 1709397024, \"model\": \"gpt-3.5-turbo-0125\", \"choices\": [ { \"index\": 0, \"message\": { \"role\": \"assistant\", \"content\": \"How can I assist you today?\" }, \"logprobs\": null, \"finish_reason\": \"stop\" } ], \"usage\": { \"prompt_tokens\": 18, \"completion_tokens\": 7, \"total_tokens\": 25 }, \"system_fingerprint\": \"fp_2b778c6b35\" }";

            ChatCompletion chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

            // 提取内容字段
            string content = chatCompletion.choices[0].message.content;


            Debug.Log(content);
        }
        else
        {
            Debug.Log("请求失败，状态码：" + request.responseCode);
        }
    }

    public class ChatCompletion
    {
        public string id { get; set; }
        public string objectType { get; set; } // 将 "object" 改为其他标识符，如 "objectType"
        public long created { get; set; }
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usage usage { get; set; }
        public string system_fingerprint { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

}
