using CommonUtilLib.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SceneOpenPopup;


public class EmulationCanvasControl : SingleTonForGameObject<EmulationCanvasControl>
{
    [SerializeField] private Image m_image_Background;
    [SerializeField] private GameObject m_gameObject_BackgroundBlur;

    [Header("Conversation")]
    [SerializeField] private Transform m_transform_ConversationParent;
    [SerializeField] private TMP_Text m_text_Name;
    [SerializeField] private TMP_Text m_text_SpeakText;

    [Header("Character")]
    [SerializeField] private Transform m_transform_CharacterParent;
    [SerializeField] private Image[] m_image_Characters;
    private RectTransform[] m_rectTranform_Characters;
    
    [SerializeField] private HorizontalLayoutGroup m_horizontalLayoutGroup;

    [Header("Branch")]
    [SerializeField] private Transform m_transform_BranchParent;
    [SerializeField] private Image[] m_image_Branches;


    public void Awake()
    {
        SetInstance(this);

        m_rectTranform_Characters = new RectTransform[m_image_Characters.Length];
        for (int index = 0; index < m_image_Characters.Length; index++)
        {
            m_rectTranform_Characters[index] = m_image_Characters[index].GetComponent<RectTransform>();
        }
    }

    internal void Render()
    {
        GC.Collect();

        if(ChiefUIManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefUIManager.Instance.CurSelectedFrameIndex < 0)
        {
            m_image_Background.sprite = null;

            m_transform_CharacterParent.gameObject.SetActive(false);
            m_transform_ConversationParent.gameObject.SetActive(false);
            m_transform_BranchParent.gameObject.SetActive(false);

            return;
        }

        SceneData targetSceneData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[ChiefUIManager.Instance.CurSelectedSceneID];
        FrameData? targetFrameData = targetSceneData.Frames.Count > 0 ? targetSceneData.Frames[ChiefUIManager.Instance.CurSelectedFrameIndex] : null;

        if (targetFrameData.HasValue)
        {
            m_image_Background.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(targetFrameData.Value.CurBasicData.BackgroundType);
            switch(targetFrameData.Value.CurBasicData.BackgroundEffect)
            {
                case FrameData.BasicData.BackgroundEffectTyoe.None:
                    m_gameObject_BackgroundBlur.SetActive(false);
                    break;

                case FrameData.BasicData.BackgroundEffectTyoe.Blur:
                    m_gameObject_BackgroundBlur.SetActive(true);
                    break;
            }

            m_transform_CharacterParent.gameObject.SetActive(false);
            m_transform_BranchParent.gameObject.SetActive(false);
            m_transform_ConversationParent.gameObject.SetActive(false);

            switch(targetFrameData.Value.CurFrameType)
            {
                case FrameData.FrameType.Conversation:
                    {
                        RenderCharacters(targetFrameData.Value);
                        m_transform_CharacterParent.gameObject.SetActive(true);

                        m_transform_BranchParent.gameObject.SetActive(false);

                        string speakText = targetFrameData.Value.CurBasicData.CurConversationData.SpeakText;
                        if (targetFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                        {
                            CharacterResourceDesc.CharacterListType curSpeakCharacter = targetFrameData.Value.CurBasicData.AppearCharacters[targetFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1;
                            if (targetFrameData.Value.CurBasicData.BisDisplaySpeakerName)
                            {
                                if (curSpeakCharacter == CharacterResourceDesc.CharacterListType.MainCharacter)
                                {
                                    m_text_Name.text = NoonCanvasControl.MainCharacterName;
                                }
                                else
                                {
                                    m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(curSpeakCharacter).CharacterName;
                                }
                            }
                            else
                            {
                                m_text_Name.text = "???";
                            }
                            speakText = speakText.Replace("<Player>", NoonCanvasControl.MainCharacterName);
                        }

                        if (targetFrameData.Value.CurBasicData.CurConversationData.BIsBold)
                        {
                            m_text_SpeakText.fontSize = SettingDataBuffer.Instance.SettingData.ControlSetting.FontSize * 1.5f;
                        }
                        else
                        {
                            m_text_SpeakText.fontSize = SettingDataBuffer.Instance.SettingData.ControlSetting.FontSize;
                        }

                        m_text_SpeakText.text = speakText;
                        m_transform_ConversationParent.gameObject.SetActive(true);
                    }
                    break;

                case FrameData.FrameType.Branch:
                    RenderCharacters(targetFrameData.Value);
                    m_transform_CharacterParent.gameObject.SetActive(true);

                    for (int index = 0; index < m_image_Branches.Length; index++)
                    {
                        m_image_Branches[index].gameObject.SetActive(false);
                        if (index < targetFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                        {
                            m_image_Branches[index].transform.GetChild(0).GetComponent<TMP_Text>().text = targetFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets[index];
                            m_image_Branches[index].gameObject.SetActive(true);
                        }
                    }
                    m_transform_BranchParent.gameObject.SetActive(true);

                    m_transform_ConversationParent.gameObject.SetActive(false);
                    break;

                case FrameData.FrameType.CutScene:
                    {
                        //RenderCharacters(targetFrameData.Value);
                        //m_transform_CharacterParent.gameObject.SetActive(true);

                        m_image_Background.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[targetFrameData.Value.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;

                        m_transform_BranchParent.gameObject.SetActive(false);

                        string speakText = targetFrameData.Value.CurBasicData.CurConversationData.SpeakText;
                        if (targetFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                        {
                            m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(targetFrameData.Value.CurBasicData.AppearCharacters[targetFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;

                            if(speakText != null)
                            {
                                speakText = speakText.Replace("<Player>", m_text_Name.text);
                            }
                        }

                        m_text_SpeakText.text = speakText;
                        if(!string.IsNullOrEmpty(m_text_SpeakText.text))
                        {
                            m_transform_ConversationParent.gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }
        else
        {
            m_image_Background.sprite = null;

            m_transform_CharacterParent.gameObject.SetActive(false);
            m_transform_ConversationParent.gameObject.SetActive(false);
            m_transform_BranchParent.gameObject.SetActive(false);
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void RenderCharacters(in FrameData targetFrameData)
    {
        //for(int index = 0; index < m_image_Characters.Length; index++)
        //{
        //    m_image_Characters[index].gameObject.SetActive(false);
        //    m_image_Characters[index].sprite = null;

        //    if (index < targetFrameData.CurBasicData.AppearCharacters.Count)
        //    {
        //        m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
        //            targetFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
        //            (int)targetFrameData.CurBasicData.AppearCharacters[index].Item2];
        //        m_image_Characters[index].gameObject.SetActive(true);
        //    }
        //}

        int realAppearCharactorCount = targetFrameData.CurBasicData.AppearCharacters.Count - 1;
        for (int index = 0; index < targetFrameData.CurBasicData.AppearCharacters.Count; index++)
        {
            if (targetFrameData.CurBasicData.AppearCharacters[index].Item1 == CharacterResourceDesc.CharacterListType.MainCharacter
                || targetFrameData.CurBasicData.AppearCharacters[index].Item1 == CharacterResourceDesc.CharacterListType.Description)
            {
                realAppearCharactorCount--;
            }
        }
        //for (int index = 0; index < targetFrameData.CurBasicData.AppearCharacters.Count; index++)
        //{
        //    if (targetFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
        //        && targetFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
        //    {
        //        realAppearCharactorCount++;
        //    }
        //}

        if (realAppearCharactorCount >= 0)
        {
            m_horizontalLayoutGroup.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].HorizontalSpacing;
            m_horizontalLayoutGroup.padding = new RectOffset()
            {
                left = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.x,
                right = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.y,
                top = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.z,
                bottom = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.w
            };
            for (int index = 0; index < m_image_Characters.Length; index++)
            {
                m_image_Characters[index].gameObject.SetActive(false);
                m_image_Characters[index].sprite = null;

                if (targetFrameData.CurBasicData.AppearCharacters.Count > 0)
                {
                    //int targetCharacterConfigIndex = targetFrameData.CurBasicData.AppearCharacters.Count - 1;
                    m_rectTranform_Characters[index].sizeDelta = new Vector2()
                    {
                        x = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.x,
                        y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.y
                    };
                    //m_rectTranform_Characters[index].pivot = new Vector2()
                    //{
                    //    x = 0.5f,
                    //    y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].PerCharacterImagePositionPivot[index]
                    //};
                }

                if (index < targetFrameData.CurBasicData.AppearCharacters.Count)
                {
                    m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                        targetFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
                        (int)targetFrameData.CurBasicData.AppearCharacters[index].Item2];
                    m_rectTranform_Characters[index].pivot = new Vector2()
                    {
                        x = 0.5f,
                        y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].PerCharacterImagePositionPivot[(int)targetFrameData.CurBasicData.AppearCharacters[index].Item1]
                    };

                    if (index == targetFrameData.CurBasicData.SpeakerCharacterIndex)
                    {
                        m_image_Characters[index].color = Color.white;
                    }
                    else
                    {
                        m_image_Characters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
                    }

                    if (targetFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
                        && targetFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
                    {
                        m_image_Characters[index].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int index = 0; index < m_image_Characters.Length; index++)
            {
                m_image_Characters[index].gameObject.SetActive(false);
                m_image_Characters[index].sprite = null;
            }
        }
    }
}