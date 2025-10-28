using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class LinkSpot : MonoBehaviour
{
    [SerializeField] private PlotTreeNodeLinkSpot m_parent;


    #region Unity Callbacks
    public void SelectLinkSpot()
    {
        // 아직 연결이 안 된 상태, 즉, Scene Node의 Output 쪽은 연결이 되어 있지만 상대쪽이 아직 연결이 안되어서 그쪽은 마우스 커서에 따라다니는 상태
        switch (m_parent.CurLinkSpotType)
        {
            case PlotTreeNodeLinkSpot.LinkSpotType.Input:
                if (m_parent.ParentSceneNodeID != Guid.Empty /*&& m_parent.SceneNodeLinkID == Guid.Empty*/)
                {
                    LinkBroker.Instance.ToLinkSelectedInfo = new LinkBroker.LinkSelectedInfo()
                    {
                        TargetSceneNodeID = m_parent.ParentSceneNodeID,
                        TargetSceneNodeBranchIndex = m_parent.ParentSceneNodeBranchIndex,

                        TargetEnterEventName = string.Empty,
                        TargetScreenTransitionIndex = -1
                    };
                    LinkBroker.Instance.BIsToIsSelected = true;
                }
                else if (!string.IsNullOrEmpty(m_parent.ParentEnterEventName) && m_parent.EnterEventLinkIDs != null /*&& m_parent.EnterEventLinkIDs.Length == 0*/)
                {
                    throw new InvalidOperationException("EnterEventLinkIDs should not be empty when ParentEnterEventName is valid.");
                }
                else if (m_parent.ParentScreenTransitionIndex >= 0 && m_parent.ScreenTransitionLinkIDs != null /*&& m_parent.ScreenTransitionLinkIDs.Length == 0*/)
                {
                    LinkBroker.Instance.ToLinkSelectedInfo = new LinkBroker.LinkSelectedInfo()
                    {
                        TargetSceneNodeID = Guid.Empty,
                        TargetSceneNodeBranchIndex = -1,

                        TargetEnterEventName = string.Empty,
                        TargetScreenTransitionIndex = m_parent.ParentScreenTransitionIndex
                    };
                    LinkBroker.Instance.BIsToIsSelected = true;
                }
                break;

            case PlotTreeNodeLinkSpot.LinkSpotType.Output:
                if (m_parent.ParentSceneNodeID != Guid.Empty && m_parent.SceneNodeLinkID == Guid.Empty)
                {
                    LinkBroker.Instance.FromLinkSelectedInfo = new LinkBroker.LinkSelectedInfo()
                    {
                        TargetSceneNodeID = m_parent.ParentSceneNodeID,
                        TargetSceneNodeBranchIndex = m_parent.ParentSceneNodeBranchIndex,

                        TargetEnterEventName = string.Empty,
                        TargetScreenTransitionIndex = -1
                    };
                    LinkBroker.Instance.BIsFromIsSelected = true;
                }
                else if (!string.IsNullOrEmpty(m_parent.ParentEnterEventName) && m_parent.EnterEventLinkIDs != null /*&& m_parent.EnterEventLinkIDs.Length == 0*/)
                {
                    LinkBroker.Instance.FromLinkSelectedInfo = new LinkBroker.LinkSelectedInfo()
                    {
                        TargetSceneNodeID = Guid.Empty,
                        TargetSceneNodeBranchIndex = -1,

                        TargetEnterEventName = m_parent.ParentEnterEventName,
                        TargetScreenTransitionIndex = -1
                    };
                    LinkBroker.Instance.BIsFromIsSelected = true;
                }
                else if (m_parent.ParentScreenTransitionIndex >= 0 && m_parent.ScreenTransitionLinkIDs != null /*&& m_parent.ScreenTransitionLinkIDs.Length == 0*/)
                {
                    throw new InvalidOperationException("ScreenTransitionLinkIDs should not be empty when ParentEnterEventBranchIndex is valid.");
                }
                break;
        }

        if(LinkBroker.Instance.BIsFromIsSelected && LinkBroker.Instance.BIsToIsSelected)
        {
            LinkBroker.LinkSelectedInfo fromLinkSelectedInfo = LinkBroker.Instance.FromLinkSelectedInfo;

            PlotData.LinkNodeType fromNodeType = PlotData.LinkNodeType.SceneNode;
            Guid fromNodeID = Guid.Empty;
            PlotData.LinkData.LinkCondition fromLinkCondition = default;
            if (fromLinkSelectedInfo.TargetSceneNodeID != Guid.Empty && fromLinkSelectedInfo.TargetSceneNodeBranchIndex != -1)
            {
                fromNodeType = PlotData.LinkNodeType.SceneNode;
                fromNodeID = fromLinkSelectedInfo.TargetSceneNodeID;
                fromLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = new PlotData.LinkData.LinkCondition.SceneNodeLinkCondition()
                    {
                        FromBranchIndex = fromLinkSelectedInfo.TargetSceneNodeBranchIndex
                    },
                    CurEnterEventNodeLinkCondition = null,
                    CurScreenTransitionNodeLinkCondition = null
                };
            }
            else if (!string.IsNullOrEmpty(fromLinkSelectedInfo.TargetEnterEventName))
            {
                fromNodeType = PlotData.LinkNodeType.EnterEventNode;
                fromNodeID = PlotData.EnterEventNodeID;
                fromLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = null,
                    CurEnterEventNodeLinkCondition = new PlotData.LinkData.LinkCondition.EnterEventNodeLinkCondition()
                    {
                        EnterEventName = fromLinkSelectedInfo.TargetEnterEventName
                    },
                    CurScreenTransitionNodeLinkCondition = null
                };
            }
            else if (fromLinkSelectedInfo.TargetScreenTransitionIndex != -1)
            {
                fromNodeType = PlotData.LinkNodeType.ScreenTransitionNode;
                fromNodeID = PlotData.ScreenTransitionNodeID;
                fromLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = null,
                    CurEnterEventNodeLinkCondition = null,
                    CurScreenTransitionNodeLinkCondition = new PlotData.LinkData.LinkCondition.ScreenTransitionNodeLinkCondition()
                    {
                        ScreenTransitionIndex = fromLinkSelectedInfo.TargetScreenTransitionIndex
                    }
                };
            }

            LinkBroker.LinkSelectedInfo toLinkSelectedInfo = LinkBroker.Instance.ToLinkSelectedInfo;

            PlotData.LinkNodeType tpNodeType = PlotData.LinkNodeType.SceneNode;
            Guid toNodeID = Guid.Empty;
            PlotData.LinkData.LinkCondition toLinkCondition = default;
            if (toLinkSelectedInfo.TargetSceneNodeID != Guid.Empty && toLinkSelectedInfo.TargetSceneNodeBranchIndex != -1)
            {
                tpNodeType = PlotData.LinkNodeType.SceneNode;
                toNodeID = toLinkSelectedInfo.TargetSceneNodeID;
                toLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = new PlotData.LinkData.LinkCondition.SceneNodeLinkCondition()
                    {
                        FromBranchIndex = toLinkSelectedInfo.TargetSceneNodeBranchIndex
                    },
                    CurEnterEventNodeLinkCondition = null,
                    CurScreenTransitionNodeLinkCondition = null
                };
            }
            else if (!string.IsNullOrEmpty(toLinkSelectedInfo.TargetEnterEventName))
            {
                tpNodeType = PlotData.LinkNodeType.EnterEventNode;
                toNodeID = PlotData.EnterEventNodeID;
                toLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = null,
                    CurEnterEventNodeLinkCondition = new PlotData.LinkData.LinkCondition.EnterEventNodeLinkCondition()
                    {
                        EnterEventName = toLinkSelectedInfo.TargetEnterEventName
                    },
                    CurScreenTransitionNodeLinkCondition = null
                };
            }
            else if (toLinkSelectedInfo.TargetScreenTransitionIndex != -1)
            {
                tpNodeType = PlotData.LinkNodeType.ScreenTransitionNode;
                toNodeID = PlotData.ScreenTransitionNodeID;
                toLinkCondition = new PlotData.LinkData.LinkCondition()
                {
                    CurSceneNodeLinkCondition = null,
                    CurEnterEventNodeLinkCondition = null,
                    CurScreenTransitionNodeLinkCondition = new PlotData.LinkData.LinkCondition.ScreenTransitionNodeLinkCondition()
                    {
                        ScreenTransitionIndex = toLinkSelectedInfo.TargetScreenTransitionIndex
                    }
                };
            }

            PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
            curPlotData.LinkTable.Add(Guid.NewGuid(), new PlotData.LinkData()
            {
                FromNodeType = fromNodeType,
                ToNodeType = tpNodeType,

                FromNodeID = fromNodeID,
                ToNodeID = toNodeID,

                FromLinkCondition = fromLinkCondition,
                ToLinkCondition = toLinkCondition,

                GlobalVariableConditionTable = new Dictionary<string, Tuple<PlotData.LinkData.LinkConditionCompareType, int>>(),
                PostEffectTable = new Dictionary<string, Tuple<PlotData.LinkData.LinkConditionChangeType, int>>()
            });

            PlotDataBuffer.Instance.PlotData = curPlotData;

            PlotDataBuffer.Instance.TrySaveData();

            LinkBroker.Instance.BIsFromIsSelected = false;
            LinkBroker.Instance.BIsToIsSelected = false;

            LinkBroker.Instance.FromLinkSelectedInfo = default;
            LinkBroker.Instance.ToLinkSelectedInfo = default;

            PlotTreeActiveUIControl.Instance.Render();
        }
    }
    #endregion
}