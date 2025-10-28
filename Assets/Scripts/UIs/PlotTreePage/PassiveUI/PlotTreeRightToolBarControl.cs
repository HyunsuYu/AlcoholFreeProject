using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

using CommonUtilLib.ThreadSafe;


public class PlotTreeRightToolBarControl : SingleTonForGameObject<PlotTreeRightToolBarControl>
{
    [SerializeField] private Transform m_Layout_LinkDetailInfo;

    [SerializeField] private GlobalVariableDesc m_globalVariableDesc;

    [Header("Global Variable Conditions")]
    [SerializeField] private GameObject m_prefab_GlobalVariableCondition;
    [SerializeField] private Transform m_transform_GlobalVariableConditionParent;

    private IObjectPool<GlobalVariableConditionItem> m_pool_GlobalVariableConditionitem;

    private List<GlobalVariableConditionItem> m_list_GlobalVariableConditionItems = new List<GlobalVariableConditionItem>();

    [Header("Post Effects")]
    [SerializeField] private GameObject m_prefab_PostEffect;
    [SerializeField] private Transform m_transform_PostEffectParent;

    private IObjectPool<PostEffectItem> m_pool_PostEffectItem;

    private List<PostEffectItem> m_list_PostEffectItems = new List<PostEffectItem>();


    public void Awake()
    {
        SetInstance(this);

        m_pool_GlobalVariableConditionitem = new ObjectPool<GlobalVariableConditionItem>(CreateGlobalVariableConditionItem, OnTakeGlobalVariableConditionItemFromPool, OnReturnedGlobalVariableConditionItemToPool, OnDestroyGlobalVariableConditionItem, true, 10, 100);
        m_pool_PostEffectItem = new ObjectPool<PostEffectItem>(CreatePostEffectItem, OnTakePostEffectItemFromPool, OnReturnedPostEffectItemToPool, OnDestroyPostEffectItem, true, 10, 100);
    }
    public void Start()
    {
        //m_pool_GlobalVariableConditionitem = new ObjectPool<GlobalVariableConditionItem>(CreateGlobalVariableConditionItem, OnTakeGlobalVariableConditionItemFromPool, OnReturnedGlobalVariableConditionItemToPool, OnDestroyGlobalVariableConditionItem, true, 10, 100);
        //m_pool_PostEffectItem = new ObjectPool<PostEffectItem>(CreatePostEffectItem, OnTakePostEffectItemFromPool, OnReturnedPostEffectItemToPool, OnDestroyPostEffectItem, true, 10, 100);
    }

    #region Unity Callbacks
    /// <summary>
    /// 만약 추가로 선택 가능한 Global Variable이 없다면 그냥 return
    /// </summary>
    public void TryAddGlobalVariableConditionItem()
    {
        if(PlotTreeActiveUIControl.Instance.CurSelectedLinkID == Guid.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        if(linkData.GlobalVariableConditionTable.Count == m_globalVariableDesc.GlobalVariables.Length)
        {
            return;
        }

        string targetNewGlobalVariable = string.Empty;
        foreach (string globalVariable in m_globalVariableDesc.GlobalVariables)
        {
            if(!linkData.GlobalVariableConditionTable.ContainsKey(globalVariable))
            {
                targetNewGlobalVariable = globalVariable;
            }
        }

        linkData.GlobalVariableConditionTable.Add(targetNewGlobalVariable, new Tuple<PlotData.LinkData.LinkConditionCompareType, int>(PlotData.LinkData.LinkConditionCompareType.Equal, 0));
        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        Render();
    }
    public void TryClearGlobalVariableConditionItems()
    {
        if (PlotTreeActiveUIControl.Instance.CurSelectedLinkID == Guid.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.GlobalVariableConditionTable.Clear();

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        Render();
    }

    public void TryAddPostEffectItem()
    {
        if (PlotTreeActiveUIControl.Instance.CurSelectedLinkID == Guid.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        if (linkData.PostEffectTable.Count == m_globalVariableDesc.GlobalVariables.Length)
        {
            return;
        }

        string targetPostEffect = string.Empty;
        foreach (string globalVariable in m_globalVariableDesc.GlobalVariables)
        {
            if (!linkData.PostEffectTable.ContainsKey(globalVariable))
            {
                targetPostEffect = globalVariable;
            }
        }

        linkData.PostEffectTable.Add(targetPostEffect, new Tuple<PlotData.LinkData.LinkConditionChangeType, int>(PlotData.LinkData.LinkConditionChangeType.Set, 0));
        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        Render();
    }
    public void TryClearPostEffectItems()
    {
        if (PlotTreeActiveUIControl.Instance.CurSelectedLinkID == Guid.Empty)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        PlotData.LinkData linkData = curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

        linkData.PostEffectTable.Clear();

        curPlotData.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID] = linkData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        Render();
    }
    #endregion

    internal void Render()
    {
        if (PlotTreeActiveUIControl.Instance.CurSelectedLinkID == Guid.Empty)
        {
            m_Layout_LinkDetailInfo.gameObject.SetActive(false);
        }
        else
        {
            m_Layout_LinkDetailInfo.gameObject.SetActive(false);

            var linkData = PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotTreeActiveUIControl.Instance.CurSelectedLinkID];

            #region Global Variable Conditions
            for (int index = m_list_GlobalVariableConditionItems.Count - 1; index >= 0; index--)
            {
                m_pool_GlobalVariableConditionitem.Release(m_list_GlobalVariableConditionItems[index]);
            }
            m_list_GlobalVariableConditionItems.Clear();

            foreach(string globalVariable in linkData.GlobalVariableConditionTable.Keys)
            {
                GlobalVariableConditionItem item = m_pool_GlobalVariableConditionitem.Get();
                item.transform.SetParent(m_transform_GlobalVariableConditionParent, false);

                item.Render(globalVariable, linkData.GlobalVariableConditionTable[globalVariable].Item1, linkData.GlobalVariableConditionTable[globalVariable].Item2);
                m_list_GlobalVariableConditionItems.Add(item);
            }
            #endregion

            #region Post Effects
            for(int index = m_list_PostEffectItems.Count - 1; index >= 0; index--)
            {
                m_pool_PostEffectItem.Release(m_list_PostEffectItems[index]);
            }
            m_list_PostEffectItems.Clear();

            foreach (string postEffect in linkData.PostEffectTable.Keys)
            {
                PostEffectItem item = m_pool_PostEffectItem.Get();
                item.transform.SetParent(m_transform_PostEffectParent, false);

                item.Render(postEffect, linkData.PostEffectTable[postEffect].Item1, linkData.PostEffectTable[postEffect].Item2);
                m_list_PostEffectItems.Add(item);
            }
            #endregion

            m_Layout_LinkDetailInfo.gameObject.SetActive(true);
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    #region Global Variable Conditions
    private GlobalVariableConditionItem CreateGlobalVariableConditionItem()
    {
        return Instantiate(m_prefab_GlobalVariableCondition, m_transform_GlobalVariableConditionParent).GetComponent<GlobalVariableConditionItem>();
    }
    private void OnReturnedGlobalVariableConditionItemToPool(GlobalVariableConditionItem item)
    {
        item.gameObject.SetActive(false);
    }
    private void OnTakeGlobalVariableConditionItemFromPool(GlobalVariableConditionItem item)
    {
        item.gameObject.SetActive(true);
    }
    private void OnDestroyGlobalVariableConditionItem(GlobalVariableConditionItem item)
    {
        Destroy(item.gameObject);
    }
    #endregion

    #region Post Effects
    private PostEffectItem CreatePostEffectItem()
    {
        return Instantiate(m_prefab_PostEffect, m_transform_PostEffectParent).GetComponent<PostEffectItem>();
    }
    private void OnReturnedPostEffectItemToPool(PostEffectItem item)
    {
        item.gameObject.SetActive(false);
    }
    private void OnTakePostEffectItemFromPool(PostEffectItem item)
    {
        item.gameObject.SetActive(true);
    }
    private void OnDestroyPostEffectItem(PostEffectItem item)
    {
        Destroy(item.gameObject);
    }
    #endregion
}