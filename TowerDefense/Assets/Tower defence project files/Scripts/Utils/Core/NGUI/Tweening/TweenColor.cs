//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("Tween/Tween Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;
	public Color to = Color.white;

	bool mCached = false;
	Material mMat;
	Light mLight;
    Image mImage;
    Text mText;
    SpriteRenderer mSpriteRenderer;
    ParticleSystem mParticleSystem;

	void Cache ()
	{
		mCached = true;

		mImage = GetComponent<Image>();
        if (mImage == null)
        {
            mLight = GetComponent<Light>();
            if (mLight == null)
            {
                mSpriteRenderer = GetComponent<SpriteRenderer>();
                if (mSpriteRenderer == null)
                {
                    mText = GetComponent<Text>();
                    if (mText == null)
                    {
                        mParticleSystem = GetComponent<ParticleSystem>();
                        if (mParticleSystem == null)
                        {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		                    Renderer ren = renderer;
#else
                            Renderer ren = GetComponent<Renderer>();
#endif
                            if (ren != null)
                            {
                                mMat = ren.material;
                            }
                        }
                    }
                }
            }
        }
	}

	[System.Obsolete("Use 'value' instead")]
	public Color color { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Color value
	{
		get
		{
			if (!mCached) Cache();
			if (mImage != null) return mImage.color;
            if (mText != null) return mText.color;
            if (mSpriteRenderer != null) return mSpriteRenderer.color;
            if (mMat != null) return mMat.color;
			if (mLight != null) return mLight.color;
            if (mParticleSystem != null) return mParticleSystem.main.startColor.color;

            return Color.black;
		}
		set
		{
			if (!mCached) Cache();
            if (mImage != null) mImage.color = value;
            else if (mText != null) mText.color = value;
            else if (mSpriteRenderer != null) mSpriteRenderer.color = value;
            else if (mMat != null) mMat.color = value;
            else if (mLight != null)
            {
                mLight.color = value;
                mLight.enabled = (value.r + value.g + value.b) > 0.01f;
            }
            else if (mParticleSystem != null)
            {
                var mainModule = mParticleSystem.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(value);
            }
        }
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenColor Begin (GameObject go, float duration, Color color)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
		TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
		comp.from = comp.value;
		comp.to = color;

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
