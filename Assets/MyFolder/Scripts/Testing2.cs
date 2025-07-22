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
    private static readonly int MAIN_TEX = Shader.PropertyToID(MainTex);
    private static readonly int OPACITY = Shader.PropertyToID(Opacity);
    private MeshRenderer targetRenderer;
    private List<Texture2D> frames;
    [SerializeField] private float fps = 30f;

    private AssetBundle _bundle;
    private float _currentOpacity;

    [SerializeField] private bool dontAppear;
    [SerializeField] private bool dontDisappear;

    
    private const string MainTex = "_MainTex";
    private const string Opacity = "_Opacity";

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
        _mat.SetFloat(OPACITY,  0);
    }

    private IEnumerator Start()
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
    }
    
    void Update()
    {
        int camFrame = GetAnimFrame.Instance.currentFrame;
        
        if (camFrame  >= appearStart && camFrame <= appearEnd)
        {
            if (frames == null) return;

            if (Mathf.Approximately(appearStart, appearEnd))
            {
                if (!Mathf.Approximately(_currentOpacity, 1))
                {
                    _mat.SetFloat(OPACITY,  1);
                }
            }
            else
            {
                float opa = (camFrame-appearStart)/ (appearEnd - appearStart);
                
                if (!Mathf.Approximately(_currentOpacity, opa))
                {
                    _currentOpacity = opa;
                    _mat.SetFloat(OPACITY, (camFrame - appearStart) / (appearEnd - appearStart));
                }
            }
            
            _timer += Time.deltaTime;
            int newFrame = Mathf.FloorToInt(_timer * fps) % frames.Count;
            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;

            _mat.SetTexture(MAIN_TEX, frames[_currentFrame]);
        }
        else if (camFrame >= disappearStart && camFrame <= disappearEnd)
        {
            if (frames == null) return;
            
            float opa = 1 - (camFrame-disappearStart)/ (disappearEnd - disappearStart);
                
            if (!Mathf.Approximately(_currentOpacity, opa))
            {
                _currentOpacity = opa;
                _mat.SetFloat(OPACITY,  opa);
            }
            
            _timer += Time.deltaTime;
            int newFrame = Mathf.FloorToInt(_timer * fps) % frames.Count;
            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;

            _mat.SetTexture(MAIN_TEX, frames[_currentFrame]);
        }
    }

    private void OnDisable()
    {
        if (_bundle != null)
        {
            _bundle.Unload(true);
            _bundle = null;
        }
        
        StopAllCoroutines();
    }
}
