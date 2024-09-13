//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

[AddComponentMenu("Tween/Tween Scale")]
public class TweenScale : UITweener
{
	public Vector3 from = Vector3.one;
	public Vector3 to = Vector3.one;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	public Vector3 value { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 scale { get { return this.value; } set { this.value = value; } }

    public bool updateAxisX = true;
    public bool updateAxisY = true;
    public bool updateAxisZ = true;

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate (float factor, bool isFinished)
	{
        float negativeFactor = (1f - factor);

        Vector3 _value = value;

        if (updateAxisX)
            _value.x = from.x * negativeFactor + to.x * factor;
        if (updateAxisY)
            _value.y = from.y * negativeFactor + to.y * factor;
        if (updateAxisZ)
            _value.z = from.z * negativeFactor + to.z * factor;

        value = _value;

        //value = from * (1f - factor) + to * factor;
    }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenScale Begin (GameObject go, float duration, Vector3 scale)
	{
		TweenScale comp = UITweener.Begin<TweenScale>(go, duration);
		comp.from = comp.value;
		comp.to = scale;

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
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
