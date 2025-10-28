using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


internal sealed class LinkBroker : SingleTon<LinkBroker>
{
    public struct LinkSelectedInfo
    {
        public Guid TargetSceneNodeID;
        public int TargetSceneNodeBranchIndex;

        public string TargetEnterEventName;
        public int TargetScreenTransitionIndex;
    }


    private bool m_bisFromIsSelected = false;
    private bool m_bisToIsSelected = false;

    private LinkSelectedInfo m_fromLinkSelectedInfo;
    private LinkSelectedInfo m_toLinkSelectedInfo;


    internal bool BIsFromIsSelected
    {
        get
        {
            return m_bisFromIsSelected;
        }
        set
        {
            m_bisFromIsSelected = value;
        }
    }
    internal bool BIsToIsSelected
    {
        get
        {
            return m_bisToIsSelected;
        }
        set
        {
            m_bisToIsSelected = value;
        }
    }

    internal LinkSelectedInfo FromLinkSelectedInfo
    {
        get
        {
            return m_fromLinkSelectedInfo;
        }
        set
        {
            m_fromLinkSelectedInfo = value;
        }
    }
    internal LinkSelectedInfo ToLinkSelectedInfo
    {
        get
        {
            return m_toLinkSelectedInfo;
        }
        set
        {
            m_toLinkSelectedInfo = value;
        }
    }

    internal void SetFromLinkSelectedInfo_SceneNode(in Guid sceneNodeID)
    {
        m_bisFromIsSelected = true;
        m_fromLinkSelectedInfo.TargetSceneNodeID = sceneNodeID;
        m_fromLinkSelectedInfo.TargetEnterEventName = string.Empty;
        m_fromLinkSelectedInfo.TargetScreenTransitionIndex = -1;
    }
    internal void SetFromLinkSelectedInfo_EnterEventNode(in string enterEventName)
    {
        m_bisFromIsSelected = true;
        m_fromLinkSelectedInfo.TargetSceneNodeID = Guid.Empty;
        m_fromLinkSelectedInfo.TargetEnterEventName = enterEventName;
        m_fromLinkSelectedInfo.TargetScreenTransitionIndex = -1;
    }
    internal void SetFromLinkSelectedInfo_ScreenTransitionNode(in int screenTransitionIndex)
    {
        m_bisFromIsSelected = true;
        m_fromLinkSelectedInfo.TargetSceneNodeID = Guid.Empty;
        m_fromLinkSelectedInfo.TargetEnterEventName = string.Empty;
        m_fromLinkSelectedInfo.TargetScreenTransitionIndex = screenTransitionIndex;
    }

    internal void SetToLinkSelectedInfo_SceneNode(in Guid sceneNodeID)
    {
        m_bisToIsSelected = true;
        m_toLinkSelectedInfo.TargetSceneNodeID = sceneNodeID;
        m_toLinkSelectedInfo.TargetEnterEventName = string.Empty;
        m_toLinkSelectedInfo.TargetScreenTransitionIndex = -1;
    }
    internal void SetToLinkSelectedInfo_EnterEventNode(in string enterEventName)
    {
        m_bisToIsSelected = true;
        m_toLinkSelectedInfo.TargetSceneNodeID = Guid.Empty;
        m_toLinkSelectedInfo.TargetEnterEventName = enterEventName;
        m_toLinkSelectedInfo.TargetScreenTransitionIndex = -1;
    }
    internal void SetToLinkSelectedInfo_ScreenTransitionNode(in int screenTransitionIndex)
    {
        m_bisToIsSelected = true;
        m_toLinkSelectedInfo.TargetSceneNodeID = Guid.Empty;
        m_toLinkSelectedInfo.TargetEnterEventName = string.Empty;
        m_toLinkSelectedInfo.TargetScreenTransitionIndex = screenTransitionIndex;
    }

    internal void ClearFromLinkSelectedInfo()
    {
        m_bisFromIsSelected = false;
        m_fromLinkSelectedInfo = new LinkSelectedInfo();
    }
    internal void ClearToLinkSelectedInfo()
    {
        m_bisToIsSelected = false;
        m_toLinkSelectedInfo = new LinkSelectedInfo();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}