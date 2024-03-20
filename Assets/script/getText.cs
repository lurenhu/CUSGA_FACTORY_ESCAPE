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
            if (string.IsNullOrEmpty(row))
            { continue; }
            string text = row.ToString();
            string[] row_list = text.Split(',');          
            string name = row_list[0];
            string role = row_list[1];
            string content = row_list[2];
            instance.name_list.Add(name);
            instance.role_list.Add(content);
            instance.content_list.Add(content);
        }
        
    }
}
