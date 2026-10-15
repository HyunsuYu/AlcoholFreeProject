using CommonUtilLib.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public sealed class BarReceiptControl : SingleTonForGameObject<BarReceiptControl>
{
    [Serializable] public struct UIs
    {
        [Header("Popup Itself")]
        public Animator Animator_Popup;

        [Header("Receipt Total")]
        public TMP_Text Text_ReceiptTotal;

        [Header("QTY")]
        public GameObject Prefab_ReceiptQTYItem;
        public Transform Transform_ReceiptItemParent;

        [Header("Payment Instruction")]
        public TMP_Text Text_SubTotal;
        public TMP_Text Text_SalesTax;

        public Animator Animator_CharacterSign;
    }


    [SerializeField] private UIs m_UIs;

    private List<InventoryControl.ItemType> m_itemTypes = new List<InventoryControl.ItemType>();
    private List<BarReceiptQTYItem> m_barReceiptQTYItems = new List<BarReceiptQTYItem>();


    public void Awake()
    {
        SetInstance(this);
    }

    #region Unity Callbacks
    public void OpenAndCloseReceipt()
    {
        if (m_UIs.Animator_Popup.GetCurrentAnimatorStateInfo(0).IsName("Receipt_Close")
            || m_UIs.Animator_Popup.GetCurrentAnimatorStateInfo(0).IsName("Receipt_Open"))
        {
            return;
        }

        if(m_UIs.Animator_Popup.GetCurrentAnimatorStateInfo(0).IsName("Receipt_Closing"))
        {
            m_UIs.Animator_Popup.Play("Receipt_Open");

            switch(CharacterSelectionPopup.SelectedCharacter)
            {
                case "Absinthe":
                    m_UIs.Animator_CharacterSign.SetTrigger("AbsintheSign");
                    break;

                case "HayaseuraTokusenGinjo":
                    m_UIs.Animator_CharacterSign.SetTrigger("HayaseuraTokusenGinjoSign");
                    break;

                case "Kahlua":
                    m_UIs.Animator_CharacterSign.SetTrigger("KahluaSign");
                    break;

                case "":
                case "Hpnotiq":
                    m_UIs.Animator_CharacterSign.SetTrigger("HpnotiqSign");
                    break;

                case "CabernetSauvignon":
                    m_UIs.Animator_CharacterSign.SetTrigger("CabernetSauvignonSign");
                    break;
            }
        }
        else if (m_UIs.Animator_Popup.GetCurrentAnimatorStateInfo(0).IsName("Receipt_Opening"))
        {
            m_UIs.Animator_Popup.Play("Receipt_Close");
        }
    }
    #endregion

    internal void AddItem(in InventoryControl.ItemType item)
    {
        switch(item)
        {
            case InventoryControl.ItemType.AbsintheGlass:
            case InventoryControl.ItemType.OnTheRocksGlass:
            case InventoryControl.ItemType.CocktailGlass:
            case InventoryControl.ItemType.WineGlass:
            case InventoryControl.ItemType.HighballGlass:
            case InventoryControl.ItemType.OldFashionedGlass:
            case InventoryControl.ItemType.Shaker:
            case InventoryControl.ItemType.HandShaking:
            case InventoryControl.ItemType.SpoonShaking:

                break;

            default:
                m_itemTypes.Add(item);

                int curItemCount = 0;
                for(int index = m_itemTypes.Count - 1; index >= 0; index--)
                {
                    if (m_itemTypes[index] == item)
                    {
                        curItemCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (m_barReceiptQTYItems.Count > 0 && m_barReceiptQTYItems.Last().ItemType == item)
                {
                    m_barReceiptQTYItems.Last().Render(curItemCount, item, MiniGameItemResourceInterface.Instance.GetItemPrice(item));
                }
                else
                {
                    var curItem = Instantiate(m_UIs.Prefab_ReceiptQTYItem, m_UIs.Transform_ReceiptItemParent).GetComponent<BarReceiptQTYItem>();
                    curItem.Render(curItemCount, item, MiniGameItemResourceInterface.Instance.GetItemPrice(item));
                    m_barReceiptQTYItems.Add(curItem);
                }

                Render();
                break;
        }
    }

    internal void Render()
    {
        int totalPrice = 0;
        foreach(var item in m_itemTypes)
        {
            totalPrice += MiniGameItemResourceInterface.Instance.GetItemPrice(item);
        }

        m_UIs.Text_SubTotal.text = totalPrice.ToString("0.00");

        float salesTax = Mathf.Ceil(totalPrice * 0.0625f * 100.0f) / 100.0f;
        m_UIs.Text_SalesTax.text = salesTax.ToString("0.00");

        m_UIs.Text_ReceiptTotal.text = $"${(totalPrice + salesTax).ToString("0.00")}";
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}