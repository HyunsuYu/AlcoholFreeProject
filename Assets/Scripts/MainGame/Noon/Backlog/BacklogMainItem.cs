using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;


public sealed class BacklogMainItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text_Name;
    [SerializeField] private TMP_Text m_text_SpeakText;

    private Guid m_curSceneID;
    private int m_curFrameIndex;


    internal Tuple<Guid, int> BacklogIndex
    {
        get
        {
            return new Tuple<Guid, int>(m_curSceneID, m_curFrameIndex);
        }
    }

    internal void Init(in Guid targetSceneID, in int targetFrameIndex)
    {
        m_curSceneID = targetSceneID;
        m_curFrameIndex = targetFrameIndex;

        Render();
    }
    internal void Render()
    {
        FrameData curFrameData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[m_curSceneID].Frames[m_curFrameIndex];

        switch (curFrameData.CurFrameType)
        {
            case FrameData.FrameType.Conversation:
            case FrameData.FrameType.CutScene:
                string speakText = "{fade}" + curFrameData.CurBasicData.CurConversationData.SpeakText + "{/fade}";
                if (curFrameData.CurBasicData.AppearCharacters.Count > 0)
                {
                    CharacterResourceDesc.CharacterListType curSpeakCharacter = curFrameData.CurBasicData.AppearCharacters[curFrameData.CurBasicData.SpeakerCharacterIndex].Item1;

                    if (curSpeakCharacter == CharacterResourceDesc.CharacterListType.MainCharacter)
                    {
                        m_text_Name.text = NoonCanvasControl.MainCharacterName;

                        if (speakText != null)
                        {
                            speakText = speakText.Replace("<Player>", m_text_Name.text);
                        }
                    }
                    else
                    {
                        m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(curSpeakCharacter).CharacterName;
                    }
                }
                //if (curFrameData.CurBasicData.AppearCharacters.Count > 0)
                //{
                //    m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(curFrameData.CurBasicData.AppearCharacters[curFrameData.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;
                //}
                //if(curFrameData.CurBasicData.CurConversationData.HasValue)
                //{
                m_text_SpeakText.text = speakText;
                //}

                if (string.IsNullOrEmpty(curFrameData.CurBasicData.CurConversationData.SpeakText))
                {
                    m_text_Name.text = string.Empty;
                    m_text_SpeakText.text = "<컷신>";
                }
                break;

            case FrameData.FrameType.Branch:
                m_text_Name.text = string.Empty;
                m_text_SpeakText.text = "<선택지>";
                break;
        }
    }
}