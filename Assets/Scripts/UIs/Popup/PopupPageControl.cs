using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class PopupPageControl : MonoBehaviour
{
    [SerializeField] private GameObject[] popupPages;
    [SerializeField] private Image[] m_image_PageButtonBackgrounds;


    public void Awake()
    {
        OpenPopupPage(0);
    }

    public void OpenPopupPage(int targetPageIndex)
    {
        for(int index = 0; index < popupPages.Length; index++)
        {
            if (index == targetPageIndex)
            {
                popupPages[index].SetActive(true);
                m_image_PageButtonBackgrounds[index].enabled = true;
            }
            else
            {
                popupPages[index].SetActive(false);
                m_image_PageButtonBackgrounds[index].enabled = false;
            }
        }
    }
}