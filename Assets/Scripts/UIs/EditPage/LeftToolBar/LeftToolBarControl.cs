using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
using UnityEngine.Pool;

using CommonUtilLib.ThreadSafe;


public sealed class LeftToolBarControl : SingleTonForGameObject<LeftToolBarControl>
{
    [SerializeField] private GameObject m_prefab_FrameItem;
    [SerializeField] private Transform m_transform_FrameItemParent;

    [SerializeField] private TMP_Text m_text_CurSelectedSceneName;

    private IObjectPool<FrameItem> m_pool_FrameItem;
    private List<FrameItem> m_frameItems = new List<FrameItem>();


    public void Awake()
    {
        SetInstance(this);

        m_pool_FrameItem = new ObjectPool<FrameItem>(CreateFrameItem, OnTakeFrameItemFromPool, OnReturnedFrameItemToPool, OnDestroyFrameItem, true, 10, 500);
    }
    public void Update()
    {
        if (ChiefUIManager.Instance.CurSelectedAppearCharacterIndex == -1
            && ChiefUIManager.Instance.CurSelectedBranchIndex == -1
            && ChiefUIManager.Instance.CurSelectedCutSceneIndex == -1
            && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            ChiefUIManager.Instance.CurSelectedFrameIndex = -1;
            ChiefUIManager.Instance.RenderAll();
            return;
        }

        if (ChiefUIManager.Instance.CurSelectedSceneID != Guid.Empty)
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                TryAddFrameItem();

                if(ChiefUIManager.Instance.CurSelectedFrameIndex == -1)
                {
                    ChiefUIManager.Instance.CurSelectedFrameIndex = 0;
                }
            }
            else if(ChiefUIManager.Instance.CurSelectedAppearCharacterIndex == -1
                && ChiefUIManager.Instance.CurSelectedBranchIndex == -1
                && ChiefUIManager.Instance.CurSelectedCutSceneIndex == -1
                && (Input.GetKeyDown(KeyCode.Delete)))
            {
                TryRemoveFrameItem();

                if(PlotDataBuffer.Instance.GetSceneData(ChiefUIManager.Instance.CurSelectedSceneID).Frames.Count == 0)
                {
                    ChiefUIManager.Instance.CurSelectedFrameIndex = -1;
                }
            }
        }

        if (!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && ChiefUIManager.Instance.CurSelectedFrameIndex - 1 >= 0)
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

                FrameData curFrameData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex - 1];
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex - 1] = curFrameData;

                List<Guid> removalLinkIDs = new List<Guid>();
                var curLinkTableKeys = curPlotData.LinkTable.Keys;
                foreach (Guid curLinkID in curLinkTableKeys)
                {
                    PlotData.LinkData curLinkData = curPlotData.LinkTable[curLinkID];
                    if (curLinkData.ToNodeID == ChiefUIManager.Instance.CurSelectedSceneID
                        || curLinkData.FromNodeID == ChiefUIManager.Instance.CurSelectedSceneID)
                    {
                        removalLinkIDs.Add(curLinkID);
                    }
                }
                foreach (Guid removalLinkID in removalLinkIDs)
                {
                    curPlotData.LinkTable.Remove(removalLinkID);
                }

                PlotDataBuffer.Instance.PlotData = curPlotData;
                PlotDataBuffer.Instance.TrySaveData();

                ChiefUIManager.Instance.CurSelectedFrameIndex--;

                ChiefUIManager.Instance.RenderAll();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && ChiefUIManager.Instance.CurSelectedFrameIndex + 1 < PlotDataBuffer.Instance.GetSceneData(ChiefUIManager.Instance.CurSelectedSceneID).Frames.Count)
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;

                FrameData curFrameData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex + 1];
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex + 1] = curFrameData;

                List<Guid> removalLinkIDs = new List<Guid>();
                var curLinkTableKeys = curPlotData.LinkTable.Keys;
                foreach (Guid curLinkID in curLinkTableKeys)
                {
                    PlotData.LinkData curLinkData = curPlotData.LinkTable[curLinkID];
                    if (curLinkData.ToNodeID == ChiefUIManager.Instance.CurSelectedSceneID
                        || curLinkData.FromNodeID == ChiefUIManager.Instance.CurSelectedSceneID)
                    {
                        removalLinkIDs.Add(curLinkID);
                    }
                }
                foreach (Guid removalLinkID in removalLinkIDs)
                {
                    curPlotData.LinkTable.Remove(removalLinkID);
                }

                PlotDataBuffer.Instance.PlotData = curPlotData;
                PlotDataBuffer.Instance.TrySaveData();

                ChiefUIManager.Instance.CurSelectedFrameIndex++;

                ChiefUIManager.Instance.RenderAll();
            }
        }
        else if(ChiefUIManager.Instance.CurSelectedAppearCharacterIndex == -1
            && ChiefUIManager.Instance.CurSelectedBranchIndex == -1
            && ChiefUIManager.Instance.CurSelectedCutSceneIndex == -1)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChiefUIManager.Instance.CurSelectedFrameIndex--;

                ChiefUIManager.Instance.RenderAll();
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChiefUIManager.Instance.CurSelectedFrameIndex++;

                ChiefUIManager.Instance.RenderAll();
            }
        }
    }

    #region Unity Callbacks
    public void TryAddFrameItem()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        curSceneData.Frames.Add(new FrameData()
        {
            CurFrameType = FrameData.FrameType.Conversation,
            CurTransitionType = FrameData.TransitionType.None,

            CurBasicData = new FrameData.BasicData()
            {
                BackgroundType = BackgroundResourceDesc.BackgroundType.HomeRoom,

                AppearCharacters = new List<Tuple<CharacterResourceDesc.CharacterListType, CharacterResourceDesc.CharacterPoseType>>(),
                SpeakerCharacterIndex = -1,

                CurConversationData = new FrameData.BasicData.ConversationData()
                {
                    SpeakText = string.Empty
                },
                CurBranchData = default,
                CurCutSceneData = default
            },

            //CurConversationData = new FrameData.ConversationData()
            //{
            //    SpeakText = string.Empty
            //},
            //CurBranchData = null,
            //CurCutSceneData = null
        });

        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

        List<Guid> removalLinkIDs = new List<Guid>();
        var curLinkTableKeys = curPlotData.LinkTable.Keys;
        foreach (Guid curLinkID in curLinkTableKeys)
        {
            PlotData.LinkData curLinkData = curPlotData.LinkTable[curLinkID];
            if (curLinkData.ToNodeID == ChiefUIManager.Instance.CurSelectedSceneID
                || curLinkData.FromNodeID == ChiefUIManager.Instance.CurSelectedSceneID)
            {
                removalLinkIDs.Add(curLinkID);
            }
        }
        foreach (Guid removalLinkID in removalLinkIDs)
        {
            curPlotData.LinkTable.Remove(removalLinkID);
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void TryRemoveFrameItem()
    {
        if(ChiefUIManager.Instance.CurSelectedFrameIndex == -1)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];

        curSceneData.Frames.RemoveAt(ChiefUIManager.Instance.CurSelectedFrameIndex);

        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

        List<Guid> removalLinkIDs = new List<Guid>();
        var curLinkTableKeys = curPlotData.LinkTable.Keys;
        foreach (Guid curLinkID in curLinkTableKeys)
        {
            PlotData.LinkData curLinkData = curPlotData.LinkTable[curLinkID];
            if (curLinkData.ToNodeID == ChiefUIManager.Instance.CurSelectedSceneID
                || curLinkData.FromNodeID == ChiefUIManager.Instance.CurSelectedSceneID)
            {
                removalLinkIDs.Add(curLinkID);
            }
        }
        foreach (Guid removalLinkID in removalLinkIDs)
        {
            curPlotData.LinkTable.Remove(removalLinkID);
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;
        ChiefUIManager.Instance.CurSelectedFrameIndex = -1;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    internal void Render()
    {
        for (int index = m_frameItems.Count - 1; index >= 0; index--)
        {
            m_pool_FrameItem.Release(m_frameItems[index]);
        }
        m_frameItems.Clear();

        //int childCount = m_transform_FrameItemParent.childCount;
        //for(int index = childCount - 1; index >= 0; index--)
        //{
        //    Destroy(m_transform_FrameItemParent.GetChild(index).gameObject);
        //}
        //m_frameItems.Clear();

        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty)
        {
            return;
        }

        SceneData curSceneData = PlotDataBuffer.Instance.GetSceneData(ChiefUIManager.Instance.CurSelectedSceneID);
        for(int index = 0; index < curSceneData.Frames.Count; index++)
        {
            FrameItem frameItem = m_pool_FrameItem.Get();
            frameItem.transform.SetParent(m_transform_FrameItemParent, false);

            //FrameItem frameItem = Instantiate(m_prefab_FrameItem, m_transform_FrameItemParent).GetComponent<FrameItem>();

            frameItem.Render(index);

            m_frameItems.Add(frameItem);
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private FrameItem CreateFrameItem()
    {
        return Instantiate(m_prefab_FrameItem, m_transform_FrameItemParent).GetComponent<FrameItem>();
    }
    private void OnTakeFrameItemFromPool(FrameItem frameItem)
    {
        frameItem.gameObject.SetActive(true);
    }
    private void OnReturnedFrameItemToPool(FrameItem frameItem)
    {
        frameItem.gameObject.SetActive(false);
    }
    private void OnDestroyFrameItem(FrameItem frameItem)
    {
        Destroy(frameItem.gameObject);
    }
}