using System;
using System.Collections;
using System.Collections.Generic;
    
using UnityEngine;


[CreateAssetMenu(fileName = "BarHintTextData", menuName = "ResourceDesc/BarHintTextData", order = 1)]
public class BarHintTextData : ScriptableObject
{
    [Serializable] public struct SingleRecipeScriptData
    {
        public enum TasteType : byte
        {
            Sweet,
            Sour,
            Salty,
            Bitter,
            Hot,
            Umami
        }
        public enum TasteStrength : byte
        {

        }

        [Serializable] public struct MainTextPack
        {
            [Serializable] public struct SingleTextData
            {
                public enum BarHintTextType : byte
                {
                    Common,
                    Variant
                }


                public BarHintTextType TextType;
                public string Text;
            }


            public SingleTextData[] TextDatas;
        }
        [Serializable] public struct TasteTextPack
        {
            /// <summary>
            /// TasteStrength의 크기 == TasteTexts.Length
            /// </summary>
            public string[] TasteTexts;
        }


        public RecipeSelectionControl.RecipeType RecipeType;
        public MainTextPack[] MainTextPacks;
        /// <summary>
        /// TasteType의 크기 == TasteTextPacks.Length
        /// </summary>
        public TasteTextPack[] PositiveTasteTextPacks;
        /// <summary>
        /// TasteType의 크기 == TasteTextPacks.Length
        /// </summary>
        public TasteTextPack[] NegativeTasteTextPacks;
    }


    public SingleRecipeScriptData[] SingleRecipeScripts;
}