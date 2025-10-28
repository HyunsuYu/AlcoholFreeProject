using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public sealed class ScreenSettingControl : MonoBehaviour
{
    [Serializable] public struct UIs
    {
        public RectTransform RectTransform_Contents;

        public TMP_Dropdown Dropdown_CurResolution;

        //public SettingToggle Toggle_BIsFullScreen;
        public TMP_Dropdown Dropdown_BIsFullScreen;
        public SettingToggle Toggle_BIsVSync;

        public Image Image_PreviewBackground;
        public Image Image_PreviewConversationBackground;
        public Slider Slider_ConversationBackgroundOpacity;
    }


    [SerializeField] private UIs m_uis;

    private BackgroundResourceDesc.BackgroundType m_curBackgroundType = BackgroundResourceDesc.BackgroundType.HomeRoom;


    #region Unity Callbacks
    public void TrySetResolution()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ScreenSettingData curScreenSettingData = curSettingData.ScreenSetting;

        curScreenSettingData.CurResolution = (SettingData.ScreenSettingData.Resolution)m_uis.Dropdown_CurResolution.value;

        curSettingData.ScreenSetting = curScreenSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        RenderRsolution();
    }
    public void TrySetFullScreen()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ScreenSettingData curScreenSettingData = curSettingData.ScreenSetting;

        curScreenSettingData.BIsFullScreen = m_uis.Dropdown_BIsFullScreen.value == 0;

        curSettingData.ScreenSetting = curScreenSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        RenderRsolution();
    }
    public void TrySetVSync()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ScreenSettingData curScreenSettingData = curSettingData.ScreenSetting;

        curScreenSettingData.BIsVSync = m_uis.Toggle_BIsVSync.Value;

        curSettingData.ScreenSetting = curScreenSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        RenderRsolution();
    }
    public void TrySetConversationBackgroundOpacity()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ScreenSettingData curScreenSettingData = curSettingData.ScreenSetting;

        curScreenSettingData.ConversationBackgroundOpacity = m_uis.Slider_ConversationBackgroundOpacity.value;

        curSettingData.ScreenSetting = curScreenSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        m_uis.Image_PreviewConversationBackground.color = new Color()
        {
            r = 0.1607843f,
            g = 0.1607843f,
            b = 0.1607843f,
            a = curScreenSettingData.ConversationBackgroundOpacity
        };

        ChiefSettingManager.Instance.ControlSettingControl.RenderPreview();
    }

    public void OpenLeftPreviewBackground()
    {
        if(m_curBackgroundType == 0)
        {
            m_curBackgroundType = (BackgroundResourceDesc.BackgroundType)(Enum.GetNames(typeof(BackgroundResourceDesc.BackgroundType)).Length - 1);
        }
        else
        {
            m_curBackgroundType = (BackgroundResourceDesc.BackgroundType)((int)m_curBackgroundType - 1);
        }

        m_uis.Image_PreviewBackground.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(m_curBackgroundType);
    }
    public void OpenRightPreviewBackground()
    {
        if(m_curBackgroundType == (BackgroundResourceDesc.BackgroundType)(Enum.GetNames(typeof(BackgroundResourceDesc.BackgroundType)).Length - 1))
        {
            m_curBackgroundType = 0;
        }
        else
        {
            m_curBackgroundType = (BackgroundResourceDesc.BackgroundType)((int)m_curBackgroundType + 1);
        }

        m_uis.Image_PreviewBackground.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(m_curBackgroundType);
    }
    #endregion

    internal void Render()
    {
        SettingData.ScreenSettingData screenSetting = SettingDataBuffer.Instance.SettingData.ScreenSetting;

        Resolution[] resolutions = Screen.resolutions;
        m_uis.Dropdown_CurResolution.ClearOptions();
        Dictionary<int, string> resolutionOptions = new Dictionary<int, string>();
        foreach (Resolution resolution in resolutions)
        {
            if(resolution.width == 1920 && resolution.height == 1080 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "FHD 1920x1080");
            }
            else if (resolution.width == 2560 && resolution.height == 1440 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "QHD 2560x1440");
            }
            else if (resolution.width == 3840 && resolution.height == 2160 && !resolutionOptions.ContainsKey(resolution.width * resolution.height))
            {
                resolutionOptions.Add(resolution.width * resolution.height, "UHD 3840x2160");
            }
        }
        m_uis.Dropdown_CurResolution.AddOptions(resolutionOptions.Values.ToList());
        m_uis.Dropdown_CurResolution.SetValueWithoutNotify((int)screenSetting.CurResolution);

        m_uis.Dropdown_BIsFullScreen.SetValueWithoutNotify(screenSetting.BIsFullScreen ? 0 : 1);
        m_uis.Toggle_BIsVSync.SetValueWithoutNotify(screenSetting.BIsVSync);

        m_uis.Image_PreviewBackground.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(m_curBackgroundType);
        m_uis.Image_PreviewConversationBackground.color = new Color()
        {
            r = 0.1607843f,
            g = 0.1607843f,
            b = 0.1607843f,
            a = screenSetting.ConversationBackgroundOpacity
        };

        m_uis.Slider_ConversationBackgroundOpacity.SetValueWithoutNotify(screenSetting.ConversationBackgroundOpacity);

        RenderRsolution();
    }
    internal void RenderRsolution()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        switch (curSettingData.ScreenSetting.CurResolution)
        {
            case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
                m_uis.RectTransform_Contents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
                m_uis.RectTransform_Contents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
                m_uis.RectTransform_Contents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;
    }
}