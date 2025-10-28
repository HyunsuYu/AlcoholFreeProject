using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class ScreenFadeTransition : MonoBehaviour
{
    [SerializeField] private Image m_image_Fade;

    private bool m_bisStart = false;
    private float m_timer = 0.0f;

    private float m_timeDuration = -1.0f;

    private bool m_bisEnterScreen = false;

    private AudioSource m_bgmAudioSource = null;
    private AudioSource m_sfxAudioSource = null;

    private SettingData? m_curSettingData = null;


    public void FixedUpdate()
    {
        if (m_bisStart)
        {
            m_timer += Time.fixedDeltaTime;

            if (m_timeDuration < m_timer)
            {
                m_bisStart = false;
                m_timer = 0.0f;

                m_bgmAudioSource = null;
                m_sfxAudioSource = null;

                m_curSettingData = null;

                m_image_Fade.gameObject.SetActive(false);
            }
            else if (m_timeDuration / 2.0f < m_timer)
            {
                float curProgress = 1.0f - ((m_timer - (m_timeDuration / 2.0f)) / (m_timeDuration / 2.0f));

                m_image_Fade.color = new Color(0.0f, 0.0f, 0.0f, curProgress);

                m_bgmAudioSource.volume = (m_curSettingData.Value.AudioSetting.MasterVolume * m_curSettingData.Value.AudioSetting.BGMVolume) * (1.0f - curProgress);
                m_sfxAudioSource.volume = (m_curSettingData.Value.AudioSetting.MasterVolume * m_curSettingData.Value.AudioSetting.SFXVolume) * (1.0f - curProgress);
            }
            else
            {
                float curProgress = m_timer / (m_timeDuration / 2.0f);
                m_image_Fade.color = new Color(0.0f, 0.0f, 0.0f, curProgress);

                m_bgmAudioSource.volume = (m_curSettingData.Value.AudioSetting.MasterVolume * m_curSettingData.Value.AudioSetting.BGMVolume) * (1.0f - curProgress);
                m_sfxAudioSource.volume = (m_curSettingData.Value.AudioSetting.MasterVolume * m_curSettingData.Value.AudioSetting.SFXVolume) * (1.0f - curProgress);
            }
        }
    }

    public void StartTransition(in float timeDuration, in bool bisEnterScreen = false, in AudioSource bgmAudioSources = null, in AudioSource sfxAudioSources = null)
    {
        m_timeDuration = timeDuration;
        m_bisEnterScreen = bisEnterScreen;
        if (m_bisEnterScreen)
        {
            m_timer = m_timeDuration / 2.0f;
        }
        else
        {
            m_timer = 0.0f;
        }

        m_bgmAudioSource = bgmAudioSources;
        m_sfxAudioSource = sfxAudioSources;

        m_curSettingData = SettingDataBuffer.Instance.SettingData;

        m_image_Fade.gameObject.SetActive(true);

        m_bisStart = true;
    }
}