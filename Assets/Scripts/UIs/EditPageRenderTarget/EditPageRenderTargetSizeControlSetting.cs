using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "EditPageRenderTargetSizeControlSetting", menuName = "VisualNovel/EditPageRenderTargetSizeControlSetting", order = 1)]
public sealed class EditPageRenderTargetSizeControlSetting : ScriptableObject
{
    public Vector2 RenderTargetOriginSize;
    public Vector2 RenderTargetMaximizeSize;

    [Range(0.01f, 10.0f)]
    public float ResizeSpeed = 0.1f;
}