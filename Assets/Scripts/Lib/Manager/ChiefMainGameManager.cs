using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using UnityEngine.SceneManagement;


public sealed class ChiefMainGameManager : SingleTon<ChiefMainGameManager>
{
    private SaveData.GamePhase m_curGamePhase = SaveData.GamePhase.Morning;

    private Guid m_curSelectedSceneID = Guid.Empty;
    private int m_curSelectedFrameIndex = -1;


    internal Guid CurSelectedSceneID
    {
        get
        {
            return m_curSelectedSceneID;
        }
        set
        {
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

    internal void RenderForEditorPreview(in Guid targetSceneID, in int targetFrameIndex)
    {
        m_curGamePhase = SaveData.GamePhase.Noon;

        m_curSelectedSceneID = targetSceneID;
        m_curSelectedFrameIndex = targetFrameIndex;

        SceneManager.LoadScene("Conversation");

        NoonCanvasControl.Request2Render(m_curSelectedSceneID, m_curSelectedFrameIndex, true);
    }
    internal void Render(in string targetEnterEventName)
    {
        Guid[] targetSceneIDs = PlotDataInterface.FindOutputLinks(targetEnterEventName);

        m_curGamePhase = SaveData.GamePhase.Noon;

        m_curSelectedSceneID = PlotDataBuffer.Instance.PlotData.Value.LinkTable[targetSceneIDs[0]].ToNodeID;
        m_curSelectedFrameIndex = 0;

        SceneManager.LoadScene("Conversation");

        NoonCanvasControl.Request2Render(m_curSelectedSceneID, m_curSelectedFrameIndex);
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}