using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Debug = DebugEx;

public class Testing2 : MonoBehaviour
{
    private static readonly int MAIN_TEX = Shader.PropertyToID(MainTex);
    private static readonly int OPACITY = Shader.PropertyToID(Opacity);
    [SerializeField] private MeshRenderer targetRenderer;
    [SerializeField] private MeshRenderer targetRenderer2;
    private Texture2D[] frames;
    [SerializeField] private float fps = 30f;

    [SerializeField] private Animator anim;

    private Camera cam;
    
    private const string MainTex = "_MainTex";
    private const string Opacity = "_Opacity";

    private Material _mat;
    private int _currentFrame;
    private float _timer;

    [SerializeField] private float start;
    [SerializeField] private float end;
    
    void Awake()
    {
        cam = Camera.main;
        // .material은 인스턴스를 복제하니 주의
        _mat = targetRenderer.material;
        targetRenderer2.material = _mat;
    }

    private void Start()
    {
        frames = Resources.LoadAll<Texture2D>("Sample");
    }
    
    void Update()
    {
        

        if (frames != null)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

            AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];
            float clipLength = clip.length;           // 초 단위 길이
            float frameRate  = clip.frameRate;        // 초당 프레임 수
            
            Debug.Log(clipLength);
            Debug.Log(frameRate);
            
            float timeInCurrentLoop = (state.normalizedTime % 1) * clipLength;
            int camFrame = Mathf.FloorToInt(timeInCurrentLoop * frameRate);


            // 180때 0
            // 60때 1

         
            
            if (camFrame  >= start && camFrame <= end)
            {
                _mat.SetFloat(OPACITY,  1 - (camFrame-start)/ (end - start));
            }
            
            _timer += Time.deltaTime;
            int newFrame = Mathf.FloorToInt(_timer * fps) % frames.Length;
            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;

            // 프로퍼티 이름 그대로 사용 (아래 예시는 "Texture2D")
            _mat.SetTexture(MAIN_TEX, frames[_currentFrame]);
            
        }
    }
}
