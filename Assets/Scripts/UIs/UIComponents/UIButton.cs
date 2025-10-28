using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIButton : MonoBehaviour
{
    [SerializeField] private Image m_image_ButtonBackground;
    [SerializeField] private TMP_Text m_text_ButtonText;


    internal void SetTheme(in Color color, in Color textColor, in float fontSize)
    {
        m_image_ButtonBackground.color = color;

        m_text_ButtonText.color = textColor;
        m_text_ButtonText.fontSize = fontSize;
    }
}