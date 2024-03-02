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
    public  string CookieValue = "b-user-id=d968b1f3-c59c-5545-1f08-8abe63838109; app-satoken=6a905cdc-ca23-4b98-8ddb-71431b0729fc; Hm_lvt_11ee634290fd232d05132bc7c7c9ad3b=1708756486,1708909854; Hm_lpvt_11ee634290fd232d05132bc7c7c9ad3b=1708911528";
    
    /// <summary>
    /// ��ȡ��������content��
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
        string content = chat_input_field.text;     //�������ȡ�ı�����Ϣ
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
        // ����һ��UnityWebRequest����ָ�����󷽷�ΪPOST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // �������������
        var requestBody = new
        {
            botId = botid,      // ��ɫid
            input = message,                   // ��������
            channel = 123,
            width = 659,
            height = 704
        };

        // �������������л�ΪJSON�ַ���
        string json = JsonConvert.SerializeObject(requestBody);

        // ��JSON�ַ�����Ϊ����������
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // ��������ͷ
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Cookie", CookieValue);

        // �������󲢵ȴ�����
        yield return request.SendWebRequest();

        // �����ؽ��
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("�ɹ�������Ϣ��");
            Debug.Log("��Ӧ���ݣ�" + responseContent);

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
                        //Debug.Log("�Ե���һ�з��ص��û��Ի���Ϣ");
                    }
                }
            }

            Debug.Log(requestBody.input);
            Debug.Log(finalContent);
        }
        else
        {
            Debug.Log("����ʧ�ܣ�״̬�룺" + request.responseCode);
        }
    }
    private IEnumerator RefleshRequest(long botid)
    {
        string Url = "https://xuanheai.com/chat/clear.do";
        // ����һ��UnityWebRequest����ָ�����󷽷�ΪPOST
        UnityWebRequest request = UnityWebRequest.PostWwwForm(Url, "");

        // �������������
        var requestBody = new
        {
            botId = botid,      // ��ɫid
        };

        // �������������л�ΪJSON�ַ���
        string json = JsonConvert.SerializeObject(requestBody);

        // ��JSON�ַ�����Ϊ����������
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // ��������ͷ
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Cookie", CookieValue);

        // �������󲢵ȴ�����
        yield return request.SendWebRequest();

        // �����ؽ��
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseContent = request.downloadHandler.text;
            Debug.Log("�ɹ�������Ϣ��");
            Debug.Log("��Ӧ���ݣ�" + responseContent);
        }
        else
        {
            Debug.Log("����ʧ�ܣ�״̬�룺" + request.responseCode);
        }
    }
    public class MyData    //��ȡ���ݵķ���
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
