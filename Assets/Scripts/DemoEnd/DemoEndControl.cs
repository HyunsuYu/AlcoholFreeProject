using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public sealed class DemoEndControl : MonoBehaviour
{
    [SerializeField] private RectTransform m_rectTransform_Contents;
    [SerializeField] private AudioSource m_audioSource_BGM;

    [SerializeField] private TMP_Text m_text_ClearMsg;


    public void Awake()
    {
        string name = string.Empty;
        switch (ChiefCharacterSelectionManager.CurSelectedCharacterType)
        {
            case CharacterResourceDesc.CharacterListType.Absinthe:
                name = "마타 하리";
                break;

            case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                name = "시노노메 하야세";
                break;

            case CharacterResourceDesc.CharacterListType.Hpnotiq:
                name = "타치바나 헬레나";
                break;

            case CharacterResourceDesc.CharacterListType.Kahlua:
                name = "강로아";
                break;

            case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                name = "카넬리안 메디치";
                break;
        }

        m_text_ClearMsg.text = name + " 루트\n데모 플레이 클리어를 축하합니다";
    }
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
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TempSaveBuffer.ClearTempSaveDatas();
            SceneManager.LoadScene("Title");
        }
    }
}