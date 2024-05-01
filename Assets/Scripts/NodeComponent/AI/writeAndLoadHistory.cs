using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.UIElements.UxmlAttributeDescription;
public class writeAndLoadHistory : MonoBehaviour
{
    public static writeAndLoadHistory instance;
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

    static public void loadmodel(string name) 
    {
        string folderPath = Application.dataPath + "/chatHistory";
        string filePath = folderPath + $"/{name}.txt";
        string content;
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
        using (StreamReader reader = new StreamReader(filePath))
        {
            content = reader.ReadToEnd();
        }
        filePath = folderPath + "/chatHistory.txt";
        // 将内容写入目标文件
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.Write(content);
            writer.Close();
        }
    }

    
    static public void clearHistory()
    {
        string folderPath = Application.dataPath + "/chatHistory";
        string filePath = folderPath + "/chatHistory.txt";
        File.WriteAllText(filePath, string.Empty);
    }
}
