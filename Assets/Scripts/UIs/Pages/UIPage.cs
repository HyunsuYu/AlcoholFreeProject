using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class UIPage : AUIPage
{
    private int m_pageIndex = -1;


    public void ClosePopup()
    {
        PageSynchronizer.Instance.OpenPage(m_pageIndex);
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    internal override void Init(in int pageIndex)
    {
        m_pageIndex = pageIndex;
    }

    internal override void OpenThisPage(in PageSynchronizer.PageOpenType pageOpenType)
    {

    }
    internal override void CloseThisPage(in PageSynchronizer.PageOpenType pageOpenType)
    {

    }
}