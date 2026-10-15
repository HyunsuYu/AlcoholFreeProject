using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public sealed class TitleButtonControl : MonoBehaviour
{
    [SerializeField] private RectTransform m_rectTransform_Contents;

    [SerializeField] private AudioSource m_audioSource_BGM;


    public void Start()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        switch (curSettingData.ScreenSetting.CurResolution)
        {
            case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;

        m_audioSource_BGM.volume = curSettingData.AudioSetting.BGMVolume * curSettingData.AudioSetting.MasterVolume;

        GameObject miniGameBGMObject = GameObject.Find("OldBGM");
        if (miniGameBGMObject != null)
        {
            Destroy(m_audioSource_BGM.gameObject);
            miniGameBGMObject.name = "BGM";
        }
    }

    #region Unity Callbacks
    public void StartNewGame()
    {
        //MusicDataBuffer.Instance.PreloadMusicDatas();

        GameObject targetBGMObject = GameObject.Find("BGM");
        if (targetBGMObject == null)
        {
            targetBGMObject = m_audioSource_BGM.gameObject;
        }

        DontDestroyOnLoad(targetBGMObject);
        SceneManager.LoadScene("CharacterSelection");
    }
    public void ContinueGame()
    {

    }
    public void OpenAlbum()
    {
        //MusicDataBuffer.Instance.PreloadMusicDatas();
        SceneManager.LoadScene("Album");
    }
    public void OpenMemories()
    {
        SceneManager.LoadScene("Memories");
    }
    public void OpenSetting()
    {
        //MusicDataBuffer.Instance.PreloadMusicDatas();
        ChiefSettingManager.FromPage = ChiefSettingManager.FromPageType.Title;
        SceneManager.LoadScene("Setting");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenScreenShots()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "ScreenShots");

        if (System.IO.Directory.Exists(path))
        {
            System.Diagnostics.Process.Start(path);
        }
        else
        {
            Debug.LogWarning("ScreenShots directory does not exist: " + path);
        }
    }
    #endregion
}