using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class tongyi_AI : MonoBehaviour
{
    [Header("Ui组件")]
    [SerializeField] public InputField chat_input_field;
    [SerializeField] public Button send_button;
    public GameObject input_field;
    [Header("Ai设置")]
    [Header("机器人ID")]
    [SerializeField] public string character_id = "e7cd826cf38f470797c3593ee822341f";
    [Header("用户cookie")]
    //public  string CookieValue = "b-user-id=d968b1f3-c59c-5545-1f08-8abe63838109; app-satoken=6a905cdc-ca23-4b98-8ddb-71431b0729fc; Hm_lvt_11ee634290fd232d05132bc7c7c9ad3b=1708756486,1708909854; Hm_lpvt_11ee634290fd232d05132bc7c7c9ad3b=1708911528";
    public string Apikey = "lm-dXxiQGyE363suBUpwRUMMQ==";
    public static tongyi_AI instance;
    DialogSystem DialogSystem = DialogSystem.instance;
    /// <summary>
    /// 获取的文字在content里
    /// 
    /// </summary>


    private void Awake()   //单例的默认写法
    {

        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        setChatUIActive(true);
        send_button.onClick.AddListener(delegate {sendMessage(character_id);});
        
    }

    public void setChatUIActive(bool activeSelf)
    {   
        
        input_field.SetActive(activeSelf);
    }

    public async void  sendMessage(string bot_id)
    {
        
        if (chat_input_field.text.Equals(""))
            return;
        string content = chat_input_field.text;     //在这里获取文本的信息
        Debug.Log(content);
        chat_input_field.text = "";
        await PostMessage(bot_id,content);
        //post(bot_id,content);

    }
    public async  Task PostMessage(string bot_id,string message)
    {
        Debug.Log("post");
        StartCoroutine(SendRequest(bot_id,message));
    }

    private IEnumerator SendRequest(string bot_id, string message)
    {
        string Url = "https://nlp.aliyuncs.com/v2/api/chat/send";
        // 创建一个UnityWebRequest对象，指定请求方法为POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");
        
        // 构建请求消息
        
        int seed = 1683806810;

        var requestBody = string.Format(@"{{
    ""input"": {{
        ""messages"": [
            {{
                ""name"": ""陶特"",
                ""role"": ""user"",
                ""content"": ""{0}""
            }}
        ],
        ""aca"": {{
            ""botProfile"": {{
                ""characterId"": ""{1}"",
                ""version"": 1
            }},
            ""userProfile"": {{
                ""userId"": ""123456789"",
                ""userName"": ""云账号名称"",
                ""basicInfo"": """"
            }},
            ""scenario"": {{
                ""description"": ""我是陶特，是你的主人""
            }},
            ""context"": {{
                ""useChatHistory"": false,
                ""isRegenerate"": true,
                ""queryId"": ""fd26039ac11f4ca0960a66d7c0520091""
            }}
        }}
    }},
    ""parameters"": {{
        ""seed"": {2},
        ""incrementalOutput"": false
    }}
}}", message, bot_id, seed);


        // 将请求内容序列化为JSON字符串
        //string json = JsonConvert.SerializeObject(requestBody);

        // 将JSON字符串作为请求体内容
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {Apikey}");
        //request.SetRequestHeader("Expect", "");
        request.SetRequestHeader("Accept", "text/event-stream;charset=UTF-8");
        request.SetRequestHeader("X-AcA-DataInspection", "enable");
        request.SetRequestHeader("X-AcA-SSE", "enable");
        request.SetRequestHeader("x-fag-servicename", "aca-chat-send-sse");
        request.SetRequestHeader("x-fag-appcode", "aca");
        // 发送请求并等待返回
        yield return request.SendWebRequest();

        // 处理返回结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("成功发送信息！");
            Debug.Log("响应内容：" + responseContent);
            string[] content_list = responseContent.Split("data:");
            string json = content_list[content_list.Length-1];

                      
            //整理成json格式
            //ChatCompletion chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

            // 解析 JSON
            var jsonObject = JObject.Parse(json);

            // 提取所需的字符串
            string reply_text = (string)jsonObject["choices"][0]["messages"][0]["content"];
            Debug.Log(reply_text);
            DialogSystem.get_text_in_other_ways("823", reply_text, new string[2]);
            
        }
        else
        {
            Debug.Log("请求失败，状态码：" + request.responseCode);
        }
    }
}
