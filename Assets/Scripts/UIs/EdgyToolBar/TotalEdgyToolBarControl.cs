using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class TotalEdgyToolBarControl : MonoBehaviour
{
    public enum PageType
    {
        EditPage,
        PlotTree,
        PlayPage
    }

    [Serializable] public struct PageUIEdgyToolBars
    {
        public UIEdgyToolBar[] EdgyToolBars;
    }


    //[SerializeField] private UIEdgyToolBar[] m_UIEdgyToolBars;
    [SerializeField] private PageUIEdgyToolBars[] m_PageUIEdgyToolBars;


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            bool bisSomthingVisible = false;
            UIEdgyToolBar[] curUIEdgyToolBars = m_PageUIEdgyToolBars[(int)ChiefUIManager.Instance.CurPageType].EdgyToolBars;

            foreach (var edgyToolBar in curUIEdgyToolBars)
            {
                if(edgyToolBar.BIsMainContentVisible)
                {
                    bisSomthingVisible = true;
                    break;
                }
            }

            if(bisSomthingVisible)
            {
                foreach(var edgyToolBar in curUIEdgyToolBars)
                {
                    if(edgyToolBar.BIsMainContentVisible)
                    {
                        edgyToolBar.BIsMainContentVisible = true;
                        edgyToolBar.OpenMainContent();
                    }
                }
            }
            else
            {
                foreach (var edgyToolBar in curUIEdgyToolBars)
                {
                    if (!edgyToolBar.BIsMainContentVisible)
                    {
                        edgyToolBar.BIsMainContentVisible = false;
                        edgyToolBar.OpenMainContent();
                    }
                }
            }
        }
    }
}