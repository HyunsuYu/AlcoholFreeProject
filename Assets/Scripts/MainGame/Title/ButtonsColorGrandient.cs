using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public sealed class ButtonsColorGrandient : MonoBehaviour
{
    [SerializeField] private Color m_colorStart = Color.white;
    [SerializeField] private Color m_colorEnd = Color.black;

    [SerializeField] private Image[] m_images;


    public void Awake()
    {
        for(int index = 0; index < m_images.Length; index++)
        {
            if (m_images[index] != null)
            {
                m_images[index].color = Color.Lerp(m_colorStart, m_colorEnd, (float)index / (m_images.Length - 1));
            }
        }
    }
}