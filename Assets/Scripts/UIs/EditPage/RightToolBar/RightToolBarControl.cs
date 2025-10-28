using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using CommonUtilLib.ThreadSafe;
using UnityEngine.Pool;


public class RightToolBarControl : SingleTonForGameObject<RightToolBarControl>
{
    [Serializable] public struct BasicData
    {
        public Transform m_transform_BasicLayoutParent;

        public TMP_Dropdown m_dropdown_CurFrameType;
        public TMP_Dropdown m_dropdown_CurTransitionType;
        public TMP_Dropdown m_dropdown_CurBackgroundType;

        public TMP_Dropdown m_dropdown_CharacterThemeBGM;
        public TMP_Dropdown m_dropdown_SFX;
    }
    [Serializable] public struct AppearCharacterData
    {
        public Transform m_transform_AppearCharacterParent;

        public GameObject m_prefab_AppearCharacterItem;
        public Transform m_transform_AppearCharacterLayoutParent;
    }

    [Serializable] public struct ConversationData
    {
        public Transform m_transform_ConversationLayoutParent;

        public TMP_InputField m_inputField_SpeakText;
    }
    [Serializable] public struct BranchData
    {
        public GameObject m_prefab_BranchItem;
        public Transform m_transform_BranchItemParent;

        public Transform m_transform_BranchLayoutParent;
    }
    [Serializable] public struct CutSceneData
    {
        public GameObject m_prefab_CutSceneItem;
        public Transform m_transform_CutSceneItemParent;

        public Transform m_transform_CutSceneLayoutParent;
    }


    [Header("UIs")]
    [SerializeField] private BasicData m_basicData;
    [SerializeField] private AppearCharacterData m_appearCharacterData;

    [SerializeField] private ConversationData m_conversationData;
    [SerializeField] private BranchData m_branchData;
    [SerializeField] private CutSceneData m_cutSceneData;

    private List<AppearCharacterItem> m_appearCharacterItems = new List<AppearCharacterItem>();
    private List<BranchItem> m_branchItems = new List<BranchItem>();
    private List<CutSceneItem> m_cutSceneItems = new List<CutSceneItem>();


    public void Awake()
    {
        SetInstance(this);
    }
    public void Update()
    {
        if ((ChiefUIManager.Instance.CurSelectedAppearCharacterIndex >= 0 || ChiefUIManager.Instance.CurSelectedBranchIndex >= 0 || ChiefUIManager.Instance.CurSelectedCutSceneIndex >= 0)
            && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = -1;
            ChiefUIManager.Instance.CurSelectedBranchIndex = -1;
            ChiefUIManager.Instance.CurSelectedCutSceneIndex = -1;
            ChiefUIManager.Instance.RenderAll();
            return;
        }

        if (ChiefUIManager.Instance.CurSelectedAppearCharacterIndex >= 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && ChiefUIManager.Instance.CurSelectedAppearCharacterIndex - 1 >= 0)
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
                SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
                FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

                var curAppearCharacterData = curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex];
                curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex] = curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex - 1];
                curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex - 1] = curAppearCharacterData;

                curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
                PlotDataBuffer.Instance.PlotData = curPlotData;
                PlotDataBuffer.Instance.TrySaveData();

                ChiefUIManager.Instance.CurSelectedAppearCharacterIndex--;

                ChiefUIManager.Instance.RenderAll();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
                SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
                FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

                if (ChiefUIManager.Instance.CurSelectedAppearCharacterIndex + 1 < curFrameData.CurBasicData.AppearCharacters.Count)
                {
                    var curAppearCharacterData = curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex];
                    curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex] = curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex + 1];
                    curFrameData.CurBasicData.AppearCharacters[ChiefUIManager.Instance.CurSelectedAppearCharacterIndex + 1] = curAppearCharacterData;

                    curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
                    curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
                    PlotDataBuffer.Instance.PlotData = curPlotData;
                    PlotDataBuffer.Instance.TrySaveData();

                    ChiefUIManager.Instance.CurSelectedAppearCharacterIndex++;

                    ChiefUIManager.Instance.RenderAll();
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TryRemoveAppearCharacter();
            }
        }
        else if (ChiefUIManager.Instance.CurSelectedBranchIndex >= 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && ChiefUIManager.Instance.CurSelectedBranchIndex - 1 >= 0)
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
                SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
                FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

                var curBranchData = curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex];
                curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex] = curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex - 1];
                curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex - 1] = curBranchData;

                curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
                curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
                PlotDataBuffer.Instance.PlotData = curPlotData;
                PlotDataBuffer.Instance.TrySaveData();

                ChiefUIManager.Instance.CurSelectedBranchIndex--;

                ChiefUIManager.Instance.RenderAll();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
                SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
                FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

                if (ChiefUIManager.Instance.CurSelectedBranchIndex + 1 < curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                {
                    var curBranchData = curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex];
                    curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex] = curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex + 1];
                    curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[ChiefUIManager.Instance.CurSelectedBranchIndex + 1] = curBranchData;

                    curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
                    curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
                    PlotDataBuffer.Instance.PlotData = curPlotData;
                    PlotDataBuffer.Instance.TrySaveData();

                    ChiefUIManager.Instance.CurSelectedBranchIndex++;

                    ChiefUIManager.Instance.RenderAll();
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                TryRemoveBranch();
            }
        }
    }

    #region Basic Data Unity Callbacks
    public void TrySetCurFrameType()
    {
        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurFrameType = (FrameData.FrameType)m_basicData.m_dropdown_CurFrameType.value;
        switch (curFrameData.CurFrameType)
        {
            case FrameData.FrameType.Conversation:
                curFrameData.CurBasicData.CurConversationData = new FrameData.BasicData.ConversationData()
                {
                    SpeakText = string.Empty
                };
                curFrameData.CurBasicData.CurBranchData = default;
                curFrameData.CurBasicData.CurCutSceneData = default;
                break;

            case FrameData.FrameType.Branch:
                curFrameData.CurBasicData.CurConversationData = default;
                curFrameData.CurBasicData.CurBranchData = new FrameData.BasicData.BranchData()
                {
                    BranchTransitionTargets = new List<string>()
                };
                curFrameData.CurBasicData.CurCutSceneData = default;
                break;

            case FrameData.FrameType.CutScene:
                curFrameData.CurBasicData.AppearCharacters.Clear();
                curFrameData.CurBasicData.SpeakerCharacterIndex = -1;

                curFrameData.CurBasicData.CurConversationData = default;
                curFrameData.CurBasicData.CurBranchData = default;
                curFrameData.CurBasicData.CurCutSceneData = new FrameData.BasicData.CutSceneData()
                {
                    CutSceneIndex = 0
                };
                break;
        }

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

        if (ChiefUIManager.Instance.CurSelectedFrameIndex == curSceneData.Frames.Count - 1)
        {
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
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void TrySetCurTransitionType()
    {
        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurTransitionType = (FrameData.TransitionType)m_basicData.m_dropdown_CurTransitionType.value;

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void TrySetCurBackgroundType()
    {
        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        if (Enum.IsDefined(typeof(BackgroundResourceDesc.BackgroundType), (BackgroundResourceDesc.BackgroundType)m_basicData.m_dropdown_CurBackgroundType.value))
        {
            curFrameData.CurBasicData.BackgroundType = (BackgroundResourceDesc.BackgroundType)m_basicData.m_dropdown_CurBackgroundType.value;

            curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
            curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
            PlotDataBuffer.Instance.PlotData = curPlotData;

            PlotDataBuffer.Instance.TrySaveData();

            ChiefUIManager.Instance.RenderAll();
        }
        else
        {
            m_basicData.m_dropdown_CurBackgroundType.SetValueWithoutNotify((int)curFrameData.CurBasicData.BackgroundType);
        }
    }

    public void TryAddAppearCharacter()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        //if (curFrameData.CurFrameType == FrameData.FrameType.CutScene)
        //{
        //    return;
        //}
        /*else*/ if (curFrameData.CurBasicData.AppearCharacters.Count + 1 > 3)
        {
            return;
        }

        CharacterResourceDesc.CharacterListType newCharacterType = CharacterResourceDesc.CharacterListType.Absinthe;
        foreach (CharacterResourceDesc.CharacterListType characterListType in Enum.GetValues(typeof(CharacterResourceDesc.CharacterListType)))
        {
            bool bisAlreadyExists = false;
            for (int index = 0; index < curFrameData.CurBasicData.AppearCharacters.Count; index++)
            {
                if (curFrameData.CurBasicData.AppearCharacters[index].Item1 == characterListType)
                {
                    bisAlreadyExists = true;
                    break;
                }
            }

            if (!bisAlreadyExists)
            {
                newCharacterType = characterListType;
                break;
            }
        }

        curFrameData.CurBasicData.AppearCharacters.Add(new Tuple<CharacterResourceDesc.CharacterListType, CharacterResourceDesc.CharacterPoseType>(newCharacterType, CharacterResourceDesc.CharacterPoseType.Idle));
        if (curFrameData.CurBasicData.SpeakerCharacterIndex < 0)
        {
            curFrameData.CurBasicData.SpeakerCharacterIndex = 0;
        }

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void TryRemoveAppearCharacter()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        if (ChiefUIManager.Instance.CurSelectedAppearCharacterIndex < curFrameData.CurBasicData.AppearCharacters.Count)
        {
            curFrameData.CurBasicData.AppearCharacters.RemoveAt(ChiefUIManager.Instance.CurSelectedAppearCharacterIndex);
            if (curFrameData.CurBasicData.SpeakerCharacterIndex >= curFrameData.CurBasicData.AppearCharacters.Count)
            {
                curFrameData.CurBasicData.SpeakerCharacterIndex = -1;
                ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = -1;
            }

            curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
            curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
            PlotDataBuffer.Instance.PlotData = curPlotData;
            PlotDataBuffer.Instance.TrySaveData();

            ChiefUIManager.Instance.RenderAll();
        }
    }
    public void TryClearAppearCharacters()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.AppearCharacters.Clear();
        curFrameData.CurBasicData.SpeakerCharacterIndex = -1;

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }

    public void TrySetCharacterThemeBGM()
    {
        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        //if (m_basicData.m_dropdown_CharacterThemeBGM.value == 0)
        //{
        //    curFrameData.CurBasicData.BIsUseCharacterThemeBGM = false;
        //    curFrameData.CurBasicData.CharacterThemeBGMCharacterType = CharacterResourceDesc.CharacterListType.Absinthe;
        //}
        //else
        //{
            curFrameData.CurBasicData.BIsUseCharacterThemeBGM = true;
            curFrameData.CurBasicData.CharacterThemeBGMCharacterType = (CharacterResourceDesc.CharacterListType)(m_basicData.m_dropdown_CharacterThemeBGM.value - 1);
        //}

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();

        //if (Enum.IsDefined(typeof(CharacterResourceDesc.CharacterListType), (CharacterResourceDesc.CharacterListType)m_basicData.m_dropdown_CharacterThemeBGM.value))
        //{
        //    curFrameData.CurBasicData.CharacterThemeBGMCharacterType = (CharacterResourceDesc.CharacterListType)m_basicData.m_dropdown_CharacterThemeBGM.value;

        //    curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        //    curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        //    PlotDataBuffer.Instance.PlotData = curPlotData;

        //    PlotDataBuffer.Instance.TrySaveData();

        //    ChiefUIManager.Instance.RenderAll();
        //}
        //else
        //{
        //    m_basicData.m_dropdown_CurBackgroundType.SetValueWithoutNotify((int)curFrameData.CurBasicData.BackgroundType);
        //}
    }
    public void TrySetSFX()
    {
        if (ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            return;
        }

        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.SFXMusicType = (MusicDataBuffer.SFXMusicType)m_basicData.m_dropdown_SFX.value;

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    #region Conversation Data Unity Callbacks
    public void TrySetSpeakText()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.CurConversationData = new FrameData.BasicData.ConversationData()
        {
            SpeakText = m_conversationData.m_inputField_SpeakText.text,
        };

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    #region Branch Data Unity Callbacks
    public void TryAddBranch()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        if (curFrameData.CurFrameType == FrameData.FrameType.CutScene)
        {
            return;
        }
        else if (curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count + 1 > 3)
        {
            return;
        }

        curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Add(string.Empty);

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

        if (ChiefUIManager.Instance.CurSelectedFrameIndex == curSceneData.Frames.Count - 1)
        {
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
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    public void TryRemoveBranch()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        if (ChiefUIManager.Instance.CurSelectedBranchIndex < curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
        {
            curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.RemoveAt(ChiefUIManager.Instance.CurSelectedBranchIndex);
            if (ChiefUIManager.Instance.CurSelectedBranchIndex >= curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
            {
                ChiefUIManager.Instance.CurSelectedBranchIndex = -1;
            }

            curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
            curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

            if (ChiefUIManager.Instance.CurSelectedFrameIndex == curSceneData.Frames.Count - 1)
            {
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
            }

            PlotDataBuffer.Instance.PlotData = curPlotData;
            PlotDataBuffer.Instance.TrySaveData();

            ChiefUIManager.Instance.RenderAll();
        }
    }
    public void TryClearBranches()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Clear();

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;

        if (ChiefUIManager.Instance.CurSelectedFrameIndex == curSceneData.Frames.Count - 1)
        {
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
        }

        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    #region Cut Scene Data Unity Callbacks
    internal void TrySetCutScene(in int targetCutSceneIndex)
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.CurCutSceneData = new FrameData.BasicData.CutSceneData()
        {
            CutSceneIndex = targetCutSceneIndex
        };

        curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID] = curSceneData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    internal void Render()
    {
        if(ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            m_basicData.m_transform_BasicLayoutParent.gameObject.SetActive(false);
            m_conversationData.m_transform_ConversationLayoutParent.gameObject.SetActive(false);
            m_branchData.m_transform_BranchLayoutParent.gameObject.SetActive(false);
            m_cutSceneData.m_transform_CutSceneLayoutParent.gameObject.SetActive(false);

            m_appearCharacterData.m_transform_AppearCharacterParent.gameObject.SetActive(false);
            return;
        }

        if(PlotDataBuffer.Instance.PlotData.Value.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames.Count > 0)
        {
            FrameData curFrameData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

            m_basicData.m_transform_BasicLayoutParent.gameObject.SetActive(true);
            m_basicData.m_dropdown_CurFrameType.SetValueWithoutNotify((int)curFrameData.CurFrameType);
            m_basicData.m_dropdown_CurTransitionType.SetValueWithoutNotify((int)curFrameData.CurTransitionType);
            m_basicData.m_dropdown_CurBackgroundType.SetValueWithoutNotify((int)curFrameData.CurBasicData.BackgroundType);
            m_basicData.m_dropdown_CharacterThemeBGM.SetValueWithoutNotify(curFrameData.CurBasicData.BIsUseCharacterThemeBGM ? (int)curFrameData.CurBasicData.CharacterThemeBGMCharacterType - 1 : 0);
            m_basicData.m_dropdown_SFX.SetValueWithoutNotify((int)curFrameData.CurBasicData.SFXMusicType);

            switch (curFrameData.CurFrameType)
            {
                case FrameData.FrameType.Conversation:
                    m_conversationData.m_transform_ConversationLayoutParent.gameObject.SetActive(true);

                    m_branchData.m_transform_BranchLayoutParent.gameObject.SetActive(false);
                    m_cutSceneData.m_transform_CutSceneLayoutParent.gameObject.SetActive(false);

                    m_appearCharacterData.m_transform_AppearCharacterParent.gameObject.SetActive(true);

                    m_conversationData.m_inputField_SpeakText.SetTextWithoutNotify(curFrameData.CurBasicData.CurConversationData.SpeakText);
                    break;

                case FrameData.FrameType.Branch:
                    m_conversationData.m_transform_ConversationLayoutParent.gameObject.SetActive(false);

                    m_branchData.m_transform_BranchLayoutParent.gameObject.SetActive(true);

                    m_cutSceneData.m_transform_CutSceneLayoutParent.gameObject.SetActive(false);

                    m_appearCharacterData.m_transform_AppearCharacterParent.gameObject.SetActive(true);

                    int branchItemChildCount = m_branchData.m_transform_BranchItemParent.childCount;
                    for(int index = branchItemChildCount - 1; index >= 0; index--)
                    {
                        Destroy(m_branchData.m_transform_BranchItemParent.GetChild(index).gameObject);
                    }
                    m_branchItems.Clear();

                    for(int index = 0; index < curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count; index++)
                    {
                        BranchItem branchItem = Instantiate(m_branchData.m_prefab_BranchItem, m_branchData.m_transform_BranchItemParent).GetComponent<BranchItem>();
                        branchItem.Render(index);
                        m_branchItems.Add(branchItem);
                    }
                    break;

                case FrameData.FrameType.CutScene:
                    m_conversationData.m_transform_ConversationLayoutParent.gameObject.SetActive(true);
                    m_branchData.m_transform_BranchLayoutParent.gameObject.SetActive(false);

                    m_cutSceneData.m_transform_CutSceneLayoutParent.gameObject.SetActive(true);

                    m_appearCharacterData.m_transform_AppearCharacterParent.gameObject.SetActive(true);

                    int cutSceneItemChildCount = m_cutSceneData.m_transform_CutSceneItemParent.childCount;
                    for(int index = cutSceneItemChildCount - 1; index >= 0; index--)
                    {
                        Destroy(m_cutSceneData.m_transform_CutSceneItemParent.GetChild(index).gameObject);
                    }
                    m_cutSceneItems.Clear();

                    for(int index = 0; index < ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas.Count; index++)
                    {
                        CutSceneItem cutSceneItem = Instantiate(m_cutSceneData.m_prefab_CutSceneItem, m_cutSceneData.m_transform_CutSceneItemParent).GetComponent<CutSceneItem>();
                        cutSceneItem.Render(index);
                        m_cutSceneItems.Add(cutSceneItem);
                    }
                    break;
            }

            #region Appear Characters
            int appearCharacterItemChildCount = m_appearCharacterData.m_transform_AppearCharacterLayoutParent.childCount;
            for(int index = appearCharacterItemChildCount - 1; index >= 0; index--)
            {
                Destroy(m_appearCharacterData.m_transform_AppearCharacterLayoutParent.GetChild(index).gameObject);
            }
            m_appearCharacterItems.Clear();

            for(int index = 0; index < curFrameData.CurBasicData.AppearCharacters.Count; index++)
            {
                AppearCharacterItem appearCharacterItem = Instantiate(m_appearCharacterData.m_prefab_AppearCharacterItem, m_appearCharacterData.m_transform_AppearCharacterLayoutParent).GetComponent<AppearCharacterItem>();
                appearCharacterItem.Render(index);
                m_appearCharacterItems.Add(appearCharacterItem);
            }
            #endregion
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}