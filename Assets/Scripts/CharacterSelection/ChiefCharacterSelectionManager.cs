using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefCharacterSelectionManager : SingleTonForGameObject<ChiefCharacterSelectionManager>
{
    private static CharacterResourceDesc.CharacterListType m_curSelectedCharacterType = CharacterResourceDesc.CharacterListType.MainCharacter;

    [SerializeField] private RectTransform m_rectTransform_Contents;

    [SerializeField] private CharacterSelectionItem[] m_characterSelectionItems;

    [SerializeField] private CharacterSelectionPopup m_layout_Popup;

    [SerializeField] private TMP_InputField m_inputField_MainCharacterName;

    private bool m_bisPopupActive = false;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        SettingData curSettingData = SettingDataBuffer.Instance.SettingData;
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
    }
    public void FixedUpdate()
    {
        if(m_bisPopupActive)
        {
            return;
        }

        RaycastHit[] hits = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);

        foreach(CharacterSelectionItem item in m_characterSelectionItems)
        {
            item.BIsFloating = false;
        }

        bool bisSelectSomething = false;
        for (int index = 0; index < hits.Length; index++)
        {
            if (hits[index].collider.transform.parent != null)
            {
                CharacterSelectionItem selectionItem = hits[index].collider.transform.parent.GetComponent<CharacterSelectionItem>();
                if (selectionItem != null)
                {
                    m_curSelectedCharacterType = selectionItem.CharacterListType;
                    bisSelectSomething = true;
                    selectionItem.BIsFloating = true;
                    break;
                }
            }
        }

        if (!bisSelectSomething)
        {
            m_curSelectedCharacterType = CharacterResourceDesc.CharacterListType.MainCharacter;
        }
    }
    public void Update()
    {
        if(!m_bisPopupActive
            && m_curSelectedCharacterType != CharacterResourceDesc.CharacterListType.MainCharacter
            && Input.GetMouseButtonDown(0))
        {
            m_bisPopupActive = true;
            m_layout_Popup.gameObject.SetActive(true);
        }

        if(!string.IsNullOrEmpty(NoonCanvasControl.MainCharacterName) && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
        {
            OpenTutorial();
        }
    }

    internal static CharacterResourceDesc.CharacterListType CurSelectedCharacterType
    {
        get
        {
            return m_curSelectedCharacterType;
        }
        set
        {
            m_curSelectedCharacterType = value;
        }
    }
    internal bool BIsPopupActive
    {
        set
        {
            m_bisPopupActive = value;
            m_layout_Popup.gameObject.SetActive(value);
        }
    }

    #region Unity Callbacks
    public void GoBack2Title()
    {
        GameObject titleSCreenBGMObjectt = GameObject.Find("BGM");
        if(titleSCreenBGMObjectt != null)
        {
            titleSCreenBGMObjectt.name = "OldBGM";
        }

        SceneManager.LoadScene("Title");
    }

    public void TrySetMainCharacterName()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        NoonCanvasControl.MainCharacterName = m_inputField_MainCharacterName.text;
    }
    public void OpenTutorial()
    {
        Invoke(nameof(OpenTutorialWithDelay), 0.5f);
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void OpenTutorialWithDelay()
    {
        //ChiefMainGameManager.Instance.Render("Tutorial");
        switch (m_curSelectedCharacterType)
        {
            case CharacterResourceDesc.CharacterListType.Absinthe:
                //m_selectedCharacter = "Absinthe";
                ChiefMainGameManager.Instance.Render("Absinthe");
                break;

            case CharacterResourceDesc.CharacterListType.HayaseuraTokusenGinjo:
                //m_selectedCharacter = "HayaseuraTokusenGinjo";
                ChiefMainGameManager.Instance.Render("HayaseuraTokusenGinjo");
                break;

            case CharacterResourceDesc.CharacterListType.Kahlua:
                //m_selectedCharacter = "Kahlua";
                ChiefMainGameManager.Instance.Render("Kahlua");
                break;

            case CharacterResourceDesc.CharacterListType.Hpnotiq:
                //m_selectedCharacter = "Hpnotiq";
                ChiefMainGameManager.Instance.Render("Hpnotiq");
                break;

            case CharacterResourceDesc.CharacterListType.CabernetSauvignon:
                //m_selectedCharacter = "CabernetSauvignon";
                ChiefMainGameManager.Instance.Render("CabernetSauvignon");
                break;
        }

        Destroy(GameObject.Find("BGM"));
    }
}