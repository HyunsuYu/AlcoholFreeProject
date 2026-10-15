using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public struct FrameData
{
    public enum FrameType
    {
        Conversation,
        Branch,
        CutScene
    }
    public enum TransitionType
    {
        None,
        Appear
    }

    public struct BasicData
    {
        public enum BackgroundEffectTyoe
        {
            None,
            Blur
        }

        public struct ConversationData
        {
            public string SpeakText;
            public bool BIsBold;
        }
        public struct BranchData
        {
            public List<string> BranchTransitionTargets;
        }
        public struct CutSceneData
        {
            public int CutSceneIndex;
        }


        public BackgroundResourceDesc.BackgroundType BackgroundType;
        public BackgroundEffectTyoe BackgroundEffect;

        public bool BIsUseCharacterThemeBGM;
        public CharacterResourceDesc.CharacterListType CharacterThemeBGMCharacterType;

        public MusicDataBuffer.SFXMusicType SFXMusicType;

        public List<Tuple<CharacterResourceDesc.CharacterListType, CharacterResourceDesc.CharacterPoseType>> AppearCharacters;
        public int SpeakerCharacterIndex;
        public bool BisDisplaySpeakerName;

        public ConversationData CurConversationData;
        public BranchData CurBranchData;
        public CutSceneData CurCutSceneData;
    }

    //public struct ConversationData
    //{
    //    public string SpeakText;
    //}
    //public struct BranchData
    //{
    //    public List<string> BranchTransitionTargets;
    //}
    //public struct CutSceneData
    //{
    //    public int CutSceneIndex;
    //}


    public FrameType CurFrameType;
    public TransitionType CurTransitionType;

    public BasicData CurBasicData;

    //public ConversationData? CurConversationData;
    //public BranchData? CurBranchData;
    //public CutSceneData? CurCutSceneData;
}