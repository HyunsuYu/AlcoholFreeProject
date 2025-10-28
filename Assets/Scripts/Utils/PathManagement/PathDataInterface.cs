using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


internal static class PathDataInterface
{
    internal static string GetReplicateModelDataTableFullPath()
    {
        return Path.Combine(Application.streamingAssetsPath, PathDataBuffer.Instance.PathData.ReplicateModelDataTableFullPath);
    }
}