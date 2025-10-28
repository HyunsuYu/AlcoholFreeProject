using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public struct SaveData
{
    public enum GamePhase
    {
        Morning,
        Noon,
        Evening,
    }
    /// <summary>
    /// 추후에 적절한 선언 위치와 내용을 결정할 것
    /// </summary>
    public enum ItemType
    {

    }

    public struct SingleSaveData
    {
        public GamePhase CurGamePhase;

        public List<Guid> CompletedSceneIDs;

        public Guid CurProgressSceneID;
        public int CurProgressFrameIndex;

        public Dictionary<string, int> CurGlobalVariableStateTable;

        public Tuple<ItemType, int>[] CurItemStateTable;
    }


    public Dictionary<Guid, SingleSaveData> SaveDataTable;
}