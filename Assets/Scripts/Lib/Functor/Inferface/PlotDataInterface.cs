using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


internal static class PlotDataInterface
{
    #region For Scene Node
    /// <summary>
    /// target Node로부터 나가는 Link의 목록을 찾아주는 함수
    /// </summary>
    /// <param name="targetNodeID"></param>
    /// <returns></returns>
    public static Guid[] FindOutputLinks(in Guid targetNodeID)
    {
        List<Guid> outputLinkIDs = new List<Guid>();
        var linkTable = PlotDataBuffer.Instance.PlotData.Value.LinkTable;
        foreach(Guid linkID in linkTable.Keys)
        {
            if (linkTable[linkID].FromNodeID == targetNodeID)
            {
                outputLinkIDs.Add(linkID);
            }
        }

        return outputLinkIDs.ToArray();
    }
    /// <summary>
    /// target Node로 들어오는 Link를 찾아주는 함수
    /// </summary>
    /// <param name="targetNodeID"></param>
    /// <returns></returns>
    public static Guid FindInputLink(in Guid targetNodeID)
    {
        var linkTable = PlotDataBuffer.Instance.PlotData.Value.LinkTable;
        foreach (Guid linkID in linkTable.Keys)
        {
            if (linkTable[linkID].ToNodeID == targetNodeID)
            {
                return linkID;
            }
        }

        return Guid.Empty;
    }
    #endregion

    #region For Enter Event Node
    public static Guid[] FindOutputLinks(in string targetEnterEventName)
    {
        List<Guid> outputLinkIDs = new List<Guid>();
        var linkTable = PlotDataBuffer.Instance.PlotData.Value.LinkTable;
        foreach (Guid linkID in linkTable.Keys)
        {
            if (linkTable[linkID].FromNodeType == PlotData.LinkNodeType.EnterEventNode
                && linkTable[linkID].FromNodeID == PlotData.EnterEventNodeID
                && linkTable[linkID].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName == targetEnterEventName)
            {
                outputLinkIDs.Add(linkID);
            }
        }

        return outputLinkIDs.ToArray();
    }
    #endregion

    #region For Screen Transition Node
    public static Guid[] FindScreenTranstionNodeInputLinks()
    {
        List<Guid> inputLinkIDs = new List<Guid>();
        var linkTable = PlotDataBuffer.Instance.PlotData.Value.LinkTable;
        foreach (Guid linkID in linkTable.Keys)
        {
            if (linkTable[linkID].ToNodeID == PlotData.ScreenTransitionNodeID)
            {
                inputLinkIDs.Add(linkID);
            }
        }

        return inputLinkIDs.ToArray();
    }
    #endregion
}