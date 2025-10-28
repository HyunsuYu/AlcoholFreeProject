using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


internal sealed class PathDataBuffer : SingleTonForGameObject<PathDataBuffer>
{
    [SerializeField] private PathData m_pathData;


    public void Awake()
    {
        SetInstance(this);
    }

    internal PathData PathData
    {
        get
        {
            return m_pathData;
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}