using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class SettingPageControl : MonoBehaviour
{
    public enum SettingPageType : byte
    {
        Screen,
        Control,
        Audio
    }


    [SerializeField] private GameObject[] m_layout_SettingPages;
    [SerializeField] private GameObject[] m_gameObject_SelectedStates;

    private SettingPageType m_curSettingPageType = SettingPageType.Screen;


    public void Start()
    {
        OpenScreenSettingPage();
    }

    #region Unity Callbacks
    public void OpenScreenSettingPage()
    {
        if(m_curSettingPageType == SettingPageType.Audio)
        {
            ChiefSettingManager.Instance.AudioSettingControl.ResetPlayAudioSource();
        }

        m_layout_SettingPages[0].SetActive(true);
        m_layout_SettingPages[1].SetActive(false);
        m_layout_SettingPages[2].SetActive(false);

        m_gameObject_SelectedStates[0].SetActive(true);
        m_gameObject_SelectedStates[1].SetActive(false);
        m_gameObject_SelectedStates[2].SetActive(false);

        m_curSettingPageType = SettingPageType.Screen;

        ChiefSettingManager.Instance.RenderAll();
    }
    public void OpenControlSettingPage()
    {
        if (m_curSettingPageType == SettingPageType.Audio)
        {
            ChiefSettingManager.Instance.AudioSettingControl.ResetPlayAudioSource();
        }

        m_layout_SettingPages[0].SetActive(false);
        m_layout_SettingPages[1].SetActive(true);
        m_layout_SettingPages[2].SetActive(false);

        m_gameObject_SelectedStates[0].SetActive(false);
        m_gameObject_SelectedStates[1].SetActive(true);
        m_gameObject_SelectedStates[2].SetActive(false);

        m_curSettingPageType = SettingPageType.Control;

        ChiefSettingManager.Instance.RenderAll();
    }
    public void OpenAudioSettingPage()
    {
        m_layout_SettingPages[0].SetActive(false);
        m_layout_SettingPages[1].SetActive(false);
        m_layout_SettingPages[2].SetActive(true);

        m_gameObject_SelectedStates[0].SetActive(false);
        m_gameObject_SelectedStates[1].SetActive(false);
        m_gameObject_SelectedStates[2].SetActive(true);

        m_curSettingPageType = SettingPageType.Audio;

        ChiefSettingManager.Instance.RenderAll();
    }
    #endregion
}