using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[CreateAssetMenu(fileName = "CutSceneResourceDesc", menuName = "ResourceDesc/CutSceneResourceDesc", order = 1)]
public sealed class CutSceneResourceDesc : ScriptableObject
{
    /// <summary>
    /// 기본적으로 키 프레임 애니메이션이며, 이를 Ressource.Load를 통해 그때그때 Load해서 사용하는 방식으로?
    /// </summary>
    [Serializable] public struct CutSceneData
    {
        public string CutSceneName;
        public Sprite CurSceneSprite;
    }


    [SerializeField] private List<CutSceneData> CutSceneDataList;


    public List<CutSceneData> CutSceneDatas
    {
        get
        {
            return CutSceneDataList;
        }
    }
}