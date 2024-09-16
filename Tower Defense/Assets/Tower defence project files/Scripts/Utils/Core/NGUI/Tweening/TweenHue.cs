//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's hue.
/// </summary>

[AddComponentMenu("Tween/Tween Hue")]
public class TweenHue : UITweener
{
	public float from = 0.0f;
	public float to = 0.0f;

	bool mCached = false;
	Material mMat;

	void Cache ()
	{
		mCached = true;

        Image image = GetComponent<Image>();
        if (image != null)
        {
            mMat = image.material;
        }
	}

	[System.Obsolete("Use 'value' instead")]
	public float hue { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get
		{
			if (!mCached) Cache();
            if (mMat != null) return mMat.GetFloat("_Hue");
			return 0.0f;
		}
		set
		{
			if (!mCached) Cache();
			if (mMat != null) mMat.SetFloat("_Hue", value);
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>
   
	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

    static public TweenHue Begin(GameObject go, float duration, float hue)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
        TweenHue comp = UITweener.Begin<TweenHue>(go, duration);
		comp.from = comp.value;
		comp.to = hue;

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
