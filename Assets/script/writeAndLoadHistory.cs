using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.UIElements.UxmlAttributeDescription;
public class writeAndLoadHistory : MonoBehaviour
{
    public static writeAndLoadHistory instance;
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
    //本质是个add
    static public void writeText(string[] write_content)
    {
        // 检查文件夹是否存在，不存在则创建
        string folderPath = Application.dataPath + "/chatHistory";
        string filePath = folderPath + "/chatHistory.txt";

        // 检查文件夹是否存在，不存在则创建
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            // 创建文件
            File.Create(filePath).Close();
        }

        // 打开文件并写入内容
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
