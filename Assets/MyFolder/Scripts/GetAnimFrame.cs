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
        
        // ① 재생 중인 클립
        AnimationClip playingClip = infos[0].clip;
        // ② clips[] 배열에서 인덱스 찾기
        int idx = Array.IndexOf(clips, playingClip);
        if (idx < 0) return;  // 혹시 배열에 없으면 종료

        // ③ 저장해 둔 길이 가져오기
        float length = _clipLength[idx];

        // ④ normalizedTime으로 현재 루프 내 시간 계산
        float normalized = state.normalizedTime;

        if (normalized >= 1)
        {
            return;   
        }
        float timeSec = normalized * length;

        // ⑤ 프레임 계산 (고정 frameRate 사용 시)
        currentFrame = Mathf.FloorToInt(timeSec * _frameRate);
    }
}
