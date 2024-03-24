﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
[System.Serializable]
public class tongyi_AI : MonoBehaviour
{
    [Header("Ui组件")]
    [SerializeField] public InputField chat_input_field;
    [SerializeField] public Button send_button;
    public GameObject input_field;
    [Header("Ai设置")]
    public string name = "对话角色1";
    public TextAsset chatHistory;
    public bool use_history;
    [Header("用户cookie")]    
    public string Apikey = "lm-dXxiQGyE363suBUpwRUMMQ==";
    [Header("机器人id列表")]
    public robotCollection[] robots;    
    [Header("对接用变量")]
    public int anxiety_change_value = 0;
    public string reply_text;
    public bool reply_is_finished=false;
    public AudioClip test_SFX;
    public static tongyi_AI instance;

    private void Awake()   //单例的默认写法
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        robots=new robotCollection[]
            { 
              new robotCollection("焦虑评估器","e7cd826cf38f470797c3593ee822341f"),
              new robotCollection("对话角色1","89f40467361e43ecb565ab323063bea4")               
            };
    }
    // Start is called before the first frame update
    void Start()
    {
        //给button绑方法,确定机器人id
        setChatUIActive(true);
        
        robotCollection bot = Array.Find(robots, x => x.name == name);
        if (bot == null)
        {
            Debug.Log("Not found");
        }
        else
        {            
            send_button.onClick.AddListener(delegate { sendMessage(bot); });
        }                           
    }

    public void setChatUIActive(bool activeSelf)
    {   
        input_field.SetActive(activeSelf);
    }

    //获取内容兼发送
    public async void  sendMessage(robotCollection bot)
    {
        if (chat_input_field.text.Equals(""))
            soundManager.playSFX(test_SFX);
            return;
        string content = chat_input_field.text;     //在这里获取文本的信息,并将它记录        
        getText.WriteText(new string[] { "陶特", "user", content });
        chat_input_field.text = "";        
        await PostMessage(bot,content);      

    }
    //根据bot_name选择发送的requestBody
    public async  Task PostMessage(robotCollection bot, string message)
    {
        Debug.Log("post");
        string bot_name = bot.name;
        string bot_id = bot.botid;
        float delay = 0.1f;
        // 构建请求消息
        if (bot_name == "焦虑评估器")
        {
            int seed = 1683806810;
            var requestBody = string.Format(@"{{
    ""input"": {{
        ""messages"": [            
            {{
                ""name"": ""陶特"",
                ""role"": ""user"",
                ""content"": ""评估消息焦虑值：{0}""
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
                ""description"": ""我是你的主人""
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
            StartCoroutine(SendRequest(requestBody,bot));            
            while (!reply_is_finished) { await Task.Delay(TimeSpan.FromSeconds(delay)); }
            check_anxiety_change();            
            reply_is_finished = false;

        }
        else if (bot_name == "对话角色1")
        {   
            int seed = 1683806810;
            //用对话历史
            if(use_history)
            {
                string chat_history =getText.loadText(chatHistory);
                Debug.Log($"chat_history:{chat_history}");
                var requestBody = string.Format(@"{{
    ""input"": {{
        ""messages"": [   {0}            
            {{
                ""name"": ""陶特"",
                ""role"": ""user"",
                ""content"": ""{1}""
            }}
        ],
        ""aca"": {{
            ""botProfile"": {{
                ""characterId"": ""{2}"",
                ""version"": 1
            }},
            ""userProfile"": {{
                ""userId"": ""123456789"",
                ""userName"": ""云账号名称"",
                ""basicInfo"": """"
            }},
            ""scenario"": {{
                ""description"": ""我是陶特，是你的朋友""
            }},
            ""context"": {{
                ""useChatHistory"": false,
                ""isRegenerate"": true,
                ""queryId"": ""fd26039ac11f4ca0960a66d7c0520091""
            }}
        }}
    }},
    ""parameters"": {{
        ""seed"": {3},
        ""incrementalOutput"": false
    }}
}}", chat_history,message, bot_id, seed);
                Debug.Log($"requestbody:{requestBody}");
                StartCoroutine(SendRequest(requestBody,bot));
            }
            else
            {
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
                ""description"": ""我是陶特，是你的朋友""
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
                StartCoroutine(SendRequest(requestBody,bot));
            }
            
            //将获取的信息发去评估焦虑
            while (!reply_is_finished) { await Task.Delay(TimeSpan.FromSeconds(delay)); }
            string name = "焦虑评估器";
            robotCollection bot1 = Array.Find(robots, x => x.name == name);
            reply_is_finished = false;
            await PostMessage(bot1, reply_text);       
            

        }
        else
        {
            Debug.Log($"bot_name:{bot_name}");
            
        }
        
    }

    private IEnumerator  SendRequest(string requestBody, robotCollection bot)
    {
        string Url = "https://nlp.aliyuncs.com/v2/api/chat/send";
        // 创建一个UnityWebRequest对象，指定请求方法为POST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");        
        // 将请求内容序列化为JSON字符串
        //string json = JsonConvert.SerializeObject(requestBody);

        // 将JSON字符串作为请求体内容
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {Apikey}");        
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
            
            string[] content_list = responseContent.Split("data:");
            string json = content_list[content_list.Length-1];                      
            //整理成json格式
            //ChatCompletion chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

            // 解析 JSON
            var jsonObject = JObject.Parse(json);

            // 提取所需的字符串
            reply_text = (string)jsonObject["choices"][0]["messages"][0]["content"];
            Debug.Log(reply_text);
            reply_is_finished = true;
            if (bot.name== "对话角色1")
            {   
                getText.WriteText(new string[] { "823", "assistant", reply_text});
                DialogSystem.get_text_in_other_ways("823", reply_text, new string[2]);//最后一个是演出列表
            }
           
        }
        else
        {
            Debug.Log("请求失败，状态码：" + request.responseCode);
        }
    }
    private async void check_anxiety_change()
    {
        Debug.Log("check_anxiety_change");
        
        string text;
        reply_text.Replace("了", "");
        //int anxiety_change_value=0;
        if (reply_text.Contains("焦虑值"))
        {            
            // 使用正则表达式提取数字                
            Match match = Regex.Match(reply_text, @"为(\d+)");
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                anxiety_change_value =int.Parse(result);
            }
        }        
        else
        {
            anxiety_change_value = 0;            
        }
        Debug.Log(anxiety_change_value);
        System.Random random = new System.Random();
        int multiplier = 10;  // 扩大的倍数
        double randomMultiplier = random.NextDouble() * 0.2 + 0.9;  // 生成随机浮动倍数（范围为0.9到1.1之间）

        double fianal_value = anxiety_change_value * multiplier * randomMultiplier;
        anxiety_change_value = (int)fianal_value;
        Debug.Log($"anxiety_change_value:{anxiety_change_value}");
        StaticEventHandler.CallCommit(anxiety_change_value);
    }
}
