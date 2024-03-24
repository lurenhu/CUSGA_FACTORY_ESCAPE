using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.UIElements.UxmlAttributeDescription;
public class getText : MonoBehaviour
{
    public static getText instance;
    List<string> name_list = new List<string>();
    List<string> role_list = new List<string>();
    List<string> content_list = new List<string>();
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    //�����Ǹ�add
    static public void WriteText(string[] write_content)
    {
        // ����ļ����Ƿ���ڣ��������򴴽�
        string folderPath = Application.dataPath + "/chatHistory";
        string filePath = folderPath + "/chatHistory.txt";

        // ����ļ����Ƿ���ڣ��������򴴽�
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ����ļ��Ƿ����
        if (!File.Exists(filePath))
        {
            // �����ļ�
            File.Create(filePath).Close();
        }

        // ���ļ���д������
        using (StreamWriter writer = new StreamWriter(filePath,true))
        {
            foreach (string content in write_content) 
            { 
                writer.Write(content+','); 
                
            }
            writer.Write("\n");
            writer.Close();          
            
        }
        //Debug.Log("File written successfully.");
    }
    static public string loadText(TextAsset textFile)
    {
        //instance.name_list.Clear();
        //instance.role_list.Clear();
        //instance.content_list.Clear();
        string[] rows = textFile.text.Split('\n');
        string final_request_body = "";
        foreach (string row in rows)
        {   
            if (string.IsNullOrEmpty(row))
            { continue; }
            string text = row.ToString();
            string[] row_list = text.Split(',');
            //Debug.Log($"text:{text}");
            string name = row_list[0];
            string role = row_list[1];
            string content = row_list[2];
            //instance.name_list.Add(name);
            //instance.role_list.Add(role);
            //instance.content_list.Add(content);
            string partRequestbody = string.Format(@"
            {{
                    ""name"": ""{0}"",
                ""role"": ""{1}"",
                ""content"": ""{2}""
            }}",name,role,content);
            final_request_body += partRequestbody;
        }
        return final_request_body;
        
    }
}
