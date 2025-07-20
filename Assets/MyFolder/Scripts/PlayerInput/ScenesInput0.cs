using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = DebugEx;


public class ScenesInput0 : RegistInputControl
{
    // 에디터에서 현재 입력 페이지를 보기위한 Serialize
    [SerializeField] private int currentPage;
    
    // 에디어테엇 현재 입력 인덱스를 보기위한 Serialize   
    [SerializeField] private int currentIndex;

    public override void ChangeIndex()
    {
        base.ChangeIndex();
        SetCurrentInput(currentPage, currentIndex+1);
    }
    
    public override void SetCurrentInput(int page, int index)
    {
        base.SetCurrentInput(page, index);
        currentPage =  page;
        currentIndex = index;
    }

    public override void ExecuteInput(Key key, bool performed)
    {
        base.ExecuteInput(key, performed);
        if (key == Key.None)
        {
            Debug.LogWarning("Invalid or empty key received.");
            return;
        }

        // 현재 페이지에 따라 1차 함수 분기
        Action<Key,bool> action = (currentPage, currentIndex ) switch
        {
            (0,0) => P0I0,
            (0, < 3) => P0I2,
            (0, >= 3) => P0I4,
            _ => DefaultInput // 정의되지 않은 페이지 입력은 아무것도 안함
        };

        action(key,performed);
    }
    
    private void DefaultInput(Key context, bool performed)
    {
        Debug.Log($"P{currentPage}I{currentIndex} : Default - {context}");
        
        switch (context)
        {
            case Key.UpArrow:
                Debug.Log($"{currentPage}{currentIndex} : UpArrow Pressed");
                break;
        }
    }

    #region page0

    private void P0I0(Key context, bool performed)
    {
        Debug.Log($"Page0 Index0 Selected : {context}");
        switch (context)
        {
            case Key.UpArrow:
            case Key.DownArrow:
                Debug.Log("Its Up/Down Arrow");
                break;
            case Key.Space:
                Debug.Log("Its Up/Space");
                SceneManager.LoadScene(1);
                break;
        }
    }
    private void P0I2(Key context, bool performed)
    {
        Debug.Log($"Page0 Index2 Selected : {context}");
        switch (context)
        {
            case Key.UpArrow:
            case Key.DownArrow:
                Debug.Log("Its Up/Down Arrow");
                break;
            case Key.Space:
                Debug.Log("Its Up/Space");
                break;
        }
    }
    private void P0I4(Key context, bool performed)
    {
        Debug.Log($"Page0 Index4 Selected : {context}");
        switch (context)
        {
            case Key.UpArrow:
            case Key.DownArrow:
                Debug.Log("Its Up/Down Arrow");
                break;
            case Key.Space:
                Debug.Log("Its Space ");
                break;
        }
    }

    #endregion

}
