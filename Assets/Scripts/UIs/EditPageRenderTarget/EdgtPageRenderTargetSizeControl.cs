using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class EdgtPageRenderTargetSizeControl : MonoBehaviour
{
    [Header("Render Target")]
    [SerializeField] private RectTransform m_rectTransform_RenderTarget;

    [Header("Tool Bars")]
    [SerializeField] private UIEdgyToolBar m_leftToolBar;
    [SerializeField] private UIEdgyToolBar m_rightToolBar;
    [SerializeField] private UIEdgyToolBar m_bottomToolBar;

    [Header("Settings")]
    [SerializeField] private EditPageRenderTargetSizeControlSetting m_setting;

    private bool m_bisCanMaximize = false;
    private bool m_bisNowMoving = false;

    private float m_curMovingRate = 0.0f;


    public void Awake()
    {
        m_rectTransform_RenderTarget.sizeDelta = GetFixedResolution(m_setting.RenderTargetOriginSize);
    }
    public void FixedUpdate()
    {
        if(!m_bisNowMoving)
        {
            if (!m_bisCanMaximize && !m_leftToolBar.BIsMainContentVisible && !m_rightToolBar.BIsMainContentVisible && !m_bottomToolBar.BIsMainContentVisible)
            {
                m_bisCanMaximize = true;
                m_bisNowMoving = true;
                m_curMovingRate = 0.0f;
            }
            else if(m_bisCanMaximize && (m_leftToolBar.BIsMainContentVisible || m_rightToolBar.BIsMainContentVisible || m_bottomToolBar.BIsMainContentVisible))
            {
                m_bisCanMaximize = false;
                m_bisNowMoving = true;
                m_curMovingRate = 0.0f;
            }
        }

        if(m_bisNowMoving)
        {
            m_curMovingRate += Time.fixedDeltaTime * m_setting.ResizeSpeed;

            Vector2 originSize = GetFixedResolution(m_setting.RenderTargetOriginSize);
            Vector2 maximizeSize = GetFixedResolution(m_setting.RenderTargetMaximizeSize);
            if (m_bisCanMaximize)
            {
                m_rectTransform_RenderTarget.sizeDelta = new Vector2()
                {
                    x = Mathf.Lerp(originSize.x, maximizeSize.x, m_curMovingRate),
                    y = Mathf.Lerp(originSize.y, maximizeSize.y, m_curMovingRate)
                };
            }
            else
            {
                m_rectTransform_RenderTarget.sizeDelta = new Vector2()
                {
                    x = Mathf.Lerp(maximizeSize.x, originSize.x, m_curMovingRate),
                    y = Mathf.Lerp(maximizeSize.y, originSize.y, m_curMovingRate)
                };
            }

            if (m_curMovingRate >= 1.0f)
            {
                m_bisNowMoving = false;
                m_curMovingRate = 0.0f;

                RenderTargetControl.Instance.UpdateRenderTargetSize();
            }
        }
    }

    private Vector2 GetFixedResolution(in Vector2 targetResolution)
    {
        return new Vector2()
        {
            x = (1920.0f / 1080.0f) * targetResolution.y,
            y = targetResolution.y
        };
    }
}