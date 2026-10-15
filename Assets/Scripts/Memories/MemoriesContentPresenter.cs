using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;


public sealed class MemoriesContentPresenter : MonoBehaviour
{
    [SerializeField] private MemoriesPageControl.MemoriesPageType m_curMemoriesPageType;

    [Header("Page Index")]
    [SerializeField] private GameObject m_prefab_PageIndexItem;
    [SerializeField] private Transform m_transform_PageIndexParent;

    [SerializeField] private Sprite[] m_sprite_PageIndexSprites;

    private int m_curPageIndex = 0;

    [Header("Content Item")]
    [SerializeField] private GameObject m_prefab_MemoriesContentItem;
    [SerializeField] private GameObject m_prefab_MemoriesContentEmpryItem;
    [SerializeField] private Transform m_transform_MemeoriesContentParent;

    [SerializeField] private int m_singlePageContentItemCount = 12;

    [Header("COntrol")]
    [SerializeField] private Button m_button_OpenLeftPageBtn;
    [SerializeField] private Button m_button_OpenRightPageBtn;


    private string StoryMemoriesDictionaryPath
    {
        get
        {
            return System.IO.Path.Combine(Application.streamingAssetsPath, $"ScreenShots/Story");
        }
    }
    private string CutSceneMemoriesDictionaryPath
    {
        get
        {
            return System.IO.Path.Combine(Application.streamingAssetsPath, $"ScreenShots/CutScene");
        }
    }
    private string MiniGameMemoriesDictionaryPath
    {
        get
        {
            return System.IO.Path.Combine(Application.streamingAssetsPath, $"ScreenShots/MiniGame");
        }
    }

    internal void Render()
    {
        #region Page Index
        int pageIndexChildCount = m_transform_PageIndexParent.childCount;
        for(int index = pageIndexChildCount - 1;  index >= 0; index--)
        {
            Destroy(m_transform_PageIndexParent.GetChild(index).gameObject);
        }

        if(GetTotalContentCount() == 0)
        {
            Instantiate(m_prefab_PageIndexItem, m_transform_PageIndexParent).GetComponent<Image>().sprite = m_sprite_PageIndexSprites[1];
        }
        else
        {
            //int totalPageCount = GetTotalPageCount();

            List<int> pageIndexNums = new List<int>();
            int powFactor = 0;
            while (true)
            {
                float val = (m_curPageIndex + 1) / Mathf.Pow(10.0f, powFactor);
                if (val >= 1.0f)
                {
                    pageIndexNums.Add((int)val);
                }
                else
                {
                    break;
                }

                powFactor++;
            }

            pageIndexNums.Reverse();

            foreach(int pageNum in pageIndexNums)
            {
                Instantiate(m_prefab_PageIndexItem, m_transform_PageIndexParent).GetComponent<Image>().sprite = m_sprite_PageIndexSprites[pageNum];
            }
        }
        #endregion

        #region Content
        List<string> contentFileNames = new List<string>();
        switch (m_curMemoriesPageType)
        {
            case MemoriesPageControl.MemoriesPageType.Story:
                {
                    if(!Directory.Exists(StoryMemoriesDictionaryPath))
                    {
                        Directory.CreateDirectory(StoryMemoriesDictionaryPath);
                    }
                    string[] fileNames = System.IO.Directory.GetFiles(StoryMemoriesDictionaryPath);

                    int count = 0;
                    for (int index = 2 * m_curPageIndex * m_singlePageContentItemCount; index < fileNames.Length; index++)
                    {
                        if(count == m_singlePageContentItemCount)
                        {
                            break;
                        }

                        if (System.IO.Path.GetExtension(fileNames[index]) == ".png")
                        {
                            contentFileNames.Add(fileNames[index]);
                            count++;
                        }
                    }
                }
                break;

            case MemoriesPageControl.MemoriesPageType.CutScene:
                {
                    if (!Directory.Exists(CutSceneMemoriesDictionaryPath))
                    {
                        Directory.CreateDirectory(CutSceneMemoriesDictionaryPath);
                    }
                    string[] fileNames = System.IO.Directory.GetFiles(CutSceneMemoriesDictionaryPath);

                    int count = 0;
                    for (int index = 2 * m_curPageIndex * m_singlePageContentItemCount; index < fileNames.Length; index++)
                    {
                        if (count == m_singlePageContentItemCount)
                        {
                            break;
                        }

                        if (System.IO.Path.GetExtension(fileNames[index]) == ".png")
                        {
                            contentFileNames.Add(fileNames[index]);
                            count++;
                        }
                    }
                }
                break;

            case MemoriesPageControl.MemoriesPageType.MiniGame:
                {
                    if (!Directory.Exists(MiniGameMemoriesDictionaryPath))
                    {
                        Directory.CreateDirectory(MiniGameMemoriesDictionaryPath);
                    }
                    string[] fileNames = System.IO.Directory.GetFiles(MiniGameMemoriesDictionaryPath);

                    int count = 0;
                    for (int index = 2 * m_curPageIndex * m_singlePageContentItemCount; index < fileNames.Length; index++)
                    {
                        if (count == m_singlePageContentItemCount)
                        {
                            break;
                        }

                        if (System.IO.Path.GetExtension(fileNames[index]) == ".png")
                        {
                            contentFileNames.Add(fileNames[index]);
                            count++;
                        }
                    }
                }
                break;
        }

        int memoriesContentItemCount = m_transform_MemeoriesContentParent.childCount;
        for(int index = memoriesContentItemCount - 1; index >= 0; index--)
        {
            Destroy(m_transform_MemeoriesContentParent.GetChild(index).gameObject);
        }

        for(int index = 0; index < contentFileNames.Count; index++)
        {
            Instantiate(m_prefab_MemoriesContentItem, m_transform_MemeoriesContentParent).GetComponent<MemoriesContentItem>().Render(contentFileNames[index], m_curPageIndex * m_singlePageContentItemCount + index);
        }
        for(int leftCount = m_singlePageContentItemCount - contentFileNames.Count; leftCount > 0; leftCount--)
        {
            Instantiate(m_prefab_MemoriesContentEmpryItem, m_transform_MemeoriesContentParent);
        }
        #endregion

        #region Control
        if (m_curPageIndex - 1 >= 0)
        {
            m_button_OpenLeftPageBtn.enabled = true;
            m_button_OpenLeftPageBtn.GetComponent<Image>().enabled = true;
        }
        else
        {
            m_button_OpenLeftPageBtn.enabled = false;
            m_button_OpenLeftPageBtn.GetComponent<Image>().enabled = false;
        }

        if (m_curPageIndex + 1 < GetTotalPageCount())
        {
            m_button_OpenRightPageBtn.enabled = true;
            m_button_OpenRightPageBtn.GetComponent<Image>().enabled = true;
        }
        else
        {
            m_button_OpenRightPageBtn.enabled = false;
            m_button_OpenRightPageBtn.GetComponent<Image>().enabled = false;
        }
        #endregion
    }

    internal void OpenLeftPageContent()
    {
        if (m_curPageIndex - 1 >= 0)
        {
            m_curPageIndex--;

            Render();
        }
    }
    internal void OpenRightPageContent()
    {
        if (m_curPageIndex + 1 < GetTotalPageCount())
        {
            m_curPageIndex++;

            Render();
        }
    }

    private int GetTotalContentCount()
    {
        int totalContentCount = 0;
        switch (m_curMemoriesPageType)
        {
            case MemoriesPageControl.MemoriesPageType.Story:
                totalContentCount = System.IO.Directory.GetFiles(StoryMemoriesDictionaryPath).Length;
                break;

            case MemoriesPageControl.MemoriesPageType.CutScene:
                totalContentCount = System.IO.Directory.GetFiles(CutSceneMemoriesDictionaryPath).Length;
                break;

            case MemoriesPageControl.MemoriesPageType.MiniGame:
                totalContentCount = System.IO.Directory.GetFiles(MiniGameMemoriesDictionaryPath).Length;
                break;
        }

        return totalContentCount / 2;
    }
    private int GetTotalPageCount()
    {
        int totalContentCount = GetTotalContentCount();

        int totalPageCount = totalContentCount / m_singlePageContentItemCount;
        if (totalContentCount % m_singlePageContentItemCount != 0)
        {
            totalPageCount++;
        }

        return totalPageCount;
    }
}