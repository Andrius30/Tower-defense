//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("Tween/Tween Position")]
public class TweenPositionByList : UITweener
{
	public Vector3 from;
	public Vector3 to;

    public List<Transform> targetTo = null;
    public int curIndex = 0;
    private Vector3 targetToPosition = Vector3.negativeInfinity;

    //[HideInInspector]
    public bool worldSpace = false;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 position { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>
	public Vector3 value
	{
		get
		{
			return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
		}
		set
		{
			if (worldSpace)
                cachedTransform.position = value;
			else cachedTransform.localPosition = value;
		}
	}

    public bool updateAxisX = true;
    public bool updateAxisY = true;
    public bool updateAxisZ = true;

    /// <summary>
    /// Tween the value.
    /// </summary>
    protected override void OnUpdate (float factor, bool isFinished)
    {
        if (targetTo != null && targetTo.Count > curIndex)
        {
            if (worldSpace)
            {
                to = targetTo[curIndex].transform.position;
            }
            else
            {
                if (this.transform.parent != null)
                {
                    if (targetToPosition != targetTo[curIndex].transform.position)
                    {
                        to = this.transform.parent.InverseTransformPoint(targetTo[curIndex].transform.position);
                        targetToPosition = targetTo[curIndex].transform.position;
                    }
                }
                else
                {
                    to = targetTo[curIndex].transform.position;
                }
            }
        }

        Vector3 temp_ = from * (1f - factor) + to * factor;
        Vector3 value_ = value;
        if (updateAxisX)
            value_.x = temp_.x;
        if (updateAxisY)
            value_.y = temp_.y;
        if (updateAxisZ)
            value_.z = temp_.z;

        value = value_;

        //value = from * (1f - factor) + to * factor;
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.value;
		comp.to = pos;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos, bool worldSpace)
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.worldSpace = worldSpace;
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
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
    public void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
    public void SetCurrentValueToEnd () { value = to; }
}
