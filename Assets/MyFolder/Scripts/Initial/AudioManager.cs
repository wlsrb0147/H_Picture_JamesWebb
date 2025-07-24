using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = DebugEx;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// 사용하는 오디오소스 이름을 Enum에 등록해야함
public enum AudioName
{
    Sample = 0,
    Sound = 1,
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject audioSourcePrefab;
    
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AudioManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
        private set => _instance = value;
    }
    
    [Header("AudioSourceParent와 VideoSourceParent에\n AudioSource 자식 추가하면 자동등록됨\nAudioSource는 이 스크립트에 Enum 등록 필요")]
    [Header("json에 음원파일 넣으면, AudioSource 생성함")]
    [Space]
    
    private AudioSource[] _audioSources;
    public AudioSetting[] AudioSetting { get; set; }
    
    private readonly Dictionary<string, AudioSource> _audioSourcesFromString = new ();
    private readonly Dictionary<AudioName, AudioSource> _audioSourcesDict = new ();
    
    private bool _dataSet;

    
    private void Awake()
    {
        if (_instance == null) 
        { 
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region 오디오 초기설정

    private void Start()
    {
        SetAudioSetting(JsonSaver.Instance.GetAudioSetting());
    }

    // 오디오 클립, 오디오, 비디오 볼륨 세팅
    private void SetAudioSetting(AudioSetting[] audioSetting)
    {
        if (!_dataSet)
        {
            BuildAudioDictionaries();
        }

        _dataSet = true;

        // 오디오 클립, 오디오 볼륨 설정
        StartCoroutine(LoadAndSetAudio(audioSetting));
    }
    
        
    // 딕셔너리 설정
    private void BuildAudioDictionaries()
    {


        foreach (AudioName audioEnum in Enum.GetValues(typeof(AudioName)))
        {
            // 1) 프리팹에서 인스턴스 생성
            GameObject go = Instantiate(audioSourcePrefab, transform);
            go.name = audioEnum.ToString();   // "Sample" 등

            // 2) AudioSource 컴포넌트 가져오기
            AudioSource source = go.GetComponent<AudioSource>();
            if (source == null)
            {
                Debug.LogError($"{go.name}에 AudioSource 컴포넌트가 없습니다.");
                continue;
            }

            source.loop = true;
            source.playOnAwake = false;
            
            // 3) 딕셔너리에 등록
            _audioSourcesDict[audioEnum] = source;
            _audioSourcesFromString[audioEnum.ToString()] = source;
        }
    }

    public void ChangeAudioLoop(AudioName audioEnum, bool loop)
    {
        if (_audioSourcesDict.TryGetValue(audioEnum, out AudioSource source))
        {
            source.loop = loop;
        }
        else
        {
            Debug.LogWarning($"Audio does not exist! : {audioEnum}");
        }
    }
    

    private IEnumerator LoadAndSetAudio(AudioSetting[] audioSetting)
    {
        AudioSetting = audioSetting;
        
        foreach (var setting in AudioSetting)
        {
            string[] type = setting.fileName.Split(".");
            
            if (_audioSourcesFromString.TryGetValue(type[0], out AudioSource source))
            {
                source.volume = setting.volume;
            }
            else
            {
                Debug.LogWarning($"{type[0]}이 없습니다");
                continue;
            }
            
            string path = Path.Combine(Application.streamingAssetsPath, "Audio" ,setting.fileName);
            
            using var request = UnityWebRequestMultimedia.GetAudioClip(path,AudioType.UNKNOWN);
                
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"{path} does not Exist!");
                continue;
            }
                
            source.volume = setting.volume;
            source.clip = DownloadHandlerAudioClip.GetContent(request);
        
        }
    }
    
    #endregion
    
    
    // 오디오 재생
    
    // PlayAudio(AudioName. ~~ ) 로 오디오 재생, bool값으로 재생.끄기 선택가능
    // PlayOneShotAudio로 PlayOneShot
    // 호출 : AudioManager.Instance.PlayAudio(AudioName.Sound,true);
    public void PlayAudio(AudioName audioName, bool playSound)
    {
        if (_audioSourcesDict.TryGetValue(audioName, out AudioSource source))
        {
            if (source.clip == null)
            {
                StartCoroutine(WaitAndPlay(source, playSound));
            }
            else
            {
                PlayAudio(source, playSound);
            }
        }
        else
        {
            Debug.LogWarning($"Audio does not exist! : {audioName}");
        }
    }
    
    private IEnumerator WaitAndPlay(AudioSource source, bool playSound)
    {
        float elapsed = 0f;
        // source.clip이 null인 동안, 그리고 타임아웃 전까지 매 프레임 대기
        while (source.clip == null && elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (source.clip == null)
        {
            Debug.LogWarning($"[{source}] 타임아웃, 2초 후에도 clip이 할당되지 않았습니다.");
            yield break;
        }

        PlayAudio(source, playSound);
    }

    private void PlayAudio(AudioSource source, bool playSound)
    {
        if (playSound)
            source.Play();
        else
            source.Stop();
    }
    


    // 오디오 한번 재생
    // 호출 : AudioManager.Instance.PlayOneShotAudio(AudioName.Sound);
    public void PlayOneShotAudio(AudioName audioName)
    {
        if (_audioSourcesDict.TryGetValue(audioName, out AudioSource source))
        {
            source.PlayOneShot(source.clip);
        }
        else
        {
            Debug.LogWarning($"Audio does not exist! : {audioName}");
        }
    }

    // 오디오 두개 이어서 재생
    private IEnumerator PlayContinuously(AudioSource firstAudio, AudioSource secondAudio)
    {
        firstAudio.Play();
        yield return new WaitForSeconds(firstAudio.clip.length);
        secondAudio.Play();
    }

    // 오디오 딜레이 주고 실행
    public void PlayAudioDelay(AudioName audioName, float delaySeconds)
    {
        StartCoroutine(DelayAudio(audioName, delaySeconds));
    }
    
    private IEnumerator DelayAudio(AudioName audioName, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        PlayAudio(audioName, true);
    }

    // 오디오 전부 정지
    public void StopAllAudio()
    {
        //sample.Stop();
    }

    // 오디오 소스 리턴
    public AudioSource GetAudioSource(AudioName audioName)
    {
        if (_audioSourcesDict.TryGetValue(audioName, out AudioSource source))
        {
            return source;
        }
        else
        {
            Debug.LogWarning($"Audio does not exist! : {audioName}");
            return null;
        }
    }
    
    // 오디오 Fade
    /*public void FadeAudio(AudioName audioName, float value, float fadeSeconds)
    {
        if (_audioSources.TryGetValue(audioName, out AudioSource source))
        {
            source.DOFade(value, fadeSeconds);
        }
        else
        {
            Debug.LogWarning($"Audio does not exist! : {audioName}");
        }
    }*/
}
