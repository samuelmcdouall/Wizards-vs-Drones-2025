using System.IO;
using System.Text;
using UnityEngine;

public class WVDSaveDataManager : MonoBehaviour
{
    public WVDSaveData SaveData;
    string _saveDataPath;
    Encoding _encoding;

    void Awake()
    {
        _saveDataPath = Application.persistentDataPath + "/WVDSaveData.json";
        _encoding = Encoding.GetEncoding("utf-32");
        if (File.Exists(_saveDataPath))
        {
            string loadSaveDataString = File.ReadAllText(_saveDataPath, _encoding);
            SaveData = JsonUtility.FromJson<WVDSaveData>(loadSaveDataString);
        }
        else
        {
            SaveData = new WVDSaveData();
            SaveNewData();
        }
    }
    public void SaveNewData()
    {
        string saveDataString = JsonUtility.ToJson(SaveData);
        File.WriteAllText(_saveDataPath, saveDataString, _encoding);
    }
    public void SaveNewData(WVDSaveData data) // If want to give brand new data instead of editting the SaveData in this class use this function with the WVDSaveData parameter
    {
        string saveDataString = JsonUtility.ToJson(data);
        File.WriteAllText(_saveDataPath, saveDataString, _encoding);
    }
}