using System.Collections.Generic;
using UnityEngine;

public class TweenersController : MonoBehaviour
{
    public List<UITweener> tweeners = new List<UITweener>();

    public bool playOnStart = true;
    public bool checksLoopedStatus = false;

    private bool isPlaying = false;

    void OnEnable()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    void OnDisable()
    {
        Stop();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (checksLoopedStatus)
        {
            if (isPlaying)
                return;
        }

        isPlaying = true;

        foreach (var tween in tweeners)
        {
            tween.enabled = true;
            tween.PlayForward(true);
        }
       
    }

    [ContextMenu("PlayReverse")]
    public void PlayReverse()
    {
        foreach (var tween in tweeners)
        {
            tween.PlayReverse(false);
        }
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        if (checksLoopedStatus)
        {
            if (!isPlaying)
                return;
        }

        isPlaying = false;

        foreach (var tween in tweeners)
        {
            if (tween.enabled)
            {
                tween.Stop();
                tween.enabled = false;
            }
        }
    }
}

