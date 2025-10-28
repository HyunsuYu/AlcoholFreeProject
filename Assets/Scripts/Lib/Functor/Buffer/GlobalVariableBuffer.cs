using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


public sealed class GlobalVariableBuffer : SingleTonForGameObject<GlobalVariableBuffer>
{
    [SerializeField] private GlobalVariableDesc m_globalVariableDesc;

    private Dictionary<string, int> m_globalVariableTable;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        if (!TryLoadData())
        {
            Debug.LogError("Failed to load global variable data.");
        }
    }

    internal int this[string globalVariableName]
    {
        get
        {
            if(!m_globalVariableTable.ContainsKey(globalVariableName))
            {
                throw new KeyNotFoundException($"Global variable '{globalVariableName}' not found in the global variable table.");
            }

            return m_globalVariableTable[globalVariableName];
        }
        set
        {
            if(!m_globalVariableTable.ContainsKey(globalVariableName))
            {
                throw new KeyNotFoundException($"Global variable '{globalVariableName}' not found in the global variable table.");
            }

            m_globalVariableTable[globalVariableName] = value;
        }
    }

    internal static string GlobalVariableDataFilePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "GlobalVariable.json");
        }
    }

    internal bool TryLoadData()
    {
        if (!File.Exists(GlobalVariableDataFilePath))
        {
            m_globalVariableTable = new Dictionary<string, int>();
            for(int index = 0; index < m_globalVariableDesc.GlobalVariables.Length; index++)
            {
                m_globalVariableTable.Add(m_globalVariableDesc.GlobalVariables[index], m_globalVariableDesc.InitialValues[index]);
            }

            TrySaveData();

            return true;
        }

        try
        {
            string jsonData = File.ReadAllText(GlobalVariableDataFilePath);
            m_globalVariableTable = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load global variable data from {GlobalVariableDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }
    internal bool TrySaveData()
    {
        if (m_globalVariableTable == null)
        {
            Debug.LogError("Global variable table is not initialized. Cannot save data.");
            return false;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_globalVariableTable);

            string directory = Path.GetDirectoryName(GlobalVariableDataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(GlobalVariableDataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save global variable data to {GlobalVariableDataFilePath}: {ex.Message}");
            return false;
        }

        return true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
}