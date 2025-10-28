using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// 하나의 CharacterResourceDesc는 하나의 캐릭터에 대한 리소스를 의미
/// </summary>
[CreateAssetMenu(fileName = "CharacterResourceDesc", menuName = "ResourceDesc/CharacterResourceDesc", order = 1)]
public sealed class CharacterResourceDesc : ScriptableObject
{
    public enum CharacterListType : byte
    {
        Absinthe = 0,
        CabernetSauvignon = 1,
        HayaseuraTokusenGinjo = 2,
        Hpnotiq = 3,
        Kahlua = 4,
        MainCharacter = 5,
        Description = 6,
        Librarian = 7,
    }
    public enum CharacterPoseType : byte
    {
        Idle            = 0,
        Smile           = 1,
        Angry           = 2,
        Surprise        = 3,
        Expressionless  = 4,
        Sad             = 5,
        Upset           = 6
    }


    /// <summary>
    /// CharacterSprites의 크기는 CharacterPoseType의 크기와 같아야 한다
    /// </summary>
    public Sprite[] CharacterSprites;
    public string CharacterName;
}