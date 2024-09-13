//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenScale))]
public class TweenScaleEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		UIEditorTools.SetLabelWidth(120f);

		TweenScale tw = target as TweenScale;
		GUI.changed = false;

		Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
		Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);

        tw.updateAxisX = EditorGUILayout.Toggle("Update X Axis", tw.updateAxisX);
        tw.updateAxisY = EditorGUILayout.Toggle("Update Y Axis", tw.updateAxisY);
        tw.updateAxisZ = EditorGUILayout.Toggle("Update Z Axis", tw.updateAxisZ);

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
