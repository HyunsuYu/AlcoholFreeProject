using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[CreateAssetMenu(fileName = "MiniGameItemResourceDesc", menuName = "ResourceDesc/MiniGameItemResourceDesc", order = 1)]
public class MiniGameItemResourceDesc : ScriptableObject
{
    [Serializable] public struct ItemData
    {
        public string Name;
        public Sprite[] sprites;

        public int Price;

        public string Description;

        public Color AdditionBarColor;

        public Color FrontTopColor;
        public Color FrontBottomColor;
        public Color BackTopColor;
        public Color BackBottomColor;
    }

    [Serializable] public struct RecipeData
    {
        [Serializable] public struct RecipeStepData
        {
            public InventoryControl.ItemType Ingredient;

            //public Color FrontTopColor;
            //public Color FrontBottomColor;
            //public Color BackTopColor;
            //public Color BackBottomColor;

            public int LayerOrder;
        }
        [Serializable] public struct RecipeColorData
        {
            public Color FrontTopColor;
            public Color FrontBottomColor;
            public Color BackTopColor;
            public Color BackBottomColor;
        }


        public string Name;

        public GlassItem.GlassType GlassType;
        //public InventoryControl.ItemType[] RecipeOrders;
        public RecipeStepData[] RecipeStepDatas;
        public RecipeColorData[] RecipeColorDatas;

        public Sprite Background;
        public Color ThemeColor;

        public Sprite[] ResultSprites;
    }


    [Header("Alcohol")]
    public ItemData[] AlcoholItems;

    [Header("Non Alcohol")]
    public ItemData[] NonAlcoholItems;

    [Header("Sub Ingredient")]
    public ItemData[] SubIngredientItems;

    [Header("Garnish")]
    public ItemData[] GarnishItems;

    [Header("Glass")]
    public ItemData[] GlassItems;

    [Header("Ice")]
    public ItemData[] IceItems;

    [Header("Tool")]
    public ItemData[] ToolItems;

    [Header("Action")]
    public ItemData[] ActionItems;

    [Header("Recipe Data")]
    public RecipeData[] RecipeDatas;
}