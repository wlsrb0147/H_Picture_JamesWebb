using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;

public class GetAnimFrame : MonoBehaviour
{
    public static GetAnimFrame Instance;
    private static readonly int END = Animator.StringToHash(End);

    private Animator anim;
    
    public int currentFrame;
    
    [SerializeField] private AnimationClip[] clips;

    private const string End = "End";

    private float[] _clipLength;
    private float _frameRate = 30;
    
    public void GoTitle()
    {
        PageController.Instance.GoTitle();
        anim.SetTrigger(END);
    }
    
    private void Awake()
    {
        Instance = this;
        anim = GetComponent<Animator>();

        _clipLength = new float[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            _clipLength[i] = clips[i].length;
        }
    }

    private void Update()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        var infos = anim.GetCurrentAnimatorClipInfo(0);
        if (infos.Length == 0) return;
        
        AnimationClip playingClip = infos[0].clip;
        int idx = Array.IndexOf(clips, playingClip);
        if (idx < 0) return;

        float length = _clipLength[idx];

        float normalized = state.normalizedTime;

        if (normalized >= 1)
        {
            return;   
        }
        float timeSec = normalized * length;

        currentFrame = Mathf.FloorToInt(timeSec * _frameRate);
    }
}
