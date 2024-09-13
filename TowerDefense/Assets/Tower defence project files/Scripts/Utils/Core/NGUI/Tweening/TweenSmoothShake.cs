using UnityEngine;

[AddComponentMenu("Tween/Tween Smooth Shake")]
public class TweenSmoothShake : UITweener
{
    public enum ShakeType
    {
        Position = 0,
        Rotation = 1,
        Scale = 2
    }

    public ShakeType shakeType = ShakeType.Position;

    public Vector3 amount;
    public float frequency = 0f;
    private float elapsedTime = 0f;

    [HideInInspector]
	public bool worldSpace = false;

    public bool debug_mode = false;

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
            switch (shakeType)
            {
                case ShakeType.Position:
                    return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
                case ShakeType.Rotation:
                    return worldSpace ? cachedTransform.rotation.eulerAngles : cachedTransform.localRotation.eulerAngles;
                case ShakeType.Scale:
                    return worldSpace ? cachedTransform.lossyScale : cachedTransform.localScale;
                default:
                    return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
            }  
		}
		set
		{
            switch (shakeType)
            {
                case ShakeType.Position:
                    if (worldSpace)
                        cachedTransform.position = value;
                    else
                        cachedTransform.localPosition = value;
                    break;
                case ShakeType.Rotation:
                    if (worldSpace)
                        cachedTransform.rotation = Quaternion.Euler(value);
                    else
                        cachedTransform.localRotation = Quaternion.Euler(value);
                    break;
                case ShakeType.Scale:
                    if (worldSpace)
                    {
                        Vector3 globalScale = cachedTransform.lossyScale;
                        Vector3 localScale = Vector3.one;
                        localScale.x = globalScale.x / value.x;
                        localScale.y = globalScale.y / value.y;
                        localScale.z = globalScale.z / value.z;
                        cachedTransform.localScale = localScale;
                    }
                    else
                    {
                        cachedTransform.localScale = value;
                    }
                    break;
            }
		}
	}

    private void OnEnable()
    {
        currValue = Vector3.zero;
        prevValue = Vector3.zero;
        targetValue = Vector3.zero;
        //elapsedDeltaValue = Vector3.zero;

        startedValue = value;
        prevFactor = 0f;
    }

    Vector3 startedValue;
    Vector3 currValue;
    Vector3 prevValue;
    Vector3 targetValue;
    Vector3 elapsedDeltaValue;
    float prevFactor = 0f;

    public override void Play()
    {
        base.Play();

        elapsedTime = frequency;
    }

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
            targetValue = Vector3.zero;
            //elapsedDeltaValue = Vector3.zero;
            elapsedTime = frequency;

            startedValue = value;
            prevFactor = 0f;
        }
        else
        {
            float deltaFactor = factor - prevFactor;
            elapsedTime += Mathf.Abs(deltaFactor);// Time.deltaTime;

            if (elapsedTime >= frequency)
            {
                targetValue = Vector3.zero;
                if(elapsedDeltaValue.x >= 0)
                    targetValue.x = UnityEngine.Random.Range(-amount[0], 0);
                else
                    targetValue.x = UnityEngine.Random.Range(0, amount[0]);
                if (elapsedDeltaValue.y >= 0)
                    targetValue.y = UnityEngine.Random.Range(-amount[1], 0);
                else
                    targetValue.y = UnityEngine.Random.Range(0, amount[1]);
                if (elapsedDeltaValue.z >= 0)
                    targetValue.z = UnityEngine.Random.Range(-amount[2], 0);
                else
                    targetValue.z = UnityEngine.Random.Range(0, amount[2]);

                elapsedTime = Time.deltaTime;
                prevValue = Vector3.zero;
            }

            float normalizedTime = elapsedTime / frequency;

            if (debug_mode)
            {
                Debug.LogError("deltaFactor " + deltaFactor, this.transform);
            }

            currValue = Vector3.Lerp(Vector3.zero, targetValue, normalizedTime);

            Vector3 delta = currValue - prevValue;
            value = value + delta;

            elapsedDeltaValue += delta;

            prevValue = currValue;
        }

        prevFactor = factor;
    }

    protected override void OnFinishedLoop()
    {
        prevFactor = mFactor;
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
	public override void SetStartToCurrentValue () { }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { }
}
