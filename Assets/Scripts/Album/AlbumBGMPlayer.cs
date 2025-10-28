using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class AlbumBGMPlayer : MonoBehaviour
{
    [Serializable] public struct ColorSet
    {
        public Color SliderBackground;
        public Color SliderPassedColor;

        public Color SpectrumBarColor;

        public Color SelectedBtnColor;
        public Color UnselectedBtnColor;
    }

    [SerializeField] private AudioSource m_audioSource;

    [SerializeField] private ColorSet[] m_colorSets;

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

    [Header("Related with ColorSet")]
    [SerializeField] private Image m_image_SliderBackground;
    [SerializeField] private Image m_image_SliderPassedFill;


    public void Awake()
    {
        for (int index = 0; index < m_spectrumGraphBarCount; index++)
        {
            m_image_spectrumGraphBars.Add(Instantiate(m_spectrumGraphBar, m_transform_SpectrumGraphBarParent).GetComponent<Image>());
            m_image_spectrumGraphBars[index].rectTransform.sizeDelta = new Vector2(m_image_spectrumGraphBars[index].rectTransform.sizeDelta.x, 0.0f);
        }
    }
    public void FixedUpdate()
    {
        if(m_continueTimer > 0.0f)
        {
            m_continueTimer -= Time.fixedDeltaTime;
        }
        else if(m_continueTimer <= 0.0f && m_bisAdjustTime)
        {
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;
            m_audioSource.Play();
        }

        if(m_audioSource.isPlaying)
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
        if(!Application.isPlaying)
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
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;

            m_image_Pause.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].SelectedBtnColor;
            m_image_Continue.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;
        }
    }
    public void ContinueBGM()
    {
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.Play();
            m_continueTimer = -1.0f;
            m_bisAdjustTime = false;

            m_image_Pause.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;
            m_image_Continue.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].SelectedBtnColor;
        }
    }
    public void StopBGM()
    {
        m_audioSource.Stop();
        m_continueTimer = -1.0f;
        m_bisAdjustTime = false;

        m_image_Pause.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;
        m_image_Continue.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;

        for (int index = 0; index < m_spectrumGraphBarCount; index++)
        {
            m_image_spectrumGraphBars[index].rectTransform.sizeDelta = new Vector2(m_image_spectrumGraphBars[index].rectTransform.sizeDelta.x, 0);
        }
        m_slider_BGMTime.SetValueWithoutNotify(0.0f);
    }
    #endregion

    internal void Render()
    {
        m_image_SliderBackground.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].SliderBackground;
        m_image_SliderPassedFill.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].SliderPassedColor;

        for (int index = 0; index < m_spectrumGraphBarCount; index++)
        {
            m_image_spectrumGraphBars[index].color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter - (int)CharacterResourceDesc.CharacterListType.Absinthe].SpectrumBarColor;
        }

        m_image_Pause.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;
        m_image_Continue.color = m_colorSets[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].UnselectedBtnColor;

        m_audioSource.clip = ResourceDescBuffer.Instance.MusicResourceDesc.CharacterMusics[(int)ChiefAlbumManager.Instance.CurSelectedCharacter].CharacterMusicClips[0];
        //m_audioSource.clip = MusicDataBuffer.Instance.TryGetCharacterMusic( (CharacterResourceDesc.CharacterListType)ChiefAlbumManager.Instance.CurSelectedCharacter, 0);
        ContinueBGM();
    }
}