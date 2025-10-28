using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public static class TempSaveBuffer
{
    private static List<Tuple<Guid, int>> m_backlogIndexes = new List<Tuple<Guid, int>>();


    internal static Tuple<Guid, int>[] BacklogIndexes
    {
        get
        {
            return m_backlogIndexes.ToArray();
        }
    }

    internal static void TryAddBacklogData(in Guid targetSceneID, in int targetFrameIndex)
    {
        Tuple<Guid, int> newData = new Tuple<Guid, int>(targetSceneID, targetFrameIndex);
        if(!m_backlogIndexes.Contains(newData))
        {
            m_backlogIndexes.Add(newData);
        }
    }

    internal static void ClearTempSaveDatas()
    {
        m_backlogIndexes.Clear();
    }
}