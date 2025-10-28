using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static SettingPageControl;


public sealed class MemoriesPageControl : MonoBehaviour
{
    public enum MemoriesPageType : byte
    {
        Story,
        CutScene,
        MiniGame
    }


    [SerializeField] private GameObject[] m_layout_MemoriesPages;
    [SerializeField] private GameObject[] m_gameObject_SelectedStates;

    private MemoriesPageType m_curMemoriesPageType = MemoriesPageType.Story;


    public void Start()
    {
        OpenStoryMemoriesPage();
    }

    #region Unity Callbacks
    public void OpenStoryMemoriesPage()
    {
        m_layout_MemoriesPages[0].SetActive(true);
        m_layout_MemoriesPages[1].SetActive(false);
        m_layout_MemoriesPages[2].SetActive(false);

        m_gameObject_SelectedStates[0].SetActive(true);
        m_gameObject_SelectedStates[1].SetActive(false);
        m_gameObject_SelectedStates[2].SetActive(false);

        m_curMemoriesPageType = MemoriesPageType.Story;

        ChiefMemoriesManager.Instance.StoryMemoriesControl.Render();
    }
    public void OpenCutSceneMemoreisPage()
    {
        m_layout_MemoriesPages[0].SetActive(false);
        m_layout_MemoriesPages[1].SetActive(true);
        m_layout_MemoriesPages[2].SetActive(false);

        m_gameObject_SelectedStates[0].SetActive(false);
        m_gameObject_SelectedStates[1].SetActive(true);
        m_gameObject_SelectedStates[2].SetActive(false);

        m_curMemoriesPageType = MemoriesPageType.CutScene;

        ChiefMemoriesManager.Instance.CutSceneMemoriesControl.Render();
    }
    public void OpenMiniGameMemoriesPage()
    {
        m_layout_MemoriesPages[0].SetActive(false);
        m_layout_MemoriesPages[1].SetActive(false);
        m_layout_MemoriesPages[2].SetActive(true);

        m_gameObject_SelectedStates[0].SetActive(false);
        m_gameObject_SelectedStates[1].SetActive(false);
        m_gameObject_SelectedStates[2].SetActive(true);

        m_curMemoriesPageType = MemoriesPageType.MiniGame;

        ChiefMemoriesManager.Instance.MiniGameMemoriesControl.Render();
    }

    public void OpenLeftPageContent()
    {
        switch(m_curMemoriesPageType)
        {
            case MemoriesPageType.Story:
                ChiefMemoriesManager.Instance.StoryMemoriesControl.OpenLeftPageContent();
                break;

            case MemoriesPageType.CutScene:
                ChiefMemoriesManager.Instance.CutSceneMemoriesControl.OpenLeftPageContent();
                break;

            case MemoriesPageType.MiniGame:
                ChiefMemoriesManager.Instance.MiniGameMemoriesControl.OpenLeftPageContent();
                break;
        }
    }
    public void OpenRightPageContent()
    {
        switch (m_curMemoriesPageType)
        {
            case MemoriesPageType.Story:
                ChiefMemoriesManager.Instance.StoryMemoriesControl.OpenRightPageContent();
                break;

            case MemoriesPageType.CutScene:
                ChiefMemoriesManager.Instance.CutSceneMemoriesControl.OpenRightPageContent();
                break;

            case MemoriesPageType.MiniGame:
                ChiefMemoriesManager.Instance.MiniGameMemoriesControl.OpenRightPageContent();
                break;
        }
    }
    #endregion
}