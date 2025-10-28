using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public class PlotTreeActiveUIControl : SingleTonForGameObject<PlotTreeActiveUIControl>
{
    #region Scene Node
    [Header("Scene Node")]
    [SerializeField] private GameObject m_prefab_SceneItem;
    [SerializeField] private Transform m_transform_SceneItemParent;

    private Dictionary<Guid, SceneItem> m_sceneItemTable = new Dictionary<Guid, SceneItem>();
    #endregion

    #region Screen Transition Node
    [Header("Screen Transition Node")]
    [SerializeField] private GameObject m_prefab_ScreenTransitionItem;
    [SerializeField] private Transform m_transform_ScreenTransitionItemParent;

    private List<ScreenTransitionItem> m_screenTransitionItems = new List<ScreenTransitionItem>();
    #endregion

    #region Enter Event Node
    [Header("Enter Event Node")]
    [SerializeField] private GameObject m_prefab_EnterEventItem;
    [SerializeField] private Transform m_transform_EnterEventItemParent;

    private Dictionary<string, EnterEventItem> m_enterEventItemTable = new Dictionary<string, EnterEventItem>();
    #endregion

    #region Link Line
    [Header("Link Line")]
    [SerializeField] private GameObject m_prefab_LinkLineItem;
    [SerializeField] private Transform m_transform_LinkLineItemParent;

    private Dictionary<Guid, LinkLineItem> m_linkLineItemTable = new Dictionary<Guid, LinkLineItem>();
    #endregion

    #region Temp Link Line
    [Header("Temp Link Line")]
    [SerializeField] private Transform m_transform_TempLinkLine;
    [SerializeField] private Transform[] m_transform_TempLinkLinePoints;
    #endregion

    private Guid m_curSelectedNodeID = Guid.Empty;
    private string m_curSelectedEnterEventName = null;
    private int m_curScreenTransitionNodeIndex = -1;

    private Guid m_curSelectedLinkID = Guid.Empty;

    private bool m_bisFirstRender = true;
    private bool m_bisLineSelectable = false;


    public void Awake()
    {
        SetInstance(this);

        BIsTempLinkLineActive = false;
    }
    public void Update()
    {
        if(BIsTempLinkLineActive && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)))
        {
            BIsTempLinkLineActive = false;

            m_transform_TempLinkLinePoints[0].position = Vector3.zero;
            m_transform_TempLinkLinePoints[1].position = Vector3.zero;

            LinkBroker.Instance.BIsFromIsSelected = false;
            LinkBroker.Instance.BIsToIsSelected = false;

            LinkBroker.Instance.FromLinkSelectedInfo = default;
            LinkBroker.Instance.ToLinkSelectedInfo = default;

            return;
        }

        if (LinkBroker.Instance.BIsFromIsSelected && LinkBroker.Instance.BIsToIsSelected)
        {
            BIsTempLinkLineActive = false;
        }
        else if(LinkBroker.Instance.BIsFromIsSelected && !LinkBroker.Instance.BIsToIsSelected)
        {
            LinkBroker.LinkSelectedInfo fromLinkSelectedInfo = LinkBroker.Instance.FromLinkSelectedInfo;
            Vector3 fromSpotPos = Vector3.zero;
            if(fromLinkSelectedInfo.TargetSceneNodeID != Guid.Empty && fromLinkSelectedInfo.TargetSceneNodeBranchIndex != -1)
            {
                fromSpotPos = GetSceneItem(fromLinkSelectedInfo.TargetSceneNodeID).OutputParts[fromLinkSelectedInfo.TargetSceneNodeBranchIndex].SpotPos;
            }
            else if(!string.IsNullOrEmpty(fromLinkSelectedInfo.TargetEnterEventName))
            {
                fromSpotPos = GetEnterEventItem(fromLinkSelectedInfo.TargetEnterEventName).OutputPartSpot.SpotPos;
            }
            else if(fromLinkSelectedInfo.TargetScreenTransitionIndex != -1)
            {
                throw new InvalidOperationException("Target Screen Transition Index should not be set when FromLinkSelectedInfo is used.");
            }

            m_transform_TempLinkLinePoints[0].position = fromSpotPos * 10.0f;
            m_transform_TempLinkLinePoints[1].position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            BIsTempLinkLineActive = true;
        }
        else if(!LinkBroker.Instance.BIsFromIsSelected && LinkBroker.Instance.BIsToIsSelected)
        {
            LinkBroker.LinkSelectedInfo toLinkSelectedInfo = LinkBroker.Instance.ToLinkSelectedInfo;
            Vector3 toSpotPos = Vector3.zero;
            if (toLinkSelectedInfo.TargetSceneNodeID != Guid.Empty && toLinkSelectedInfo.TargetSceneNodeBranchIndex != -1)
            {
                toSpotPos = GetSceneItem(toLinkSelectedInfo.TargetSceneNodeID).OutputParts[toLinkSelectedInfo.TargetSceneNodeBranchIndex].SpotPos;
            }
            else if (!string.IsNullOrEmpty(toLinkSelectedInfo.TargetEnterEventName))
            {
                throw new InvalidOperationException("Target Enter Event Name should not be set when ToLinkSelectedInfo is used.");
            }
            else if (toLinkSelectedInfo.TargetScreenTransitionIndex != -1)
            {
                toSpotPos = GetScreenTransitionItem(toLinkSelectedInfo.TargetScreenTransitionIndex).InputPartSpot.SpotPos;
            }

            m_transform_TempLinkLinePoints[0].position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_transform_TempLinkLinePoints[1].position = toSpotPos * 10.0f;

            BIsTempLinkLineActive = true;
        }
        else
        {
            BIsTempLinkLineActive = false;
        }

        if(CurSelectedLinkID != Guid.Empty && PlotDataBuffer.Instance.PlotData.HasValue && Input.GetKeyDown(KeyCode.Delete))
        {
            PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
            curPlotData.LinkTable.Remove(CurSelectedLinkID);
            PlotDataBuffer.Instance.PlotData = curPlotData;
            PlotDataBuffer.Instance.TrySaveData();

            m_curSelectedLinkID = Guid.Empty;

            ChiefUIManager.Instance.RenderAll();
        }

        if(m_bisLineSelectable)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.GetComponent<LinkLineItem>() != null)
                {
                    m_curSelectedLinkID = hit.collider.gameObject.GetComponent<LinkLineItem>().LinkID;
                    ChiefUIManager.Instance.RenderAll();
                    return;
                }
            }
            m_bisLineSelectable = false;
        }
        else if (Input.GetMouseButtonUp(0) && !m_bisLineSelectable)
        {
            m_bisLineSelectable = false;
        }
        else if (Input.GetMouseButton(0) && !m_bisLineSelectable)
        {
            m_bisLineSelectable = true;
        }
    }

    internal Guid CurSelectedNodeID
    {
        get
        {
            return m_curSelectedNodeID;
        }
        set
        {
            m_curSelectedEnterEventName = string.Empty;
            m_curScreenTransitionNodeIndex = -1;
            m_curSelectedNodeID = value;
        }
    }
    internal string CurSelectedEnterEventName
    {
        get
        {
            return m_curSelectedEnterEventName;
        }
        set
        {
            m_curSelectedNodeID = Guid.Empty;
            m_curScreenTransitionNodeIndex = -1;
            m_curSelectedEnterEventName = value;
        }
    }
    internal int CurScreenTransitionNodeIndex
    {
        get
        {
            return m_curScreenTransitionNodeIndex;
        }
        set
        {
            m_curSelectedNodeID = Guid.Empty;
            m_curSelectedEnterEventName = string.Empty;
            m_curScreenTransitionNodeIndex = value;
        }
    }

    internal Guid CurSelectedLinkID
    {
        get
        {
            return m_curSelectedLinkID;
        }
    }

    internal bool BIsTempLinkLineActive
    {
        get
        {
            return m_transform_TempLinkLine.gameObject.activeSelf;
        }
        set
        {
            m_transform_TempLinkLine.gameObject.SetActive(value);
        }
    }

    public void ClearCurSelectedNode()
    {
        m_curSelectedNodeID = Guid.Empty;
        m_curSelectedEnterEventName = string.Empty;
        m_curScreenTransitionNodeIndex = -1;

        //m_curSelectedLinkID = Guid.Empty;

        ChiefUIManager.Instance.RenderAll();
    }

    internal SceneItem GetSceneItem(in Guid targetSceneItem)
    {
        if(targetSceneItem == Guid.Empty)
        {
            throw new InvalidOperationException("No scene item is selected.");
        }

        return m_sceneItemTable[targetSceneItem];
    }
    internal EnterEventItem GetEnterEventItem(in string targetEnterEventName)
    {
        if (string.IsNullOrEmpty(targetEnterEventName))
        {
            throw new InvalidOperationException("No enter event item is selected.");
        }
        else if (!m_enterEventItemTable.ContainsKey(targetEnterEventName))
        {
            throw new KeyNotFoundException($"Enter event item with name '{targetEnterEventName}' does not exist.");
        }
        else if (m_enterEventItemTable[targetEnterEventName] == null)
        {
            throw new InvalidOperationException($"Enter event item with name '{targetEnterEventName}' is not initialized properly.");
        }

        return m_enterEventItemTable[targetEnterEventName];
    }
    internal ScreenTransitionItem GetScreenTransitionItem(in int targetScreenTransitionIndex)
    {
        if(targetScreenTransitionIndex == -1)
        {
            throw new InvalidOperationException("No screen transition item is selected.");
        }

        return m_screenTransitionItems[targetScreenTransitionIndex];
    }
    internal LinkLineItem GetLinkLineItem(in Guid targetLinkID)
    {
        if (targetLinkID == Guid.Empty)
        {
            throw new InvalidOperationException("No link line item is selected.");
        }
        return m_linkLineItemTable[targetLinkID];
    }

    internal void Render()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        #region Scene Node
        List<Guid> notReusedSceneIDs = m_sceneItemTable.Keys.ToList();
        foreach (Guid sceneID in curPlotData.SceneTable.Keys)
        {
            if (m_sceneItemTable.ContainsKey(sceneID))
            {
                m_sceneItemTable[sceneID].Render(sceneID);
                notReusedSceneIDs.Remove(sceneID);
            }
            else
            {
                SceneItem newSceneItem = Instantiate(m_prefab_SceneItem, m_transform_SceneItemParent).GetComponent<SceneItem>();
                newSceneItem.Render(sceneID);

                m_sceneItemTable.Add(sceneID, newSceneItem);
            }
        }

        foreach (Guid removalSceneID in notReusedSceneIDs)
        {
            Destroy(m_sceneItemTable[removalSceneID].gameObject);
            m_sceneItemTable.Remove(removalSceneID);
        }
        #endregion

        #region Enter Event Node
        //int childEnterEventCount = m_transform_EnterEventItemParent.childCount;
        //for (int index = childEnterEventCount - 1; index >= 0; index--)
        //{
        //    Destroy(m_transform_EnterEventItemParent.GetChild(index).gameObject);
        //}
        //m_enterEventItemTable.Clear();

        List<string> notReusedEnterEventNames = m_enterEventItemTable.Keys.ToList();
        foreach (string enterEventName in curPlotData.EnterEventTable.Keys)
        {
            if (m_enterEventItemTable.ContainsKey(enterEventName))
            {
                m_enterEventItemTable[enterEventName].Render(enterEventName);
                notReusedEnterEventNames.Remove(enterEventName);
            }
            else
            {
                EnterEventItem newEnterEventItem = Instantiate(m_prefab_EnterEventItem, m_transform_EnterEventItemParent).GetComponent<EnterEventItem>();
                newEnterEventItem.Render(enterEventName);

                m_enterEventItemTable.Add(enterEventName, newEnterEventItem);
            }
        }

        foreach (string removalEnterEventName in notReusedEnterEventNames)
        {
            Destroy(m_enterEventItemTable[removalEnterEventName].gameObject);
            m_enterEventItemTable.Remove(removalEnterEventName);
        }

        //var enterEventTable = curPlotData.EnterEventTable;
        //foreach (string enterEventName in enterEventTable.Keys)
        //{
        //    EnterEventItem newEnterEventItem = Instantiate(m_prefab_EnterEventItem, m_transform_EnterEventItemParent).GetComponent<EnterEventItem>();
        //    newEnterEventItem.Render(enterEventName);
        //    m_enterEventItemTable.Add(enterEventName, newEnterEventItem);
        //}
        #endregion

        #region Screen Transition Node
        int screenTransitionNodeObjectCount = m_transform_ScreenTransitionItemParent.childCount;
        for (int index = screenTransitionNodeObjectCount - 1; index >= 0; index--)
        {
            Destroy(m_transform_ScreenTransitionItemParent.GetChild(index).gameObject);
        }
        m_screenTransitionItems.Clear();

        int screenTransitionNodeCount = PlotDataBuffer.Instance.PlotData.Value.ScreenTransitionNodePos.Count;
        //int screenTransitionNodeCountDiff = screenTransitionNodeCount - m_screenTransitionItems.Count;
        //for (int index = 0; index < screenTransitionNodeCount; index++)
        //{
        //    if (index < m_screenTransitionItems.Count)
        //    {
        //        m_screenTransitionItems[index].Render(index);
        //    }
        //    else
        //    {
        //        ScreenTransitionItem newScreenTransitionItem = Instantiate(m_prefab_ScreenTransitionItem, m_transform_ScreenTransitionItemParent).GetComponent<ScreenTransitionItem>();
        //        newScreenTransitionItem.Render(index);

        //        m_screenTransitionItems.Add(newScreenTransitionItem);
        //    }
        //}

        //for (int count = 0; count < -screenTransitionNodeCountDiff; count++)
        //{
        //    m_screenTransitionItems.RemoveAt(m_screenTransitionItems.Count - 1);
        //}

        for (int index = 0; index < screenTransitionNodeCount; index++)
        {
            ScreenTransitionItem newScreenTransitionItem = Instantiate(m_prefab_ScreenTransitionItem, m_transform_ScreenTransitionItemParent).GetComponent<ScreenTransitionItem>();
            newScreenTransitionItem.Render(index);

            m_screenTransitionItems.Add(newScreenTransitionItem);
        }
        #endregion

        if (m_bisFirstRender)
        {
            m_bisFirstRender = false;
        }

        Invoke("RenderLinkLine", 0.25f);
    }
    internal void RenderLinkLine()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        List<Guid> notReusedLinkIDs = m_linkLineItemTable.Keys.ToList();
        foreach (Guid linkID in curPlotData.LinkTable.Keys)
        {
            if (m_linkLineItemTable.ContainsKey(linkID))
            {
                m_linkLineItemTable[linkID].Render(linkID);
                notReusedLinkIDs.Remove(linkID);
            }
            else
            {
                LinkLineItem newLinkLineItem = Instantiate(m_prefab_LinkLineItem, m_transform_LinkLineItemParent).GetComponent<LinkLineItem>();
                newLinkLineItem.Render(linkID);

                m_linkLineItemTable.Add(linkID, newLinkLineItem);
            }
        }

        foreach (Guid removalLinkID in notReusedLinkIDs)
        {
            Destroy(m_linkLineItemTable[removalLinkID].gameObject);
            m_linkLineItemTable.Remove(removalLinkID);
        }

        //int childLinkLineCount = m_transform_LinkLineItemParent.childCount;
        //for (int index = childLinkLineCount - 1; index >= 0; index--)
        //{
        //    Destroy(m_transform_LinkLineItemParent.GetChild(index).gameObject);
        //}
        //m_linkLineItemTable.Clear();

        //var curLinkTable = PlotDataBuffer.Instance.PlotData.Value.LinkTable;
        //foreach (Guid linkID in curLinkTable.Keys)
        //{
        //    LinkLineItem linkLineItem = Instantiate(m_prefab_LinkLineItem, m_transform_LinkLineItemParent).GetComponent<LinkLineItem>();
        //    linkLineItem.Render(linkID);

        //    m_linkLineItemTable.Add(linkID, linkLineItem);
        //}
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}