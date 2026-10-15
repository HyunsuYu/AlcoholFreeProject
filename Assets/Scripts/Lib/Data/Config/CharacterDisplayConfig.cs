using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[CreateAssetMenu(fileName = "CharacterDisplayConfig", menuName = "Config/CharacterDisplayConfig", order = 1)]
public sealed class CharacterDisplayConfig : ScriptableObject
{
    [Serializable] public struct CharacterDisplayData
    {
        public Vector2 ImageSize;
        public Vector4 Padding;
        public float HorizontalSpacing;

        public List<float> PerCharacterImagePositionPivot;
        public List<Vector3> PerCharacterImageScale;
    }


    ///// <summary>
    ///// 0 번째는 하나의 캐릭터만 있을 때, 1 번째는 두 캐릭터가 있을 때, 2 번째는 세 캐릭터가 있을 때 등등의 각 경우에 해당하는 캐릭터 이미지 크기(비율)를 의미
    ///// </summary>
    //public List<Vector2> CharacterImageSizes;

    public List<CharacterDisplayData> CharacterDisplayDatas;

    /// <summary>
    /// 화면에 나타난 캐릭터 중 현재 말을 하고 있지 않은 캐릭터 이미지의 색상을 의미
    /// </summary>
    public Color CharacterDishilightColor = new Color(0.5f, 0.5f, 0.5f, 1f);
}