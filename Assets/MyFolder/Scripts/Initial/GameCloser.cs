using System;
using UnityEngine;
using Debug = DebugEx;
using UnityEngine.UI;

// 특정위치 화면 터치시 게임 종료
public class GameCloser : MonoBehaviour
{
    private CloseSetting _closeSetting;
    [SerializeField] private RectTransform rectTransform;
    
    private int _numToClose;
    private int _currentClick;
    private float _resetTime;
    private float _currentTime;
    
    private bool _enableCount;


    private void Start()
    {
        SetCloserSetting(JsonSaver.Instance.GetCloserSetting());
    }

    // 앵커, 피봇등 초기설정
    private void SetCloserSetting(CloseSetting closeSetting)
    {
        _closeSetting =  closeSetting;
        
        Vector2 vec1 = new(closeSetting.position.x,closeSetting.position.x);
        Vector2 vec2 = new(closeSetting.position.y, closeSetting.position.y);
        
        rectTransform.anchorMin = vec1;
        rectTransform.anchorMax = vec2;
        rectTransform.pivot = closeSetting.position;

        _numToClose = closeSetting.numToClose;
        _resetTime = closeSetting.resetClickTime;

        rectTransform.gameObject.GetComponent<Image>().color = new Color(1, 1, 1,closeSetting.imageAlpha);
    }
    
    // 버튼 클릭횟수 초기화 로직
    private void Update()
    {
        if (!_enableCount) return;
        
        _currentTime += Time.deltaTime;

        if (_currentTime >= _resetTime)
        {
            _currentClick = 0;
            _currentTime = 0;
            _enableCount = false;
        }
    }

    // 버튼에 할당된 함수
    // 클릭했을때 시간 카운팅 시작
    public void Click()
    {
        _enableCount = true;
        ++_currentClick;

        if (_currentClick >= _numToClose)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}