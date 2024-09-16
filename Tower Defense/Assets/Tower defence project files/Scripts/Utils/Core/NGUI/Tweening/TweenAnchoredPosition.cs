using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("Tween/Tween Anchored Position")]
public class TweenAnchoredPosition : UITweener
{
    public Vector2 from;
    public Vector2 to;

    RectTransform mTrans;

    public RectTransform cachedTransform { get { if (mTrans == null) mTrans = transform as RectTransform; return mTrans; } }

    [System.Obsolete("Use 'value' instead")]
    public Vector2 position { get { return this.value; } set { this.value = value; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Vector2 value
    {
        get
        {
            return cachedTransform.anchoredPosition;
        }
        set
        {
            cachedTransform.anchoredPosition = value;
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenAnchoredPosition Begin(GameObject go, float duration, Vector2 pos)
    {
        TweenAnchoredPosition comp = UITweener.Begin<TweenAnchoredPosition>(go, duration);
        comp.from = comp.value;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}
