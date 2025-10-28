using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlotTreeNodeLinkSpot : MonoBehaviour
{
    public enum LinkSpotType
    {
        Input,
        Output
    }


    [SerializeField] private LinkSpotType m_linkSpotType;

    [SerializeField] private TMP_Text m_text_LinkInfo;

    [SerializeField] private Image m_image_OuterLinkState;
    [SerializeField] private Image m_image_InnerLinkState;

    private Guid m_parentSceneNodeID = Guid.Empty;
    private int m_parentSceneNodeBranchIndex = -1;

    private string m_parentEnterEventName = string.Empty;
    private int m_parentScreenTransitionIndex = -1;

    private Guid m_sceneNodeLinkID = Guid.Empty;
    private Guid[] m_enterEventLinkIDs = null;
    private Guid[] m_screenTransitionLinkIDs = null;


    internal LinkSpotType CurLinkSpotType
    {
        get
        {
            return m_linkSpotType;
        }
    }

    internal Guid ParentSceneNodeID
    {
        get
        {
            return m_parentSceneNodeID;
        }
    }
    internal string ParentEnterEventName
    {
        get
        {
            return m_parentEnterEventName;
        }
    }
    internal int ParentSceneNodeBranchIndex
    {
        get
        {
            return m_parentSceneNodeBranchIndex;
        }
    }
    internal int ParentScreenTransitionIndex
    {
        get
        {
            return m_parentScreenTransitionIndex;
        }
    }

    internal Guid SceneNodeLinkID
    {
        get
        {
            return m_sceneNodeLinkID;
        }
    }
    internal Guid[] EnterEventLinkIDs
    {
        get
        {
            return m_enterEventLinkIDs;
        }
    }
    internal Guid[] ScreenTransitionLinkIDs
    {
        get
        {
            return m_screenTransitionLinkIDs;
        }
    }

    internal Vector3 SpotPos
    {
        get
        {
            if(m_parentSceneNodeID != Guid.Empty && PlotTreeActiveUIControl.Instance.GetSceneItem(m_parentSceneNodeID).BIsContentHidden)
            {
                switch(m_linkSpotType)
                {
                    case LinkSpotType.Input:
                        return PlotTreeActiveUIControl.Instance.GetSceneItem(m_parentSceneNodeID).HiddenInputSpot.position * 0.1f;

                    case LinkSpotType.Output:
                        return PlotTreeActiveUIControl.Instance.GetSceneItem(m_parentSceneNodeID).HiddenOutputSpot.position * 0.1f;
                }
            }
            else if(!string.IsNullOrEmpty(m_parentEnterEventName) && PlotTreeActiveUIControl.Instance.GetEnterEventItem(m_parentEnterEventName).BIsContentHidden)
            {
                return PlotTreeActiveUIControl.Instance.GetEnterEventItem(m_parentEnterEventName).HiddenOutputSpot.position * 0.1f;
            }
            else if(m_parentScreenTransitionIndex >= 0 && PlotTreeActiveUIControl.Instance.GetScreenTransitionItem(m_parentScreenTransitionIndex).BIsContentHidden)
            {
                return PlotTreeActiveUIControl.Instance.GetScreenTransitionItem(m_parentScreenTransitionIndex).HiddenInputSpot.position * 0.1f;
            }

            return m_image_InnerLinkState.transform.position * 0.1f;
        }
    }

    internal void Render(in Guid parentSceneNodeID, in int parentSceneNodeBranchIndex, in Guid linkID)
    {
        m_parentSceneNodeID = parentSceneNodeID;
        m_parentSceneNodeBranchIndex = parentSceneNodeBranchIndex;

        m_parentEnterEventName = string.Empty;
        m_parentScreenTransitionIndex = -1;

        m_sceneNodeLinkID = linkID;
        m_enterEventLinkIDs = null;
        m_screenTransitionLinkIDs = null;

        if (linkID == Guid.Empty)
        {
            m_text_LinkInfo.text = "연결된 노드 없음";

            m_image_OuterLinkState.color = Color.white;
            m_image_InnerLinkState.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            return;
        }

        switch (m_linkSpotType)
        {
            case LinkSpotType.Input:
                Guid fromNodeID = PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].FromNodeID;
                if(fromNodeID == PlotData.EnterEventNodeID)
                {
                    string fromEnterEventName = PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName;
                    m_text_LinkInfo.text = $"[{fromEnterEventName}]로부터 진입";
                }
                else
                {
                    m_text_LinkInfo.text = $"[{PlotDataBuffer.Instance.PlotData.Value.SceneTable[fromNodeID].SceneName}]로부터 진입";
                }
                break;

            case LinkSpotType.Output:
                Guid toNodeID = PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].ToNodeID;
                if (toNodeID == PlotData.ScreenTransitionNodeID)
                {
                    m_text_LinkInfo.text = "이후 화면 전환";
                }
                else
                {
                    m_text_LinkInfo.text = $"이후 [{PlotDataBuffer.Instance.PlotData.Value.SceneTable[toNodeID].SceneName}]로 전환";
                }
                break;
        }

        m_image_OuterLinkState.color = ChiefUIManager.SelectedColor;
        m_image_InnerLinkState.color = ChiefUIManager.SelectedColor;
    }
    internal void Render(in string parentEnterEventName, in Guid[] linkIDs)
    {
        m_parentSceneNodeID = Guid.Empty;
        m_parentEnterEventName = parentEnterEventName;
        m_parentSceneNodeBranchIndex = -1;
        m_parentScreenTransitionIndex = -1;

        m_sceneNodeLinkID = Guid.Empty;
        m_enterEventLinkIDs = linkIDs;
        m_screenTransitionLinkIDs = null;

        if (linkIDs.Length == 0)
        {
            m_text_LinkInfo.text = "연결된 노드 없음";

            m_image_OuterLinkState.color = Color.white;
            m_image_InnerLinkState.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            return;
        }

        m_text_LinkInfo.text = $"이후 {linkIDs.Length}개의 노드로 전환";

        m_image_OuterLinkState.color = ChiefUIManager.SelectedColor;
        m_image_InnerLinkState.color = ChiefUIManager.SelectedColor;
    }
    internal void Render(in int parentScreenTransitionIndex, in Guid[] toScreenTransitionNodeLinkIDs)
    {
        m_parentSceneNodeID = Guid.Empty;
        m_parentEnterEventName = string.Empty;
        m_parentSceneNodeBranchIndex = -1;
        m_parentScreenTransitionIndex = parentScreenTransitionIndex;

        m_sceneNodeLinkID = Guid.Empty;
        m_enterEventLinkIDs = null;
        m_screenTransitionLinkIDs = toScreenTransitionNodeLinkIDs;

        if (m_linkSpotType == LinkSpotType.Output)
        {
            throw new InvalidOperationException("This method should not be called for Output LinkSpotType in case 'Screen Transtion Node'");
        }

        m_text_LinkInfo.text = "이후 화면 전환";
    }
}