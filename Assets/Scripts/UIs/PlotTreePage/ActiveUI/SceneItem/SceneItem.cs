using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class SceneItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Guid m_curSceneID;

    [Header("For Selected State")]
    [SerializeField] private Image m_image_SelectedNodeState;

    [Header("For Hide Content")]
    private RectTransform m_rectTransform;

    [SerializeField] private RectTransform m_rectTransform_InnerContent;

    [SerializeField] private GameObject m_layout_MainContent;
    [SerializeField] private GameObject m_layout_Preview;

    [SerializeField] private RectTransform m_rectTransform_HideIcon;

    private bool m_bisContentHidden = false;

    [Header("For Scene Name")]
    [SerializeField] private TMP_InputField m_inputField_SceneName;

    [Header("For Node Movement")]
    private Vector3 m_nodeStartPos;
    private bool m_bisNodeSelected = false;

    [Header("For Preview")]
    [SerializeField] private Image m_image_PreviewBackground;

    [SerializeField] private Transform m_transform_PreviewConversationParent;
    [SerializeField] private TMP_Text m_text_PreviewCharacterName;
    [SerializeField] private TMP_Text m_text_PreviewSpeakText;

    [SerializeField] private Transform m_transform_PreviewCharactersParent;
    [SerializeField] private Image[] m_image_PreviewCharacters;
    private RectTransform[] m_rectTranform_Characters;

    [SerializeField] private HorizontalLayoutGroup m_layout_PreviewCharacterList;

    [SerializeField] private Transform m_transform_PreviewBranchParent;
    [SerializeField] private Image[] m_image_PreviewBranches;

    [Header("For Input Part")]
    [SerializeField] private PlotTreeNodeLinkSpot m_inputPart;

    [Header("For Output Part")]
    [SerializeField] private Transform m_transform_OutputPartParent;
    [SerializeField] private GameObject m_prefab_OutputPart;

    private List<PlotTreeNodeLinkSpot> m_outputParts = new List<PlotTreeNodeLinkSpot>();

    private bool m_bisClicked = false;

    [SerializeField] private Transform m_transform_HiddenInputSpot;
    [SerializeField] private Transform m_transform_HiddenOutputSpot;


    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_rectTranform_Characters = new RectTransform[m_image_PreviewCharacters.Length];
        for (int index = 0; index < m_image_PreviewCharacters.Length; index++)
        {
            m_rectTranform_Characters[index] = m_image_PreviewCharacters[index].GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        if (!m_bisNodeSelected)
        {
            m_nodeStartPos = m_rectTransform.position;
            m_bisNodeSelected = true;
        }
        else
        {
            m_bisNodeSelected = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if (Input.GetMouseButton(2))
        {
            return;
        }

        if (m_bisNodeSelected)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f; // Ensure z is zero for 2D movement
            m_rectTransform.position = mousePos;

            var plotData = PlotDataBuffer.Instance.PlotData.Value;
            var sceneData = plotData.SceneTable[m_curSceneID];

            sceneData.EditorSceneItemPos = new SerializableVector3(m_rectTransform.position);

            plotData.SceneTable[m_curSceneID] = sceneData;
            PlotDataBuffer.Instance.PlotData = plotData;

            PlotDataBuffer.Instance.TrySaveData();

            //PlotTreeActiveUIControl.Instance.RenderLinkLine();

            RerenderLinkLine();
        }
        else
        {
            m_rectTransform.position = m_nodeStartPos;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }

        if (m_bisNodeSelected)
        {
            m_bisNodeSelected = false;
        }
    }

    internal bool BIsContentHidden
    {
        get
        {
            return m_bisContentHidden;
        }
    }

    internal Transform HiddenInputSpot
    {
        get
        {
            return m_transform_HiddenInputSpot;
        }
    }
    internal Transform HiddenOutputSpot
    {
        get
        {
            return m_transform_HiddenOutputSpot;
        }
    }

    internal PlotTreeNodeLinkSpot InputPartSpot
    {
        get
        {
            return m_inputPart;
        }
    }
    internal List<PlotTreeNodeLinkSpot> OutputParts
    {
        get
        {
            return m_outputParts;
        }
    }

    #region Unity Callbacks
    public void HideContent()
    {
        if (m_bisContentHidden)
        {
            m_layout_MainContent.SetActive(true);
            m_layout_Preview.SetActive(true);

            m_rectTransform.sizeDelta = new Vector2()
            {
                x = 20.25f,
                y = 25.25f
            };
            m_rectTransform_InnerContent.sizeDelta = new Vector2()
            {
                x = 20.0f,
                y = 25.0f
            };

            m_rectTransform_HideIcon.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

            m_bisContentHidden = false;
        }
        else
        {
            m_layout_MainContent.SetActive(false);
            m_layout_Preview.SetActive(false);

            m_rectTransform.sizeDelta = new Vector2()
            {
                x = 20.25f,
                y = 3.25f
            };
            m_rectTransform_InnerContent.sizeDelta = new Vector2()
            {
                x = 20.0f,
                y = 3.0f
            };

            m_rectTransform_HideIcon.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            m_bisContentHidden = true;
        }

        Invoke("RerenderLinkLine", 0.1f);
    }

    public void ToggleSceneNameEditable()
    {
        //m_bisSceneNameEditable = !m_bisSceneNameEditable;
        //if (m_bisSceneNameEditable)
        //{
        //    m_inputField_SceneName.activ
        //}
        //else
        //{
        //    Debug.Log("D");
        //    m_inputField_SceneName.DeactivateInputField();
        //}
    }
    public void ApplySceneName()
    {
        if(!PlotDataBuffer.Instance.PlotData.HasValue)
        {
            return;
        }

        if (/*m_bisSceneNameEditable &&*/ !string.IsNullOrEmpty(m_inputField_SceneName.text))
        {
            var plotData = PlotDataBuffer.Instance.PlotData.Value;
            var sceneData = plotData.SceneTable[m_curSceneID];

            sceneData.SceneName = m_inputField_SceneName.text;

            plotData.SceneTable[m_curSceneID] = sceneData;
            PlotDataBuffer.Instance.PlotData = plotData;

            PlotDataBuffer.Instance.TrySaveData();

            //m_bisSceneNameEditable = false;

            ChiefUIManager.Instance.RenderAll();
        }
        else
        {
            m_inputField_SceneName.SetTextWithoutNotify(PlotDataBuffer.Instance.PlotData.Value.SceneTable[m_curSceneID].SceneName);
        }
    }

    public void OpenThisScene()
    {
        if (ChiefUIManager.Instance.CurPageType != TotalEdgyToolBarControl.PageType.PlotTree)
        {
            return;
        }
        else if (m_bisNodeSelected)
        {
            return;
        }

        if(!m_bisClicked)
        {
            m_bisClicked = true;
            Invoke("DisableClickedState", 0.25f);
        }
        else
        {
            //SceneOpenPopup.Instance.transform.gameObject.SetActive(true);
            SceneOpenPopup.Instance.OpenPopup(m_curSceneID);
            m_bisClicked = false;
        }
    }
    #endregion

    internal void Render(in Guid targetSceneID)
    {
        m_curSceneID = targetSceneID;

        if (targetSceneID == PlotTreeActiveUIControl.Instance.CurSelectedNodeID)
        {
            m_image_SelectedNodeState.color = new Color(0.1803922f, 0.4862745f, 0.8196079f, 1.0f);
        }
        else
        {
            m_image_SelectedNodeState.color = new Color(0.1686275f, 0.1686275f, 0.1686275f, 1.0f);
        }

        #region Preview
        SceneData sceneData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[m_curSceneID];
        if (sceneData.Frames.Count == 0)
        {
            m_image_PreviewBackground.sprite = null;

            m_transform_PreviewConversationParent.gameObject.SetActive(false);
            m_text_PreviewCharacterName.text = string.Empty;
            m_text_PreviewSpeakText.text = string.Empty;

            m_transform_PreviewCharactersParent.gameObject.SetActive(false);
            foreach (var image in m_image_PreviewCharacters)
            {
                image.sprite = null;
            }

            foreach (var branch in m_image_PreviewBranches)
            {
                branch.gameObject.SetActive(false);
            }
        }
        else
        {
            FrameData firstFrame = sceneData.Frames[0];

            m_image_PreviewBackground.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(firstFrame.CurBasicData.BackgroundType);

            m_transform_PreviewConversationParent.gameObject.SetActive(false);
            m_transform_PreviewCharactersParent.gameObject.SetActive(false);
            m_transform_PreviewBranchParent.gameObject.SetActive(false);

            switch (firstFrame.CurFrameType)
            {
                case FrameData.FrameType.Conversation:
                    //if(firstFrame.CurConversationData.Value.AppearCharacters.Count > 0)
                    //{
                    //    m_text_PreviewCharacterName.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrame.CurConversationData.Value.AppearCharacters[firstFrame.CurConversationData.Value.SpeakerCharacterIndex].Item1).CharacterName;
                    //}
                    //m_text_PreviewSpeakText.text = firstFrame.CurConversationData.Value.SpeakText;

                    //for (int index = 0; index < firstFrame.CurConversationData.Value.AppearCharacters.Count; index++)
                    //{
                    //    m_image_PreviewCharacters[index].sprite = null;
                    //    if (index < firstFrame.CurConversationData.Value.AppearCharacters.Count)
                    //    {
                    //        m_image_PreviewCharacters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrame.CurConversationData.Value.AppearCharacters[index].Item1).CharacterSprites[(int)firstFrame.CurConversationData.Value.AppearCharacters[index].Item2];
                    //    }
                    //}

                    //foreach (var branchText in m_text_PreviewBranched)
                    //{
                    //    branchText.text = string.Empty;
                    //    branchText.transform.parent.gameObject.SetActive(false);
                    //}
                    {
                        RenderCharacters(firstFrame);
                        m_transform_PreviewCharactersParent.gameObject.SetActive(true);

                        m_transform_PreviewBranchParent.gameObject.SetActive(false);

                        string speakText = firstFrame.CurBasicData.CurConversationData.SpeakText;
                        if (firstFrame.CurBasicData.AppearCharacters.Count > 0)
                        {
                            m_text_PreviewCharacterName.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrame.CurBasicData.AppearCharacters[firstFrame.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;
                            speakText = speakText.Replace("<Player>", m_text_PreviewCharacterName.text);
                        }
                        m_text_PreviewSpeakText.text = speakText;
                        m_transform_PreviewConversationParent.gameObject.SetActive(true);
                    }
                    break;

                case FrameData.FrameType.Branch:
                    //m_text_PreviewCharacterName.text = string.Empty;
                    //m_text_PreviewSpeakText.text = string.Empty;

                    //for (int index = 0; index < firstFrame.CurConversationData.Value.AppearCharacters.Count; index++)
                    //{
                    //    m_image_PreviewCharacters[index].sprite = null;
                    //    if (index < firstFrame.CurConversationData.Value.AppearCharacters.Count)
                    //    {
                    //        m_image_PreviewCharacters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrame.CurConversationData.Value.AppearCharacters[index].Item1).CharacterSprites[(int)firstFrame.CurConversationData.Value.AppearCharacters[index].Item2];
                    //    }
                    //}

                    //for (int index = 0; index < firstFrame.CurBranchData.Value.BranchTransitionTargets.Count; index++)
                    //{
                    //    m_text_PreviewBranched[index].text = string.Empty;
                    //    m_text_PreviewBranched[index].transform.parent.gameObject.SetActive(false);
                    //    if (index < firstFrame.CurBranchData.Value.BranchTransitionTargets.Count)
                    //    {
                    //        m_text_PreviewBranched[index].text = firstFrame.CurBranchData.Value.BranchTransitionTargets[index];
                    //        m_text_PreviewBranched[index].transform.parent.gameObject.SetActive(true);
                    //    }
                    //}

                    RenderCharacters(firstFrame);
                    m_transform_PreviewCharactersParent.gameObject.SetActive(true);

                    for (int index = 0; index < m_image_PreviewBranches.Length; index++)
                    {
                        m_image_PreviewBranches[index].gameObject.SetActive(false);
                        if (index < firstFrame.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                        {
                            m_image_PreviewBranches[index].transform.GetChild(0).GetComponent<TMP_Text>().text = firstFrame.CurBasicData.CurBranchData.BranchTransitionTargets[index];
                            m_image_PreviewBranches[index].gameObject.SetActive(true);
                        }
                    }
                    m_transform_PreviewBranchParent.gameObject.SetActive(true);

                    m_transform_PreviewConversationParent.gameObject.SetActive(false);
                    break;

                case FrameData.FrameType.CutScene:
                    {
                        //RenderCharacters(firstFrame);
                        //m_transform_PreviewCharactersParent.gameObject.SetActive(true);

                        m_image_PreviewBackground.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[firstFrame.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;

                        m_transform_PreviewBranchParent.gameObject.SetActive(false);

                        string speakText = firstFrame.CurBasicData.CurConversationData.SpeakText;
                        if (firstFrame.CurBasicData.AppearCharacters.Count > 0)
                        {
                            m_text_PreviewCharacterName.text = ResourceDescBuffer.Instance.GetCharacterResourceDesc(firstFrame.CurBasicData.AppearCharacters[firstFrame.CurBasicData.SpeakerCharacterIndex].Item1).CharacterName;
                            
                            if(speakText != null)
                            {
                                speakText = speakText.Replace("<Player>", m_text_PreviewCharacterName.text);
                            }
                        }
                        m_text_PreviewSpeakText.text = speakText;
                        if(!string.IsNullOrEmpty(m_text_PreviewSpeakText.text))
                        {
                            m_transform_PreviewConversationParent.gameObject.SetActive(true);
                        }
                    }
                    break;
            }
        }
        #endregion

        m_inputField_SceneName.SetTextWithoutNotify(sceneData.SceneName);

        m_rectTransform.position = sceneData.EditorSceneItemPos.ToVector3();

        #region Input Part
        Guid inputLinkID = PlotDataInterface.FindInputLink(targetSceneID);
        m_inputPart.Render(targetSceneID, 0, inputLinkID);
        #endregion

        #region output Part
        int outputPartChildCount = m_transform_OutputPartParent.childCount;
        for (int index = outputPartChildCount - 1; index >= 0; index--)
        {
            Destroy(m_transform_OutputPartParent.GetChild(index).gameObject);
        }
        m_outputParts.Clear();

        Guid[] outputLinkIDs = PlotDataInterface.FindOutputLinks(targetSceneID);
        //for(int index = 0; index < outputLinkIDs.Length; index++)
        //{
        //    PlotTreeNodeLinkSpot outputPart = Instantiate(m_prefab_OutputPart, m_transform_OutputPartParent).GetComponent<PlotTreeNodeLinkSpot>();
        //    outputPart.Render(targetSceneID, index, outputLinkIDs[index]);

        //    m_outputParts.Add(outputPart);
        //}

        if (PlotDataBuffer.Instance.PlotData.Value.SceneTable[targetSceneID].Frames.Count > 0)
        {
            FrameData lastFrameData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[targetSceneID].Frames.Last();

            switch(lastFrameData.CurFrameType)
            {
                case FrameData.FrameType.Conversation:
                case FrameData.FrameType.CutScene:
                    {
                        PlotTreeNodeLinkSpot outputPart = Instantiate(m_prefab_OutputPart, m_transform_OutputPartParent).GetComponent<PlotTreeNodeLinkSpot>();
                        Guid curOutputLinkID = Guid.Empty;
                        if(outputLinkIDs.Length > 0)
                        {
                            curOutputLinkID = outputLinkIDs[0];
                        }
                        outputPart.Render(targetSceneID, 0, curOutputLinkID);

                        m_outputParts.Add(outputPart);
                    }
                    break;

                case FrameData.FrameType.Branch:
                    {
                        for(int index = 0; index < lastFrameData.CurBasicData.CurBranchData.BranchTransitionTargets.Count; index++)
                        {
                            PlotTreeNodeLinkSpot outputPart = Instantiate(m_prefab_OutputPart, m_transform_OutputPartParent).GetComponent<PlotTreeNodeLinkSpot>();

                            Guid targetLinkID = Guid.Empty;
                            for(int linkIDIndex = 0; linkIDIndex < outputLinkIDs.Length; linkIDIndex++)
                            {
                                PlotData.LinkData targetLinkData = PlotDataBuffer.Instance.PlotData.Value.LinkTable[outputLinkIDs[linkIDIndex]];
                                if(targetLinkData.FromNodeID == targetSceneID && targetLinkData.FromLinkCondition.Value.CurSceneNodeLinkCondition.Value.FromBranchIndex == index)
                                {
                                    targetLinkID = outputLinkIDs[linkIDIndex];
                                    break;
                                }
                            }

                            outputPart.Render(targetSceneID, index, targetLinkID);

                            m_outputParts.Add(outputPart);
                        }
                    }
                    break;
            }
        }
        #endregion
    }

    private void DisableClickedState()
    {
        if(m_bisClicked)
        {
            PlotTreeActiveUIControl.Instance.CurSelectedEnterEventName = string.Empty;
            PlotTreeActiveUIControl.Instance.CurSelectedNodeID = m_curSceneID;
            PlotTreeActiveUIControl.Instance.Render();
        }

        m_bisClicked = false;
    }

    private void RerenderLinkLine()
    {
        Guid inputLinkID = PlotDataInterface.FindInputLink(m_curSceneID);
        if(inputLinkID != Guid.Empty)
        {
            PlotTreeActiveUIControl.Instance.GetLinkLineItem(inputLinkID).SetToLinkLineSpot(inputLinkID);
        }

        Guid[] outputLinkIDs = PlotDataInterface.FindOutputLinks(m_curSceneID);
        foreach (Guid outputLinkID in outputLinkIDs)
        {
            PlotTreeActiveUIControl.Instance.GetLinkLineItem(outputLinkID).SetFromLinkLineSpot(outputLinkID);
        }
    }

    private void RenderCharacters(in FrameData firstFrameData)
    {
        //for (int index = 0; index < m_image_PreviewCharacters.Length; index++)
        //{
        //    m_image_PreviewCharacters[index].gameObject.SetActive(false);
        //    m_image_PreviewCharacters[index].sprite = null;

        //    if (index < firstFrameData.CurBasicData.AppearCharacters.Count)
        //    {
        //        m_image_PreviewCharacters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
        //            firstFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
        //            (int)firstFrameData.CurBasicData.AppearCharacters[index].Item2];

        //        if (index == firstFrameData.CurBasicData.SpeakerCharacterIndex)
        //        {
        //            m_image_PreviewCharacters[index].color = Color.white;
        //        }
        //        else
        //        {
        //            m_image_PreviewCharacters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
        //        }

        //        m_image_PreviewCharacters[index].gameObject.SetActive(true);
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
        //for(int index = 0; index < firstFrameData.CurBasicData.AppearCharacters.Count; index++)
        //{
        //    if (firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
        //        && firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
        //    {
        //        realAppearCharactorCount++;
        //    }
        //}

        if (realAppearCharactorCount >= 0)
        {
            m_layout_PreviewCharacterList.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].HorizontalSpacing * (m_rectTransform.sizeDelta.x / 1600.0f);
            for (int index = 0; index < m_image_PreviewCharacters.Length; index++)
            {
                m_image_PreviewCharacters[index].gameObject.SetActive(false);
                m_image_PreviewCharacters[index].sprite = null;

                if (firstFrameData.CurBasicData.AppearCharacters.Count > 0)
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
                                y = 0.35f
                            };
                            break;

                        case 1:
                            m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = 0.3f
                            };
                            break;

                        case 2:
                            m_rectTranform_Characters[index].pivot = new Vector2()
                            {
                                x = 0.5f,
                                y = 0.35f
                            };
                            break;
                    }
                }

                if (index < firstFrameData.CurBasicData.AppearCharacters.Count)
                {
                    m_image_PreviewCharacters[index].sprite = ResourceDescBuffer.Instance.GetCharacterResourceDesc(
                        firstFrameData.CurBasicData.AppearCharacters[index].Item1).CharacterSprites[
                        (int)firstFrameData.CurBasicData.AppearCharacters[index].Item2];

                    if (index == firstFrameData.CurBasicData.SpeakerCharacterIndex)
                    {
                        m_image_PreviewCharacters[index].color = Color.white;
                    }
                    else
                    {
                        m_image_PreviewCharacters[index].color = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDishilightColor;
                    }

                    if (firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.MainCharacter
                        && firstFrameData.CurBasicData.AppearCharacters[index].Item1 != CharacterResourceDesc.CharacterListType.Description)
                    {
                        m_image_PreviewCharacters[index].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int index = 0; index < m_image_PreviewCharacters.Length; index++)
            {
                m_image_PreviewCharacters[index].gameObject.SetActive(false);
                m_image_PreviewCharacters[index].sprite = null;
            }
        }
    }
}