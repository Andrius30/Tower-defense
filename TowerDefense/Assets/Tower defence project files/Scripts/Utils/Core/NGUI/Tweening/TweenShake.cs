using UnityEngine;

[AddComponentMenu("Tween/Tween Shake")]
public class TweenShake : UITweener
{
    public Vector3 amount;
    public float frequency = 0f;

    [HideInInspector]
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
			if (worldSpace) cachedTransform.position = value;
			else cachedTransform.localPosition = value;
		}
	}

    private void OnEnable()
    {
        currValue = Vector3.zero;
        prevValue = Vector3.zero;

        startedValue = value;
        prevFactor = 0f;
    }

    public override void ResetToBeginning()
    {
        if (mStarted)
        {
            mStarted = false;
            mFactor = 0f;

            value = startedValue;
        }
        else
        {
            base.ResetToBeginning();
        }
    }

    Vector3 startedValue;
    Vector3 currValue;
    Vector3 prevValue;
    float prevFactor = 0f;

    /// <summary>
    /// Tween the value.
    /// </summary>
    protected override void OnUpdate (float factor, bool isFinished)
    {
        //Debug.Log("factor " + factor + " isFinished " + isFinished);

        if (factor == 0)
        {
            currValue = Vector3.zero;
            prevValue = Vector3.zero;

            startedValue = value;
            prevFactor = 0f;
        }
        else if (factor == 1)
        {
            //Debug.Log("end value " + value);

            Vector3 delta = startedValue - value;
            value = value + delta;
        }
        else
        {
            //float deltaFactor = factor - prevFactor;

            float diminishingControl = 1f;// - factor;// * (t * 60f);//1 - percentage;

            if (ignoreTimeScale)
            {
                currValue.x = UnityEngine.Random.Range(-amount[0] * diminishingControl, amount[0] * diminishingControl) * Time.unscaledDeltaTime;
                currValue.y = UnityEngine.Random.Range(-amount[1] * diminishingControl, amount[1] * diminishingControl) * Time.unscaledDeltaTime;
                currValue.z = UnityEngine.Random.Range(-amount[2] * diminishingControl, amount[2] * diminishingControl) * Time.unscaledDeltaTime;
            }
            else
            {
                currValue.x = UnityEngine.Random.Range(-amount[0] * diminishingControl, amount[0] * diminishingControl) * Time.deltaTime;
                currValue.y = UnityEngine.Random.Range(-amount[1] * diminishingControl, amount[1] * diminishingControl) * Time.deltaTime;
                currValue.z = UnityEngine.Random.Range(-amount[2] * diminishingControl, amount[2] * diminishingControl) * Time.deltaTime;
            }

            Vector3 delta = prevValue - currValue;

            value = value + delta;

            prevValue = currValue;
        }

        prevFactor = factor;
    }

    //private float Shake2(float amplitude, float value, float period_)
    //{
    //    if (value == 0)
    //    {
    //        return 0;
    //    }
    //    else if (value == 1)
    //    {
    //        return 0;
    //    }
    //    float period = period_;

    //    return amplitude * Mathf.Sin((value * 2 * Mathf.PI) / period);
    //}

    //private float Shake3(float amplitude, float speed)
    //{
    //    return amplitude * Mathf.Sin(Time.time * speed);
    //}

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
	public override void SetStartToCurrentValue () { }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { }

	//[ContextMenu("Assume value of 'From'")]
	//void SetCurrentValueToStart () { value = from; }

	//[ContextMenu("Assume value of 'To'")]
	//void SetCurrentValueToEnd () { value = to; }
}
