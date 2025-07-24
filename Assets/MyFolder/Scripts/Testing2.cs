using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = DebugEx;

public class Testing2 : MonoBehaviour
{
    private static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
    private static readonly int OPACITY = Shader.PropertyToID("_Opacity");
    private static readonly int COLOR = Shader.PropertyToID("_Color");
    private MeshRenderer targetRenderer;
    private List<Texture2D> frames;
    [SerializeField] private float fps = 30f;

    private AssetBundle _bundle;
    private byte _currentOpacity;

    [SerializeField] private bool dontAppear;
    [SerializeField] private bool dontDisappear;

    private Material _mat;
    private int _currentFrame;
    private float _timer;
    [SerializeField] private string folderName;

    [SerializeField] private float appearStart;
    [SerializeField] private float appearEnd;
    [SerializeField] private float disappearStart;
    [SerializeField] private float disappearEnd;
    
    void Awake()
    {
        // .material은 인스턴스를 복제하니 주의
        targetRenderer = GetComponent<MeshRenderer>();
        _mat = targetRenderer.material;
        Color32 col = new Color32(255, 255, 255, 0);
        _mat.SetColor(COLOR, col );
    }

    /*private IEnumerator Start()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles", folderName);
        var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleRequest;
        
        _bundle = bundleRequest.assetBundle;
        
        if (_bundle == null) {
            Debug.LogError("번들 로드 실패!");
            yield break;
        }
        
        var assetReq = _bundle.LoadAllAssetsAsync<Texture2D>();
        yield return assetReq;

        var textures = assetReq.allAssets
            .Cast<Texture2D>()
            .ToList();

        frames = textures;
        _mat.SetTexture(MAIN_TEX, frames[0]);

        _bundle.Unload(false);
        
        Debug.Log($"AssetBundle {folderName} Load Complete");
    }*/
    
    void Update()
    {
        int camFrame = GetAnimFrame.Instance.currentFrame;
        
        if (camFrame  >= appearStart && camFrame <= appearEnd)
        {
            if (Mathf.Approximately(appearStart, appearEnd))
            {
                if (!Mathf.Approximately(_currentOpacity, 231))
                {
                    _currentOpacity = 231;
                    Color32 col = new Color32(255,255,255,231);
                    _mat.SetColor(COLOR,  col);
                }
            }
            else
            {
                byte opa =(byte)((camFrame-appearStart)/ (appearEnd - appearStart) * 231);
                
                if (!Mathf.Approximately(_currentOpacity, opa))
                {
                    _currentOpacity = opa;
                    
                    Color32 col = new Color32(255,255,255,opa);
                    _mat.SetColor(COLOR, col);
                }
            }
        }
        else if (camFrame >= disappearStart && camFrame <= disappearEnd)
        {
            byte opa = (byte)((1 - (camFrame-disappearStart)/ (disappearEnd - disappearStart)) * 231);
                
            if (!Mathf.Approximately(_currentOpacity, opa))
            {
                _currentOpacity = opa;
                Color32 col = new Color32(255,255,255,opa);
                _mat.SetColor(COLOR,  col);
            }
        }
    }

    /*private void OnDisable()
    {
        if (_bundle != null)
        {
            _bundle.Unload(true);
            _bundle = null;
        }
        
        StopAllCoroutines();
    }*/
}
