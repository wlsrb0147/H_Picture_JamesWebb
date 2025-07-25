using System;
using System.IO;
using UnityEngine;
using Debug = DebugEx;

#region SerializableSettingClass

[Serializable]
public class CloseSetting
{
    public Vector2 position;
    public int numToClose;
    public float resetClickTime;
    public float imageAlpha;
}

[Serializable]
public class ArduinoSetting
{
    public string[] portNames;
    public string pressString;
    public string releaseString;
    public string spacePressed;
    public string spaceReleased;
}

[Serializable]
public class StickInput
{
    public string up;
    public string down;
    public string left;
    public string right;
    public string button;
}

[Serializable]
public class AudioSetting
{
    public string fileName;
    public float volume;
}

[Serializable]
public class VideoSetting
{
    public string fileName;
    public float volume;
}

#endregion


[Serializable]
public class Settings
{
    public CloseSetting closeSetting;
    public AudioSetting[] audioSetting;
    public VideoSetting[] videoSetting;
    public ArduinoSetting arduinoSetting;
    public StickInput stickInput;
    public float animSpeed;
}

public class JsonSaver : MonoBehaviour
{
    [NonSerialized] public Settings Settings;
    
    // json 파싱 및 초기설정
    // jsonSaver가 가장 먼저 실행되어야함
    
    private static JsonSaver _instance;
    public static JsonSaver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<JsonSaver>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
        private set => _instance = value;
    }
    
    private void Awake()
    {
        if (_instance == null) 
        { 
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        
        Settings = LoadJsonData<Settings>("Settings.json");
    }


    public CloseSetting GetCloserSetting()
    {
        return Settings.closeSetting;
    }

    public ArduinoSetting GetArduinoSetting()
    {
        return Settings.arduinoSetting;
    }

    public AudioSetting[] GetAudioSetting()
    {
        return Settings.audioSetting;
    }

    public VideoSetting[] GetVideoSetting()
    {
        return Settings.videoSetting;
    }
    
    private T LoadJsonData<T>(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        filePath = filePath.Replace("\\", "/");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("Loaded JSON: " + json); // JSON 문자열 출력
            return JsonUtility.FromJson<T>(json);
        }

        Debug.LogWarning("File does not exist!");
        return default;
    }
    
    private static bool _isCreated;
    private static void SaveLog()
    {
        if (_isCreated)
        {
            return;
        }

        _isCreated = true;
        
        string folderPath = Path.Combine(Application.persistentDataPath, "Log", DateTime.Now.ToString("yyyy-MM") );
        folderPath = folderPath.Replace("\\", "/");

        // 폴더가 없다면 생성
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        
        folderPath = Path.Combine(folderPath, DateTime.Now.ToString("dd") );
        
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // 제어하고 있는 프로그램의 로그 메모장 파일
        string logPath = Path.Combine(Application.persistentDataPath, "Player.log");
        
        logPath = logPath.Replace("\\", "/");
        
        // 파일 복사
        if (File.Exists(logPath))
        {
            string fileName = Path.Combine(folderPath, DateTime.Now.ToString("HH꞉mm꞉ss ") + ".log");
            if (!File.Exists(fileName))
            {
                File.Copy(logPath, fileName);
            }
        }
    }
    
    private void OnApplicationQuit()
    {
#if !Unity_Editor
        SaveLog();
#endif
    }
}
