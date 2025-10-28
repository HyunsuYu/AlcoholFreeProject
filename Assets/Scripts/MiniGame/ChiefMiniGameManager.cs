using CommonUtilLib.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public sealed class ChiefMiniGameManager : SingleTonForGameObject<ChiefMiniGameManager>
{
    [SerializeField] private RectTransform m_rectTransform_FrontContents;
    [SerializeField] private RectTransform m_rectTransform_BackContents;

    private static RecipeSelectionControl.RecipeType m_curRecipe;

    private static IBottle m_curSelectedBottle = null;
    private static GlassItem m_curGlassItem = null;
    private static ShakerItem m_curShakerItem = null;

    private static List<IBottle> m_bottles = new List<IBottle>();
    private Vector3 m_prevDragPos = -Vector3.one;

    [SerializeField] private GameObject m_layout_Popup;
    [SerializeField] private GameObject m_layout_TutorialPopup;

    [Header("Result")]
    [SerializeField] private GameObject m_layout_Result;

    [SerializeField] private GameObject[] m_gameObject_ScoreIcons;

    [SerializeField] private float m_scoreIconAnimationDelay = 1.0f;

    [SerializeField] private TMP_Text m_text_RecipeName;

    [SerializeField] private Sprite[] m_sprite_RecipeResults;
    [SerializeField] private Image m_image_RecipeResult;

    [SerializeField] private Animator m_animator_CameraFlash;
    [SerializeField] private CustomRenderTexture m_renderTexture;

    [SerializeField] private Image m_image_StampBackground;
    [SerializeField] private Image m_image_Stamp;
    [SerializeField] private Sprite[] m_sprite_Stamps;
    [SerializeField] private AudioSource m_audioSource_Stamp;
    [SerializeField] private float m_stampSFXTiming = 1.0f;

    private int m_curScoreIconIndex = 0;

    [SerializeField] private GameObject m_gameObject_ResultText;
    private bool m_bisMiniGameFinished = false;

    [SerializeField] private AudioSource m_audioSource_BGM;

    [Header("Recipe")]
    [SerializeField] private GameObject m_layout_RecipePopup;
    [SerializeField] private Image m_image_Recipe;

    private static bool m_bisInit = false;
    private static InventoryControl.ItemType[] m_liqurePlaceholderTable = new InventoryControl.ItemType[4];

    [SerializeField] private GameObject m_gameObject_GlassBtn;
    [SerializeField] private GameObject m_gameObject_ShakerBtn;

    [Header("Selected Liqure Mouse Effect")]
    [SerializeField] private RectTransform m_rectTransform_SelectedLiqureMouseIcon;
    [SerializeField] private Image m_image_SelectedLiqureMouseIcon;
    [SerializeField] private Animator m_animator_SelectedLiqureMouseIcon;
    private int m_curSelectedLiqurePlaceholderIndex = -1;
    private int m_prevSelectedLiqurePlaceholderIndex = -1;

    [Header("Liqure Fall")]
    [SerializeField] private GameObject m_prefab_MetaballItem;

    private IObjectPool<Transform> m_metaballPool = null;
    private List<Transform> m_metaballTransforms = new List<Transform>();

    [SerializeField] private bool collectionChecks = true;
    [SerializeField] private int maxPoolSize = 10;

    [SerializeField] private Material m_metaballMat = null;

    [SerializeField] private Vector3 m_posMidifier;

    private float m_ballDropTimer = 0.0f;
    [SerializeField] private float m_ballDropDistance = 1.0f;

    private int m_curPourCount = 0;
    [SerializeField] private int m_pourItemThreshold = 10;

    [SerializeField] private Vector3 m_liqureDropPosDistance;


    public void Awake()
    {
        SetInstance(this);

        //m_curRecipe = RecipeSelectionControl.CurRecipeType;

        m_metaballPool = new ObjectPool<Transform>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
    }
    public void Start()
    {
        RenderAll();

        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        switch (curSettingData.ScreenSetting.CurResolution)
        {
            case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
                Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_FrontContents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_rectTransform_BackContents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
                Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_FrontContents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                m_rectTransform_BackContents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
                break;

            case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
                Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
                m_rectTransform_FrontContents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                m_rectTransform_BackContents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
        QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;

        //GameObject miniGameBGMObject = GameObject.Find("BGM");
        //if (miniGameBGMObject != null)
        //{
        //    Destroy(m_audioSource_BGM.gameObject);
        //    miniGameBGMObject.GetComponent<AudioSource>().volume = curSettingData.AudioSetting.BGMVolume * curSettingData.AudioSetting.MasterVolume;
        //}
        //else
        //{
        m_audioSource_BGM.volume = curSettingData.AudioSetting.BGMVolume * curSettingData.AudioSetting.MasterVolume;
        //}

        //m_image_Recipe.sprite = MiniGameItemResourceInterface.Instance.GetRecipeData(m_curRecipe).Background;

        if(m_bisInit)
        {
            m_layout_TutorialPopup.SetActive(false);

            if(m_curGlassItem != null)
            {
                m_curGlassItem.gameObject.SetActive(true);
                m_gameObject_GlassBtn.SetActive(false);
            }
            if(m_curShakerItem != null)
            {
                m_curShakerItem.gameObject.SetActive(true);
                m_gameObject_ShakerBtn.SetActive(false);
            }

            for(int index = 0; index < m_liqurePlaceholderTable.Length; index++)
            {
                InventoryControl.Instance.InventoryLiqurePlaceholders[index].Render(m_liqurePlaceholderTable[index]);
            }

            RecipeSelectionControl.Instance.Render();
        }
    }
    public void Update()
    {
        if(!m_bisInit)
        {
            return;
        }

        #region Liqure Hold
        if (m_curSelectedLiqurePlaceholderIndex != -1)
        {
            m_rectTransform_SelectedLiqureMouseIcon.anchoredPosition = Input.mousePosition - new Vector3(960.0f, 540.0f, 0.0f);

            if(Input.GetMouseButtonDown(1))
            {
                CurSelectedLiqurePlaceholderIndex = -1;
            }


            if (Input.GetMouseButtonDown(0))
            {
                m_ballDropTimer = 0.0f;
                m_image_SelectedLiqureMouseIcon.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
                m_animator_SelectedLiqureMouseIcon.Play("MouseLiqureHolder_StartPour");
                m_animator_SelectedLiqureMouseIcon.ResetTrigger("StopPour");
            }
            else if (Input.GetMouseButton(0) && m_animator_SelectedLiqureMouseIcon.GetCurrentAnimatorStateInfo(0).IsName("MouseLiqureHolder_Pouring"))
            {
                m_ballDropTimer += Time.deltaTime;
                if (m_ballDropTimer >= m_ballDropDistance)
                {
                    m_metaballTransforms.Add(m_metaballPool.Get());
                    m_ballDropTimer = 0.0f;
                }
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_animator_SelectedLiqureMouseIcon.SetTrigger("StopPour");
            }
        }
        if (m_metaballTransforms.Count > 0)
        {
            Vector4[] poses = new Vector4[20];
            for (int index = 0; index < m_metaballTransforms.Count; index++)
            {
                if (index >= poses.Length)
                {
                    break;
                }

                poses[index] = new Vector4()
                {
                    x = (m_metaballTransforms[index].position.x + m_posMidifier.x / 2.0f) / m_posMidifier.x / 2.0f + 0.25f,
                    y = (m_metaballTransforms[index].position.y + m_posMidifier.y / 2.0f) / m_posMidifier.y / 2.0f + 0.25f,
                    z = 0.0f,
                    w = 0.0f
                };
            }

            m_metaballMat.SetVectorArray("_MetaballPoses", poses);
        }
        m_metaballMat.SetInteger("_MetaballPosCount", m_metaballTransforms.Count);
        if(m_curSelectedLiqurePlaceholderIndex >= 0)
        {
            m_metaballMat.SetVector("_Color", MiniGameItemResourceInterface.Instance.GetItemWaveColor(InventoryControl.Instance.InventoryLiqurePlaceholders[m_curSelectedLiqurePlaceholderIndex].CurItemType)[0]);
        }
        #endregion

        if (m_bisMiniGameFinished && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            ResetStatic();
            RecipeSelectionTutorial.Instance.ResetStatic();
            RecipeSelectionControl.Instance.ResetStatic();

            GameObject miniGameBGMObject = GameObject.Find("BGM");
            if (miniGameBGMObject != null)
            {
                Destroy(miniGameBGMObject);
            }
            switch (ChiefCharacterSelectionManager.CurSelectedCharacterType)
            {
                case CharacterResourceDesc.CharacterListType.Absinthe:
                    ChiefMainGameManager.Instance.Render("AbsintheEnd");
                    break;

                case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                    ChiefMainGameManager.Instance.Render("HayaseuraTokusenGinjoEnd");
                    break;

                case CharacterResourceDesc.CharacterListType.Kahlua:
                    ChiefMainGameManager.Instance.Render("KahluaEnd");
                    break;

                case CharacterResourceDesc.CharacterListType.Hpnotiq:
                    ChiefMainGameManager.Instance.Render("HpnotiqEnd");
                    break;

                case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                    ChiefMainGameManager.Instance.Render("CabernetSauvignonEnd");
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_layout_Popup.SetActive(!m_layout_Popup.activeSelf);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        //bool bisSomethingHit = false;

        bool bisOnShaker = false;
        bool bisOnGlass = false;

        bool bisSomthingSelected = false;
        for (int index = raycastHits.Length - 1; index >= 0; index--)
        {
            IBottle curItem = raycastHits[index].collider.GetComponent<IBottle>();

            if (raycastHits[index].collider != null && curItem != null)
            {
                if (curItem.BisShaker)
                {
                    bisOnShaker = true;
                }
                else
                {
                    bisOnGlass = true;
                }

                if(!bisSomthingSelected)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_curSelectedBottle = curItem;
                        InventoryControl.Instance.RenderInventoryLiqurePlaceholders();

                        //bisSomethingHit = true;

                        m_curPourCount = 0;
                    }
                    if (m_curSelectedBottle != null && Input.GetMouseButton(0))
                    {
                        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        if (targetPos.x <= -4.0f)
                        {
                            targetPos.x = -4.0f;
                        }
                        else if (targetPos.x >= 0.0f)
                        {
                            targetPos.x = 0.0f;
                        }

                        if (-1.6f <= targetPos.y)
                        {
                            targetPos.y = -1.6f;
                        }
                        else if (targetPos.y <= -2.75f)
                        {
                            targetPos.y = -2.75f;
                        }

                        m_curSelectedBottle.transform.position = new Vector3()
                        {
                            x = targetPos.x,
                            y = targetPos.y,
                            z = 0.0f
                        };

                        if (!m_curSelectedBottle.BisShaker && m_prevDragPos != -Vector3.one)
                        {
                            m_curGlassItem.AddDragWaveScale(targetPos - m_prevDragPos);
                        }

                        m_prevDragPos = targetPos;

                        //bisSomethingHit = true;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        m_prevDragPos = -Vector3.one;
                    }

                    RenderBottles();
                }
                bisSomthingSelected = true;
            }
        }

        if(m_curSelectedBottle != null)
        {
            //Debug.Log($"bisOnShader: {bisOnShaker}, bisOnGlass: {bisOnGlass}, CurGlassItem: {m_curGlassItem != null}, BIsShader: {m_curSelectedBottle.BisShaker}");
            if (bisOnShaker && bisOnGlass && m_curGlassItem != null && m_curSelectedBottle.BisShaker && Input.GetMouseButtonDown(1))
            {
                m_curGlassItem.AddIngredient(InventoryControl.ItemType.Shaker);
                m_curGlassItem.AddIngredientWaveScale();
            }
        }

        //if(!bisSomethingHit)
        //{
        //    m_curSelectedBottle = null;
        //}
    }

    internal static RecipeSelectionControl.RecipeType CurRecipe
    {
        get
        {
            return m_curRecipe;
        }
    }

    internal IBottle CurSelectedBottle
    {
        get
        {
            return m_curSelectedBottle;
        }
        set
        {
            m_curSelectedBottle = value;
        }
    }

    internal GlassItem CurGlassItem
    {
        get
        {
            return m_curGlassItem;
        }
    }
    internal ShakerItem CurShakerItem
    {
        get
        {
            return m_curShakerItem;
        }
    }

    internal int CurSelectedLiqurePlaceholderIndex
    {
        get
        {
            return m_curSelectedLiqurePlaceholderIndex;
        }
        set
        {
            m_prevSelectedLiqurePlaceholderIndex = m_curSelectedLiqurePlaceholderIndex;
            m_curSelectedLiqurePlaceholderIndex = value;
            if(m_prevSelectedLiqurePlaceholderIndex == -1)
            {
                m_prevSelectedLiqurePlaceholderIndex = m_curSelectedLiqurePlaceholderIndex;
            }

            if (m_curSelectedLiqurePlaceholderIndex == -1)
            {
                m_image_SelectedLiqureMouseIcon.sprite = null;
                m_rectTransform_SelectedLiqureMouseIcon.gameObject.SetActive(false);

                m_curPourCount = 0;
            }
            else
            {
                m_image_SelectedLiqureMouseIcon.sprite = MiniGameItemResourceInterface.Instance.GetItemTitleSprite(InventoryControl.Instance.InventoryLiqurePlaceholders[m_curSelectedLiqurePlaceholderIndex].CurItemType);
                m_rectTransform_SelectedLiqureMouseIcon.gameObject.SetActive(true);
            }

            foreach(var liqurePlaceholder in InventoryControl.Instance.InventoryLiqurePlaceholders)
            {
                liqurePlaceholder.Render(liqurePlaceholder.CurItemType);
            }
        }
    }

    internal static InventoryControl.ItemType[] LiqurePlaceholderTable
    {
        get
        {
            return m_liqurePlaceholderTable;
        }
        set
        {
            m_liqurePlaceholderTable = value;
        }
    }

    #region Unity Callbacks
    public void Reset()
    {
        if(!m_bisInit)
        {
            return;
        }

        SceneManager.LoadScene("MiniGame");

        GameObject glassItem = GameObject.Find("glassItem");
        if (glassItem != null)
        {
            Destroy(glassItem);
        }

        GameObject shakerItem = GameObject.Find("ShakerItem");
        if (shakerItem != null)
        {
            Destroy(shakerItem);
        }

        ResetStatic();
        //RecipeSelectionTutorial.Instance.ResetStatic();
        //RecipeSelectionControl.Instance.ResetStatic(false);

        Init();
    }
    public void CompleteMaking()
    {
        if (!m_bisInit)
        {
            return;
        }

        if (m_curGlassItem != null)
        {
            MiniGameItemResourceDesc.RecipeData recipeData = MiniGameItemResourceInterface.Instance.GetRecipeData(m_curRecipe);

            InventoryControl.ItemType curIngredient = InventoryControl.ItemType.None;

            List<InventoryControl.ItemType> addedIngredientOrder = new List<InventoryControl.ItemType>();
            List<int> addedIngredientCounts = new List<int>();
            foreach(var ingredient in m_curGlassItem.AddedIngredients)
            {
                if(ingredient != curIngredient)
                {
                    addedIngredientOrder.Add(ingredient);
                    addedIngredientCounts.Add(1);
                    curIngredient = ingredient;
                }
                else
                {
                    addedIngredientCounts[addedIngredientCounts.Count - 1]++;
                }
            }

            curIngredient = InventoryControl.ItemType.None;
            List<InventoryControl.ItemType> recipeIngredientOrder = new List<InventoryControl.ItemType>();
            List<int> recipeIngredientCounts = new List<int>();
            foreach(var recipeStepData in recipeData.RecipeStepDatas)
            {
                if(recipeStepData.Ingredient != curIngredient)
                {
                    recipeIngredientOrder.Add(recipeStepData.Ingredient);
                    recipeIngredientCounts.Add(1);
                    curIngredient = recipeStepData.Ingredient;
                }
                else
                {
                    recipeIngredientCounts[recipeIngredientCounts.Count - 1]++;
                }
            }

            Debug.Log($"{addedIngredientOrder.Count} / {recipeIngredientOrder.Count}");

            bool bisFailed = true;
            float score = 0.0f;
            if (recipeData.GlassType != m_curGlassItem.CurGlassType)
            {
                Debug.Log($"Recipe Failed! Wrong Glass Type : {recipeData.GlassType} / {m_curGlassItem.CurGlassType}");
                goto End;
            }
            else if (recipeIngredientOrder.Count == addedIngredientOrder.Count)
            {
                for (int index = 0; index < recipeIngredientOrder.Count; index++)
                {
                    if (recipeIngredientOrder[index] != addedIngredientOrder[index])
                    {
                        Debug.Log("Recipe Failed! Wrong Ingredient Order");
                        goto End;
                    }
                }

                for (int index = 0; index < recipeIngredientCounts.Count; index++)
                {
                    if (Mathf.Abs(recipeIngredientCounts[index] - addedIngredientCounts[index]) > 1)
                    {
                        Debug.Log("Recipe Failed! Wrong Ingredient Count");
                        goto End;
                    }
                }

                for (int index = 0; index < addedIngredientCounts.Count; index++)
                {
                    score += (recipeIngredientCounts[index] - Mathf.Abs(addedIngredientCounts[index] - recipeIngredientCounts[index])) / (float)recipeIngredientCounts[index];
                }
                Debug.Log($"Score: {score}");

                score /= (float)recipeIngredientCounts.Count;
                Debug.Log($"Score: {score}");

                //score *= 10.0f;
                bisFailed = false;
            }
            else
            {
                Debug.Log("Recipe Failed! Wrong Ingredient Count");
            }
            End:

            if (m_curGlassItem != null)
            {
                Destroy(m_curGlassItem.gameObject);
            }
            if (m_curShakerItem != null)
            {
                Destroy(m_curShakerItem.gameObject);
            }

            m_layout_Result.SetActive(true);
            
            m_text_RecipeName.text = recipeData.Name;
            m_image_RecipeResult.sprite = m_sprite_RecipeResults[(int)m_curRecipe];

            if (!bisFailed)
            {
                //m_gameObject_Success.SetActive(true);
                //m_gameObject_Failed.SetActive(false);

                //m_text_Score.text = $"Score: {score * 100.0f:F2}";

                int scoreIconCount = (int)Mathf.Lerp(1, m_gameObject_ScoreIcons.Length, score);
                Debug.Log($"Score Icon Count: {scoreIconCount}");
                for (int index = 0; index < m_gameObject_ScoreIcons.Length; index++)
                {
                    if (index < scoreIconCount)
                    {
                        Invoke(nameof(PlayResultScoreIconAnimation), index * m_scoreIconAnimationDelay);
                    }
                    else
                    {
                        m_gameObject_ScoreIcons[index].SetActive(false);
                    }
                }
                Invoke(nameof(SetMiniGameFinished), scoreIconCount * m_scoreIconAnimationDelay);

                Debug.Log("Recipe Success! Score: " + score);

                m_image_Stamp.sprite = m_sprite_Stamps[0];
                m_image_StampBackground.sprite = m_sprite_Stamps[0];
            }
            else
            {
                //m_gameObject_Success.SetActive(false);
                //m_gameObject_Failed.SetActive(true);

                //m_text_Score.text = $"Score: {score * 100.0f:F2}";

                foreach(GameObject scoreIcon in m_gameObject_ScoreIcons)
                {
                    scoreIcon.SetActive(false);
                }
                m_gameObject_ScoreIcons[0].SetActive(true);

                Invoke(nameof(SetMiniGameFinished), m_scoreIconAnimationDelay);

                Debug.Log("Recipe Failed! Score: " + score);

                m_image_Stamp.sprite = m_sprite_Stamps[1];
                m_image_StampBackground.sprite = m_sprite_Stamps[1];
            }

            Invoke(nameof(PlatResultStampSFX), m_stampSFXTiming);
        }

        //SceneManager.LoadScene("DemoEnd");
    }
    public void TrySaveResultScreenShot()
    {
        if (!m_bisInit)
        {
            return;
        }

        m_animator_CameraFlash.Play("CameraFlash");

        string path = System.IO.Path.Combine(Application.streamingAssetsPath,
            $"ScreenShots/MiniGame/{MiniGameItemResourceInterface.Instance.GetRecipeData(m_curRecipe).Name}_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.png");
        
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
            Debug.Log($"Saved MiniGame Result Screen Shot to {path}");
        }
        else
        {
            Debug.LogError("Render Texture is not set!");
        }
    }

    public void OpenSetting()
    {
        ChiefSettingManager.FromPage = ChiefSettingManager.FromPageType.MiniGame;

        GameObject miniGameBGMObject = GameObject.Find("BGM");
        if (miniGameBGMObject != null)
        {
            Destroy(miniGameBGMObject);
        }

        if (m_curGlassItem != null)
        {
            m_curGlassItem.gameObject.SetActive(false);
        }
        if (m_curShakerItem != null)
        {
            m_curShakerItem.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("Setting");
    }
    public void OpenAlbum()
    {
        ChiefAlbumManager.FromPage = ChiefAlbumManager.FromPageType.MiniGame;

        GameObject miniGameBGMObject = GameObject.Find("BGM");
        if (miniGameBGMObject != null)
        {
            Destroy(miniGameBGMObject);
        }

        if(m_curGlassItem != null)
        {
            m_curGlassItem.gameObject.SetActive(false);
        }
        if(m_curShakerItem != null)
        {
            m_curShakerItem.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("Album");
    }
    public void OpenTitle()
    {
        GameObject miniGameBGMObject = GameObject.Find("BGM");
        if (miniGameBGMObject != null)
        {
            Destroy(miniGameBGMObject);
        }

        if (m_curGlassItem != null)
        {
            Destroy(m_curGlassItem.gameObject);
        }
        if (m_curShakerItem != null)
        {
            Destroy(m_curShakerItem.gameObject);
        }

        SceneManager.LoadScene("Title");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ClosePopup()
    {
        m_layout_Popup.SetActive(false);
    }

    public void OpenRecipePopup()
    {
        m_layout_RecipePopup.SetActive(true);
    }
    public void CloseRecipePopup()
    {
        if(!RecipeSelectionControl.Instance.BisRecipeSelected)
        {
            return;
        }

        m_layout_RecipePopup.SetActive(false);
    }
    #endregion

    internal void Init()
    {
        m_curRecipe = RecipeSelectionControl.CurRecipeType;

        RenderAll();

        m_image_Recipe.sprite = MiniGameItemResourceInterface.Instance.GetRecipeData(m_curRecipe).Background;

        m_bisInit = true;
    }

    internal void RenderAll()
    {
        InventoryControl.Instance.Render();
    }
    internal void RenderBottles()
    {
        foreach (IBottle bottle in m_bottles)
        {
            bottle.Render();
        }
    }

    internal void AddBottle(in IBottle bottle)
    {
        m_bottles.Add(bottle);

        if(!bottle.BisShaker)
        {
            m_curGlassItem = bottle.GetComponent<GlassItem>();
        }
        else
        {
            m_curShakerItem = bottle.GetComponent<ShakerItem>();
        }
    }

    internal void ResetStatic()
    {
        m_curRecipe = RecipeSelectionControl.RecipeType.DeathInTheAfternoon;
        
        m_curSelectedBottle = null;
        m_curGlassItem = null;
        m_curShakerItem = null;

        m_bottles.Clear();

        m_bisInit = false;
        m_liqurePlaceholderTable = new InventoryControl.ItemType[4];
    }

    internal void ReleaseLiqurePourItem(in Transform item, in bool bisAddLiqure, in bool bis2Glass)
    {
        m_metaballPool.Release(item);
        m_metaballTransforms.Remove(item);

        if (bisAddLiqure)
        {
            if (bis2Glass)
            {
                if (!m_curSelectedBottle.BisShaker)
                {
                    m_curPourCount++;

                    if (m_curPourCount >= m_pourItemThreshold)
                    {
                        m_curSelectedBottle.AddIngredient(InventoryControl.Instance.InventoryLiqurePlaceholders[m_prevSelectedLiqurePlaceholderIndex].CurItemType);
                        m_curPourCount = 0;
                    }
                }
            }
            else
            {
                if (m_curSelectedBottle.BisShaker)
                {
                    m_curPourCount++;

                    if (m_curPourCount >= m_pourItemThreshold)
                    {
                        m_curSelectedBottle.AddIngredient(InventoryControl.Instance.InventoryLiqurePlaceholders[m_prevSelectedLiqurePlaceholderIndex].CurItemType);
                        m_curPourCount = 0;
                    }
                }
            }
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void PlayResultScoreIconAnimation()
    {
        if(m_curScoreIconIndex < m_gameObject_ScoreIcons.Length)
        {
            Debug.Log(Time.time);
            m_gameObject_ScoreIcons[m_curScoreIconIndex].SetActive(true);
            m_curScoreIconIndex++;
        }
    }
    private void SetMiniGameFinished()
    {
        m_bisMiniGameFinished = true;
        m_gameObject_ResultText.SetActive(true);
    }

    private void PlatResultStampSFX()
    {
        m_audioSource_Stamp.Play();
    }

    #region Selected Liqure Mouse Effect
    private Transform CreatePooledItem()
    {
        return Instantiate(m_prefab_MetaballItem, Camera.main.ScreenToWorldPoint(Input.mousePosition) + m_liqureDropPosDistance, Quaternion.identity).GetComponent<Transform>();
    }
    private void OnReturnedToPool(Transform system)
    {
        system.gameObject.SetActive(false);
    }
    private void OnTakeFromPool(Transform system)
    {
        system.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + m_liqureDropPosDistance;
        system.gameObject.SetActive(true);
    }
    private void OnDestroyPoolObject(Transform system)
    {
        Destroy(system.gameObject);
    }
    #endregion
}