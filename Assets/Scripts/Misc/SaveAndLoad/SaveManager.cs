using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveManager
{
    private static readonly string saveFolder = Application.persistentDataPath + "/GameData";

    /// <summary>
    /// 删除配置文件
    /// </summary>
    /// <param name="profileName">配置文件名称</param>
    public static void Delete(string profileName)
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
            throw new Exception($"Save Profile {profileName} does not exist");

        Debug.Log($"Successfully Delete {saveFolder}/{profileName}");
        File.Delete($"{saveFolder}/{profileName}");
    }

    /// <summary>
    /// 导出所保存的Json数据
    /// </summary>
    /// <param name="profileName">配置文件名称</param>
    /// <returns>导出的数据</returns>
    public static SaveProfile<T> Load<T>(string profileName) where T : SaveProfileData
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
            throw new Exception($"Save Profile {profileName} does not exist");

        var fileContent = File.ReadAllText($"{saveFolder}/{profileName}");
        Debug.Log($"Successfully Load {saveFolder}/{profileName}");
        return JsonConvert.DeserializeObject<SaveProfile<T>>(fileContent);
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    /// <param name="save">保存的数据</param>
    public static void Save<T>(SaveProfile<T> save) where T : SaveProfileData
    {
        if (!File.Exists($"{saveFolder}/{save.profileName}"))
            throw new Exception($"Save Profile {save.profileName} does not exist");

        var jsonString = JsonConvert.SerializeObject(save,Formatting.Indented,
            new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

        // 当存储路径不存在时
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        File.WriteAllText($"{saveFolder}/{save.profileName}", jsonString);
    }
}
