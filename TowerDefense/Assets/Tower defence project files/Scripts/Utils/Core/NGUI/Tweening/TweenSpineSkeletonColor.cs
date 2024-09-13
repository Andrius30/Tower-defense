////----------------------------------------------
////            NGUI: Next-Gen UI kit
//// Copyright © 2011-2015 Tasharen Entertainment
////----------------------------------------------

//using UnityEngine;
//using UnityEngine.UI;
//using Spine.Unity;
//using Spine;

///// <summary>
///// Tween the object's color.
///// </summary>

//[AddComponentMenu("Tween/Tween Spine Skeleton Color")]
//public class TweenSpineSkeletonColor : UITweener
//{
//    public Color from = Color.white;
//    public Color to = Color.white;

//    bool mCached = false;
//    SkeletonRenderer mSkeleton;

//    void Cache()
//    {
//        mCached = true;

//        mSkeleton = GetComponent<SkeletonRenderer>();
//    }

//    [System.Obsolete("Use 'value' instead")]
//    public Color color { get { return this.value; } set { this.value = value; } }

//    /// <summary>
//    /// Tween's current value.
//    /// </summary>

//    public Color value
//    {
//        get
//        {
//            if (!mCached) Cache();
//            if (mSkeleton != null) return mSkeleton.Skeleton.GetColor();
//            return Color.black;
//        }
//        set
//        {
//            if (!mCached) Cache();
//            if (mSkeleton != null) mSkeleton.Skeleton.SetColor(value);
//        }
//    }

//    /// <summary>
//    /// Tween the value.
//    /// </summary>

//    protected override void OnUpdate(float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

//    /// <summary>
//    /// Start the tweening operation.
//    /// </summary>

//    static public TweenColor Begin(GameObject go, float duration, Color color)
//    {
//#if UNITY_EDITOR
//        if (!Application.isPlaying) return null;
//#endif
//        TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
//        comp.from = comp.value;
//        comp.to = color;

//        if (duration <= 0f)
//        {
//            comp.Sample(1f, true);
//            comp.enabled = false;
//        }
//        return comp;
//    }

//    [ContextMenu("Set 'From' to current value")]
//    public override void SetStartToCurrentValue() { from = value; }

//    [ContextMenu("Set 'To' to current value")]
//    public override void SetEndToCurrentValue() { to = value; }

//    [ContextMenu("Assume value of 'From'")]
//    void SetCurrentValueToStart() { value = from; }

//    [ContextMenu("Assume value of 'To'")]
//    void SetCurrentValueToEnd() { value = to; }
//}
