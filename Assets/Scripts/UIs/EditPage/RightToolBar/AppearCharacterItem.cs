using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AppearCharacterItem : MonoBehaviour
{
    [SerializeField] private Image m_image_SelectedState;

    [SerializeField] private Image m_image_SpeakerCharacterState;
    [SerializeField] private Image m_image_Character;

    [SerializeField] private TMP_Dropdown m_dropDown_CharacterType;
    [SerializeField] private TMP_Dropdown m_dropDown_CharacterPoseType;

    private int m_appearCharacterIndex;


    #region Unity Callbacks
    public void SelectThis()
    {
        if (ChiefUIManager.Instance.CurSelectedAppearCharacterIndex != m_appearCharacterIndex)
        {
            ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = m_appearCharacterIndex;
        }
        else
        {
            PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
            SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
            FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

            curFrameData.CurBasicData.SpeakerCharacterIndex = m_appearCharacterIndex;

            curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
            PlotDataBuffer.Instance.PlotData = curPlotData;

            PlotDataBuffer.Instance.TrySaveData();

            ChiefUIManager.Instance.RenderAll();
        }
        ChiefUIManager.Instance.RenderAll();
    }

    public void TrySetCurCharacterType()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        bool bisValid = true;
        for(int index = 0; index < curFrameData.CurBasicData.AppearCharacters.Count; index++)
        {
            if (curFrameData.CurBasicData.AppearCharacters[index].Item1 == (CharacterResourceDesc.CharacterListType)m_dropDown_CharacterType.value)
            {
                bisValid = false;
                break;
            }
        }

        if(bisValid)
        {
            curFrameData.CurBasicData.AppearCharacters[m_appearCharacterIndex] = new Tuple<CharacterResourceDesc.CharacterListType, CharacterResourceDesc.CharacterPoseType>((CharacterResourceDesc.CharacterListType)m_dropDown_CharacterType.value, curFrameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item2);

            curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
            PlotDataBuffer.Instance.PlotData = curPlotData;

            PlotDataBuffer.Instance.TrySaveData();

            ChiefUIManager.Instance.RenderAll();
        }
        else
        {
            m_dropDown_CharacterType.SetValueWithoutNotify((int)curFrameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item1);
        }
    }
    public void TrySetCurCharacterPoseType()
    {
        PlotData curPlotData = PlotDataBuffer.Instance.PlotData.Value;
        SceneData curSceneData = curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData curFrameData = curSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex];

        curFrameData.CurBasicData.AppearCharacters[m_appearCharacterIndex] = new Tuple<CharacterResourceDesc.CharacterListType, CharacterResourceDesc.CharacterPoseType>(curFrameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item1, (CharacterResourceDesc.CharacterPoseType)m_dropDown_CharacterPoseType.value);

        curPlotData.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID].Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] = curFrameData;
        PlotDataBuffer.Instance.PlotData = curPlotData;

        PlotDataBuffer.Instance.TrySaveData();

        ChiefUIManager.Instance.RenderAll();
    }
    #endregion

    internal void Render(in int targetAppearCharacterIndex)
    {
        m_appearCharacterIndex = targetAppearCharacterIndex;

        if (m_appearCharacterIndex == ChiefUIManager.Instance.CurSelectedAppearCharacterIndex)
        {
            m_image_SelectedState.color = ChiefUIManager.SelectedColor;
        }
        else
        {
            m_image_SelectedState.color = ChiefUIManager.BasicBlackColor;
        }

        FrameData frameData = PlotDataBuffer.Instance.GetFrameData(ChiefUIManager.Instance.CurSelectedSceneID, ChiefUIManager.Instance.CurSelectedFrameIndex);
        if (m_appearCharacterIndex == frameData.CurBasicData.SpeakerCharacterIndex)
        {
            m_image_SpeakerCharacterState.color = ChiefUIManager.SelectedColor;
        }
        else
        {
            m_image_SpeakerCharacterState.color = ChiefUIManager.BasicBlackColor;
        }

        m_dropDown_CharacterType.SetValueWithoutNotify((int)frameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item1);
        m_dropDown_CharacterPoseType.SetValueWithoutNotify((int)frameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item2);

        m_image_Character.sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                    frameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item1).CharacterSprites[
                    (int)frameData.CurBasicData.AppearCharacters[m_appearCharacterIndex].Item2];
    }
}