using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefSettingManager : SingleTonForGameObject<ChiefSettingManager>
{
    public enum FromPageType
    {
        Title,
        Conversation,
        RecipeSelection,
        MiniGame
    }

    [SerializeField] private RectTransform m_rectTransform_Contents;

    [SerializeField] private ScreenSettingControl m_screenSettingControl;
    [SerializeField] private ControlSettingControl m_controlSettingControl;
    [SerializeField] private AudioSettingControl m_audioSettingControl;

    [SerializeField] private GameObject m_layout_ResetPopup;

    private static FromPageType m_fromPageType = FromPageType.Title;
    private static Guid m_fromSceneID;
    private static int m_fromFrameIndex;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        RenderAll();

        //GameObject bgm = GameObject.Find("BGM");
        //if(bgm != null)
        //{
        //    Destroy(bgm);
        //}
    }

    internal static FromPageType FromPage
    {
        set
        {
            m_fromPageType = value;
        }
    }
    internal static Guid FromSceneID
    {
        set
        {
            m_fromSceneID = value;
        }
    }
    internal static int FromFrameIndex
    {
        set
        {
            m_fromFrameIndex = value;
        }
    }

    internal ScreenSettingControl ScreenSettingControl
    {
        get
        {
            return m_screenSettingControl;
        }
    }
    internal ControlSettingControl ControlSettingControl
    {
        get
        {
            return m_controlSettingControl;
        }
    }
    internal AudioSettingControl AudioSettingControl
    {
        get
        {
            return m_audioSettingControl;
        }
    }

    #region Unity Callbacks
    public void OpenResetPopup()
    {
        m_layout_ResetPopup.SetActive(true);
    }
    public void CloseResetPopup()
    {
        m_layout_ResetPopup.SetActive(false);
    }
    public void ResetAllSettings()
    {
        SettingDataBuffer.Instance.SettingData = SettingDataBuffer.Instance.DefailtSettingData;
        SettingDataBuffer.Instance.TrySaveData();

        m_layout_ResetPopup.SetActive(false);

        RenderAll();
    }
    public void Back2Page()
    {
        switch(m_fromPageType)
        {
            case FromPageType.Title:
                SceneManager.LoadScene("Title");
                break;

            case FromPageType.Conversation:
                SceneManager.LoadScene("Conversation");
                NoonCanvasControl.Request2Render(m_fromSceneID, m_fromFrameIndex);
                Invoke(nameof(RenderConversationScreen), 1.0f);
                break;

            case FromPageType.RecipeSelection:
                SceneManager.LoadScene("RecipeSelection");
                break;

            case FromPageType.MiniGame:
                SceneManager.LoadScene("MiniGame");
                break;
        }
    }
    #endregion

    internal void RenderAll()
    {
        m_screenSettingControl.Render();
        m_controlSettingControl.Render();
        m_audioSettingControl.Render();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }

    private void RenderConversationScreen()
    {
        NoonCanvasControl.Instance.Render();
    }
}