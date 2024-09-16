//  Author:
//       Justas Antanauskas <justas.antanauskas@gmail.com>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[DefaultExecutionOrder(10000)]
public class UICustomContentSizeFitter : UIBehaviour, ILayoutSelfController
{
    public enum FitMode
    {
        Unconstrained,
        MinSize,
        PreferredSize
    }

    [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;
    public FitMode horizontalFit { get { return m_HorizontalFit; } } 

    [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;
    public FitMode verticalFit { get { return m_VerticalFit; } }

    public Vector2 minSize = Vector2.zero;
    public Vector2 maxSize = Vector2.zero;
    public Vector2 extraSize = Vector2.zero;

    public HorizontalLayoutGroup layoutGroup;

    public bool layoutUpdateOnStart = false;
    public bool layoutUpdateOnFirstFrame = false;
    public bool autoRebuild = false;
    public float rebuildDelay = 0f;
    public bool toRebuild = false;

    public UnityEvent OnRebuiltSize;

    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    private DrivenRectTransformTracker m_Tracker;

    private Vector2 prevSizeDelta;

    public bool _debugMode = false;
    
    #region Unity Lifetime calls

    protected override void OnEnable()
    {
        SetDirty();
    }

    protected override void Start()
    {
        if (layoutUpdateOnStart)
        {
            if (rebuildDelay > 0)
                Invoke("LayoutUpdate", rebuildDelay);
            else
                LayoutUpdate();
        }
    }

    protected void LateUpdate()
    {
        if (layoutUpdateOnFirstFrame)
        {
            if (toRebuild)
            {
                toRebuild = false;
                LayoutUpdate();
            }
        }
        else if (autoRebuild)
        {
            if (toRebuild)
            {
                toRebuild = false;
                LayoutUpdate();
            }
        }
    }

    void LayoutUpdate()
    {
        //for (int i = 0; i < childContentSizeFitters.Count; i++)
        //{
        //    childContentSizeFitters[i].LayoutUpdate();
        //}

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
    }
    #endregion

    //protected override void OnRectTransformDimensionsChange()
    //{
    //    //if (_debugMode)
    //    //{
    //    //    if (Application.isPlaying)
    //    //    {
    //    //        Debug.LogError("OnRectTransformDimensionsChange " + this.name, this.gameObject);
    //    //    }
    //    //}

    //    LayoutUpdate();
    //}

    private void HandleSelfFittingAlongAxis(int axis)
    {
        FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
        if (fitting == FitMode.Unconstrained)
            return;

        m_Tracker.Add(this, rectTransform,
            (axis == 0 ? DrivenTransformProperties.AnchorMaxX : DrivenTransformProperties.AnchorMaxY) |
            (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

        // Set anchor max to same as anchor min along axis
        Vector2 anchorMax = rectTransform.anchorMax;
        anchorMax[axis] = rectTransform.anchorMin[axis];
        rectTransform.anchorMax = anchorMax;

        // Set size to min size
        Vector2 sizeDelta = rectTransform.sizeDelta;
        if (fitting == FitMode.MinSize)
            sizeDelta[axis] = LayoutUtility.GetMinSize(m_Rect, axis);
        else
            sizeDelta[axis] = LayoutUtility.GetPreferredSize(m_Rect, axis);

        if (axis == 0)
        {
            if (minSize.x > 0)
            {
                if (sizeDelta.x < minSize.x)
                    sizeDelta.x = minSize.x;
            }
            if (maxSize.x > 0)
            {
                if (sizeDelta.x > maxSize.x)
                    sizeDelta.x = maxSize.x;
            }
        }
        else
        {
            if (minSize.y > 0)
            {
                if (sizeDelta.y < minSize.y)
                    sizeDelta.y = minSize.y;
            }
            if (maxSize.y > 0)
            {
                if (sizeDelta.y > maxSize.y)
                    sizeDelta.y = maxSize.y;
            }
        }

        sizeDelta.x += extraSize.x;
        sizeDelta.y += extraSize.y;

        if (rectTransform.sizeDelta != sizeDelta)
        {
            if (_debugMode)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("HandleSelfFittingAlongAxis " + this.name, this.gameObject);
                    Debug.LogError("old sizeDelta " + rectTransform.sizeDelta + " new sizeDelta " + sizeDelta);
                }
            }

            rectTransform.sizeDelta = sizeDelta;
            InvokeOnRebuiltSize();
        }
    }

    [ContextMenu("Debug_OnRebuiltSize")]
    public void InvokeOnRebuiltSize()
    {
        //if (Application.isPlaying)
        //{
        //    Debug.LogError(this.name + " InvokeOnRebuiltSize");
        //}
        OnRebuiltSize?.Invoke();
    }

    //void OnBecameVisible()
    //{
    //    Debug.LogError("OnBecameVisible");

    //    SetDirty();
    //}

    [ContextMenu("SetLayoutHorizontal")]
    public void SetLayoutHorizontal()
    {
        //if (_debugMode)
        //{
        //    if (Application.isPlaying)
        //    {
        //        Debug.LogError(this.name + " SetLayoutHorizontal");
        //    }
        //}

        m_Tracker.Clear();
        HandleSelfFittingAlongAxis(0);
    }

    [ContextMenu("SetLayoutVertical")]
    public void SetLayoutVertical()
    {
        //if (_debugMode)
        //{
        //    if (Application.isPlaying)
        //    {
        //        Debug.LogError(this.name + " SetLayoutVertical");
        //    }
        //}

        m_Tracker.Clear();
        HandleSelfFittingAlongAxis(1);
    }

    public void SetDirty()
    {
        if (!IsActive())
        {
            //if(layoutUpdateOnUpdate)
            toRebuild = true;
            return;
        }

        //LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        //Invoke("LayoutUpdate", 0f);
        LayoutUpdate();
    }

    [ContextMenu("SetToRebuild")]
    public void SetToRebuild()
    {
        toRebuild = true;
    }

    [ContextMenu("Debug_SetDirty")]
    public void Debug_SetDirty()
    {
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    [ContextMenu("Debug_SetLayoutDirty")]
    public void Debug_SetLayoutDirty()
    {
        if (layoutGroup != null)
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)layoutGroup.transform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }
#endif
}