//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's alpha. Works with both UI widgets as well as renderers.
/// </summary>

[AddComponentMenu("Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;

	bool mCached = false;

	Material mMat;
	Image mImage;
    RawImage mRawImage;
    CanvasGroup mCanvasGroup;
    Text mText;
    SpriteRenderer mSprite;

    [System.Obsolete("Use 'value' instead")]
	public float alpha { get { return this.value; } set { this.value = value; } }

	void Cache ()
	{
		mCached = true;

        mImage = GetComponent<Image>();
        if (mImage == null)
        {
            mRawImage = GetComponent<RawImage>();
            if (mRawImage == null)
            {
                mText = GetComponent<Text>();
                if (mText == null)
                {
                    mCanvasGroup = GetComponent<CanvasGroup>();
                    if (mCanvasGroup == null)
                    {
                        mSprite = GetComponent<SpriteRenderer>();
                        if (mSprite == null)
                        {
                            Renderer ren = GetComponent<Renderer>();
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

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get
		{
			if (!mCached) Cache();
            if (mImage != null)
            {
                return mImage.color.a;
            }
            else if (mRawImage != null)
            {
                return mRawImage.color.a;
            }
            else if (mText != null)
            {
                return mText.color.a;
            }
            else if (mCanvasGroup != null)
            {
                return mCanvasGroup.alpha;
            }
            else if (mSprite != null)
            {
                return mSprite.color.a;
            }
            return mMat != null ? mMat.color.a : 1f;
		}
		set
		{
			if (!mCached) Cache();

			if (mImage != null)
			{
                Color c = mImage.color;
				c.a = value;
                mImage.color = c;
			}
            else if (mRawImage != null)
            {
                Color c = mRawImage.color;
                c.a = value;
                mRawImage.color = c;
            }
            else if (mText != null)
            {
                Color c = mText.color;
                c.a = value;
                mText.color = c;
            }
            else if (mCanvasGroup != null)
            {
                mCanvasGroup.alpha = value;
            }
            else if (mMat != null)
            {
                Color c = mMat.color;
                c.a = value;
                mMat.color = c;
            }
            else if (mSprite != null)
            {
                Color c = mSprite.color;
                c.a = value;
                mSprite.color = c;
            }
        }
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenAlpha Begin (GameObject go, float duration, float alpha)
	{
		TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
		comp.from = comp.value;
		comp.to = alpha;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}
