using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class ShakerItem : IBottle
{
    internal override void Render()
    {
        RenderState(true);
    }

    internal void GetShakerColor(out Vector4 frontTopColor, out Vector4 frontBottomColor, out Vector4 backTopColor, out Vector4 backBottomColor)
    {
        frontTopColor = Vector4.zero;
        frontBottomColor = Vector4.zero;
        backTopColor = Vector4.zero;
        backBottomColor = Vector4.zero;

        int fillmentCount = m_addedIngredients.Count;
        foreach (var ingredient in m_addedIngredients)
        {
            switch (ingredient)
            {
                case InventoryControl.ItemType.LemonSlice:
                case InventoryControl.ItemType.OliveSkewers:
                case InventoryControl.ItemType.LimeSlice:
                case InventoryControl.ItemType.SilverSpoonWithSugarCube:
                case InventoryControl.ItemType.SpoonShaking:
                case InventoryControl.ItemType.HandShaking:
                    fillmentCount--;
                    break;
            }

            Color[] waveColor = MiniGameItemResourceInterface.Instance.GetItemWaveColor(ingredient);
            frontTopColor += new Vector4(waveColor[0].r, waveColor[0].g, waveColor[0].b, waveColor[0].a);
            frontBottomColor += new Vector4(waveColor[1].r, waveColor[1].g, waveColor[1].b, waveColor[1].a);
            backTopColor += new Vector4(waveColor[2].r, waveColor[2].g, waveColor[2].b, waveColor[2].a);
            backBottomColor += new Vector4(waveColor[3].r, waveColor[3].g, waveColor[3].b, waveColor[3].a);
        }

        frontTopColor /= fillmentCount;
        frontBottomColor /= fillmentCount;
        backTopColor /= fillmentCount;
        backBottomColor /= fillmentCount;
    }
}