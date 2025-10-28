using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using CommonUtilLib.ThreadSafe;


public class SceneOpenPopup : SingleTonForGameObject<SceneOpenPopup>
{
    [Serializable] public struct BasicInfos
    {
        public TMP_Text m_text_SceneName;
        public TMP_Text m_text_InnerFrameCount;
    }
    [Serializable] public struct Preview
    {
        public RectTransform m_rectTransform;
        public Image m_image_Background;

        [Header("Conversation")]
        public Transform m_transform_ConversationParent;
        public TMP_Text m_text_Name;
        public TMP_Text m_text_SpeakText;

        [Header("Character")]
        public Transform m_transform_CharactersParent;
        public Image[] m_image_Characters;

        public HorizontalLayoutGroup m_horizontalLayoutGroup;
        public RectTransform[] m_rectTranform_Characters;

        [Header("Branch")]
        public Transform m_transform_BranchesParent;
        public Image[] m_image_Branches;
    }


    [SerializeField] private BasicInfos m_basicInfos;
    [SerializeField] private Preview m_preview;

    private Guid m_targetSceneID = Guid.Empty;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        gameObject.SetActive(false);
    }

    #region Unity Callbacks
    /// <summary>
    /// 렌더링도 겹한다
    /// </summary>
    /// <param name="targetSceneID"></param>
    public void OpenPopup(in Guid targetSceneID)
    {
        m_targetSceneID = targetSceneID;

        SceneData targetSceneData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[targetSceneID];
        FrameData? firstFrameData = targetSceneData.Frames.Count > 0 ? targetSceneData.Frames[0] : null;

        #region Basic Infos
        m_basicInfos.m_text_SceneName.text = targetSceneData.SceneName;
        m_basicInfos.m_text_InnerFrameCount.text = targetSceneData.Frames.Count.ToString();
        #endregion

        #region Preview
        if (firstFrameData.HasValue)
        {
            m_preview.m_image_Background.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(firstFrameData.Value.CurBasicData.BackgroundType);

            m_preview.m_transform_CharactersParent.gameObject.SetActive(false);
            m_preview.m_transform_BranchesParent.gameObject.SetActive(false);
            m_preview.m_transform_ConversationParent.gameObject.SetActive(false);

            switch (firstFrameData.Value.CurFrameType)
            {
                case FrameData.FrameType.Conversation:
                    {
                        RenderCharacters(firstFrameData.Value);
                        m_preview.m_transform_CharactersParent.gameObject.SetActive(true);

                        m_preview.m_transform_BranchesParent.gameObject.SetActive(false);

                        string speakText = firstFrameData.Value.CurBasicData.CurConversationData.SpeakText;
                        if (firstFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                        {
                            m_preview.m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrameData.Value.CurBasicData.AppearCharacters[firstFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;

                            speakText = speakText.Replace("<Player>", m_preview.m_text_Name.text);
                        }
                        m_preview.m_text_SpeakText.text = speakText;
                        m_preview.m_transform_ConversationParent.gameObject.SetActive(true);
                    }
                    break;

                case FrameData.FrameType.Branch:
                    RenderCharacters(firstFrameData.Value);
                    m_preview.m_transform_CharactersParent.gameObject.SetActive(true);

                    for(int index = 0; index < m_preview.m_image_Branches.Length; index++)
                    {
                        m_preview.m_image_Branches[index].gameObject.SetActive(false);
                        if(index < firstFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                        {
                            m_preview.m_image_Branches[index].transform.GetChild(0).GetComponent<TMP_Text>().text = firstFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets[index];
                            m_preview.m_image_Branches[index].gameObject.SetActive(true);
                        }
                    }
                    m_preview.m_transform_BranchesParent.gameObject.SetActive(true);

                    m_preview.m_transform_ConversationParent.gameObject.SetActive(false);
                    break;

                case FrameData.FrameType.CutScene:
                    {
                        //RenderCharacters(firstFrameData.Value);
                        //m_preview.m_transform_CharactersParent.gameObject.SetActive(true);

                        m_preview.m_image_Background.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[firstFrameData.Value.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;

                        m_preview.m_transform_BranchesParent.gameObject.SetActive(false);

                        string speakText = firstFrameData.Value.CurBasicData.CurConversationData.SpeakText;
                        if (firstFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                        {
                            m_preview.m_text_Name.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrameData.Value.CurBasicData.AppearCharacters[firstFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;

                            if(speakText != null)
                            {
                                speakText = speakText.Replace("<Player>", m_preview.m_text_Name.text);
                            }
                        }
                        m_preview.m_text_SpeakText.text = speakText;
                        if(!string.IsNullOrEmpty(m_preview.m_text_SpeakText.text))
                        {
                            m_preview.m_transform_ConversationParent.gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }
        else
        {
            m_preview.m_image_Background.sprite = null;

            m_preview.m_text_Name.text = string.Empty;
            m_preview.m_text_SpeakText.text = string.Empty;

            m_preview.m_transform_CharactersParent.gameObject.SetActive(false);

            m_preview.m_transform_BranchesParent.gameObject.SetActive(false);
        }
        #endregion

        gameObject.SetActive(true);
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void OpenScene()
    {
        ChiefUIManager.Instance.CurSelectedSceneID = m_targetSceneID;

        ChiefUIManager.Instance.CurPageType = TotalEdgyToolBarControl.PageType.EditPage;

        ChiefUIManager.Instance.CurSelectedFrameIndex = -1;

        ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = -1;
        ChiefUIManager.Instance.CurSelectedBranchIndex = -1;
        ChiefUIManager.Instance.CurSelectedCutSceneIndex = -1;

        PageSynchronizer.Instance.OpenPage(0);

        ChiefUIManager.Instance.RenderAll();

        ClosePopup();
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void RenderCharacters(in FrameData firstFrameData)
    {
        //for (int index = 0; index < m_preview.m_image_Characters.Length; index++)
        //{
        //    m_preview.m_image_Characters[index].gameObject.SetActive(false);
        //    m_preview.m_image_Characters[index].sprite = null;

        //    if (index < firstFrameData.CurBasicData.AppearCharacters.Count)
        //    {
        //        m_preview.m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
        //            firstFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
        //            (int)firstFrameData.CurBasicData.AppearCharacters[index].Item2];

        //        if(index == firstFrameData.CurBasicData.SpeakerCharacterIndex)
        //        {
        //            m_preview.m_image_Characters[index].color = Color.white;
        //        }
        //        else
        //        {
        //            m_preview.m_image_Characters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
        //        }

        //        m_preview.m_image_Characters[index].gameObject.SetActive(true);
        //    }
        //}

        int realAppearCharactorCount = firstFrameData.CurBasicData.AppearCharacters.Count - 1;
        for (int index = 0; index < firstFrameData.CurBasicData.AppearCharacters.Count; index++)
        {
            if (firstFrameData.CurBasicData.AppearCharacters[index].Item1 == CharacterResourceDesc.CharacterListType.MainCharacter
                || firstFrameData.CurBasicData.AppearCharacters[index].Item1 == CharacterResourceDesc.CharacterListType.Description)
            {
                realAppearCharactorCount--;
            }
        }
        //for (int index = 0; index < firstFrameData.CurBasicData.AppearCharacters.Count; index++)
        //{
        //    if (firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
        //        && firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
        //    {
        //        realAppearCharactorCount++;
        //    }
        //}

        if (realAppearCharactorCount >= 0)
        {
            m_preview.m_horizontalLayoutGroup.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].HorizontalSpacing * (m_preview.m_rectTransform.sizeDelta.x / 1600.0f);
            for (int index = 0; index < m_preview.m_image_Characters.Length; index++)
            {
                m_preview.m_image_Characters[index].gameObject.SetActive(false);
                m_preview.m_image_Characters[index].sprite = null;

                if (firstFrameData.CurBasicData.AppearCharacters.Count > 0)
                {
                    //int targetCharacterConfigIndex = targetFrameData.CurBasicData.AppearCharacters.Count - 1;
                    m_preview.m_rectTranform_Characters[index].sizeDelta = new Vector2()
                    {
                        x = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.x * (m_preview.m_rectTransform.sizeDelta.x / 1920.0f),
                        y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.y * (m_preview.m_rectTransform.sizeDelta.y / 1080.0f)
                    };
                    switch (realAppearCharactorCount)
                    {
                        case 0:
                            m_preview.m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = -1.47f
                            };
                            break;

                        case 1:
                            m_preview.m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = -4.0f
                            };
                            break;

                        case 2:
                            m_preview.m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = 0.0f
                            };
                            break;
                    }
                }

                if (index < firstFrameData.CurBasicData.AppearCharacters.Count)
                {
                    m_preview.m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                        firstFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
                        (int)firstFrameData.CurBasicData.AppearCharacters[index].Item2];

                    if (index == firstFrameData.CurBasicData.SpeakerCharacterIndex)
                    {
                        m_preview.m_image_Characters[index].color = Color.white;
                    }
                    else
                    {
                        m_preview.m_image_Characters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
                    }

                    if (firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
                        && firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
                    {
                        m_preview.m_image_Characters[index].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int index = 0; index < m_preview.m_image_Characters.Length; index++)
            {
                m_preview.m_image_Characters[index].gameObject.SetActive(false);
                m_preview.m_image_Characters[index].sprite = null;
            }
        }
    }
}