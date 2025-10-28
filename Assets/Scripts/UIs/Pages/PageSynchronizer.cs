using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;


public sealed class PageSynchronizer : SingleTonForGameObject<PageSynchronizer>
{
    internal enum PageOpenType : byte
    {
        Single,
        Additive,
    }


    [Header("Page Datas")]
    [SerializeField] private AUIPage[] m_uiPages;
    [SerializeField] private PageOpenType[] m_pageOpenTypes;
    [SerializeField] private string[] m_pageNames;

    [Header("Page Buttons")]
    [SerializeField] private GameObject m_prefab_PageButton;
    [SerializeField] private Transform m_transform_PageButtonParent;

    [SerializeField] private Transform[] m_additiveButtonParents;

    private Dictionary<int, Image> m_pageButtonBackgroundTable = new Dictionary<int, Image>();


    public void Awake()
    {
        SetInstance(this);

        for(int index = 0; index < m_pageOpenTypes.Length; index++)
        {
            switch (m_pageOpenTypes[index])
            {
                case PageOpenType.Single:
                    UIPageButton uiPageButton = Instantiate(m_prefab_PageButton, m_transform_PageButtonParent).GetComponent<UIPageButton>();
                    uiPageButton.Init(m_pageNames[index], index);

                    m_pageButtonBackgroundTable.Add(index, uiPageButton.GetComponent<Image>());
                    break;

                //case PageOpenType.Additive:
                //    uiPageButton = Instantiate(m_prefab_PageButton, m_additiveButtonParents[index]).GetComponent<UIPageButton>();
                //    break;
            }

            m_uiPages[index].Init(index);
        }

        OpenPage(0);
    }

    #region Unity Callbacks
    public void OpenPage(int targetPageIndex)
    {
        switch(m_pageOpenTypes[targetPageIndex])
        {
            case PageOpenType.Single:
                for (int index = 0; index < m_uiPages.Length; index++)
                {
                    if (index == targetPageIndex)
                    {
                        m_uiPages[index].gameObject.SetActive(true);
                        m_uiPages[index].OpenThisPage(m_pageOpenTypes[targetPageIndex]);

                        if(m_pageButtonBackgroundTable.ContainsKey(index))
                        {
                            m_pageButtonBackgroundTable[index].enabled = true;
                        }
                    }
                    else
                    {
                        m_uiPages[index].CloseThisPage(m_pageOpenTypes[targetPageIndex]);
                        m_uiPages[index].gameObject.SetActive(false);

                        if (m_pageButtonBackgroundTable.ContainsKey(index))
                        {
                            m_pageButtonBackgroundTable[index].enabled = false;
                        }
                    }
                }
                break;

            case PageOpenType.Additive:
                for (int index = 0; index < m_uiPages.Length; index++)
                {
                    if (index == targetPageIndex)
                    {
                        if(m_uiPages[index].gameObject.activeSelf)
                        {
                            m_uiPages[index].CloseThisPage(m_pageOpenTypes[targetPageIndex]);
                            m_uiPages[index].gameObject.SetActive(false);
                        }
                        else
                        {
                            m_uiPages[index].gameObject.SetActive(true);
                            m_uiPages[index].OpenThisPage(m_pageOpenTypes[targetPageIndex]);
                        }
                    }
                    else if (m_pageOpenTypes[index] == PageOpenType.Additive)
                    {
                        m_uiPages[index].CloseThisPage(m_pageOpenTypes[targetPageIndex]);
                        m_uiPages[index].gameObject.SetActive(false);
                    }
                }
                break;
        }
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}