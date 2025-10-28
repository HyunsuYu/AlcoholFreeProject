using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;


public sealed class RecipeSelectionControl : SingleTonForGameObject<RecipeSelectionControl>
{
    public enum RecipeType
    {
        DeathInTheAfternoon,
        Absinthe,
        BlackRussian,
        FlatWhiteRussian,
        SplitzerLuge,
        Calimocho,
        HipKiss,
        IncredibleHulk,
        Saketini,
        SamuraiRock
    }


    //[SerializeField] private RectTransform m_rectTransform_Contents;

    private static RecipeType m_curRecipeType = RecipeType.DeathInTheAfternoon;

    [SerializeField] private Image m_image_Background;

    [SerializeField] private GameObject m_leftRecipeBtn;
    [SerializeField] private GameObject m_rightRecipeBtn;

    //[SerializeField] private Image m_image_LeftRecipeBtn;
    //[SerializeField] private Image m_image_RightRecipeBtn;

    //[SerializeField] private AudioSource m_audioSource_BGM;

    [Header("Popup")]
    [SerializeField] private GameObject m_layout_RecipeSelectionPopup;
    [SerializeField] private GameObject m_layout_ESCPopup;
    [SerializeField] private GameObject m_layout_CheckPopup;

    [Header("Selected State")]
    [SerializeField] private Image m_image_SelectedBackground;
    [SerializeField] private Image m_image_SelectedStamp;

    [SerializeField] private Sprite[] m_sprite_SelectedBackgrounds;
    [SerializeField] private Sprite[] m_sprite_SelectedStamps;

    private static RecipeType m_selectedRecipe = RecipeType.DeathInTheAfternoon;

    private static bool m_bisSelected = false;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        Render();

        //SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
        //switch (curSettingData.ScreenSetting.CurResolution)
        //{
        //    case SettingData.ScreenSettingData.Resolution.FHD_1920x1080:
        //        Screen.SetResolution(1920, 1080, curSettingData.ScreenSetting.BIsFullScreen);
        //        m_rectTransform_Contents.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //        break;

        //    case SettingData.ScreenSettingData.Resolution.QHD_2560x1440:
        //        Screen.SetResolution(2560, 1440, curSettingData.ScreenSetting.BIsFullScreen);
        //        m_rectTransform_Contents.localScale = new Vector3(1.333333f, 1.333333f, 1.0f);
        //        break;

        //    case SettingData.ScreenSettingData.Resolution.UHD_3840x2160:
        //        Screen.SetResolution(3840, 2160, curSettingData.ScreenSetting.BIsFullScreen);
        //        m_rectTransform_Contents.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        //        break;
        //}
        //QualitySettings.vSyncCount = curSettingData.ScreenSetting.BIsVSync ? 1 : 0;

        //m_audioSource_BGM.volume = curSettingData.AudioSetting.BGMVolume * curSettingData.AudioSetting.MasterVolume;
    }
    //public void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        m_layout_ESCPopup.SetActive(true);
    //        CloseCheckPopup();
    //    }
    //}

    internal static RecipeType CurRecipeType
    {
        get
        {
            return m_curRecipeType;
        }
    }

    internal bool BisRecipeSelected
    {
        get
        {
            return m_bisSelected;
        }
    }

    #region Unity Callbacks
    public void OpenLeftRecipe()
    {
        if (Enum.IsDefined(typeof(RecipeType), m_curRecipeType - 1))
        {
            m_curRecipeType--;
        }
        else
        {
            m_curRecipeType = (RecipeType)(Enum.GetValues(typeof(RecipeType)).Length - 1);
        }
        Render();
    }
    public void OpenRightRecipe()
    {
        if (Enum.IsDefined(typeof(RecipeType), m_curRecipeType + 1))
        {
            m_curRecipeType++;
        }
        else
        {
            m_curRecipeType = 0;
        }
        Render();
    }

    //public void OpenSetting()
    //{
    //    ChiefSettingManager.FromPage = ChiefSettingManager.FromPageType.RecipeSelection;
    //    SceneManager.LoadScene("Setting");
    //}
    //public void OpenAlbum()
    //{
    //    ChiefAlbumManager.FromPage = ChiefAlbumManager.FromPageType.RecipeSelection;
    //    SceneManager.LoadScene("Album");
    //}
    //public void OpenTitle()
    //{
    //    SceneManager.LoadScene("Title");
    //}
    //public void QuitGame()
    //{
    //    Application.Quit();
    //}
    //public void CloseESCPopup()
    //{
    //    m_layout_ESCPopup.SetActive(false);
    //}

    public void StartMiniGame()
    {
        //DontDestroyOnLoad(m_audioSource_BGM.gameObject);
        //SceneManager.LoadScene("MiniGame");

        ChiefMiniGameManager.Instance.Init();
        CloseCheckPopup();
        m_layout_RecipeSelectionPopup.SetActive(false);

        m_selectedRecipe = m_curRecipeType;
        m_bisSelected = true;

        Render();
    }
    public void OpenCheckPopup()
    {
        if(m_bisSelected)
        {
            return;
        }

        m_layout_CheckPopup.SetActive(true);
    }
    public void CloseCheckPopup()
    {
        m_layout_CheckPopup.SetActive(false);
    }
    
    #endregion

    internal void Render()
    {
        //if(m_curRecipeType == 0)
        //{
        //    m_leftRecipeBtn.SetActive(false);
        //}
        //else if (Enum.GetNames(typeof(RecipeType)).Last() == m_curRecipeType.ToString())
        //{
        //    m_rightRecipeBtn.SetActive(false);
        //}
        //else
        //{
        //    m_leftRecipeBtn.SetActive(true);
        //    m_rightRecipeBtn.SetActive(true);
        //}

        var recipeData = MiniGameItemResourceInterface.Instance.GetRecipeData(m_curRecipeType);
        m_image_Background.sprite = recipeData.Background;
        //m_image_LeftRecipeBtn.color = recipeData.ThemeColor;
        //m_image_RightRecipeBtn.color = recipeData.ThemeColor;

        if (m_bisSelected)
        {
            if(m_curRecipeType == m_selectedRecipe)
            {
                switch (m_selectedRecipe)
                {
                    case RecipeType.DeathInTheAfternoon:
                    case RecipeType.Absinthe:
                        m_image_SelectedBackground.sprite = m_sprite_SelectedBackgrounds[0];
                        m_image_SelectedStamp.sprite = m_sprite_SelectedStamps[0];
                        break;

                    case RecipeType.BlackRussian:
                    case RecipeType.FlatWhiteRussian:
                        m_image_SelectedBackground.sprite = m_sprite_SelectedBackgrounds[1];
                        m_image_SelectedStamp.sprite = m_sprite_SelectedStamps[1];
                        break;

                    case RecipeType.SplitzerLuge:
                    case RecipeType.Calimocho:
                        m_image_SelectedBackground.sprite = m_sprite_SelectedBackgrounds[2];
                        m_image_SelectedStamp.sprite = m_sprite_SelectedStamps[2];
                        break;

                    case RecipeType.HipKiss:
                    case RecipeType.IncredibleHulk:
                        m_image_SelectedBackground.sprite = m_sprite_SelectedBackgrounds[3];
                        m_image_SelectedStamp.sprite = m_sprite_SelectedStamps[3];
                        break;

                    case RecipeType.Saketini:
                    case RecipeType.SamuraiRock:
                        m_image_SelectedBackground.sprite = m_sprite_SelectedBackgrounds[4];
                        m_image_SelectedStamp.sprite = m_sprite_SelectedStamps[4];
                        break;
                }
                m_image_SelectedBackground.gameObject.SetActive(true);
                m_image_SelectedStamp.gameObject.SetActive(true);
            }
            else
            {
                m_image_SelectedBackground.gameObject.SetActive(false);
                m_image_SelectedStamp.gameObject.SetActive(false);
            }
        }
        else
        {
            m_image_SelectedBackground.gameObject.SetActive(false);
            m_image_SelectedStamp.gameObject.SetActive(false);
        }
    }

    internal void ResetStatic()
    {
        m_curRecipeType = RecipeType.DeathInTheAfternoon;
        m_selectedRecipe = RecipeType.DeathInTheAfternoon;
        m_bisSelected = false;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
}