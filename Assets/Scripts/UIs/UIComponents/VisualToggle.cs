using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class VisualToggle : MonoBehaviour
{
    [SerializeField] private Sprite[] m_toggleImages;

    private Image m_image_ToggleIcon;
    private Button m_button;

    [SerializeField] private bool m_bisToggled = false;


    public void Awake()
    {
        m_image_ToggleIcon = GetComponent<Image>();

        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(ToggleState);
    }

    internal bool BIsToggled
    {
        get
        {
            return m_bisToggled;
        }
    }

    private void ToggleState()
    {
        if (m_bisToggled)
        {
            m_image_ToggleIcon.sprite = m_toggleImages[1];
        }
        else
        {
            m_image_ToggleIcon.sprite = m_toggleImages[0];
        }
        m_bisToggled = !m_bisToggled;
    }
}