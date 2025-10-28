using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using CommonUtilLib.ThreadSafe;


public class RecipeSelectionTutorial : SingleTonForGameObject<RecipeSelectionTutorial>
{
    [Serializable] public struct TutorialPageData
    {
        public Sprite Image;
        public string Description;
    }


    [SerializeField] private GameObject m_layout_Popup;

    [SerializeField] private TutorialPageData[] m_tutorialPageDatas;

    [SerializeField] private GameObject m_gameObject_LeftPageBtn;
    [SerializeField] private GameObject m_gameObject_RightPageBtn;

    [SerializeField] private Image m_image_TutorialImage;
    [SerializeField] private TMP_Text m_text_Description;

    [SerializeField] private GameObject m_gameObject_RecipeSelectionPopup;

    private int m_curPageIndex = 0;

    private static bool m_bisFirstView = true;


    public void Awake()
    {
        SetInstance(this);
    }

    internal bool BIsFirstView
    {
        set
        {
            m_bisFirstView = value;
        }
    }

    #region Unity Callbacks
    public void OpenPrevPage()
    {
        if (m_curPageIndex - 1 >= 0)
        {
            m_curPageIndex--;
        }
        else
        {
            m_curPageIndex = m_tutorialPageDatas.Length - 1;
        }
        Render();
    }
    public void OpenNextPage()
    {
        if (m_curPageIndex + 1 < m_tutorialPageDatas.Length)
        {
            m_curPageIndex++;
        }
        else
        {
            m_curPageIndex = 0;
        }
        Render();
    }

    public void OpenPopup()
    {
        m_curPageIndex = 0;
        m_layout_Popup.SetActive(true);
    }
    public void ClosePopup()
    {
        m_layout_Popup.SetActive(false);

        if(m_bisFirstView)
        {
            m_gameObject_RecipeSelectionPopup.SetActive(true);
            RecipeSelectionControl.Instance.Render();

            m_bisFirstView = false;
        }
    }
    #endregion

    internal void Render()
    {
        m_image_TutorialImage.sprite = m_tutorialPageDatas[m_curPageIndex].Image;
        m_text_Description.text = m_tutorialPageDatas[m_curPageIndex].Description;
        //if (m_curPageIndex == 0)
        //{
        //    m_gameObject_LeftPageBtn.SetActive(false);
        //}
        //else
        //{
        //    m_gameObject_LeftPageBtn.SetActive(true);
        //}
        //if (m_curPageIndex == m_tutorialPageDatas.Length - 1)
        //{
        //    m_gameObject_RightPageBtn.SetActive(false);
        //}
        //else
        //{
        //    m_gameObject_RightPageBtn.SetActive(true);
        //}
    }

    internal void ResetStatic()
    {
        m_bisFirstView = true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
}