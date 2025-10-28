using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class InventoryLiqurePlaceholder : MonoBehaviour
{
    [SerializeField] private int m_placeholderIndex;

    [SerializeField] private Image m_image_MainImage;
    [SerializeField] private TMP_Text m_text_Name;

    [SerializeField] private Image[] m_image_AdditionBars;

    [SerializeField] private GameObject m_gameObject_AnouncementLetter;

    private InventoryControl.ItemType m_curItemType = InventoryControl.ItemType.None;

    //[SerializeField] private GraphicRaycaster m_graphicRaycaster;
    //[SerializeField] private EventSystem m_eventSystem;

    //[SerializeField] private RectTransform m_rectTransform_LiqureInfo;
    //[SerializeField] private TMP_Text m_text_LiqureInfo;

    //[SerializeField] private Vector3 m_LiqureInfoPopupDistance;


    public void Update()
    {
        if(ChiefMiniGameManager.Instance.CurSelectedLiqurePlaceholderIndex != m_placeholderIndex)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            TryAddLiqure();
        }
    }
    //public void FixedUpdate()
    //{
    //    if(m_curItemType == InventoryControl.ItemType.None)
    //    {
    //        return;
    //    }

    //    List<RaycastResult> raycastResults = new List<RaycastResult>();
    //    m_graphicRaycaster.Raycast(new PointerEventData(m_eventSystem)
    //    {
    //        position = Input.mousePosition
    //    }, raycastResults);

    //    bool bisOnLiqureHolder = false;
    //    foreach (RaycastResult result in raycastResults)
    //    {
    //        if(result.gameObject.GetComponent<Button>() != null && result.gameObject.transform.parent.name.Contains("Layout_Left"))
    //        {
    //            bisOnLiqureHolder = true;
    //        }

    //        if (result.gameObject.GetComponent<Button>() != null && result.gameObject.transform.parent.name == $"Layout_Left_{m_placeholderIndex}")
    //        {
    //            m_rectTransform_LiqureInfo.gameObject.SetActive(true);
    //            m_rectTransform_LiqureInfo.anchoredPosition = Input.mousePosition - new Vector3(960.0f, 540.0f, 0.0f) + m_LiqureInfoPopupDistance;

    //            m_text_LiqureInfo.text = MiniGameItemResourceInterface.Instance.GetItemDescription(m_curItemType);
    //            return;
    //        }
    //    }

    //    if(!bisOnLiqureHolder)
    //    {
    //        m_rectTransform_LiqureInfo.gameObject.SetActive(false);
    //    }
    //}

    internal InventoryControl.ItemType CurItemType
    {
        get
        {
            return m_curItemType;
        }
    }

    #region Unity Callbacks
    public void TrySetLiqure()
    {
        if(m_curItemType != InventoryControl.ItemType.None)
        {
            ChiefMiniGameManager.Instance.CurSelectedLiqurePlaceholderIndex = m_placeholderIndex;
            return;
        }

        InventoryControl.Instance.CurInventoryPageType = InventoryControl.InventoryTabType.Alcohol;
        InventoryControl.Instance.CurSelectedPlaceholderIndex = m_placeholderIndex;

        InventoryControl.Instance.OpenInventoryTab();
    }
    public void TryAddLiqure()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in raycastHits)
        {
            if (hit.collider.GetComponent<IBottle>() != null
                && ChiefMiniGameManager.Instance.CurSelectedBottle != null
                && ChiefMiniGameManager.Instance.CurSelectedBottle == hit.collider.GetComponent<IBottle>())
            {
                ChiefMiniGameManager.Instance.CurSelectedBottle.AddIngredient(m_curItemType);
                if (!ChiefMiniGameManager.Instance.CurSelectedBottle.BisShaker)
                {
                    ChiefMiniGameManager.Instance.CurSelectedBottle.GetComponent<GlassItem>().AddIngredientWaveScale();
                }
                InventoryControl.Instance.RenderInventoryLiqurePlaceholders();
            }
        }
    }
    #endregion

    internal void Render(in InventoryControl.ItemType targetItemType)
    {
        m_curItemType = targetItemType;

        if(m_curItemType != InventoryControl.ItemType.None)
        {
            m_gameObject_AnouncementLetter.SetActive(false);

            m_image_MainImage.sprite = MiniGameItemResourceInterface.Instance.GetItemTitleSprite(m_curItemType);
            m_image_MainImage.color = Color.white;

            //m_text_Name.text = MiniGameItemResourceInterface.Instance.GetItemName(m_curItemType);
            string name = string.Empty;
            foreach(char letter in MiniGameItemResourceInterface.Instance.GetItemName(m_curItemType))
            {
                name += letter.ToString() + "\n";
            }
            m_text_Name.text = name;

            if (ChiefMiniGameManager.Instance.CurSelectedBottle != null)
            {
                int curBottleIngredientCount = ChiefMiniGameManager.Instance.CurSelectedBottle.AddedIngredients.FindAll(x => x == m_curItemType).Count;

                Color additionBarColor = MiniGameItemResourceInterface.Instance.GetItemColor(m_curItemType);
                //for (int index = 0; index < m_image_AdditionBars.Length; index++)
                //{
                //    m_image_AdditionBars[index].color = additionBarColor;

                //    if (index < curBottleIngredientCount)
                //    {
                //        m_image_AdditionBars[index].gameObject.SetActive(true);
                //    }
                //    else
                //    {
                //        m_image_AdditionBars[index].gameObject.SetActive(false);
                //    }
                //}
            }

            if(ChiefMiniGameManager.Instance.CurSelectedLiqurePlaceholderIndex == m_placeholderIndex)
            {
                m_image_MainImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                m_image_MainImage.color = Color.white;
            }
        }
    }
}