using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Debug = DebugEx;

public class Testing2 : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetRenderer;
    private Texture2D[] frames;
    [SerializeField] private float fps = 30f;

    private Material _mat;
    private int _currentFrame;
    private float _timer;

    void Awake()
    {
        // .material은 인스턴스를 복제하니 주의
        _mat = targetRenderer.material;
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            frames = Resources.LoadAll<Texture2D>("Sample");
        }

        if (frames != null)
        {
            Debug.Log("Playing");
            _timer += Time.deltaTime;
            int newFrame = Mathf.FloorToInt(_timer * fps) % frames.Length;
            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;

            // 프로퍼티 이름 그대로 사용 (아래 예시는 "Texture2D")
            _mat.SetTexture("_Texture2D", frames[_currentFrame]);
        }
    }
}
