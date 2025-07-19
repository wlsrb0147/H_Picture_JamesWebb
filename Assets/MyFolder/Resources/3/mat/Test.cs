using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;
using UnityEngine.Video;

public class Test : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField] private Material videoMaterial;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        // 1. RenderTexture 생성

        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        //videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex"; // ✅ 정확함

        Material mat = videoPlayer.targetMaterialRenderer.GetComponent<Renderer>().material;
        Debug.Log("Shader: " + mat.shader.name);

        mat.SetColor("_Color", new Color(1f, 1f, 1f, 0.1f)); // ✅ 투명도 적용

        // 4. 재생
        videoPlayer.Play();
    }

}
