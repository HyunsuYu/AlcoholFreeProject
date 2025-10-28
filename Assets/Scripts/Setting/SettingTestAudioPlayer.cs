using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingTestAudioPlayer : MonoBehaviour
{
    internal enum TestPlayType
    {
        BGM,
        CharacterThemeBGM,
        SFX
    }


    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioSource m_audioSource_BGM;

    [Header("Spectrum Graph")]
    [SerializeField] private GameObject m_spectrumGraphBar;
    [SerializeField] private Transform m_transform_SpectrumGraphBarParent;

    [SerializeField] private int m_spectrumGraphBarCount = 64;

    private List<Image> m_image_spectrumGraphBars = new List<Image>();

    [Header("Slider")]
    [SerializeField] private Slider m_slider_BGMTime;

    private float m_continueTimer = -1.0f;
    private bool m_bisAdjustTime = false;

    [Header("Control")]
    [SerializeField] private Image m_image_Pause;
    [SerializeField] private Image m_image_Continue;

    [SerializeField] private Color m_selectedColor;
    [SerializeField] private Color m_unselectedColor;

    [Header("Play Target Dropdown")]
    [SerializeField] private TMP_Dropdown m_dropdown_TestPlayBGMList;
    [SerializeField] private TMP_Dropdown m_dropdown_TestPlayCharacterThemeBGMList;
    [SerializeField] private TMP_Dropdown m_dropdown_TestPlaySFXList;

    private TestPlayType m_curTestPlayType = TestPlayType.BGM;

    //private BackgroundResourceDesc.BackgroundType m_curSelectedBackground = BackgroundResourceDesc.BackgroundType.HomeRoom;
    //private CharacterResourceDesc.CharacterListType m_curSelectedCharacter = CharacterResourceDesc.CharacterListType.Absinthe;
    //private MusicResourceDesc.SFXMusicType m_curSelectedSFX = MusicResourceDesc.SFXMusicType.Walking;


    public void Start()
    {
        for (int index = 0; index < m_spectrumGraphBarCount; index++)
        {
            m_image_spectrumGraphBars.Add(Instantiate(m_spectrumGraphBar, m_transform_SpectrumGraphBarParent).GetComponent<Image>());
            m_image_spectrumGraphBars[index].rectTransform.sizeDelta = new Vector2(m_image_spectrumGraphBars[index].rectTransform.sizeDelta.x, 0.0f);
        }
    }
    public void FixedUpdate()
    {
        switch(m_curTestPlayType)
        {
            case TestPlayType.BGM:
            case TestPlayType.CharacterThemeBGM:
                m_audioSource.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.BGMVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume;
                m_audioSource_BGM.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.BGMVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume;
                break;

            case TestPlayType.SFX:
                m_audioSource.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.SFXVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume;
                break;
        }

        if (m_continueTimer > 0.0f)
        {
            m_continueTimer -= Time.fixedDeltaTime;
        }
        else if (m_continueTimer <= 0.0f && m_bisAdjustTime)
        {
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;
            m_audioSource.Play();
            m_audioSource_BGM.Stop();
        }

        if (m_audioSource.isPlaying)
        {
            if (m_audioSource.isPlaying)
            {
                float[] spectrum = new float[m_spectrumGraphBarCount];
                m_audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
                for (int index = 0; index < m_spectrumGraphBarCount; index++)
                {
                    float height = Mathf.Clamp(spectrum[index] * 1000, 0, 100);
                    m_image_spectrumGraphBars[index].rectTransform.sizeDelta = new Vector2(m_image_spectrumGraphBars[index].rectTransform.sizeDelta.x, height);
                }
            }

            m_slider_BGMTime.SetValueWithoutNotify(m_audioSource.time / m_audioSource.clip.length);
        }
    }

    #region Unity Callbacks 
    public void TrySetBGMTime()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        m_audioSource.Stop();
        m_audioSource.time = m_slider_BGMTime.value * m_audioSource.clip.length;

        m_continueTimer = 0.5f;
        m_bisAdjustTime = true;
    }

    public void PauseBGM()
    {
        if (m_audioSource.isPlaying)
        {
            m_audioSource.Pause();
            m_audioSource_BGM.Play();
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;

            m_image_Pause.color = m_selectedColor;
            m_image_Continue.color = m_unselectedColor;
        }
    }
    public void ContinueBGM()
    {
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
            m_audioSource_BGM.Stop();
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;

            m_image_Pause.color = m_unselectedColor;
            m_image_Continue.color = m_selectedColor;
        }
    }
    public void StopBGM()
    {
        m_audioSource.Stop();
        m_audioSource_BGM.Play();
        m_continueTimer = -1.0f;
        m_bisAdjustTime = false;

        m_image_Pause.color = m_unselectedColor;
        m_image_Continue.color = m_unselectedColor;

        for (int index = 0; index < m_spectrumGraphBarCount; index++)
        {
            if(index < m_image_spectrumGraphBars.Count)
            {
                m_image_spectrumGraphBars[index].rectTransform.sizeDelta = new Vector2(m_image_spectrumGraphBars[index].rectTransform.sizeDelta.x, 0);
            }
        }
        m_slider_BGMTime.SetValueWithoutNotify(0.0f);
    }

    public void TrySetBGM()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        else if (m_dropdown_TestPlayBGMList.value - 1 < 0)
        {
            StopBGM();
            return;
        }

        StopBGM();

        m_curTestPlayType = TestPlayType.BGM;

        m_dropdown_TestPlayCharacterThemeBGMList.SetValueWithoutNotify(0);
        m_dropdown_TestPlaySFXList.SetValueWithoutNotify(0);

        m_audioSource.clip = ResourceDescBuffer.Instance.MusicResourceDesc.BackgroundMusicClips[m_dropdown_TestPlayBGMList.value - 1];
        //m_audioSource.clip = MusicDataBuffer.Instance.TryGetBackgroundMusic((BackgroundResourceDesc.BackgroundType)(m_dropdown_TestPlayBGMList.value - 1));

        ContinueBGM();
    }
    public void TrySetCharacterThemeBGM()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        else if (m_dropdown_TestPlayCharacterThemeBGMList.value - 1 < 0)
        {
            StopBGM();
            return;
        }

        StopBGM();

        m_curTestPlayType = TestPlayType.CharacterThemeBGM;

        m_dropdown_TestPlayBGMList.SetValueWithoutNotify(0);
        m_dropdown_TestPlaySFXList.SetValueWithoutNotify(0);

        m_audioSource.clip = ResourceDescBuffer.Instance.MusicResourceDesc.CharacterMusics[m_dropdown_TestPlayCharacterThemeBGMList.value - 1].CharacterMusicClips[0];
        //m_audioSource.clip = MusicDataBuffer.Instance.TryGetCharacterMusic((CharacterResourceDesc.CharacterListType)(m_dropdown_TestPlayCharacterThemeBGMList.value - 1), 0);

        ContinueBGM();
    }
    public void TrySetSFX()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        else if (m_dropdown_TestPlaySFXList.value - 1 < 0)
        {
            StopBGM();
            return;
        }

        StopBGM();

        m_curTestPlayType = TestPlayType.SFX;

        m_dropdown_TestPlayBGMList.SetValueWithoutNotify(0);
        m_dropdown_TestPlayCharacterThemeBGMList.SetValueWithoutNotify(0);

        m_audioSource.clip = ResourceDescBuffer.Instance.MusicResourceDesc.SFXMusicClips[m_dropdown_TestPlaySFXList.value - 1];
        //m_audioSource.clip = MusicDataBuffer.Instance.TryGetSFXMusic((MusicDataBuffer.SFXMusicType)(m_dropdown_TestPlaySFXList.value - 1));

        ContinueBGM();
    }
    #endregion

    internal void Render()
    {
        //m_audioSource.clip = ResourceDescBuffer.Instance.MusicResourceDesc.CharacterMusics[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].CharacterMusicClips[0];
        ////m_audioSource.clip = MusicDataBuffer.Instance.TryGetCharacterMusic(ChiefAlbumManager.Instance.CurSelectedCharacter, 0);
        //ContinueBGM();

        //m_audioSource.clip = null;
        //StopBGM();

        m_audioSource.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.BGMVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume;
        m_audioSource_BGM.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.BGMVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume;
    }
    internal void ResetPlayAudioSource()
    {
        if(m_audioSource.clip != null && m_audioSource.clip.name != "타이틀")
        {
            m_audioSource.clip = null;
            StopBGM();
        }
    }
}