using System;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public sealed class MiniGameItemResourceInterface : SingleTonForGameObject<MiniGameItemResourceInterface>
{
    [SerializeField] private MiniGameItemResourceDesc m_miniGameItemResourceDesc;


    public void Awake()
    {
        SetInstance(this);
    }

    internal MiniGameItemResourceDesc.ItemData[] AlcoholDatas
    { 
        get
        {
            return m_miniGameItemResourceDesc.AlcoholItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] NonAlcoholDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.NonAlcoholItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] SubIngredientDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.SubIngredientItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] GarnishDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.GarnishItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] IceDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.IceItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] GlassDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.GlassItems;
        }
    }
    internal MiniGameItemResourceDesc.ItemData[] ToolDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.ToolItems;
        }
    }

    internal MiniGameItemResourceDesc.RecipeData[] RecipeDatas
    {
        get
        {
            return m_miniGameItemResourceDesc.RecipeDatas;
        }
    }

    internal Sprite GetItemSprite(in InventoryControl.ItemType targetItemType)
    {
        Sprite result = null;

        if(targetItemType != InventoryControl.ItemType.None)
        {
            if(InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                int length = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].sprites.Length;
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                int length = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].sprites.Length;
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                int length = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].sprites.Length;
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                int length = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].sprites.Length;
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                int length = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].sprites.Length;
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                int length = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].sprites.Length;
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                int length = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].sprites.Length;
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].sprites[UnityEngine.Random.Range(0, length)];
            }
            else if(InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                int length = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].sprites.Length;
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].sprites[UnityEngine.Random.Range(0, length)];
            }
        }

        return result;
    }
    internal Sprite GetItemTitleSprite(in InventoryControl.ItemType targetItemType)
    {
        Sprite result = null;

        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].sprites[0];
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].sprites[0];
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].sprites[0];
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].sprites[0];
            }
            else if(InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].sprites[0];
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].sprites[0];
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].sprites[0];
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].sprites[0];
            }
        }

        return result;
    }
    internal Sprite[] GetItemSprites(in InventoryControl.ItemType targetItemType)
    {
        Sprite[] result = null;
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].sprites;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].sprites;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].sprites;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].sprites;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].sprites;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].sprites;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].sprites;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].sprites;
            }
        }
        return result;
    }

    internal string GetItemName(in InventoryControl.ItemType targetItemType)
    {
        string result = string.Empty;
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].Name;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].Name;
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].Name;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].Name;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].Name;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].Name;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].Name;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].Name;
            }
        }
        return result;
    }
    internal string GetItemDescription(in InventoryControl.ItemType targetItemType)
    {
        string result = string.Empty;
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].Description;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].Description;
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].Description;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].Description;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].Description;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].Description;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].Description;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].Description;
            }
        }
        return result;
    }
    internal int GetItemPrice(in InventoryControl.ItemType targetItemType)
    {
        int result = int.MinValue;
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].Price;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].Price;
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].Price;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].Price;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].Price;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].Price;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].Price;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].Price;
            }
        }
        return result;
    }

    internal Color GetItemColor(in InventoryControl.ItemType targetItemType)
    {
        Color result = Color.white;
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].AdditionBarColor;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].AdditionBarColor;
            }
        }
        return result;
    }
    internal Color[] GetItemWaveColor(in InventoryControl.ItemType targetItemType)
    {
        Color[] result = new Color[4];
        if (targetItemType != InventoryControl.ItemType.None)
        {
            if (InventoryControl.ItemType.Absinthe <= targetItemType && targetItemType <= InventoryControl.ItemType.Champagne)
            {
                result[0] = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.AlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Absinthe].BackBottomColor;
            }
            else if (InventoryControl.ItemType.Water <= targetItemType && targetItemType <= InventoryControl.ItemType.Milgis)
            {
                result[0] = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.NonAlcoholItems[(int)targetItemType - (int)InventoryControl.ItemType.Water].BackBottomColor;
            }
            else if (InventoryControl.ItemType.LumpSugar <= targetItemType && targetItemType <= InventoryControl.ItemType.LimeJuice)
            {
                result[0] = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.SubIngredientItems[(int)targetItemType - (int)InventoryControl.ItemType.LumpSugar].BackBottomColor;
            }
            else if (InventoryControl.ItemType.LemonSlice <= targetItemType && targetItemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
            {
                result[0] = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.GarnishItems[(int)targetItemType - (int)InventoryControl.ItemType.LemonSlice].BackBottomColor;
            }
            else if (InventoryControl.ItemType.Ice <= targetItemType && targetItemType <= InventoryControl.ItemType.CubeIce)
            {
                result[0] = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.IceItems[(int)targetItemType - (int)InventoryControl.ItemType.Ice].BackBottomColor;
            }
            else if (InventoryControl.ItemType.AbsintheGlass <= targetItemType && targetItemType <= InventoryControl.ItemType.OldFashionedGlass)
            {
                result[0] = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.GlassItems[(int)targetItemType - (int)InventoryControl.ItemType.AbsintheGlass].BackBottomColor;
            }
            else if (InventoryControl.ItemType.Shaker <= targetItemType && targetItemType <= InventoryControl.ItemType.Shaker)
            {
                result[0] = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.ToolItems[(int)targetItemType - (int)InventoryControl.ItemType.Shaker].BackBottomColor;
            }
            else if (InventoryControl.ItemType.SpoonShaking <= targetItemType && targetItemType <= InventoryControl.ItemType.HandShaking)
            {
                result[0] = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].FrontTopColor;
                result[1] = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].FrontBottomColor;
                result[2] = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].BackTopColor;
                result[3] = m_miniGameItemResourceDesc.ActionItems[(int)targetItemType - (int)InventoryControl.ItemType.SpoonShaking].BackBottomColor;
            }
        }
        return result;
    }

    internal MiniGameItemResourceDesc.RecipeData GetRecipeData(in RecipeSelectionControl.RecipeType targetRecipe)
    {
        return m_miniGameItemResourceDesc.RecipeDatas[(int)targetRecipe];
    }

    internal InventoryControl.InventoryTabType GetInventoryTabType(in InventoryControl.ItemType itemType)
    {
        InventoryControl.InventoryTabType result = InventoryControl.InventoryTabType.Alcohol;
        if (InventoryControl.ItemType.Absinthe <= itemType && itemType <= InventoryControl.ItemType.Champagne)
        {
            result = InventoryControl.InventoryTabType.Alcohol;
        }
        else if (InventoryControl.ItemType.Water <= itemType && itemType <= InventoryControl.ItemType.Milgis)
        {
            result = InventoryControl.InventoryTabType.NonAlcohol;
        }
        else if (InventoryControl.ItemType.LumpSugar <= itemType && itemType <= InventoryControl.ItemType.LimeJuice)
        {
            result = InventoryControl.InventoryTabType.SubIngredient;
        }
        else if (InventoryControl.ItemType.LemonSlice <= itemType && itemType <= InventoryControl.ItemType.SilverSpoonWithSugarCube)
        {
            result = InventoryControl.InventoryTabType.Garnish;
        }
        else if(InventoryControl.ItemType.Ice <= itemType && itemType <= InventoryControl.ItemType.CubeIce)
        {
            result = InventoryControl.InventoryTabType.Ice;
        }
        else if (InventoryControl.ItemType.AbsintheGlass <= itemType && itemType <= InventoryControl.ItemType.OldFashionedGlass)
        {
            result = InventoryControl.InventoryTabType.Glass;
        }
        else if (InventoryControl.ItemType.Shaker <= itemType && itemType <= InventoryControl.ItemType.Shaker)
        {
            result = InventoryControl.InventoryTabType.Tool;
        }
        else if (InventoryControl.ItemType.SpoonShaking <= itemType && itemType <= InventoryControl.ItemType.HandShaking)
        {
            result = InventoryControl.InventoryTabType.Action;
        }

        return result;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
}