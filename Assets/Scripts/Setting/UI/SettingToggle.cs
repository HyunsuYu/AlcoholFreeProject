using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public sealed class SettingToggle : MonoBehaviour
{
    [SerializeField] private Image m_image_stageBar;
    [SerializeField] private Animator m_animator_StateBar;

    [SerializeField] private UnityEvent m_onValueChanged;

    private bool m_bisOn = false;


    internal bool Value
    {
        get
        {
            return m_bisOn;
        }
    }

    #region Unity Callbacks
    public void ToggleState()
    {
        m_bisOn = !m_bisOn;

        m_animator_StateBar.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        if (m_bisOn)
        {
            m_animator_StateBar.Play("SettingToggle_Off2On");
        }
        else
        {
            m_animator_StateBar.Play("SettingToggle_On2Off");
        }

        m_onValueChanged.Invoke();
    }
    #endregion

    internal void SetValueWithoutNotify(in bool targetState)
    {
        m_animator_StateBar.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

        m_bisOn = targetState;
        if (!m_bisOn)
        {
            //m_animator_StateBar.Play("SettingToggle_On2Off");
            m_image_stageBar.rectTransform.anchoredPosition = new Vector2()
            {
                x = 20.0f,
                y = 0.0f
            };
        }
    }
}