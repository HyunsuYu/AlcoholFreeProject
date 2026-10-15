using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class BarHintControl : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] private GameObject m_layout_Profile;
    [SerializeField] private GameObject m_layout_SpeakText;

    [SerializeField] private GameObject[] m_gameObject_PrifileCharacters;
    [SerializeField] private TMP_Text m_text_SpeakText;

    [Header("Play Settings")]
    [SerializeField] private BarHintTextData m_barHintTextData;

    [SerializeField] private GameObject m_gameObject_RestartScriptBtn;

    //[SerializeField] private float m_speakInterval = 5.0f;

    //private float m_speakTimer = 0.0f;
    private int m_randomTextPackIndex = -1;
    private int m_curSpeakIndex = 0;

    //private bool m_bisSpeak = true;


    public void Awake()
    {
        m_gameObject_RestartScriptBtn.SetActive(false);
    }
    public void Start()
    {
        m_randomTextPackIndex = UnityEngine.Random.Range(0, m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks.Length);

        switch(ChiefMiniGameManager.CurRecipe)
        {
            case RecipeSelectionControl.RecipeType.DeathInTheAfternoon:
            case RecipeSelectionControl.RecipeType.Absinthe:
                m_gameObject_PrifileCharacters[0].SetActive(true);
                break;

            case RecipeSelectionControl.RecipeType.Calimocho:
            case RecipeSelectionControl.RecipeType.SplitzerLuge:
                m_gameObject_PrifileCharacters[1].SetActive(true);
                break;

            case RecipeSelectionControl.RecipeType.Saketini:
            case RecipeSelectionControl.RecipeType.SamuraiRock:
                m_gameObject_PrifileCharacters[2].SetActive(true);
                break;

            case RecipeSelectionControl.RecipeType.HipKiss:
            case RecipeSelectionControl.RecipeType.IncredibleHulk:
                m_gameObject_PrifileCharacters[3].SetActive(true);
                break;

            case RecipeSelectionControl.RecipeType.BlackRussian:
            case RecipeSelectionControl.RecipeType.FlatWhiteRussian:
                m_gameObject_PrifileCharacters[4].SetActive(true);
                break;
        }

        PrintTargetScript(0);
    }
    //public void FixedUpdate()
    //{
    //    if(!m_bisSpeak)
    //    {
    //        return;
    //    }

    //    m_speakTimer += Time.fixedDeltaTime;

    //    if (m_speakTimer >= m_speakInterval || m_curSpeakIndex == 0)
    //    {
    //        m_layout_Profile.SetActive(true);
    //        m_layout_SpeakText.SetActive(true);

    //        m_text_SpeakText.text = "{fade}" + m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks[m_randomTextPackIndex].TextDatas[m_curSpeakIndex].Text + "{/fade}";
    //        m_curSpeakIndex++;
    //        m_speakTimer = 0.0f;

    //        if(m_curSpeakIndex >= m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks[m_randomTextPackIndex].TextDatas.Length)
    //        {
    //            m_bisSpeak = false;

    //            m_layout_Profile.SetActive(false);
    //            m_layout_SpeakText.SetActive(false);
    //        }
    //    }
    //}

    #region Unity Callbacks
    public void PrintNextScript()
    {
        m_curSpeakIndex++;
        PrintTargetScript(m_curSpeakIndex);

        if (m_curSpeakIndex >= m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks[m_randomTextPackIndex].TextDatas.Length)
        {
            m_layout_Profile.SetActive(false);
            m_layout_SpeakText.SetActive(false);

            m_gameObject_RestartScriptBtn.SetActive(true);

            m_text_SpeakText.text = string.Empty;
        }
    }
    public void RestartScript()
    {
        m_curSpeakIndex = 0;
        PrintTargetScript(0);

        m_layout_Profile.SetActive(true);
        m_layout_SpeakText.SetActive(true);

        m_gameObject_RestartScriptBtn.SetActive(false);
    }
    #endregion

    private void PrintTargetScript(in int targetScriptIndex)
    {
        if (m_curSpeakIndex >= m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks[m_randomTextPackIndex].TextDatas.Length)
        {
            return;
        }

        m_text_SpeakText.text = "{fade}" + m_barHintTextData.SingleRecipeScripts[(int)ChiefMiniGameManager.CurRecipe].MainTextPacks[m_randomTextPackIndex].TextDatas[targetScriptIndex].Text + "{/fade}";
    }
}