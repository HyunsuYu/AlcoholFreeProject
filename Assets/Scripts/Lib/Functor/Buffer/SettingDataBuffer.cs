using CommonUtilLib.ThreadSafe;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SettingDataBuffer : SingleTonForGameObject<SettingDataBuffer>
{
    private SettingData? m_settingData;


    public void Awake()
    {
        SetInstance(this);

        if (!TryLoadData())
        {
            Debug.LogError("Failed to load plot data.");
        }
    }

    internal SettingData SettingData
    {
        get
        {
            return m_settingData.Value;
        }
        set
        {
            m_settingData = value;
        }
    }

    internal SettingData DefailtSettingData
    {
        get
        {
            return new SettingData()
            {
                ScreenSetting = new SettingData.ScreenSettingData()
                {
                    CurResolution = global::SettingData.ScreenSettingData.Resolution.FHD_1920x1080,
                    BIsFullScreen = true,
                    BIsVSync = false,
                    ConversationBackgroundOpacity = 0.5f
                },
                ControlSetting = new SettingData.ControlSettingData()
                {
                    BIsCheckDialog = true,
                    FontSize = 30.0f,
                    FontSpeed = 5.0f,
                    BIsAutoPlay = false,
                    BIsAutoSave = false
                },
                AudioSetting = new SettingData.AudioSettingData()
                {
                    MasterVolume = 1.0f,
                    BGMVolume = 1.0f,
                    SFXVolume = 1.0f
                }
            };
        }
    }

    internal static string SettingDataFilePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "SettingData.json");
        }
    }

    internal bool TryLoadData()
    {
        if (!File.Exists(SettingDataFilePath))
        {
            m_settingData = DefailtSettingData;

            TrySaveData();

            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(SettingDataFilePath);
            m_settingData = JsonConvert.DeserializeObject<SettingData>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load setting data from {SettingDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }
    internal bool TrySaveData()
    {
        if (m_settingData == null)
        {
            Debug.LogError("Setting data is null, cannot save.");
            return false;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_settingData);

            string directory = Path.GetDirectoryName(SettingDataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(SettingDataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save setting data to {SettingDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}