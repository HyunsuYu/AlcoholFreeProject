using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using System.Linq;
using System;


public sealed class BacklogControl : SingleTonForGameObject<BacklogControl>
{
    [SerializeField] private GameObject m_layout_Backlog;

    [SerializeField] private GameObject m_prefab_BacklogMainItem;
    [SerializeField] private GameObject m_prefab_BacklogLineItem;

    [SerializeField] private Transform m_transform_BacklogItemParent;

    private List<BacklogMainItem> backlogMainItems = new List<BacklogMainItem>();


    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void CloseBacklog()
    {
        m_layout_Backlog.SetActive(false);
        NoonCanvasControl.Instance.BIsBacklogOpened = false;

        NoonCanvasControl.Instance.BIsSkipBtnActive = true;
    }
    #endregion

    internal void Render()
    {
        var curBacklogIndexes = TempSaveBuffer.BacklogIndexes;

        List<Tuple<Guid, int>> need2AddBacklogIndexes = new List<Tuple<Guid, int>>();
        foreach (var backlogIndex in curBacklogIndexes)
        {
            if (!backlogMainItems.Any(item => item.BacklogIndex.Equals(backlogIndex)))
            {
                need2AddBacklogIndexes.Add(backlogIndex);
            }
        }

        foreach (var backlogIndex in need2AddBacklogIndexes)
        {
            if(backlogMainItems.Count > 0)
            {
                Instantiate(m_prefab_BacklogLineItem, m_transform_BacklogItemParent);
            }

            GameObject newItem = Instantiate(m_prefab_BacklogMainItem, m_transform_BacklogItemParent);
            BacklogMainItem newBacklogMainItem = newItem.GetComponent<BacklogMainItem>();
            newBacklogMainItem.Init(backlogIndex.Item1, backlogIndex.Item2);

            backlogMainItems.Add(newBacklogMainItem);
        }
    }
    internal void RenderTexts()
    {
        foreach(var backlogMainItem in backlogMainItems)
        {
            backlogMainItem.Render();
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}