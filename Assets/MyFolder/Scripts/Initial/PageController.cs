using System;
using UnityEngine;
using Debug = DebugEx;

public class PageController : MonoBehaviour
{
    public static PageController Instance;

    [SerializeField] public GameObject[] pages;
    private InputManager _inputManager;
    [SerializeField] private VideoManager videoManager;
    
    private int _currentPage;

    private void Start()
    {
        _inputManager = InputManager.Instance;
        GoTitle();
    }

    // 유효한 페이지만 유지하도록 함
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (pages.Length == 0) return;

            if (_currentPage == value) return;
            
            _pastPage = _currentPage;
            
            if (value >= pages.Length)
            {
                _currentPage = value % pages.Length;
            }
            else if (value < 0)
            {
                _currentPage = value + pages.Length;
            }
            else
            {
                _currentPage = value;
            }
        }
    }

    private int _pastPage;

    void Awake()
    {
        Instance = this;
    }

    // 페이지가 늦게 닫혀야할 때를 위한 페이지 닫기
    public void CloseSinglePage(int x)
    {
        Debug.Log("Close page index : " + x);
        pages[x].SetActive(false);
    }

    // 다음페이지 열기
    public void LoadNextPage()
    {
        ++CurrentPage;
        OpenPage(CurrentPage);             
        CloseSinglePage(_pastPage);  
    }
    
    public void GoTitle()
    {
        CurrentPage = 0;
        OpenPage(0);

        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }
    
    public void OpenPage(int pageNum)
    {
        // 페이지를 열었을 때, inputManager의 page와 index 갱신
        _inputManager.SetCurrentIndex(pageNum,0);
        Debug.Log("OpenPage : " + pageNum);

        if (pageNum < pages.Length)
        {
            pages[pageNum].SetActive(true);
            CurrentPage = pageNum;
        }
    }
}