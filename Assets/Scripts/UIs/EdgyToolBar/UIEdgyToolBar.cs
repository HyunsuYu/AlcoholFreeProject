using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class UIEdgyToolBar : MonoBehaviour
{
    public enum EdgyToolBarType
    {
        Left,
        Right,
        Bottom
    }


    [Header("Main Components")]
    [SerializeField] private EdgyToolBarType m_edgyToolBarType = default;
    [SerializeField] private UIEdgyToolBarSetting m_setting;

    [Header("Main RectTransforms")]
    [SerializeField] private RectTransform m_rectTransform_MainContent;
    [SerializeField] private RectTransform m_rectTransform_AppearButton;

    [Header("Quick Bar RectTransform - For EdgyToolBarType.Bottom")]
    [SerializeField] private RectTransform m_rectTransform_QuickBar;

    private bool m_bisMainContentVisible = true;
    private bool m_bisNowMoving = false;

    private float m_curMovingRate = 0.0f;


    public void FixedUpdate()
    {
        if (m_bisNowMoving)
        {
            m_curMovingRate += Time.fixedDeltaTime * m_setting.MovingSpeed;
            if (m_bisMainContentVisible)
            {
                m_rectTransform_MainContent.localPosition = new Vector3()
                {
                    x = Mathf.Lerp(m_setting.MainContentOriginPos.x, m_setting.MainContentVanishPos.x, m_curMovingRate),
                    y = Mathf.Lerp(m_setting.MainContentOriginPos.y, m_setting.MainContentVanishPos.y, m_curMovingRate),
                    z = Mathf.Lerp(m_setting.MainContentOriginPos.z, m_setting.MainContentVanishPos.z, m_curMovingRate)
                };
                m_rectTransform_AppearButton.localPosition = new Vector3()
                {
                    x = Mathf.Lerp(m_setting.AppearButtonVanishPos.x, m_setting.AppearButtonOriginPos.x, m_curMovingRate),
                    y = Mathf.Lerp(m_setting.AppearButtonVanishPos.y, m_setting.AppearButtonOriginPos.y, m_curMovingRate),
                    z = Mathf.Lerp(m_setting.AppearButtonVanishPos.z, m_setting.AppearButtonOriginPos.z, m_curMovingRate)
                };

                if(m_edgyToolBarType == EdgyToolBarType.Bottom)
                {
                    m_rectTransform_QuickBar.localPosition = new Vector3()
                    {
                        x = Mathf.Lerp(m_setting.QuickBarOriginPos.x, m_setting.QuickBarVanishPos.x, m_curMovingRate),
                        y = Mathf.Lerp(m_setting.QuickBarOriginPos.y, m_setting.QuickBarVanishPos.y, m_curMovingRate),
                        z = Mathf.Lerp(m_setting.QuickBarOriginPos.z, m_setting.QuickBarVanishPos.z, m_curMovingRate)
                    };
                }
            }
            else
            {
                m_rectTransform_MainContent.localPosition = new Vector3()
                {
                    x = Mathf.Lerp(m_setting.MainContentVanishPos.x, m_setting.MainContentOriginPos.x, m_curMovingRate),
                    y = Mathf.Lerp(m_setting.MainContentVanishPos.y, m_setting.MainContentOriginPos.y, m_curMovingRate),
                    z = Mathf.Lerp(m_setting.MainContentVanishPos.z, m_setting.MainContentOriginPos.z, m_curMovingRate)
                };
                m_rectTransform_AppearButton.localPosition = new Vector3()
                {
                    x = Mathf.Lerp(m_setting.AppearButtonOriginPos.x, m_setting.AppearButtonVanishPos.x, m_curMovingRate),
                    y = Mathf.Lerp(m_setting.AppearButtonOriginPos.y, m_setting.AppearButtonVanishPos.y, m_curMovingRate),
                    z = Mathf.Lerp(m_setting.AppearButtonOriginPos.z, m_setting.AppearButtonVanishPos.z, m_curMovingRate)
                };

                if (m_edgyToolBarType == EdgyToolBarType.Bottom)
                {
                    m_rectTransform_QuickBar.localPosition = new Vector3()
                    {
                        x = Mathf.Lerp(m_setting.QuickBarVanishPos.x, m_setting.QuickBarOriginPos.x, m_curMovingRate),
                        y = Mathf.Lerp(m_setting.QuickBarVanishPos.y, m_setting.QuickBarOriginPos.y, m_curMovingRate),
                        z = Mathf.Lerp(m_setting.QuickBarVanishPos.z, m_setting.QuickBarOriginPos.z, m_curMovingRate)
                    };
                }
            }

            if (m_curMovingRate >= 1.0f)
            {
                m_bisNowMoving = false;
                m_curMovingRate = 0.0f;
                if (m_bisMainContentVisible)
                {
                    m_rectTransform_MainContent.localPosition = m_setting.MainContentVanishPos;
                    m_rectTransform_AppearButton.localPosition = m_setting.AppearButtonOriginPos;

                    if (m_edgyToolBarType == EdgyToolBarType.Bottom)
                    {
                        m_rectTransform_QuickBar.localPosition = m_setting.QuickBarVanishPos;
                    }
                }
                else
                {
                    m_rectTransform_MainContent.localPosition = m_setting.MainContentOriginPos;
                    m_rectTransform_AppearButton.localPosition = m_setting.AppearButtonVanishPos;

                    if (m_edgyToolBarType == EdgyToolBarType.Bottom)
                    {
                        m_rectTransform_QuickBar.localPosition = m_setting.QuickBarOriginPos;
                    }
                }
                m_bisMainContentVisible = !m_bisMainContentVisible;
            }
        }
    }

    internal bool BIsMainContentVisible
    {
        get
        {
            return m_bisMainContentVisible;
        }
        set
        {
            m_bisMainContentVisible = value;
        }
    }

    public void OpenMainContent()
    {
        if(m_bisNowMoving)
        {
            return;
        }

        m_curMovingRate = 0.0f;
        m_bisNowMoving = true;
    }
}