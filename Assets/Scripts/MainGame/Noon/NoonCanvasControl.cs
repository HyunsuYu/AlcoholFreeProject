using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using CommonUtilLib.ThreadSafe;

using Febucci.UI;
using UnityEngine.EventSystems;


public class NoonCanvasControl : SingleTonForGameObject<NoonCanvasControl>
{
    [SerializeField] private RectTransform m_rectTransform_Contents;

    [SerializeField] private Image m_image_Background;
    [SerializeField] private GameObject m_gameObject_BackgroundBlur;

    [Header("Conversation")]
    [SerializeField] private Transform m_transform_ConversationParent;
    [SerializeField] private Image m_image_ConversationBackground;
    [SerializeField] private TMP_Text m_text_Name;
    [SerializeField] private TMP_Text m_text_SpeakText;

    [SerializeField] private TypewriterByCharacter m_typewriter_SpeakText;

    [SerializeField] private GameObject m_gameObject_AutoPlayIcon;

    [SerializeField] private Animator m_animator_ConversationDisplayDone;
    [SerializeField] private Animator m_animator_ConversationControl;
    [SerializeField] private Animator m_animator_ControlHide;

    private bool m_bisControlHide = false;

    [SerializeField] private GraphicRaycaster m_graphicRaycaster;
    [SerializeField] private EventSystem m_eventSystem;

    private bool m_bisMouseOnBtnUI = false;

    [Header("Character")]
    [SerializeField] private Transform m_transform_CharacterParent;
    [SerializeField] private Image[] m_image_Characters;
    private RectTransform[] m_rectTranform_Characters;

    [SerializeField] private HorizontalLayoutGroup m_horizontalLayoutGroup;

    [Header("Branch")]
    [SerializeField] private Transform m_transform_BranchParent;
    [SerializeField] private Image[] m_image_Branches;

    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource_BGM;
    [SerializeField] private AudioSource m_audioSource_SFX;

    [Header("Backlog")]
    [SerializeField] private GameObject m_layout_Backlog;
    //[SerializeField] private Scrollbar m_scrollbar_Backlog;
    [SerializeField] private ScrollRect m_scrollRect_Backlog;

    [SerializeField] private GameObject m_gameObject_SkipBtn;

    [Header("Screen Shot")]
    [SerializeField] private Animator m_animator_CameraFlash;
    [SerializeField] private CustomRenderTexture m_renderTexture;

    [Header("Transition")]
    [SerializeField] private ScreenFadeTransition m_screenTransition_Fade;
    [SerializeField] private ScreenBrushTransition m_screenTransition_Brush;

    [SerializeField] private float m_transitionDuration = 2.0f;

    [Header("Cut Scene Gliter Effect")]
    [SerializeField] private GameObject m_gameObject_CutSceneGliterEffect;

    private static bool m_bisPreview = false;

    private static Guid m_targetSceneID = Guid.Empty;
    private static int m_targetFrameIndex = -1;

    private static bool m_bisRequest2RenderArrived = false;

    private static int m_curSelectedBranchIndex = -1;

    private bool m_bisBacklogOpened = false;
    private bool m_bisNowAutoPlayWaiting = false;

    private static string m_mainCharacterName = "주인공";

    private FrameData? m_curFrameData = null;

    private bool m_bisNowTransitioning = false;


    public void Awake()
    {
        SetInstance(this);

        m_rectTranform_Characters = new RectTransform[m_image_Characters.Length];
        for (int index = 0; index < m_image_Characters.Length; index++)
        {
            m_rectTranform_Characters[index] = m_image_Characters[index].GetComponent<RectTransform>();
        }
    }
    public void Start()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;

        m_audioSource_BGM.volume = curSettingData.AudioSetting.MasterVolume * curSettingData.AudioSetting.BGMVolume;
        m_audioSource_SFX.volume = curSettingData.AudioSetting.MasterVolume * curSettingData.AudioSetting.SFXVolume;

        m_image_ConversationBackground.color = new Color()
        {
            r = 0.1607843f,
            g = 0.1607843f,
            b = 0.1607843f,
            a = curSettingData.ScreenSetting.ConversationBackgroundOpacity
        };

        m_text_Name.fontSize = curSettingData.ControlSetting.FontSize * 1.166666f;
        m_text_SpeakText.fontSize = curSettingData.ControlSetting.FontSize;

        switch (curSettingData.ScreenSetting.CurResolution)
        {
            case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_Contents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;

        if (curSettingData.ControlSetting.BIsAutoPlay)
        {
            m_gameObject_AutoPlayIcon.SetActive(true);
        }
        else
        {
            m_gameObject_AutoPlayIcon.SetActive(false);
        }
    }
    //public void Start()
    //{
    //    GameObject dtoObject = GameObject.Find("Editor2PreviewDTO(Clone)");
    //    if (dtoObject != null)
    //    {
    //        Editor2PreviewDTO dto = dtoObject.GetComponent<Editor2PreviewDTO>();
    //        if (dto != null)
    //        {
    //            m_bisPreview = dto.BIsPreview;

    //            //SaveDataBuffer.Instance.

    //            Render(dto.TargetSceneID, dto.TargetFrameIndex);
    //        }
    //    }
    //}
    public void Update()
    {
        if (m_bisRequest2RenderArrived)
        {
            Render();

            m_bisRequest2RenderArrived = false;

            m_targetSceneID = Guid.Empty;
            m_targetFrameIndex = -1;
        }

        if (m_bisMouseOnBtnUI)
        {
            return;
        }
        else if (m_bisNowTransitioning)
        {
            return;
        }

        if (m_bisPreview && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Editor");
        }

        SceneData targetSceneData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[ChiefMainGameManager.Instance.CurSelectedSceneID];
        FrameData? targetFrameData = targetSceneData.Frames.Count > 0 ? targetSceneData.Frames[ChiefMainGameManager.Instance.CurSelectedFrameIndex] : null;
        if ((targetFrameData.HasValue && targetFrameData.Value.CurFrameType != FrameData.FrameType.Branch)
            && SettingDataBuffer.Instance.SettingData.ControlSetting.BIsAutoPlay && !m_bisNowAutoPlayWaiting && !m_bisBacklogOpened)
        {
            m_bisNowAutoPlayWaiting = true;
            Invoke(nameof(CalculateNextFrame), SettingDataBuffer.Instance.SettingData.ControlSetting.FontSpeed * 1.2f);
        }
        else if ((!SettingDataBuffer.Instance.SettingData.ControlSetting.BIsAutoPlay || (targetFrameData.HasValue && targetFrameData.Value.CurFrameType == FrameData.FrameType.Branch))
            && ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) || m_curSelectedBranchIndex != -1))
            && !m_layout_Backlog.activeSelf)
        {
            if (m_typewriter_SpeakText.isShowingText)
            {
                m_typewriter_SpeakText.SkipTypewriter();
            }
            else
            {
                CalculateNextFrame();
            }
        }

        //if (Input.GetKeyDown(KeyCode.Print))
        //{
        //    string path = string.Empty;
        //    switch (targetFrameData.Value.CurFrameType)
        //    {
        //        case FrameData.FrameType.Conversation:
        //        case FrameData.FrameType.Branch:
        //            path = System.IO.Path.Combine(Application.streamingAssetsPath,
        //                    $"ScreenShots/Conversation/AlcoholFree_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.png");
        //            break;

        //        case FrameData.FrameType.CutScene:
        //            path = System.IO.Path.Combine(Application.streamingAssetsPath,
        //                    $"ScreenShots/CutScene/AlcoholFree_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.png");
        //            break;
        //    }

        //    string directory = System.IO.Path.GetDirectoryName(path);
        //    if (!System.IO.Directory.Exists(directory))
        //    {
        //        System.IO.Directory.CreateDirectory(directory);
        //    }

        //    if (m_renderTexture != null)
        //    {
        //        RenderTexture.active = m_renderTexture;
        //        Texture2D screenShot = new Texture2D(m_renderTexture.width, m_renderTexture.height, TextureFormat.RGB24, false);
        //        screenShot.ReadPixels(new Rect(0, 0, m_renderTexture.width, m_renderTexture.height), 0, 0);
        //        screenShot.Apply();
        //        byte[] bytes = screenShot.EncodeToPNG();
        //        System.IO.File.WriteAllBytes(path, bytes);
        //        Debug.Log($"Saved MiniGame Result Screen Shot to {path}");
        //    }
        //    else
        //    {
        //        Debug.LogError("Render Texture is not set!");
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (m_layout_Backlog.activeSelf)
            {
                BacklogControl.Instance.CloseBacklog();
            }
            else
            {
                OpenBacklog();
            }
        }

        if (m_typewriter_SpeakText.isShowingText)
        {
            if (m_animator_ConversationDisplayDone.GetBool("BIsConversationDisplayDone"))
            {
                m_animator_ConversationDisplayDone.SetBool("BIsConversationDisplayDone", false);
            }
        }
        else
        {
            if (!m_animator_ConversationDisplayDone.GetBool("BIsConversationDisplayDone"))
            {
                m_animator_ConversationDisplayDone.SetBool("BIsConversationDisplayDone", true);
            }
        }
    }
    public void FixedUpdate()
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        m_graphicRaycaster.Raycast(new PointerEventData(m_eventSystem)
        {
            position = Input.mousePosition
        }, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                m_bisMouseOnBtnUI = true;
                return;
            }
        }
        m_bisMouseOnBtnUI = false;
    }

    internal static int CurSelectedBranchIndex
    {
        set
        {
            m_curSelectedBranchIndex = value;
        }
    }

    internal static string MainCharacterName
    {
        get
        {
            return m_mainCharacterName;
        }
        set
        {
            m_mainCharacterName = value;
        }
    }

    internal bool BIsBacklogOpened
    {
        set
        {
            m_bisBacklogOpened = value;
        }
    }

    internal bool BIsSkipBtnActive
    {
        set
        {
            m_gameObject_SkipBtn.SetActive(value);
        }
    }

    #region Unity Callbacks
    public void OpenBacklog()
    {
        m_layout_Backlog.SetActive(true);
        BacklogControl.Instance.RenderTexts();

        m_scrollRect_Backlog.verticalNormalizedPosition = 0.0f;
        m_bisBacklogOpened = true;

        BIsSkipBtnActive = false;
    }
    public void OpenSetting()
    {
        ChiefSettingManager.FromPage = ChiefSettingManager.FromPageType.Conversation;
        ChiefSettingManager.FromSceneID = ChiefMainGameManager.Instance.CurSelectedSceneID;
        ChiefSettingManager.FromFrameIndex = ChiefMainGameManager.Instance.CurSelectedFrameIndex;

        SceneManager.LoadScene("Setting");
    }
    public void OpenTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SkipAll()
    {
        //if (PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName.Contains("End"))
        //{
        //    SceneManager.LoadScene("DemoEnd");
        //}
        //else
        //{
        //    SceneManager.LoadScene("MiniGame");
        //}

        SceneData curSceneData = PlotDataBuffer.Instance.GetSceneData(ChiefMainGameManager.Instance.CurSelectedSceneID);
        if(curSceneData.SceneName.Contains("Tutorial")
            || (PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromNodeID != PlotData.EnterEventNodeID
                && PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromNodeID != PlotData.ScreenTransitionNodeID
                && PlotDataBuffer.Instance.GetSceneData(PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromNodeID).SceneName.Contains("Tutorial")))
        {
            SceneManager.LoadScene("Map");
        }
        else if (curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Aquarium.ToString())
                    || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Park.ToString())
                    || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Mall.ToString())
                    || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.School.ToString()))
        {
            SceneManager.LoadScene("Map");
        }
        else if (curSceneData.SceneName.Contains("BarResult"))
        {
            SceneManager.LoadScene("DemoEnd");
        }
        else
        {
            SceneManager.LoadScene("MiniGame");
        }
    }

    public void HideControls()
    {
        if (!m_bisControlHide)
        {
            m_animator_ConversationControl.SetTrigger("Hide");
            m_animator_ControlHide.SetTrigger("Hide");

            m_bisControlHide = true;
        }
        else
        {
            m_animator_ConversationControl.SetTrigger("Display");
            m_animator_ControlHide.SetTrigger("Display");

            m_bisControlHide = false;
        }
    }

    public void TrySaveScreenShot()
    {
        m_animator_CameraFlash.Play("CameraFlash");

        string path = string.Empty;
        switch (m_curFrameData.Value.CurFrameType)
        {
            case FrameData.FrameType.CutScene:
                path = System.IO.Path.Combine(Application.streamingAssetsPath, 
                    $"ScreenShots/CutScene/{ChiefCharacterSelectionManager.CurSelectedCharacterType.ToString()}_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.png");
                break;

            default:
                path = System.IO.Path.Combine(Application.streamingAssetsPath,
                    $"ScreenShots/Story/{ChiefCharacterSelectionManager.CurSelectedCharacterType.ToString()}_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.png");
                break;
        }

        string directory = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        if (m_renderTexture != null)
        {
            RenderTexture.active = m_renderTexture;
            Texture2D screenShot = new Texture2D(m_renderTexture.width, m_renderTexture.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, m_renderTexture.width, m_renderTexture.height), 0, 0);
            screenShot.Apply();
            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }
        else
        {
            Debug.LogError("Render Texture is not set!");
        }
    }
    #endregion

    internal static void Request2Render(in Guid targetSceneID, in int targetFrameIndex, in bool bisPreview = false)
    {
        m_targetSceneID = targetSceneID;
        m_targetFrameIndex = targetFrameIndex;

        m_bisRequest2RenderArrived = true;

        m_bisPreview = bisPreview;
    }

    internal void Render()
    {
        TempSaveBuffer.TryAddBacklogData(ChiefMainGameManager.Instance.CurSelectedSceneID, ChiefMainGameManager.Instance.CurSelectedFrameIndex);

        if (ChiefMainGameManager.Instance.CurSelectedSceneID == Guid.Empty || ChiefMainGameManager.Instance.CurSelectedFrameIndex < 0)
        {
            m_image_Background.sprite = null;

            m_transform_CharacterParent.gameObject.SetActive(false);
            m_transform_ConversationParent.gameObject.SetActive(false);
            m_transform_BranchParent.gameObject.SetActive(false);

            return;
        }

        SceneData targetSceneData = PlotDataBuffer.Instance.PlotData.Value.SceneTable[ChiefMainGameManager.Instance.CurSelectedSceneID];
        m_curFrameData = targetSceneData.Frames.Count > 0 ? targetSceneData.Frames[ChiefMainGameManager.Instance.CurSelectedFrameIndex] : null;

        IScreenTranstion.TransitionKind transitionKind = IScreenTranstion.TransitionKind.None;
        bool bisEnterScreen = false;
        if (m_image_Background.sprite == null)
        {
            bisEnterScreen = true;
            if (m_curFrameData.Value.CurFrameType == FrameData.FrameType.CutScene)
            {
                transitionKind = IScreenTranstion.TransitionKind.Fade;
            }
            else
            {
                transitionKind = IScreenTranstion.TransitionKind.Fade;
            }
        }
        else
        {
            if (m_curFrameData.Value.CurFrameType == FrameData.FrameType.CutScene
                && m_image_Background.sprite != ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[m_curFrameData.Value.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite)
            {
                transitionKind = IScreenTranstion.TransitionKind.Fade;
            }
            else if (m_curFrameData.Value.CurFrameType != FrameData.FrameType.CutScene
                /*&& m_image_Background.sprite != ResourceDescBuffer.Instance.GetBackgroundSprite(curFrameData.Value.CurBasicData.BackgroundType)*/)
            {
                if(ChiefMainGameManager.Instance.CurSelectedFrameIndex - 1 >= 0
                    && targetSceneData.Frames[ChiefMainGameManager.Instance.CurSelectedFrameIndex - 1].CurBasicData.BackgroundType != m_curFrameData.Value.CurBasicData.BackgroundType)
                {
                    transitionKind = IScreenTranstion.TransitionKind.Fade;
                }
            }
        }

        if (m_curFrameData.HasValue)
        {
            switch (transitionKind)
            {
                case IScreenTranstion.TransitionKind.None:
                    RenderContent();
                    break;

                case IScreenTranstion.TransitionKind.Fade:
                    m_screenTransition_Fade.StartTransition(m_transitionDuration, bisEnterScreen, m_audioSource_BGM, m_audioSource_SFX);

                    m_bisNowTransitioning = true;
                    if (bisEnterScreen)
                    {
                        RenderContent();
                        Invoke(nameof(EndScreenTransition), m_transitionDuration / 2.0f);
                    }
                    else
                    {
                        Invoke(nameof(RenderContent), m_transitionDuration / 2.0f);
                        Invoke(nameof(EndScreenTransition), m_transitionDuration);
                    }
                    break;

                case IScreenTranstion.TransitionKind.Brush:
                    Texture2D beforeTex = new Texture2D((int)m_image_Background.sprite.rect.width, (int)m_image_Background.sprite.rect.height);
                    var beforePixels = beforeTex.GetPixels((int)m_image_Background.sprite.textureRect.x,
                                                            (int)m_image_Background.sprite.textureRect.y,
                                                            (int)m_image_Background.sprite.textureRect.width,
                                                            (int)m_image_Background.sprite.textureRect.height);
                    beforeTex.SetPixels(beforePixels);
                    beforeTex.Apply();

                    var afterSprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[m_curFrameData.Value.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;
                    Texture2D afterTex = new Texture2D((int)afterSprite.rect.width, (int)afterSprite.rect.height);
                    var afterPixels = afterTex.GetPixels((int)afterSprite.textureRect.x,
                                                            (int)afterSprite.textureRect.y,
                                                            (int)afterSprite.textureRect.width,
                                                            (int)afterSprite.textureRect.height);
                    afterTex.SetPixels(afterPixels);
                    afterTex.Apply();

                    m_screenTransition_Brush.StartTransition(m_transitionDuration, beforeTex, afterTex, m_audioSource_BGM, m_audioSource_SFX);

                    m_bisNowTransitioning = true;
                    if (bisEnterScreen)
                    {
                        RenderContent();
                        Invoke(nameof(EndScreenTransition), m_transitionDuration / 2.0f);
                    }
                    else
                    {
                        Invoke(nameof(RenderContent), m_transitionDuration / 2.0f);
                        Invoke(nameof(EndScreenTransition), m_transitionDuration);
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

        BacklogControl.Instance.Render();
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
            m_horizontalLayoutGroup.padding = new RectOffset()
            {
                left = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.x,
                right = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.y,
                top = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.z,
                bottom = (int)ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].Padding.w
            };
            m_horizontalLayoutGroup.spacing = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].HorizontalSpacing;
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
                    m_rectTranform_Characters[index].localScale = ResourceDescBuffer.Instance.CharacterDisplayConfig.CharacterDisplayDatas[realAppearCharactorCount].PerCharacterImageScale[(int)targetFrameData.CurBasicData.AppearCharacters[index].Item1];

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

            //AudioClip targetBGMClip = MusicDataBuffer.Instance.TryGetCharacterMusic((CharacterResourceDesc.CharacterListType)targetFrameData.CurBasicData.SpeakerCharacterIndex, 0);
            if ((int)targetFrameData.CurBasicData.CharacterThemeBGMCharacterType >= 0)
            {
                AudioClip targetBGMClip = ResourceDescBuffer.Instance.MusicResourceDesc.CharacterMusics[(int)targetFrameData.CurBasicData.CharacterThemeBGMCharacterType].CharacterMusicClips[0];
                if (targetFrameData.CurBasicData.BIsUseCharacterThemeBGM
                    && m_audioSource_BGM.clip != targetBGMClip)
                {
                    m_audioSource_BGM.Stop();
                    m_audioSource_BGM.clip = targetBGMClip;
                    m_audioSource_BGM.Play();
                }
            }
            else
            {
                AudioClip targetBGMClip = ResourceDescBuffer.Instance.MusicResourceDesc.BackgroundMusicClips[(int)targetFrameData.CurBasicData.BackgroundType];
                if (m_audioSource_BGM.clip != targetBGMClip)
                {
                    m_audioSource_BGM.Stop();
                    m_audioSource_BGM.clip = targetBGMClip;
                    m_audioSource_BGM.Play();
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

            //AudioClip targetBGMClip = MusicDataBuffer.Instance.TryGetBackgroundMusic(targetFrameData.CurBasicData.BackgroundType);
            AudioClip targetBGMClip = ResourceDescBuffer.Instance.MusicResourceDesc.BackgroundMusicClips[(int)targetFrameData.CurBasicData.BackgroundType];
            if (m_audioSource_BGM.clip != targetBGMClip)
            {
                m_audioSource_BGM.Stop();
                m_audioSource_BGM.clip = targetBGMClip;
                m_audioSource_BGM.Play();
            }
        }

        if (targetFrameData.CurBasicData.SFXMusicType != MusicDataBuffer.SFXMusicType.None)
        {
            m_audioSource_SFX.Stop();
            //m_audioSource_SFX.clip = MusicDataBuffer.Instance.TryGetSFXMusic(targetFrameData.CurBasicData.SFXMusicType - 1);
            m_audioSource_SFX.clip = ResourceDescBuffer.Instance.MusicResourceDesc.SFXMusicClips[(int)targetFrameData.CurBasicData.SFXMusicType - 1];
            m_audioSource_SFX.Play();
        }
    }

    private void CalculateNextFrame()
    {
        if (m_bisNowAutoPlayWaiting && !SettingDataBuffer.Instance.SettingData.ControlSetting.BIsAutoPlay)
        {
            m_bisNowAutoPlayWaiting = false;
            return;
        }
        else if (m_bisBacklogOpened && SettingDataBuffer.Instance.SettingData.ControlSetting.BIsAutoPlay)
        {
            m_bisNowAutoPlayWaiting = false;
            return;
        }

        SceneData curSceneData = PlotDataBuffer.Instance.GetSceneData(ChiefMainGameManager.Instance.CurSelectedSceneID);
        FrameData curFrameData = PlotDataBuffer.Instance.GetFrameData(ChiefMainGameManager.Instance.CurSelectedSceneID, ChiefMainGameManager.Instance.CurSelectedFrameIndex);
        if (ChiefMainGameManager.Instance.CurSelectedFrameIndex + 1 < curSceneData.Frames.Count)
        {
            ChiefMainGameManager.Instance.CurSelectedFrameIndex++;
            Render();
        }
        else
        {
            Guid[] outputLinks = PlotDataInterface.FindOutputLinks(ChiefMainGameManager.Instance.CurSelectedSceneID);
            Guid toNodeID = Guid.Empty;
            foreach (Guid linkID in outputLinks)
            {
                switch (curFrameData.CurFrameType)
                {
                    case FrameData.FrameType.Conversation:
                    case FrameData.FrameType.CutScene:
                        toNodeID = PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].ToNodeID;
                        break;

                    case FrameData.FrameType.Branch:
                        if (PlotDataBuffer.Instance.PlotData.HasValue
                            && PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].FromLinkCondition.HasValue
                            && PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].FromLinkCondition.Value.CurSceneNodeLinkCondition.HasValue
                            && PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].FromLinkCondition.Value.CurSceneNodeLinkCondition.Value.FromBranchIndex == m_curSelectedBranchIndex)
                        {
                            toNodeID = PlotDataBuffer.Instance.PlotData.Value.LinkTable[linkID].ToNodeID;
                            break;
                        }
                        break;
                }
            }

            if (toNodeID == Guid.Empty)
            {
                // TODO - 데이터가 변조되었을 때의 적절한 처리 필요
                //throw new Exception($"No valid output link found for scene {ChiefMainGameManager.Instance.CurSelectedSceneID} at frame {ChiefMainGameManager.Instance.CurSelectedFrameIndex}.");
                //Debug.LogError($"No valid output link found for scene {ChiefMainGameManager.Instance.CurSelectedSceneID} at frame {ChiefMainGameManager.Instance.CurSelectedFrameIndex}.");
                return;
            }

            if (toNodeID == PlotData.ScreenTransitionNodeID)
            {
                if (m_bisPreview)
                {
                    SceneManager.LoadScene("Editor");
                }
                else
                {
                    //switch (curSceneData.SceneName)
                    //{
                    //    //case "AbsintheEnd":
                    //    //case "HayaseuraTokusenGinjoEnd":
                    //    //case "KahluaEnd":
                    //    //case "HpnotiqEnd":
                    //    //case "CabernetSauvignonEnd":
                    //    //    SceneManager.LoadScene("DemoEnd");
                    //    //    break;

                    //    //default:
                    //    //    bool bisToMiniGame = true;
                    //    //    if (PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.HasValue)
                    //    //    {
                    //    //        if (PlotDataBuffer.Instance.PlotData.Value.LinkTable[PlotDataInterface.FindInputLink(ChiefMainGameManager.Instance.CurSelectedSceneID)].FromLinkCondition.Value.CurEnterEventNodeLinkCondition.Value.EnterEventName.Contains("End"))
                    //    //        {
                    //    //            bisToMiniGame = false;
                    //    //        }
                    //    //    }

                    //    //    if (!bisToMiniGame)
                    //    //    {
                    //    //        SceneManager.LoadScene("DemoEnd");
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        SceneManager.LoadScene("MiniGame");
                    //    //    }
                    //    //    break;
                    //}

                    if (curSceneData.SceneName.Contains("Tutorial"))
                    {
                        SceneManager.LoadScene("Map");
                    }
                    else if (curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Aquarium.ToString())
                                || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Park.ToString())
                                || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Mall.ToString())
                                || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.School.ToString())
                                || curSceneData.SceneName.Contains(BackgroundResourceDesc.BackgroundType.Gallery.ToString()))
                    {
                        SceneManager.LoadScene("Map");
                    }
                    else if (curSceneData.SceneName.Contains("BarResult"))
                    {
                        SceneManager.LoadScene("DemoEnd");
                    }
                    else
                    {
                        SceneManager.LoadScene("MiniGame");
                    }
                }
            }
            else
            {
                ChiefMainGameManager.Instance.CurSelectedSceneID = toNodeID;
                ChiefMainGameManager.Instance.CurSelectedFrameIndex = 0;

                Render();
            }

            m_curSelectedBranchIndex = -1;
        }

        m_bisNowAutoPlayWaiting = false;
    }

    private void RenderContent()
    {
        m_image_Background.sprite = ResourceDescBuffer.Instance.GetBackgroundSprite(m_curFrameData.Value.CurBasicData.BackgroundType);
        switch(m_curFrameData.Value.CurBasicData.BackgroundEffect)
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

        switch (m_curFrameData.Value.CurFrameType)
        {
            case FrameData.FrameType.Conversation:
                {
                    RenderCharacters(m_curFrameData.Value);
                    m_transform_CharacterParent.gameObject.SetActive(true);

                    m_transform_BranchParent.gameObject.SetActive(false);

                    string speakText = "{fade}" + m_curFrameData.Value.CurBasicData.CurConversationData.SpeakText + "{/fade}";
                    if (m_curFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                    {
                        CharacterResourceDesc.CharacterListType curSpeakCharacter = m_curFrameData.Value.CurBasicData.AppearCharacters[m_curFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1;
                        if (m_curFrameData.Value.CurBasicData.BisDisplaySpeakerName)
                        {
                            if (curSpeakCharacter == CharacterResourceDesc.CharacterListType.MainCharacter)
                            {
                                m_text_Name.text = m_mainCharacterName;
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
                        speakText = speakText.Replace("<Player>", m_mainCharacterName);
                    }

                    if (m_curFrameData.Value.CurBasicData.CurConversationData.BIsBold)
                    {
                        m_text_SpeakText.fontSize = SettingDataBuffer.Instance.SettingData.ControlSetting.FontSize * 1.5f;
                    }
                    else
                    {
                        m_text_SpeakText.fontSize = SettingDataBuffer.Instance.SettingData.ControlSetting.FontSize;
                    }

                    m_text_SpeakText.text = speakText;
                    m_transform_ConversationParent.gameObject.SetActive(true);

                    m_gameObject_CutSceneGliterEffect.SetActive(false);
                }
                break;

            case FrameData.FrameType.Branch:
                {
                    RenderCharacters(m_curFrameData.Value);
                    m_transform_CharacterParent.gameObject.SetActive(true);

                    for (int index = 0; index < m_image_Branches.Length; index++)
                    {
                        m_image_Branches[index].gameObject.SetActive(false);
                        if (index < m_curFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets.Count)
                        {
                            m_image_Branches[index].transform.GetChild(0).GetComponent<TMP_Text>().text = m_curFrameData.Value.CurBasicData.CurBranchData.BranchTransitionTargets[index];
                            m_image_Branches[index].gameObject.SetActive(true);
                        }
                    }
                    m_transform_BranchParent.gameObject.SetActive(true);

                    m_transform_ConversationParent.gameObject.SetActive(false);

                    m_gameObject_CutSceneGliterEffect.SetActive(false);
                }
                break;

            case FrameData.FrameType.CutScene:
                {
                    //RenderCharacters(targetFrameData.Value);
                    //m_transform_CharacterParent.gameObject.SetActive(true);

                    m_image_Background.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[m_curFrameData.Value.CurBasicData.CurCutSceneData.CutSceneIndex].CurSceneSprite;

                    m_transform_BranchParent.gameObject.SetActive(false);

                    string speakText = "{fade}" + m_curFrameData.Value.CurBasicData.CurConversationData.SpeakText + "{/fade}";
                    if (m_curFrameData.Value.CurBasicData.AppearCharacters.Count > 0)
                    {
                        CharacterResourceDesc.CharacterListType curSpeakCharacter = m_curFrameData.Value.CurBasicData.AppearCharacters[m_curFrameData.Value.CurBasicData.SpeakerCharacterIndex].Item1;
                        if (curSpeakCharacter == CharacterResourceDesc.CharacterListType.MainCharacter)
                        {
                            m_text_Name.text = m_mainCharacterName;

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
                    m_text_SpeakText.text = speakText;
                    if (!string.IsNullOrEmpty(m_curFrameData.Value.CurBasicData.CurConversationData.SpeakText))
                    {
                        m_transform_ConversationParent.gameObject.SetActive(true);
                    }

                    m_gameObject_CutSceneGliterEffect.SetActive(true);
                }
                break;
        }
    }
    private void EndScreenTransition()
    {
        m_bisNowTransitioning = false;
    }
}