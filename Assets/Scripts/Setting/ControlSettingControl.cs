using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class ControlSettingControl : MonoBehaviour
{
    [Serializable]
    public struct UIs
    {
        public SettingToggle Toggle_BIsCheckDialog;
        public SettingToggle Toggle_BIsAutoPlay;
        public SettingToggle Toggle_BIsAutoSave;

        public Image Image_PreviewBackground;
        public Image Image_PreviewConversationBackground;
        public TMP_Text Text_previewName;
        public TMP_Text Text_previewSpeakText;

        public Slider Slider_FontSize;
        public Slider Slider_FontSpeed;
    }


    [SerializeField] private UIs m_uis;

    private BackgroundResourceDesc.BackgroundType m_curBackgroundType = BackgroundResourceDesc.BackgroundType.HomeRoom;


    #region Unity Callbacks
    public void TrySetCheckDialog()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ControlSettingData curControlSettingData = curSettingData.ControlSetting;

        curControlSettingData.BIsCheckDialog = m_uis.Toggle_BIsCheckDialog.Value;

        curSettingData.ControlSetting = curControlSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();
    }
    public void TrySetAutoPlay()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ControlSettingData curControlSettingData = curSettingData.ControlSetting;

        curControlSettingData.BIsAutoPlay = m_uis.Toggle_BIsAutoPlay.Value;

        curSettingData.ControlSetting = curControlSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();
    }
    public void TrySetAutoSave()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ControlSettingData curControlSettingData = curSettingData.ControlSetting;

        curControlSettingData.BIsAutoSave = m_uis.Toggle_BIsAutoSave.Value;

        curSettingData.ControlSetting = curControlSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();
    }
    public void TrySetFontSize()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ControlSettingData curControlSettingData = curSettingData.ControlSetting;

        curControlSettingData.FontSize = m_uis.Slider_FontSize.value;

        curSettingData.ControlSetting = curControlSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        RenderPreview();
    }
    public void TrySetFontSpeed()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        SettingData.ControlSettingData curControlSettingData = curSettingData.ControlSetting;

        curControlSettingData.FontSpeed = m_uis.Slider_FontSpeed.value;

        curSettingData.ControlSetting = curControlSettingData;
        SettingDataBuffer.Instance.SettingData = curSettingData;

        SettingDataBuffer.Instance.TrySaveData();

        RenderPreview();
    }

    public void OpenLeftPreviewBackground()
    {
        if (m_curBackgroundType == 0)
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
        if (m_curBackgroundType == (BackgroundResourceDesc.BackgroundType)(Enum.GetNames(typeof(BackgroundResourceDesc.BackgroundType)).Length - 1))
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
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;

        m_uis.Toggle_BIsCheckDialog.SetValueWithoutNotify(curSettingData.ControlSetting.BIsCheckDialog);
        m_uis.Toggle_BIsAutoPlay.SetValueWithoutNotify(curSettingData.ControlSetting.BIsAutoPlay);
        m_uis.Toggle_BIsAutoSave.SetValueWithoutNotify(curSettingData.ControlSetting.BIsAutoSave);

        RenderPreview();

        m_uis.Slider_FontSize.SetValueWithoutNotify(curSettingData.ControlSetting.FontSize);
        m_uis.Slider_FontSpeed.SetValueWithoutNotify(curSettingData.ControlSetting.FontSpeed);
    }
    internal void RenderPreview()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;

        m_uis.Image_PreviewBackground.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(m_curBackgroundType);
        m_uis.Image_PreviewConversationBackground.color = new Color()
        {
            r = 0.1607843f,
            g = 0.1607843f,
            b = 0.1607843f,
            a = curSettingData.ScreenSetting.ConversationBackgroundOpacity
        };

        m_uis.Text_previewName.fontSize = (curSettingData.ControlSetting.FontSize * 0.4f) * 1.166666f;
        m_uis.Text_previewSpeakText.fontSize = curSettingData.ControlSetting.FontSize * 0.4f;

        m_uis.Text_previewSpeakText.text = "{fade d=" + curSettingData.ControlSetting.FontSpeed + "}하나 둘 셋 넷{/fade}";
    }
}