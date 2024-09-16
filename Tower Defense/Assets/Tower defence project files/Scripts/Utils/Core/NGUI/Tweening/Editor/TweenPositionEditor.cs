//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenPosition))]
public class TweenPositionEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		UIEditorTools.SetLabelWidth(120f);

		TweenPosition tw = target as TweenPosition;
		GUI.changed = false;

		Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
		Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
        tw.targetTo = (Transform)EditorGUILayout.ObjectField("Target To", tw.targetTo, typeof(Transform), true); 

        tw.updateAxisX = EditorGUILayout.Toggle("Update X Axis", tw.updateAxisX);
        tw.updateAxisY = EditorGUILayout.Toggle("Update Y Axis", tw.updateAxisY);
        tw.updateAxisZ = EditorGUILayout.Toggle("Update Z Axis", tw.updateAxisZ);

        tw.worldSpace = EditorGUILayout.Toggle("World Space", tw.worldSpace);
        tw.offsetFactor = EditorGUILayout.FloatField("Offset Factor", tw.offsetFactor);

        if (GUI.changed)
		{
			UIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
            //tw.worldSpace = worldSpace;
            EditorUtility.SetDirty(tw);
		}

        EditorGUILayout.BeginHorizontal();
        if (tw.worldSpace)
        {
            if (GUILayout.Button("To Local Space"))
            {
                tw.from = tw.transform.InverseTransformPoint(tw.from);
                tw.to = tw.transform.InverseTransformPoint(tw.to);

                tw.worldSpace = false;
            }
        }
        else
        {
            if (GUILayout.Button("To World Space"))
            {
                tw.from = tw.transform.TransformPoint(tw.from);
                tw.to = tw.transform.TransformPoint(tw.to);

                tw.worldSpace = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        DrawCommonProperties();
	}
}
