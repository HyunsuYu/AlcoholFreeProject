using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public struct SettingData
{
    public struct ScreenSettingData
    {
        public enum Resolution
        {
            FHD_1920x1080,
            QHD_2560x1440,
            UHD_3840x2160,
        }


        public Resolution CurResolution;
        public bool BIsFullScreen;
        public bool BIsVSync;
        public float ConversationBackgroundOpacity;
    }
    public struct ControlSettingData
    {
        public bool BIsCheckDialog;
        public float FontSize;
        public float FontSpeed;
        public bool BIsAutoPlay;
        public bool BIsAutoSave;
    }
    public struct AudioSettingData
    {
        public float MasterVolume;
        public float BGMVolume;
        public float SFXVolume;
    }


    public ScreenSettingData ScreenSetting;
    public ControlSettingData ControlSetting;
    public AudioSettingData AudioSetting;
}