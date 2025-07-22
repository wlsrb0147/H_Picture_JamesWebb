using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public interface InputControl
{
    public void SetCurrentInput(int page, int index);
    public void ExecuteInput(Key key, bool performed);
    public void ChangeIndex();
}

// 입력 제어
public class InputManager : MonoBehaviour
{
    // page, index 기반으로 입력 제어
    
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
        private set => _instance = value;
    }

    // 시리얼 컨트롤러
    [SerializeField] private SerialController controller;
    
    // true일시, 입력하면 내가 Json에서 설정한 string값으로 반환
    // arduino에서는 받은값 그대로 println 해주면 실제 아두이노랑 비슷하게 사용가능
    public bool sendStringToArduino;
    
    private InputControl _inputControl;
    private StickInput _stickInput;
    
    private int _sceneValue;
    
    // 인덱스 변경
    public void ChangeIndex()
    {
        _inputControl.ChangeIndex();
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

    private void Start()
    {
        _stickInput = JsonSaver.Instance.Settings.stickInput;
    }

    public void SetInputControl(InputControl inputControl)
    {
        _inputControl = inputControl;
    }

    // 페이지 또는 인덱스가 바뀌었을 때의 설정 적용
    public void SetCurrentIndex(int page, int index)
    {
        _sceneValue = SceneManager.GetActiveScene().buildIndex;

        if (_inputControl == null)
        {
            Debug.Log("Find first input control");
            _inputControl = FindFirstObjectByType<RegistInputControl>();
        }
        _inputControl.SetCurrentInput(page, index);
    }
    
    // 뉴인풋으로 입력 제어
    // 현재는 눌러졌을때만 입력을 받음 (preformed)
    // 뗐을때도 입력받고싶다면 canceled 도 입력받아야함
    public void KeyboardInputControl(InputAction.CallbackContext context)
    {
        bool performed;
        if (context.performed)
        {
            performed = true;
        }
        else if (context.canceled)
        {
            performed = false;
        }
        else
        {
            return;
        }
        
        // 키보드 입력이 아닐경우 리턴
        Key key;

        if (context.control is KeyControl keyControl)
        {
            key = keyControl.keyCode;
        }
        else if (context.control is ButtonControl btn)
        {
            var btnName = btn.name;
            if (string.Equals(btnName, _stickInput.up))
            {
                key = Key.UpArrow;
            }
            else if (string.Equals(btnName, _stickInput.down))
            {
                key = Key.DownArrow;
            }
            else if (string.Equals(btnName, _stickInput.left))
            {
                key = Key.LeftArrow;
            }
            else if (string.Equals(btnName, _stickInput.right))
            {
                key = Key.RightArrow;
            }
            else
            {
                Debug.Log(btnName);
                key = Key.None;
            }
        }
        else
        {
            return;
        }
        
        
        // bool값이 true라면, 키보드 입력을 SerialController에 전달
        if (sendStringToArduino)
        {
            controller.SendArduinoKey(0,key);
            return;
        }
        
        Debug.Log($"Keyboard Input Come : {key}");
        
        // bool값이 false이면 키보드 입력 그대로 실행
        ExecuteInput(key, performed);
    }
    
    // SerialController에서 전달받은 키보드 입력
    // SerialController에는 string을 key로 변환시켜 이 함수를 실행
    public void ArduinoInputControl(InputData data)
    {
        ExecuteInput(data.Key, data.Pressed);
    }
    
    
    // 입력에 따른 함수 실행
    private void ExecuteInput(Key key, bool performed)
    {
        _inputControl.ExecuteInput(key, performed);
    }
    
}
