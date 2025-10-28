using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "PathData", menuName = "VisualNovel/PathData", order = 1)]
public sealed class PathData : ScriptableObject
{
    public string ReplicateModelDataTableFullPath;
}