using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;


public sealed class PlotTreePopup : MonoBehaviour
{
    [Header("Plot Tree Popup")]
    [SerializeField] private GameObject m_layout_PlotTreePopup;

    
    public void Start()
    {
        m_layout_PlotTreePopup.SetActive(false);
    }
    public void Update()
    {
        if(ChiefUIManager.Instance.CurPageType == TotalEdgyToolBarControl.PageType.PlotTree)
        {
            if(Input.GetMouseButtonDown(1))
            {
                m_layout_PlotTreePopup.SetActive(true);

                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 0.0f;
                mousePos.x += 175f;
                mousePos.y -= 250f;

                m_layout_PlotTreePopup.transform.position = mousePos;
            }
            if(Input.GetMouseButtonDown(0))
            {
                Invoke("TurnOffPopup", 0.25f);
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                if(PlotTreeActiveUIControl.Instance.CurSelectedNodeID != Guid.Empty)
                {
                    DeleteSceneItem();
                }
                else if(PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName != string.Empty)
                {
                    DeleteEnterEventItem();
                }
                else if (PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex != -1)
                {
                    DeleteSceneTransitionItem();
                }
            }
        }
    }

    #region Unity Callbacks
    // Scene Item
    public void CreateNewSceneItem()
    {
        if(!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        Guid newSceneID = Guid.NewGuid();
        curPlotData.SceneTable.Add(newSceneID, new SceneData()
        {
            SceneName = $"New Scene - {UnityEngine.Random.Range(0, short.MaxValue)}",
            Frames = new List<FrameData>(),
            EditorSceneItemPos = new SerializableVector3(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)))
        });

        PlotDataBuffer.Instance.PlotData = curPlotData;
        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.Render();
    }
    public void DeleteSceneItem()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }
        else if (PlotTreeActiveUIControl.Instance.CurSelectedNodeID == Guid.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        curPlotData.SceneTable.Remove(PlotTreeActiveUIControl.Instance.CurSelectedNodeID);

        List<Guid> removalLinkIDs = new List<Guid>();
        foreach (Guid linkID in curPlotData.LinkTable.Keys)
        {
            PlotData.LinkData curLinkData = curPlotData.LinkTable[linkID];
            if (curLinkData.FromNodeID == PlotTreeActiveUIControl.Instance.CurSelectedNodeID)
            {
                removalLinkIDs.Add(linkID);
            }
            if (curLinkData.ToNodeID == PlotTreeActiveUIControl.Instance.CurSelectedNodeID)
            {
                removalLinkIDs.Add(linkID);
            }
        }
        foreach (Guid linkID in removalLinkIDs)
        {
            curPlotData.LinkTable.Remove(linkID);
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.CurSelectedNodeID = Guid.Empty;

        ChiefUIManager.Instance.RenderAll();
    }

    // Scene Transition Item
    public void CreateNewSceneTransitionItem()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        curPlotData.ScreenTransitionNodePos.Add(new SerializableVector3(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f))));

        PlotDataBuffer.Instance.PlotData = curPlotData;
        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.Render();
    }
    public void DeleteSceneTransitionItem()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }
        else if (PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex == -1)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        curPlotData.ScreenTransitionNodePos.RemoveAt(PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex);

        List<Guid> removalLinkIDs = new List<Guid>();
        //Dictionary<Guid, SerializableVector3> screenTransitionLinkOriginTable = new Dictionary<Guid, SerializableVector3>();
        foreach (Guid linkID in curPlotData.LinkTable.Keys)
        {
            PlotData.LinkData curLinkData = curPlotData.LinkTable[linkID];
            //if (curLinkData.FromLinkCondition.HasValue
            //    && curLinkData.FromLinkCondition.Value.CurScreenTransitionNodeLinkCondition.HasValue
            //    && curLinkData.FromLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex == PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex)
            //{
            //    removalLinkIDs.Add(linkID);
            //}
            if (curLinkData.ToLinkCondition.HasValue
                && curLinkData.ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.HasValue
                && curLinkData.ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex == PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex)
            {
                //screenTransitionLinkOriginTable.Add(linkID, curPlotData.ScreenTransitionNodePos[PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex]);
                removalLinkIDs.Add(linkID);
            }
        }

        var linkIDs = (Guid[])curPlotData.LinkTable.Keys.ToArray().Clone();
        foreach (Guid linkID in linkIDs)
        {
            if (curPlotData.LinkTable[linkID].ToNodeID == PlotData.ScreenTransitionNodeID
                && !removalLinkIDs.Contains(linkID)
                && curPlotData.LinkTable[linkID].ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex > curPlotData.LinkTable[removalLinkIDs[0]].ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex)
            {
                curPlotData.LinkTable[linkID] = new PlotData.LinkData()
                {
                    FromNodeType = curPlotData.LinkTable[linkID].FromNodeType,
                    ToNodeType = curPlotData.LinkTable[linkID].ToNodeType,

                    FromNodeID = curPlotData.LinkTable[linkID].FromNodeID,
                    ToNodeID = curPlotData.LinkTable[linkID].ToNodeID,

                    FromLinkCondition = curPlotData.LinkTable[linkID].FromLinkCondition,
                    ToLinkCondition = new PlotData.LinkData.LinkCondition()
                    {
                        CurScreenTransitionNodeLinkCondition = new PlotData.LinkData.LinkCondition.ScreenTransitionNodeLinkCondition()
                        {
                            ScreenTransitionIndex = curPlotData.LinkTable[linkID].ToLinkCondition.Value.CurScreenTransitionNodeLinkCondition.Value.ScreenTransitionIndex - 1
                        }
                    },

                    GlobalVariableConditionTable = curPlotData.LinkTable[linkID].GlobalVariableConditionTable,
                    PostEffectTable = curPlotData.LinkTable[linkID].PostEffectTable
                };
            }
        }

        foreach (Guid linkID in removalLinkIDs)
        {
            curPlotData.LinkTable.Remove(linkID);
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.CurScreenTransitionNodeIndex = -1;

        ChiefUIManager.Instance.RenderAll();
    }

    // Enter Event Item
    public void CreateNewEnterEventItem()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

        string newEnterEventName = $"New Enter Event - {UnityEngine.Random.Range(0, short.MaxValue)}";
        curPlotData.EnterEventTable.Add(newEnterEventName, new SerializableVector3(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f))));

        PlotDataBuffer.Instance.PlotData = curPlotData;
        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void DeleteEnterEventItem()
    {
        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }
        else if (PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName == string.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        curPlotData.EnterEventTable.Remove(PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName);

        List<Guid> removalLinkIDs = new List<Guid>();
        foreach (Guid linkID in curPlotData.LinkTable.Keys)
        {
            PlotData.LinkData curLinkData = curPlotData.LinkTable[linkID];
            if (curLinkData.FromLinkCondition.HasValue
                && curLinkData.FromLinkCondition.Value.CurEnterEventNodeLinkCondition.HasValue
                && curLinkData.FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName == PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName)
            {
                removalLinkIDs.Add(linkID);
            }
            if (curLinkData.ToLinkCondition.HasValue
                && curLinkData.ToLinkCondition.Value.CurEnterEventNodeLinkCondition.HasValue
                && curLinkData.ToLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName == PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName)
            {
                removalLinkIDs.Add(linkID);
            }
        }
        foreach (Guid linkID in removalLinkIDs)
        {
            curPlotData.LinkTable.Remove(linkID);
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName = string.Empty;

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    private void TurnOffPopup()
    {
        m_layout_PlotTreePopup.SetActive(false);
    }
}