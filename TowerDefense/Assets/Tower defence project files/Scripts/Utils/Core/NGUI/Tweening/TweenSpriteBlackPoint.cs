//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("Tween/Tween Sprite Black Point")]
public class TweenSpriteBlackPoint : UITweener
{
    public Color from = Color.white;
    public Color to = Color.white;

    bool mCached = false;
    SpriteRenderer _renderer;
    MaterialPropertyBlock mBlock;
    int blackPointProperty;

    void Cache()
    {
        mCached = true;

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer)
        {
            mBlock = new MaterialPropertyBlock();
            mBlock.SetTexture("_MainTex", _renderer.sprite.texture);
            _renderer.SetPropertyBlock(mBlock);

            blackPointProperty = Shader.PropertyToID("_Black");
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
            if (mBlock != null) return mBlock.GetVector(blackPointProperty);
            return Color.black;
        }
        set
        {
            if (!mCached) Cache();
            if (mBlock != null && _renderer != null)
            {
                Color input = value;
                //input.a = 0.0f;
                mBlock.SetColor(blackPointProperty, input);
                _renderer.SetPropertyBlock(mBlock);
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>
    protected override void OnUpdate(float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

    //override 

    /// <summary>
    /// Start the tweening operation.
    /// </summary>
    static public TweenColor Begin(GameObject go, float duration, Color color)
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
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}
