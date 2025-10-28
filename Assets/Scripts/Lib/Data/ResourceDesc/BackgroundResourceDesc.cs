using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[CreateAssetMenu(fileName = "BackgroundResourceDesc", menuName = "ResourceDesc/BackgroundResourceDesc", order = 1)]
public sealed class BackgroundResourceDesc : ScriptableObject
{
    public enum BackgroundType : byte
    {
        // 캐릭터 관련
        HomeRoom        = 0,
        HealthRoom      = 1,
        Restaurant      = 2,
        PaintingRoom    = 3,
        ClothingStore   = 4,
        Cafe            = 5,

        // 이벤트 관련
        Park            = 6,
        Library         = 7,
        Aquarium        = 8,
        Mall            = 9,
        Gallery         = 10,
        Street          = 11,
        School          = 12,
        Bar             = 13,
        NightStreet     = 14,
    }


    public Sprite[] BackgroundSprites;
}