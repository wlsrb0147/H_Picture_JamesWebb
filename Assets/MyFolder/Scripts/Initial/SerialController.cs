using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;
using System.Threading;
using UnityEngine.InputSystem;


public struct InputData
{
    public Key Key;                // 기본형 필드
    public bool Pressed;      // Unity 엔진 타입도 가능
    
    // 생성자(Constructor)
    public InputData(Key key, bool pressed)
    {
        this.Key = key;
        this.Pressed = pressed;
    }
}

public class SerialController : MonoBehaviour
{
    [Tooltip("Port name with which the SerialPort object will be created.")]
    
    //이거 안씀, json에서 불러온값만 사용
    public string[] portName = {"COM3"};

    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int baudRate = 9600;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int reconnectionDelay = 1000;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public int maxUnreadMessages = 1;
    
    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    protected Thread[] thread;
    protected SerialThreadLines[] serialThread;

    private bool _isReady;
    
    private ArduinoSetting _arduinoSetting;

    private InputManager _inputManager;
    
    // 쓰레드 생성순서 보장용
    private readonly List<string> _nameList = new ();

    // 아두이노 테스트용 딕서녀리 
    // 키 입력을 string으로 변환
    private readonly Dictionary<Key, string> _keyToString = new ();
    
    // 내가 받을 스트링 딕셔너리
    // string을 key 입력으로 변환
    private readonly Dictionary<string, Key> _stringToKey = new ();
    
    private static SerialController _instance;
    public static SerialController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SerialController>();
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
    }

    // 아두이노 정보 등록
    private void SetArduinoData(ArduinoSetting setting)
    {
        if (!gameObject.activeSelf) return;
        
        _arduinoSetting = setting;
        
        maxUnreadMessages = 120;
        
        thread = new Thread[setting.portNames.Length];
        serialThread = new SerialThreadLines[setting.portNames.Length];
        
        for (int i = 0; i < setting.portNames.Length; i++)
        {
            CreateSerialThread(i, setting.portNames[i]);
        }
        
        SetDictionary();
        _isReady = true;
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        SetArduinoData(JsonSaver.Instance.GetArduinoSetting());
    }

    private void SetDictionary()
    {
        _keyToString.Clear();
        _stringToKey.Clear();

        if (_arduinoSetting == null)
        {
            Debug.LogWarning("ArduinoSetting is null or empty. Skipping dictionary setup.");
            return;
        }
        
        RegisterKeyCodes(Key.Space, _arduinoSetting.spacePressed);
        RegisterKeyCodes(Key.Space, _arduinoSetting.spaceReleased);
    }
    
    private void RegisterKeyCodes(Key key, string value)
    {
        _keyToString[key] = value;
        _stringToKey[value] = key;
    }

    private void CreateSerialThread(int index, string port)
    {
        Debug.Log($"index :{index} port : {port}");
        serialThread[index] = new SerialThreadLines(port, 
            baudRate, 
            reconnectionDelay,
            maxUnreadMessages);
        thread[index] = new Thread(serialThread[index].RunForever);
        thread[index].Start();
        
        while (_nameList.Count <= index)
        {
            _nameList.Add(null);  // 리스트 확장
        }
        _nameList[index] = port;
    }
    
    void OnDisable()
    {
        for (int i = 0; i < serialThread.Length; i++)
        {
            if (serialThread[i] != null)
            {
                serialThread[i].RequestStop();
                serialThread[i] = null;
            }
        }

        for (int i = 0; i < thread.Length; i++)
        {
            if (thread[i] != null)
            {
                thread[i].Join();
                thread[i] = null;
            }
        }
    }
    
    void Update()
    {
        if (!_isReady) return;
        
        if (serialThread.Length == 0)
            return;

        for (int i = 0; i < serialThread.Length; i++)
        {
            ReadSerialMessage(i);
        }
    }

    private void ReadSerialMessage(int index)
    {
        string message = (string)serialThread[index].ReadMessage();
        
        if (message == null) return;

        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
        {
            Debug.Log("On connection event : true On " + _nameList[index]);
        }
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
        {
            Debug.Log("On connection event : false On " + _nameList[index]);
        }
        else
        {
            // string을 key로 변환
            InputData key = ConvertStringToKey(message);
            Debug.Log($"Get Message From {_nameList[index]} : {message}");
            // inputManager에 key 입력
            _inputManager.ArduinoInputControl(key);
        }
    }
    
    private InputData ConvertStringToKey(string input)
    {
        InputData data = new InputData();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            data.Key = Key.None;
            data.Pressed = true;
            return data;
        }

        input = input.Trim().Replace("\n", "");

        data.Key = _stringToKey.GetValueOrDefault(input, Key.None);
        data.Pressed = !input.EndsWith(_arduinoSetting.releaseString);

        return data;
    }

    public void SendArduinoKey(int index, Key key)
    {
        string s = _keyToString.GetValueOrDefault(key, key.ToString());
        
        SendSerialMessage(index, s);
    }
    
    public void SendSerialMessage(int index, string message)
    {
        if (serialThread[index] == null)
        {
            Debug.Log("serialThread is null");
        }
        
        Debug.Log($"Send Arduino{index} : {message}");
        serialThread[index].SendMessage(message);
    }
}
