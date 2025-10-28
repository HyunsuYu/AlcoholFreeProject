using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class AudioSettingControl : MonoBehaviour
{
    [Serializable] public struct UIs
    {
        public Slider Slider_MasterVolume;
        public Slider Slider_BGMVolume;
        public Slider Slider_SFXVolume;

        public SettingTestAudioPlayer SettingTestAudioPlayer;
    }


    [SerializeField] private UIs m_uis;


    #region Unity Callbacks
    public void TrySetMasterVolume()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.AudioSettingData curAudioSettingData = curSettingData.AudioSetting;

        curAudioSettingData.MasterVolume = m_uis.Slider_MasterVolume.value;

        curSettingData.AudioSetting = curAudioSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        m_uis.SettingTestAudioPlayer.Render();
    }
    public void TrySetBGMVolume()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.AudioSettingData curAudioSettingData = curSettingData.AudioSetting;

        curAudioSettingData.BGMVolume = m_uis.Slider_BGMVolume.value;

        curSettingData.AudioSetting = curAudioSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        m_uis.SettingTestAudioPlayer.Render();
    }
    public void TrySetSFXVolume()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.AudioSettingData curAudioSettingData = curSettingData.AudioSetting;

        curAudioSettingData.SFXVolume = m_uis.Slider_SFXVolume.value;

        curSettingData.AudioSetting = curAudioSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        m_uis.SettingTestAudioPlayer.Render();
    }
    #endregion

    internal void Render()
    {
        SettingData.AudioSettingData curAudioSettingData = SettingDataBuffer.Instance.SettingData.AudioSetting;

        m_uis.Slider_MasterVolume.SetValueWithoutNotify(curAudioSettingData.MasterVolume);
        m_uis.Slider_BGMVolume.SetValueWithoutNotify(curAudioSettingData.BGMVolume);
        m_uis.Slider_SFXVolume.SetValueWithoutNotify(curAudioSettingData.SFXVolume);

        m_uis.SettingTestAudioPlayer.Render();
    }
    internal void ResetPlayAudioSource()
    {
        m_uis.SettingTestAudioPlayer.ResetPlayAudioSource();
    }
}