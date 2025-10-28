using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "UIEdgyToolBarSetting", menuName = "VisualNovel/UIEdgyToolBarSetting", order = 1)]
public sealed class UIEdgyToolBarSetting : ScriptableObject
{
    [Header("Main Content")]
    public Vector3 MainContentOriginPos;
    public Vector3 MainContentVanishPos;

    [Header("Appear Button")]
    public Vector3 AppearButtonOriginPos;
    public Vector3 AppearButtonVanishPos;

    [Header("Quick Bar - For Bottom Tool Bar Only")]
    public Vector3 QuickBarOriginPos;
    public Vector3 QuickBarVanishPos;

    [Header("Move Complete Time")]
    [Range(0.01f, 10.0f)]
    public float MovingSpeed = 0.1f;
}