using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PassiveEnterEventItem : MonoBehaviour
{
    [SerializeField] private Image m_image_SelectedState;

    [SerializeField] private TMP_InputField m_inputField_EnterEventName;

    private string m_curEnterEventName;


    internal string CurEnterEventName
    {
        get
        {
            return m_curEnterEventName;
        }
    }

    #region Unity Callbacks
    public void TrySetEnterEventName()
    {
        if(string.IsNullOrEmpty(m_inputField_EnterEventName.text))
        {
            m_inputField_EnterEventName.SetTextWithoutNotify(m_curEnterEventName);
            return;
        }
        else if(m_inputField_EnterEventName.text == m_curEnterEventName)
        {
            return;
        }

        var curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        curPlotData.EnterEventTable.Add(m_inputField_EnterEventName.text, curPlotData.EnterEventTable[m_curEnterEventName]);
        curPlotData.EnterEventTable.Remove(m_curEnterEventName);

        if(PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName == m_curEnterEventName)
        {
            PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName = m_inputField_EnterEventName.text;
        }
        Guid[] linkIDs = curPlotData.LinkTable.Keys.ToArray();
        for (int index = 0; index < curPlotData.LinkTable.Keys.Count; index++)
        {
            Guid linkID = linkIDs[index];
            if (curPlotData.LinkTable[linkID].FromLinkCondition.HasValue
                && curPlotData.LinkTable[linkID].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.HasValue
                && curPlotData.LinkTable[linkID].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName == m_curEnterEventName)
            {
                PlotData.LinkData linkData = curPlotData.LinkTable[linkID];
                linkData.FromLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = linkData.FromLinkCondition.Value.CurSceneNodeLinkCondition,
                    CurEnterEventNodeLinkCondition = new PlotData.LinkData.LinkCondition.EnterEventNodeLinkCondition()
                    {
                        EnterEventName = m_inputField_EnterEventName.text
                    },
                    CurScreenTransitionNodeLinkCondition = linkData.FromLinkCondition.Value.CurScreenTransitionNodeLinkCondition
                };
                curPlotData.LinkTable[linkID] = linkData;
            }
            if (curPlotData.LinkTable[linkID].ToLinkCondition.HasValue
                && curPlotData.LinkTable[linkID].ToLinkCondition.Value.CurEnterEventNodeLinkCondition.HasValue
                && curPlotData.LinkTable[linkID].ToLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName == m_curEnterEventName)
            {
                PlotData.LinkData linkData = curPlotData.LinkTable[linkID];
                linkData.ToLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = linkData.ToLinkCondition.Value.CurSceneNodeLinkCondition,
                    CurEnterEventNodeLinkCondition = new PlotData.LinkData.LinkCondition.EnterEventNodeLinkCondition()
                    {
                        EnterEventName = m_inputField_EnterEventName.text
                    },
                    CurScreenTransitionNodeLinkCondition = linkData.ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition
                };
                curPlotData.LinkTable[linkID] = linkData;
            }
        }
        m_curEnterEventName = m_inputField_EnterEventName.text;

        PlotDataBuffer.Instance.PlotData = curPlotData;
        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.CurSelectedNodeID = Guid.Empty;
        PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName = string.Empty;

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    internal void Render(in string targetEnterEventName)
    {
        m_curEnterEventName = targetEnterEventName;

        if (m_curEnterEventName == PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName)
        {
            m_image_SelectedState.color = ChiefUIManager.SelectedColor;
        }
        else
        {
            m_image_SelectedState.color = ChiefUIManager.BasicBlackColor;
        }

        m_inputField_EnterEventName.SetTextWithoutNotify(m_curEnterEventName);
    }
}