using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public sealed class LiqurePourItem : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GlassBoundary")
        {
            ChiefMiniGameManager.Instance.ReleaseLiqurePourItem(transform, true, true);
        }
        else if(collision.tag == "ShakerBoundary")
        {
            ChiefMiniGameManager.Instance.ReleaseLiqurePourItem(transform, true, false);
        }
        else if (collision.tag == "Termin")
        {
            ChiefMiniGameManager.Instance.ReleaseLiqurePourItem(transform, false, false);
        }
    }
}