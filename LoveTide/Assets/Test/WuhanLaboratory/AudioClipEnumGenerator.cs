using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class AudioClipEnumGenerator : MonoBehaviour
{
    // 音效 Enum 的名稱
    public string enumName = "AudioClipEnum";

    // 生成 Enum 的路徑
    public string enumPath = "Assets/Scripts/";

    public AudioClipEnum num;

    static public Dictionary<string, string> lists = new Dictionary<string, string>();
    
    [MenuItem("Tools/Move Audio Files to Resources")]
    static void MoveAudioFilesToResources()
    {
        // 原始音效檔案路徑
        string[] audioFiles = Directory.GetFiles(Application.dataPath, "*.mp3", SearchOption.AllDirectories);

        // 目標資料夾路徑
        string targetFolder = Application.dataPath + "/Resources/AudioAssets";

        // 如果目標資料夾不存在，則創建
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            Debug.Log("Created folder: " + targetFolder);
        }

        // 移動檔案到目標資料夾
        foreach (string file in audioFiles)
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetFolder, fileName);
            
            // 檢查如果檔案已經存在，先刪除
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            
            File.Move(file, destFile);
            Debug.Log("Moved file: " + fileName);
        }

        // 更新Unity編輯器資源視窗
        AssetDatabase.Refresh();
        Debug.Log("Audio files moved to Resources/AudioAssets");
    }

    // 執行生成 Enum 的方法
    [MenuItem("Tools/Generate AudioClip Enum")]
    public static void GenerateEnum()
    {
        lists.Clear();
        
        // 獲取所有的 AudioClip 路徑
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        List<string> audioClipPaths = new List<string>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            audioClipPaths.Add(path);
        }

        // 寫入 Enum 的內容
        string enumContent = "public enum " + typeof(AudioClipEnum).Name + "\n{\n";
        foreach (string path in audioClipPaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            enumContent += "    " + ToValidEnumName(fileName) + ",\n";
        }
        enumContent += "}\n";
        
        // 將 Enum 寫入檔案
        string enumFilePath = Path.Combine(Application.dataPath, "Scripts/" + typeof(AudioClipEnum).Name + ".cs");
        File.WriteAllText(enumFilePath, enumContent);
        AssetDatabase.Refresh();

        int i = 0;
        
        foreach (AudioClipEnum item in System.Enum.GetValues(typeof(AudioClipEnum)))
        {
            audioClipPaths[i] = audioClipPaths[i].Replace("Assets/Resources/", "");
            audioClipPaths[i] = audioClipPaths[i].Replace(".mp3", "");
            
            lists.Add(item.ToString() , audioClipPaths[i]);
            Debug.Log("Enum item: " + item + "------Paths " + audioClipPaths[i]);

            i++;
        }
    }

    // 將字串轉換為有效的 Enum 名稱
    private static string ToValidEnumName(string name)
    {
        // 移除非法字元
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        name = name.Replace("(", "");
        name = name.Replace(")", "");
        name = name.Replace(".", "");
        name = name.Replace("[", "");
        name = name.Replace("]", "");
        name = name.Replace("/", "_");
        name = name.Replace("\\", "_");

        // 確保名稱以字母開頭
        if (!char.IsLetter(name[0]))
        {
            name = "_" + name;
        }

        return name;
    }

    public AudioClip GetAudioClip(AudioClipEnum _enum)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(lists[_enum.ToString()].ToString());

        Debug.Log(lists[_enum.ToString()]); 
        Debug.Log(audioClip);
        return audioClip;
    }

    private void Awake()
    {
        GenerateEnum();
    }
}

// 定義 AudioClipEnum
