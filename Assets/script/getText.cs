using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    static public void WriteText(string[] write_content)
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
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string content in write_content) 
            { 
                writer.Write(content+','); 
                
            }
            writer.Write("\n");
            writer.Close();          
            
        }
        Debug.Log("File written successfully.");
    }
    static public void loadText(TextAsset textFile)
    {
        instance.name_list.Clear();
        instance.role_list.Clear();
        instance.content_list.Clear();
        var rows = textFile.text.Split('\n');
        foreach (var row in rows)
        {
            string text = row.ToString();
            string[] row_list = text.Split(',');
            string sign = row_list[0];
            if (sign == "立绘标志")
            {
                continue;
            }

            string position = row_list[4];
            string name = row_list[2];
            string content = row_list[3];
            instance.name_list.Add(name);
            instance.role_list.Add(content);
            instance.content_list.Add(content);
            
            //Debug.Log($"image_list:{sign + position}");
        }
        
    }
}
