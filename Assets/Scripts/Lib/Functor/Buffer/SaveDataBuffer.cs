using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


public sealed class SaveDataBuffer : SingleTonForGameObject<SaveDataBuffer>
{
    private SaveData? m_saveData = null;


    public void Awake()
    {
        SetInstance(this);

        if (!TryLoadData())
        {
            Debug.LogError("Failed to load save data.");
        }
    }

    //internal SaveData.SingleSaveData? CurSelectedSingleSaveData
    //{
    //    get
    //    {
    //        if (!m_saveData.HasValue)
    //        {
    //            return null;
    //        }
    //        else if(m_curSelectedSaveData == Guid.Empty)
    //        {
    //            return null;
    //        }
    //        else if (m_saveData.Value.SaveDataTable.TryGetValue(m_curSelectedSaveData, out SaveData.SingleSaveData targetSingleSaveData))
    //        {
    //            return targetSingleSaveData;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //    set
    //    {
    //        if (value.HasValue)
    //        {
    //            m_saveData.Value.SaveDataTable[m_curSelectedSaveData] = value.Value;
    //        }
    //        else
    //        {
    //            m_curSelectedSaveData = Guid.Empty;
    //        }

    //        NoonCanvasControl.Instance.Render();
    //    }
    //}
    //internal Guid CurSeelctedSceneID
    //{
    //    get
    //    {
    //        if (!m_saveData.HasValue)
    //        {
    //            return Guid.Empty;
    //        }
    //        else if (m_curSelectedSaveData == Guid.Empty)
    //        {
    //            return Guid.Empty;
    //        }
    //        else if (m_saveData.Value.SaveDataTable.TryGetValue(m_curSelectedSaveData, out SaveData.SingleSaveData targetSingleSaveData))
    //        {
    //            return targetSingleSaveData.CurProgressSceneID;
    //        }
    //        else
    //        {
    //            return Guid.Empty;
    //        }
    //    }
    //    set
    //    {
    //        if (!m_saveData.HasValue || m_curSelectedSaveData == Guid.Empty)
    //        {
    //            return;
    //        }
    //        if (m_saveData.Value.SaveDataTable.TryGetValue(m_curSelectedSaveData, out SaveData.SingleSaveData targetSingleSaveData))
    //        {
    //            targetSingleSaveData.CurProgressSceneID = value;
    //            m_saveData.Value.SaveDataTable[m_curSelectedSaveData] = targetSingleSaveData;
    //        }
    //        else
    //        {
    //            return;
    //        }
    //        NoonCanvasControl.Instance.Render();
    //    }
    //}
    //internal int CurSelectedFrameIndex
    //{
    //    get
    //    {
    //        if (!m_saveData.HasValue)
    //        {
    //            return -1;
    //        }
    //        else if (m_curSelectedSaveData == Guid.Empty)
    //        {
    //            return -1;
    //        }
    //        else if (m_saveData.Value.SaveDataTable.TryGetValue(m_curSelectedSaveData, out SaveData.SingleSaveData targetSingleSaveData))
    //        {
    //            return targetSingleSaveData.CurProgressFrameIndex;
    //        }
    //        else
    //        {
    //            return -1;
    //        }
    //    }
    //    set
    //    {
    //        if (!m_saveData.HasValue || m_curSelectedSaveData == Guid.Empty)
    //        {
    //            return;
    //        }
    //        if (m_saveData.Value.SaveDataTable.TryGetValue(m_curSelectedSaveData, out SaveData.SingleSaveData targetSingleSaveData))
    //        {
    //            targetSingleSaveData.CurProgressFrameIndex = value;
    //            m_saveData.Value.SaveDataTable[m_curSelectedSaveData] = targetSingleSaveData;
    //        }
    //        else
    //        {
    //            return;
    //        }
    //        NoonCanvasControl.Instance.Render();
    //    }
    //}

    internal static string SaveDataFilePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "SaveData.json");
        }
    }

    internal SaveData.SingleSaveData[] GetAllSingleSaveDatas()
    {
        if(!m_saveData.HasValue)
        {
            return null;
        }

        return m_saveData.Value.SaveDataTable.Values.ToArray();
    }

    //internal void TrySetCurSaveData(in Guid targetSaveDataID)
    //{
    //    if(targetSaveDataID == Guid.Empty)
    //    {
    //        return;
    //    }

    //    m_curSelectedSaveData = targetSaveDataID;

    //    NoonCanvasControl.Instance.Render();
    //}

    internal Guid TryAddNewSingleSaveData()
    {
        if (!m_saveData.HasValue)
        {
            return Guid.Empty;
        }

        Guid newSaveDataID = Guid.NewGuid();
        SaveData.SingleSaveData newSingleSaveData = new SaveData.SingleSaveData
        {
            CurGamePhase = SaveData.GamePhase.Noon,
            CompletedSceneIDs = new List<Guid>(),
            CurProgressSceneID = Guid.Empty,
            CurProgressFrameIndex = 0,
            CurGlobalVariableStateTable = new Dictionary<string, int>(),
            CurItemStateTable = Array.Empty<Tuple<SaveData.ItemType, int>>()
        };
        m_saveData.Value.SaveDataTable.Add(newSaveDataID, newSingleSaveData);

        return newSaveDataID;
    }
    internal bool TryRemoveSingleSaveData(in Guid targetSaveDataID)
    {
        if (!m_saveData.HasValue)
        {
            return false;
        }
        else if(targetSaveDataID == Guid.Empty)
        {
            return false;
        }

        if (m_saveData.Value.SaveDataTable.ContainsKey(targetSaveDataID))
        {
            m_saveData.Value.SaveDataTable.Remove(targetSaveDataID);
            //if (m_curSelectedSaveData == targetSaveDataID)
            //{
            //    m_curSelectedSaveData = Guid.Empty;
            //}
            return true;
        }

        return false;
    }

    internal bool TryLoadData()
    {
        if (!File.Exists(SaveDataFilePath))
        {
            //m_plotData = new PlotData
            //{
            //    SceneTable = new Dictionary<Guid, SceneData>(),
            //    LinkTable = new Dictionary<Guid, PlotData.LinkData>(),
            //    EnterEventTable = new Dictionary<string, SerializableVector3>(),
            //    ScreenTransitionNodePos = new List<SerializableVector3>()
            //};
            m_saveData = new SaveData()
            {
                SaveDataTable = new Dictionary<Guid, SaveData.SingleSaveData>(),
            };

            TrySaveData();

            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(SaveDataFilePath);
            m_saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save data from {SaveDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }
    internal bool TrySaveData()
    {
        if (m_saveData == null)
        {
            Debug.LogError("SaveDataBuffer has not been initialized with SaveData.");
            return false;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_saveData);

            string directory = Path.GetDirectoryName(SaveDataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(SaveDataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save save data to {SaveDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}