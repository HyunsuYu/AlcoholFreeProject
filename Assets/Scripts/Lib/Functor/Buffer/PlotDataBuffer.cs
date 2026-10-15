using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Newtonsoft.Json;

using CommonUtilLib.ThreadSafe;


public class PlotDataBuffer : SingleTonForGameObject<PlotDataBuffer>
{
    private PlotData? m_plotData;

    //[SerializeField] private float m_saveTimeLimit = 3.0f;
    //private float m_saveTimer = 0.0f;
    //private bool m_bisTrying2Save = false;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        if (!TryLoadData())
        {
            Debug.LogError("Failed to load plot data.");
        }
    }
    //public void FixedUpdate()
    //{
    //    if(m_bisTrying2Save)
    //    {
    //        m_saveTimer += Time.fixedDeltaTime;

    //        if (m_saveTimer > m_saveTimeLimit)
    //        {
    //            m_bisTrying2Save = false;
    //            m_saveTimer = 0.0f;

    //            try
    //            {
    //                string jsonData = JsonConvert.SerializeObject(m_plotData);

    //                string directory = Path.GetDirectoryName(PlotDataFilePath);
    //                if (!Directory.Exists(directory))
    //                {
    //                    Directory.CreateDirectory(directory);
    //                }

    //                File.WriteAllText(PlotDataFilePath, jsonData);
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.LogError($"Failed to save plot data to {PlotDataFilePath}: {ex.Message}");
    //            }

    //            Debug.Log("Save Sussess");
    //        }
    //    }
    //}

    internal PlotData? PlotData
    {
        get
        {
            return m_plotData;
        }
        set
        {
            m_plotData = value;
        }
    }

    internal static string PlotDataFilePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "PlotData.json");
        }
    }

    internal bool TryLoadData()
    {
        if(!File.Exists(PlotDataFilePath))
        {
            m_plotData = new PlotData
            {
                SceneTable = new Dictionary<Guid, SceneData>(),
                LinkTable = new Dictionary<Guid, PlotData.LinkData>(),
                EnterEventTable = new Dictionary<string, SerializableVector3>(),
                ScreenTransitionNodePos = new List<SerializableVector3>()
            };

            TrySaveData();

            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(PlotDataFilePath);
            m_plotData = JsonConvert.DeserializeObject<PlotData>(jsonData);
        }
        catch(Exception ex)
        {
            Debug.LogError($"Failed to load plot data from {PlotDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }
    internal bool TrySaveData()
    {
        if (m_plotData == null)
        {
            Debug.LogError("PlotDataBuffer has not been initialized with PlotData.");
            return false;
        }
        //else if (m_bisTrying2Save)
        //{
        //    return true;
        //}

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_plotData);

            string directory = Path.GetDirectoryName(PlotDataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(PlotDataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save plot data to {PlotDataFilePath}: {ex.Message}");
            return false;
        }

        //m_bisTrying2Save = true;
        //m_saveTimer = 0.0f;

        return true;
    }

    internal SceneData GetSceneData(in Guid targetSceneID)
    {
        if (m_plotData == null)
        {
            throw new System.InvalidOperationException("PlotDataBuffer has not been initialized with PlotData.");
        }
        if (!m_plotData.Value.SceneTable.TryGetValue(targetSceneID, out SceneData sceneData))
        {
            throw new System.InvalidOperationException($"Scene with ID {targetSceneID} does not exist in PlotData.");
        }
        return sceneData;
    }
    internal FrameData GetFrameData(in Guid targetSceneID, in int targetFrameIndex)
    {
        if (m_plotData == null)
        {
            throw new System.InvalidOperationException("PlotDataBuffer has not been initialized with PlotData.");
        }
        if (!m_plotData.Value.SceneTable.TryGetValue(targetSceneID, out SceneData sceneData))
        {
            throw new System.InvalidOperationException($"Scene with ID {targetSceneID} does not exist in PlotData.");
        }
        if (targetFrameIndex < 0 || targetFrameIndex >= sceneData.Frames.Count)
        {
            throw new System.ArgumentOutOfRangeException(nameof(targetFrameIndex), 
                $"Frame index {targetFrameIndex} is out of range for scene with ID {targetSceneID}.");
        }

        return sceneData.Frames[targetFrameIndex];
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}