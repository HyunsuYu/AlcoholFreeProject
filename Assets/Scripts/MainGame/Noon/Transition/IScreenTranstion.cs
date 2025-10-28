using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public interface IScreenTranstion
{
    internal enum TransitionKind
    {
        None,
        Fade,
        Brush
    }


    public void StartTransition(in float timeDuration, in bool bisEnterScreen = false, in AudioSource bgmAudioSources = null, in AudioSource sfxAudioSources = null);
}