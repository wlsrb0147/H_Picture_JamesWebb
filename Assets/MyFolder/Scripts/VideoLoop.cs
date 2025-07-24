using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Debug = DebugEx;

public class VideoLoop : MonoBehaviour
{
    private VideoPlayer vp;

    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        vp.isLooping = false;
        vp.loopPointReached += VpOnloopPointReached;
    }

    private void VpOnloopPointReached(VideoPlayer source)
    {
        vp.frame = 0;
        vp.Play();
    }

   
}
