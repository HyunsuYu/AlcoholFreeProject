using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefAlbumManager : SingleTonForGameObject<ChiefAlbumManager>
{
    public enum FromPageType
    {
        Title,
        RecipeSelection,
        MiniGame
    }


    [SerializeField] private RectTransform m_rectTransform_MainContents;
    [SerializeField] private RectTransform m_rectTransform_BGMContents;
    [SerializeField] private RectTransform m_rectTransform_Photos;
    [SerializeField] private RectTransform m_rectTransform_Zoom;

    [SerializeField] private AudioSource m_audioSource_CharacterBGM;

    [SerializeField] private AlbumBGMPlayer m_albumBGMPlayer;
    [SerializeField] private AlbumSignControl m_albumSignControl;
    [SerializeField] private AlbumPhotoControl m_albumPhotoControl;

    private CharacterResourceDesc.CharacterListType m_characterListType = CharacterResourceDesc.CharacterListType.Absinthe;

    private static FromPageType m_fromPageType = FromPageType.Title;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        RenderAll();

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;

        m_audioSource_CharacterBGM.volume = curSettingData.AudioSetting.MasterVolume * curSettingData.AudioSetting.BGMVolume;

        switch (curSettingData.ScreenSetting.CurResolution)
        {
            case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_MainContents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_rectTransform_BGMContents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_rectTransform_Photos.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_rectTransform_Zoom.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_MainContents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                m_rectTransform_BGMContents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                m_rectTransform_Photos.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                m_rectTransform_Zoom.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_MainContents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                m_rectTransform_BGMContents.localScale = new Vector3(2.0f, 2.0f, 1.0f);
                m_rectTransform_Photos.localScale = new Vector3(2.0f, 2.0f, 1.0f);
                m_rectTransform_Zoom.localScale = new Vector3(2.0f, 2.0f, 1.0f);
                break;
        }
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;

        GameObject bgm = GameObject.Find("BGM");
        if(bgm != null)
        {
            Destroy(bgm);
        }
    }

    internal static FromPageType FromPage
    {
        set
        {
            m_fromPageType = value;
        }
    }

    internal CharacterResourceDesc.CharacterListType CurSelectedCharacter
    {
        get
        {
            return m_characterListType;
        }
        set
        {
            m_characterListType = value;
        }
    }

    #region Unity Callbacks
    public void Back2Page()
    {
        switch (m_fromPageType)
        {
            case FromPageType.Title:
                SceneManager.LoadScene("Title");
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
        m_albumBGMPlayer.Render();
        m_albumSignControl.Render();
        m_albumPhotoControl.Render();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}