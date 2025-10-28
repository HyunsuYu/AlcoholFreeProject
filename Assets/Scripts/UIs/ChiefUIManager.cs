using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefUIManager : SingleTonForGameObject<ChiefUIManager>
{
    private Guid m_curSelectedSceneID = Guid.Empty;
    private int m_curSelectedFrameIndex = -1;

    private int m_curSelectedAppearCharacterIndex = -1;
    private int m_curSelectedBranchIndex = -1;
    private int m_curSelectedCutSceneIndex = -1;

    private TotalEdgyToolBarControl.PageType m_curPageType;

    private bool m_bisInited = false;


    public void Awake()
    {
        SetInstance(this);
    }
    public void FixedUpdate()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        if(Instance == null)
        {
            Debug.Log("ChiefUIManager instance is null. Please ensure it is initialized before use.");
            return;
        }

        if (!m_bisInited)
        {
            RenderAll();

            m_bisInited = true;
        }
    }

    internal Guid CurSelectedSceneID
    {
        get
        {
            return m_curSelectedSceneID;
        }
        set
        {
            //if(value == Guid.Empty)
            //{
            //    throw new ArgumentException("CurSelectedSceneID cannot be Guid.Empty");
            //}
            m_curSelectedSceneID = value;
        }
    }
    internal int CurSelectedFrameIndex
    {
        get
        {
            return m_curSelectedFrameIndex;
        }
        set
        {
            m_curSelectedFrameIndex = value;
        }
    }

    internal int CurSelectedAppearCharacterIndex
    {
        get
        {
            return m_curSelectedAppearCharacterIndex;
        }
        set
        {
            m_curSelectedBranchIndex = -1;
            m_curSelectedCutSceneIndex = -1;
            m_curSelectedAppearCharacterIndex = value;
        }
    }
    internal int CurSelectedBranchIndex
    {
        get
        {
            return m_curSelectedBranchIndex;
        }
        set
        {
            m_curSelectedAppearCharacterIndex = -1;
            m_curSelectedCutSceneIndex = -1;
            m_curSelectedBranchIndex = value;
        }
    }
    internal int CurSelectedCutSceneIndex
    {
        get
        {
            return m_curSelectedCutSceneIndex;
        }
        set
        {
            m_curSelectedAppearCharacterIndex = -1;
            m_curSelectedBranchIndex = -1;
            m_curSelectedCutSceneIndex = value;
        }
    }

    internal TotalEdgyToolBarControl.PageType CurPageType
    {
        get
        {
            return m_curPageType;
        }
        set
        {
            m_curPageType = value;
        }
    }

    internal static Color SelectedColor = new Color(0.8470589f, 0.4627451f, 0.1254902f, 1.0f);
    internal static Color BasicBlackColor = new Color(0.09803922f, 0.09803922f, 0.09803922f, 1.0f);

    internal void RenderAll()
    {
        LeftToolBarControl.Instance.Render();
        RightToolBarControl.Instance.Render();

        PlotTreeActiveUIControl.Instance.Render();

        PlotTreeLeftToolBarControl.Instance.Render();
        PlotTreeRightToolBarControl.Instance.Render();

        EmulationCanvasControl.Instance.Render();
    }

    protected override void Dispose(bool bisDispose)
    {
        if(bisDispose)
        {
            throw new NotImplementedException();
        }
    }
}