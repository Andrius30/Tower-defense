//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenAnchoredPosition))]
public class TweenAnchoredPositionEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		UIEditorTools.SetLabelWidth(120f);

        TweenAnchoredPosition tw = target as TweenAnchoredPosition;
		GUI.changed = false;

		Vector2 from = EditorGUILayout.Vector2Field("From", tw.from);
		Vector2 to = EditorGUILayout.Vector2Field("To", tw.to);

		if (GUI.changed)
		{
			UIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
            EditorUtility.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
