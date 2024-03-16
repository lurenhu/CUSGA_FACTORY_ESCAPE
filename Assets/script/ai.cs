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
    [Header("Ui���")]
    [SerializeField] public InputField chat_input_field;
    [SerializeField] public Button send_button;
    public GameObject input_field;
    [Header("Ai����")]
    [Header("������ID")]
    [SerializeField] public long bot_id;
    [Header("�û�cookie")]
    //public  string CookieValue = "b-user-id=d968b1f3-c59c-5545-1f08-8abe63838109; app-satoken=6a905cdc-ca23-4b98-8ddb-71431b0729fc; Hm_lvt_11ee634290fd232d05132bc7c7c9ad3b=1708756486,1708909854; Hm_lpvt_11ee634290fd232d05132bc7c7c9ad3b=1708911528";
    public string Apikey = "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";
    /// <summary>
    /// ��ȡ��������content��
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
        string content = chat_input_field.text;     //�������ȡ�ı�����Ϣ
        Debug.Log(content);
        chat_input_field.text = "";
        await PostMessage(content);
        //post(bot_id,content);

    }
    public async Task PostMessage(string message)
    {
        Debug.Log("post");
        StartCoroutine(SendRequest(message));
    }

    private IEnumerator SendRequest(string message)
    {
        string Url = "https://api.openai.com/v1/chat/completions";
        // ����һ��UnityWebRequest����ָ�����󷽷�ΪPOST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // ����������Ϣ
        var requestBody = new
        {
            messages = new[]//chatgpt��messages����
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            },
            max_tokens = 50,
            model = "gpt-3.5-turbo"
        };

        // �������������л�ΪJSON�ַ���
        string json = JsonConvert.SerializeObject(requestBody);

        // ��JSON�ַ�����Ϊ����������
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // ��������ͷ
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {Apikey}");

        // �������󲢵ȴ�����
        yield return request.SendWebRequest();

        // �������ؽ��
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("�ɹ�������Ϣ��");
            Debug.Log("��Ӧ���ݣ�" + responseContent);

            //string jsonString = "{ \"id\": \"chatcmpl-8yMXg6UFUecHISzgTaCijmCmCPnFV\", \"object\": \"chat.completion\", \"created\": 1709397024, \"model\": \"gpt-3.5-turbo-0125\", \"choices\": [ { \"index\": 0, \"message\": { \"role\": \"assistant\", \"content\": \"How can I assist you today?\" }, \"logprobs\": null, \"finish_reason\": \"stop\" } ], \"usage\": { \"prompt_tokens\": 18, \"completion_tokens\": 7, \"total_tokens\": 25 }, \"system_fingerprint\": \"fp_2b778c6b35\" }";

            ChatCompletion chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(responseContent);

            // ��ȡ�����ֶ�
            string content = chatCompletion.choices[0].message.content;


            Debug.Log(content);
        }
        else
        {
            Debug.Log("����ʧ�ܣ�״̬�룺" + request.responseCode);
        }
    }

    public class ChatCompletion
    {
        public string id { get; set; }
        public string objectType { get; set; } // �� "object" ��Ϊ������ʶ������ "objectType"
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
