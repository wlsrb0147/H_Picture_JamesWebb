using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Debug = DebugEx;


public class ScenesInput0 : RegistInputControl
{
    private static readonly int SPEED = Animator.StringToHash(Speed);

    // 에디터에서 현재 입력 페이지를 보기위한 Serialize
    [SerializeField] private int currentPage;
    
    // 에디어테엇 현재 입력 인덱스를 보기위한 Serialize   
    [SerializeField] private int currentIndex;

    [SerializeField] private Animator anim;

    [SerializeField] private GameObject cam;
    
    private Vector3[] positions = new Vector3[4];
    private Quaternion[] rotations = new Quaternion[4];
    
    [SerializeField] private VideoPlayer[] videoPlayers;
    
    private const string Speed  = "Speed";

    
    public override void ChangeIndex()
    {
        base.ChangeIndex();
        SetCurrentInput(currentPage, currentIndex+1);
    }

    private void ChangeIndex2(int index)
    {
        SetCurrentInput(currentPage, index);
    }

    public override void Start()
    {
        base.Start();
        SetCurrentInput(0,0);

        positions[0] = new Vector3(-0.719999433f, -1.63998413f, 28.1000175f);
        positions[1] = new Vector3(-48.7280464f, 8.02865314f, -26.6154537f);
        positions[2] = new Vector3(-104.07753f, 1.58276498f, -92.6162796f);
        positions[3] = new Vector3(-167.413086f, 0.70109117f, -25.0587044f);
        
        rotations[0] = new Quaternion(0,1,0,0);
        rotations[1] = new Quaternion(-0.0350527689f,0.966171861f,-0.0921914652f,-0.238293961f);
        rotations[2] = new Quaternion(0.0480275787f,0.79107815f,0.0809217617f,-0.604433894f);
        rotations[3] = new Quaternion(-0.0506136753f,0.676819682f,-0.0319978818f,-0.733709455f);
    }
    
    public override void SetCurrentInput(int page, int index)
    {
        base.SetCurrentInput(page, index);
        currentPage =  page;
        currentIndex = index;

        switch (page, index)
        {
            case (0,0):
                DataSaver.Instance.CurrentSelected = 0;
                foreach (var v in videoPlayers)
                {
                    if (v.isPlaying)
                    {
                        v.Stop();
                    }
                }
                foreach (var v in videoPlayers)
                {
                    if (!v.isPrepared)
                    {
                        v.Prepare();
                    }
                }
                break;
            case (1,0):
                anim.SetFloat(SPEED,0); 
                if (DataSaver.Instance.CurrentSelected == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        videoPlayers[i].Play();
                    }
                }
                else if (DataSaver.Instance.CurrentSelected == 3)
                {
                    videoPlayers[^1].Play();
                }
                
                break;

        }
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
            (1, 0) => P1I0,
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
        if (!performed)
        {
            return;
        }
        
        Debug.Log($"Page0 Index0 Selected : {context}");
        switch (context)
        {
            case Key.LeftArrow:
                --DataSaver.Instance.CurrentSelected;
                break;
            case Key.RightArrow:
                ++DataSaver.Instance.CurrentSelected;
                break;
            case Key.Space:
                ChangeIndex2(DataSaver.Instance.CurrentSelected);
                cam.transform.position = positions[DataSaver.Instance.CurrentSelected];
                cam.transform.rotation = rotations[DataSaver.Instance.CurrentSelected];
                anim.SetTrigger($"Select{DataSaver.Instance.CurrentSelected}");
                PageController.Instance.LoadNextPage();
                break;
        }
    }

    private void P1I0(Key context, bool performed)
    {
        Debug.Log($"Page1 Index0 Selected : {context}");
        switch (context)
        {
            case Key.UpArrow:
                float val = performed ? 1f : 0f;
                anim.SetFloat(SPEED,val); 
                break;
            case Key.DownArrow:
                float val2 = performed ? -1f : 0f;
                anim.SetFloat(SPEED,val2); 
                break;
        }
    }

    #endregion

}
