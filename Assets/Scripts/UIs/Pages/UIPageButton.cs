using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public sealed class UIPageButton : MonoBehaviour
{
    [SerializeField] private Image m_image_Icon;
    [SerializeField] private TMP_Text m_text_ButtonText;

    [SerializeField] private Sprite[] m_icons;

    private int m_pageIndex = -1;


    public void OpenPage()
    {
        if(m_text_ButtonText.text == "Play")
        {
            ////NoonCanvasControl.Instance.Render(ChiefUIManager.Instance.CurSelectedSceneID, ChiefUIManager.Instance.CurSelectedFrameIndex);
            //GameObject dtoObject = Instantiate(m_prefab_Editor2PreviewDTO);

            //Editor2PreviewDTO dto = dtoObject.GetComponent<Editor2PreviewDTO>();
            //dto.TargetSceneID = ChiefUIManager.Instance.CurSelectedSceneID;
            //dto.TargetFrameIndex = ChiefUIManager.Instance.CurSelectedFrameIndex;
            //dto.BIsPreview = true;

            //DontDestroyOnLoad(dtoObject);

            //SceneManager.LoadScene("MainGame_Noon");

            Guid curSelectedSceneID = ChiefUIManager.Instance.CurSelectedSceneID;
            int curSelectedFrameIndex = ChiefUIManager.Instance.CurSelectedFrameIndex;

            ChiefUIManager.Instance.CurSelectedSceneID = Guid.Empty;
            ChiefUIManager.Instance.CurSelectedFrameIndex = -1;

            ChiefUIManager.Instance.CurSelectedAppearCharacterIndex = -1;
            ChiefUIManager.Instance.CurSelectedBranchIndex = -1;
            ChiefUIManager.Instance.CurSelectedCutSceneIndex = -1;

            ChiefMainGameManager.Instance.RenderForEditorPreview(curSelectedSceneID, curSelectedFrameIndex);

            return;
        }

        PageSynchronizer.Instance.OpenPage(m_pageIndex);

        if(Enum.IsDefined(typeof(TotalEdgyToolBarControl.PageType), m_pageIndex))
        {
            ChiefUIManager.Instance.CurPageType = (TotalEdgyToolBarControl.PageType)m_pageIndex;
        }
    }

    internal void Init(in string buttonText, in int pageIndex)
    {
        m_text_ButtonText.text = buttonText;
        switch(buttonText)
        {
            case "Edit":
                m_image_Icon.sprite = m_icons[0];
                break;

            case "Plot Tree":
                m_image_Icon.sprite = m_icons[1];
                break;

            case "Play":
                m_image_Icon.sprite = m_icons[2];
                break;
        }

        m_pageIndex = pageIndex;
    }
}