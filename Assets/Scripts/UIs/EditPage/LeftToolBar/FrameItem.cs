using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static SceneOpenPopup;


public sealed class FrameItem : MonoBehaviour
{
    private RectTransform m_rectTransform;

    [Header("Subs")]
    [SerializeField] private TMP_Text m_text_FrameIndex;

    [SerializeField] private Image m_image_BorderLine;

    [Header("Mains")]
    [SerializeField] private Image m_image_Background;

    [Header("Conversation")]
    [SerializeField] private Transform m_transform_ConversationParent;
    [SerializeField] private TMP_Text m_text_CharacterName;
    [SerializeField] private TMP_Text m_text_CharacterSpeckText;

    [Header("Character")]
    [SerializeField] private Transform m_transform_CharactersParent;
    [SerializeField] private Image[] m_image_Characters;
    private RectTransform[] m_rectTranform_Characters;

    [SerializeField] private HorizontalLayoutGroup m_horizontalLayoutGroup;

    [Header("Branch")]
    [SerializeField] private Transform m_transform_BranchesParent;
    [SerializeField] private Image[] m_image_Branches;

    private int m_frameIndex;


    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_rectTranform_Characters = new RectTransform[m_image_Characters.Length];
        for (int index = 0; index < m_image_Characters.Length; index++)
        {
            m_rectTranform_Characters[index] = m_image_Characters[index].GetComponent<RectTransform>();
        }
    }

    public void OpenThisFrame()
    {
        ChiefUIManager.Instance.CurSelectedFrameIndex = m_frameIndex;
        ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = -1;
        ChiefUIManager.Instance.CurSelectedBranchIndex = -1;
        ChiefUIManager.Instance.CurSelectedCutSceneIndex = -1;

        ChiefUIManager.Instance.RenderAll();
    }

    internal void Render(in int frameIndex)
    {
        // Subs
        m_frameIndex = frameIndex;
        m_text_FrameIndex.text = frameIndex.ToString();

        if(ChiefUIManager.Instance.CurSelectedFrameIndex == m_frameIndex)
        {
            m_image_BorderLine.color = ChiefUIManager.SelectedColor;
        }
        else
        {
            m_image_BorderLine.color = ChiefUIManager.BasicBlackColor;
        }

        // Mains
        FrameData curFrameData = PlotDataBuffer.Instance.GetFrameData(ChiefUIManager.Instance.CurSelectedSceneID, m_frameIndex);

        m_image_Background.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(curFrameData.CurBasicData.BackgroundType);

        m_transform_ConversationParent.gameObject.SetActive(false);
        m_transform_CharactersParent.gameObject.SetActive(false);
        m_transform_BranchesParent.gameObject.SetActive(false);

        switch (curFrameData.CurFrameType)
        {
            case FrameData.FrameType.Conversation:
                {
                    RenderCharacters(curFrameData);
                    m_transform_CharactersParent.gameObject.SetActive(true);

                    m_transform_BranchesParent.gameObject.SetActive(false);

                    string speakText = curFrameData.CurBasicData.CurConversationData.SpeakText;
                    if (curFrameData.CurBasicData.AppearCharacters.Count > 0)
                    {
                        m_text_CharacterName.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(curFrameData.CurBasicData.AppearCharacters[curFrameData.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;

                        speakText = speakText.Replace("<Player>", m_text_CharacterName.text);
                    }
                    m_text_CharacterSpeckText.text = speakText;
                    m_transform_ConversationParent.gameObject.SetActive(true);
                }
                break;

            case FrameData.FrameType.Branch:
                RenderCharacters(curFrameData);
                m_transform_CharactersParent.gameObject.SetActive(true);

                for(int index = 0; index < m_image_Branches.Length; index++)
                {
                    m_image_Branches[index].gameObject.SetActive(false);
                    if(index < curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                    {
                        m_image_Branches[index].transform.GetChild(0).GetComponent<TMP_Text>().text = curFrameData.CurBasicData.CurBranchData.BranchTransitionTargets[index];
                        m_image_Branches[index].gameObject.SetActive(true);
                    }
                }
                m_transform_BranchesParent.gameObject.SetActive(true);

                m_transform_ConversationParent.gameObject.SetActive(false);
                break;

            case FrameData.FrameType.CutScene:
                {
                    //RenderCharacters(curFrameData);
                    //m_transform_CharactersParent.gameObject.SetActive(true);

                    m_image_Background.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[curFrameData.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;

                    m_transform_BranchesParent.gameObject.SetActive(false);

                    string speakText = curFrameData.CurBasicData.CurConversationData.SpeakText;
                    if (curFrameData.CurBasicData.AppearCharacters.Count > 0)
                    {
                        m_text_CharacterName.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(curFrameData.CurBasicData.AppearCharacters[curFrameData.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;

                        if(speakText != null)
                        {
                            speakText = speakText.Replace("<Player>", m_text_CharacterName.text);
                        }
                    }
                    m_text_CharacterSpeckText.text = speakText;
                    if(!string.IsNullOrEmpty(m_text_CharacterSpeckText.text))
                    {
                        m_transform_ConversationParent.gameObject.SetActive(true);
                    }
                }
                break;
        }
    }

    private void RenderCharacters(in FrameData targetFrameData)
    {
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
            m_horizontalLayoutGroup.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].HorizontalSpacing * (m_rectTransform.sizeDelta.x / 1600.0f);
            for (int index = 0; index < m_image_Characters.Length; index++)
            {
                m_image_Characters[index].gameObject.SetActive(false);
                m_image_Characters[index].sprite = null;

                if (targetFrameData.CurBasicData.AppearCharacters.Count > 0)
                {
                    //int targetCharacterConfigIndex = targetFrameData.CurBasicData.AppearCharacters.Count - 1;
                    m_rectTranform_Characters[index].sizeDelta = new Vector2()
                    {
                        x = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.x * (m_rectTransform.sizeDelta.x / 1920.0f),
                        y = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].ImageSize.y * (m_rectTransform.sizeDelta.y / 1080.0f)
                    };
                    switch (realAppearCharactorCount)
                    {
                        case 0:
                            m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = -1.5f
                            };
                            break;

                        case 1:
                            m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = -2.85f
                            };
                            break;

                        case 2:
                            m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = -0.2f
                            };
                            break;
                    }
                }

                if (index < targetFrameData.CurBasicData.AppearCharacters.Count)
                {
                    m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                        targetFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
                        (int)targetFrameData.CurBasicData.AppearCharacters[index].Item2];

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

        //for (int index = 0; index < m_image_Characters.Length; index++)
        //{
        //    m_image_Characters[index].gameObject.SetActive(false);
        //    m_image_Characters[index].sprite = null;

        //    if(index < targetFrameData.CurBasicData.AppearCharacters.Count)
        //    {
        //        m_image_Characters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
        //            targetFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
        //            (int)targetFrameData.CurBasicData.AppearCharacters[index].Item2];
                
        //        if(index == targetFrameData.CurBasicData.SpeakerCharacterIndex)
        //        {
        //            m_image_Characters[index].color = Color.white;
        //        }
        //        else
        //        {
        //            m_image_Characters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
        //        }

        //        m_image_Characters[index].gameObject.SetActive(true);
        //    }
        //}
    }
}