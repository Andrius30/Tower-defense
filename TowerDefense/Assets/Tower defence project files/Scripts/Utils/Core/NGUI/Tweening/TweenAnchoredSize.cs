//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

[AddComponentMenu("Tween/Tween Anchored Size")]
public class TweenAnchoredSize : UITweener
{
    public float from ;
    public float to;

    [SerializeField]
    private RectTransform.Axis axis;
    public RectTransform.Axis Axis { get { return axis; } set { axis = value; } }

    RectTransform mTrans;

    public RectTransform cachedTransform { get { if (mTrans == null) mTrans = transform as RectTransform; return mTrans; } }

    public float value
    {
        get { return (Axis == RectTransform.Axis.Horizontal ? cachedTransform.rect.width : cachedTransform.rect.height); }
        set
        {
            cachedTransform.SetSizeWithCurrentAnchors(Axis, value);
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenAnchoredSize Begin(GameObject go, float duration, float size)
    {
        TweenAnchoredSize comp = Begin<TweenAnchoredSize>(go, duration);
        comp.from = comp.value;
        comp.to = size;

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
