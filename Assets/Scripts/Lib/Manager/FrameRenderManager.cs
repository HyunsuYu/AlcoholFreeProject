using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using System;


public sealed class FrameRenderManager : SingleTonForGameObject<FrameRenderManager>
{
    private Guid m_targetSceneID;
    private int m_targetFrameIndex;


    internal void RenderFrame()
    {
        FrameData frameData = PlotDataBuffer.Instance.GetFrameData(m_targetSceneID, m_targetFrameIndex);
        foreach(IDescriptor descriptor in DescriptorBuffer.Instance.Descriptors)
        {
            descriptor.Write(frameData);
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}