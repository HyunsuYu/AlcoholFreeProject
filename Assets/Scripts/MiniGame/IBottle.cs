using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public abstract class IBottle : MonoBehaviour
{
    protected List<InventoryControl.ItemType> m_addedIngredients = new List<InventoryControl.ItemType>();

    [SerializeField] private bool m_bisShaker = false;

    [SerializeField] protected GameObject m_layout_AddedBarBackground;
    [SerializeField] protected Image[] m_image_AddedBars;

    [SerializeField] protected Image[] m_image_selectedState;
    [SerializeField] protected SpriteRenderer m_spriteRenderer_SelectedState;

    [SerializeField] private Animator m_shakeAnimator;
    [SerializeField] protected float m_animationDuration = 2.0f;

    protected int m_invisibleIngredientIndex = 0;


    internal bool BisShaker
    {
        get
        {
            return m_bisShaker;
        }
    }
    internal List<InventoryControl.ItemType> AddedIngredients
    {
        get
        {
            return m_addedIngredients;
        }
    }

    internal int InvisibleIngredientIndex
    {
        set
        {
            m_invisibleIngredientIndex = value;
        }
    }

    internal void AddIngredient(in InventoryControl.ItemType targetIngredient, in bool bisFromShaker = false)
    {
        switch (targetIngredient)
        {
            case InventoryControl.ItemType.None:

            case InventoryControl.ItemType.AbsintheGlass:
            case InventoryControl.ItemType.OnTheRocksGlass:
            case InventoryControl.ItemType.CocktailGlass:
            case InventoryControl.ItemType.WineGlass:
            case InventoryControl.ItemType.HighballGlass:
            case InventoryControl.ItemType.OldFashionedGlass:

            //case InventoryControl.ItemType.Shaker:
                return;
        }

        if (m_bisShaker)
        {
            switch (targetIngredient)
            {
                case InventoryControl.ItemType.LemonSlice:
                case InventoryControl.ItemType.OliveSkewers:
                case InventoryControl.ItemType.LimeSlice:
                case InventoryControl.ItemType.SilverSpoonWithSugarCube:
                case InventoryControl.ItemType.BarIce:
                case InventoryControl.ItemType.SphereIce:
                case InventoryControl.ItemType.CubeIce:
                    return;

                case InventoryControl.ItemType.HandShaking:
                    m_shakeAnimator.gameObject.SetActive(true);
                    m_shakeAnimator.Play("ShakeShaker");

                    Invoke(nameof(DisableShakerAnimator), m_animationDuration);
                    break;
            }
        }
        else
        {
            switch (targetIngredient)
            {
                //case InventoryControl.ItemType.HandShaking:
                //    return;

                case InventoryControl.ItemType.Ice:
                case InventoryControl.ItemType.BarIce:
                case InventoryControl.ItemType.SphereIce:
                case InventoryControl.ItemType.CubeIce:
                    if (!bisFromShaker)
                    {
                        GetComponent<GlassItem>().AddIce((GlassItem.IceType)targetIngredient);
                    }
                    break;

                case InventoryControl.ItemType.SpoonShaking:
                    GetComponent<GlassItem>().ShakeWithSpoon();
                    break;
            }
        }

        //if (m_addedIngredients.Count + 1 >= 11)
        //{
        //    return;
        //}
        m_addedIngredients.Add(targetIngredient);
        Debug.Log($"Adding ingredient: {targetIngredient}. Total ingredients: {m_addedIngredients.Count}");
        Render();
    }

    internal abstract void Render();

    protected void RenderState(in bool bisShaker, in GlassItem.GlassType glassType = GlassItem.GlassType.AbsintheGlass)
    {
        if (m_addedIngredients.Count == 0)
        {
            m_layout_AddedBarBackground.SetActive(false);
        }
        else
        {
            m_layout_AddedBarBackground.SetActive(true);
            for (int index = 0; index < m_image_AddedBars.Length; index++)
            {
                int visiableIngredientIndex = index + m_invisibleIngredientIndex;
                if (visiableIngredientIndex < m_addedIngredients.Count)
                {
                    m_image_AddedBars[index].color = MiniGameItemResourceInterface.Instance.GetItemColor(m_addedIngredients[visiableIngredientIndex]);
                    m_image_AddedBars[index].gameObject.SetActive(true);
                }
                else
                {
                    m_image_AddedBars[index].color = Color.clear;
                    m_image_AddedBars[index].gameObject.SetActive(false);
                }
            }
        }

        foreach(var item in m_image_selectedState)
        {
            if(item == null)
            {
                continue;
            }

            item.gameObject.SetActive(false);
        }
        if (bisShaker)
        {
            m_spriteRenderer_SelectedState.gameObject.SetActive(false);
            if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
            {
                //m_image_selectedState[0].gameObject.SetActive(true);
                m_spriteRenderer_SelectedState.gameObject.SetActive(true);
            }
        }
        else
        {
            switch(glassType)
            {
                case GlassItem.GlassType.AbsintheGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[1].gameObject.SetActive(true);
                    }
                    break;

                case GlassItem.GlassType.OnTheRocksGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[2].gameObject.SetActive(true);
                    }
                    break;

                case GlassItem.GlassType.CocktailGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[3].gameObject.SetActive(true);
                    }
                    break;

                case GlassItem.GlassType.WineGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[4].gameObject.SetActive(true);
                    }
                    break;

                case GlassItem.GlassType.HighballGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[5].gameObject.SetActive(true);
                    }
                    break;

                case GlassItem.GlassType.OldFashionedGlass:
                    if (ChiefMiniGameManager.Instance.CurSelectedBottle == this)
                    {
                        m_image_selectedState[6].gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }

    private void DisableShakerAnimator()
    {
        m_shakeAnimator.Play("Idle");
    }
}