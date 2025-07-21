using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;

public class NamDither : MonoBehaviour
{
    private static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
    private static readonly int OPACITY = Shader.PropertyToID("_Opacity");

    [SerializeField] private MeshRenderer targetRenderer;
    private Camera cam;
    private Material _mat;

    [SerializeField] private Animator anim;
    [SerializeField] private float start;
    [SerializeField] private float end;
    
    private void Awake()
    {
        cam = Camera.main;
        // .material은 인스턴스를 복제하니 주의
        _mat = targetRenderer.material;
    }


    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];
        float clipLength = clip.length;   
        float frameRate  = clip.frameRate;     
            
        float timeInCurrentLoop = (state.normalizedTime % 1) * clipLength;
        int camFrame = Mathf.FloorToInt(timeInCurrentLoop * frameRate);
        
        if (camFrame  >= start && camFrame <= end)
        {
            _mat.SetFloat(OPACITY,  1 - (camFrame-start)/ (end - start));
        }
    }
}
