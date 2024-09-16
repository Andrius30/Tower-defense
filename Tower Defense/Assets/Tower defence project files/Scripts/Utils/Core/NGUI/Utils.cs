//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Core.NGUI
{

    /// <summary>
    /// Helper class containing generic functions used throughout the UI library.
    /// </summary>
    static public class Utils
    {
        /// <summary>
        /// Helper function -- whether the disk access is allowed.
        /// </summary>

        static public bool fileAccess
        {
            get
            {
#pragma warning disable 618
                return Application.platform != RuntimePlatform.WebGLPlayer;
#pragma warning restore 618
            }
        }

        /// <summary>
        /// Same as Random.Range, but the returned value is between min and max, inclusive.
        /// Unity's Random.Range is less than max instead, unless min == max.
        /// This means Range(0,1) produces 0 instead of 0 or 1. That's unacceptable.
        /// </summary>

        static public int RandomRange(int min, int max)
        {
            if (min == max) return min;
            return UnityEngine.Random.Range(min, max + 1);
        }

        /// <summary>
        /// Returns the hierarchy of the object in a human-readable format.
        /// </summary>

        static public string GetHierarchy(GameObject obj)
        {
            if (obj == null) return "";
            string path = obj.name;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "\\" + path;
            }
            return path;
        }

        /// <summary>
        /// Find all active objects of specified type.
        /// </summary>

        static public T[] FindActive<T>() where T : Component
        {
            return GameObject.FindObjectsOfType(typeof(T)) as T[];
        }

        /// <summary>
        /// Helper function that returns the string name of the type.
        /// </summary>

        static public string GetTypeName<T>()
        {
            string s = typeof(T).ToString();
            if (s.StartsWith("UI")) s = s.Substring(2);
            else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
            return s;
        }

        /// <summary>
        /// Helper function that returns the string name of the type.
        /// </summary>

        static public string GetTypeName(UnityEngine.Object obj)
        {
            if (obj == null) return "Null";
            string s = obj.GetType().ToString();
            if (s.StartsWith("UI")) s = s.Substring(2);
            else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
            return s;
        }

        /// <summary>
        /// Convenience method that works without warnings in both Unity 3 and 4.
        /// </summary>

        static public void RegisterUndo(UnityEngine.Object obj, string name)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(obj, name);
            Utils.SetDirty(obj);
#endif
        }

        /// <summary>
        /// Convenience function that marks the specified object as dirty in the Unity Editor.
        /// </summary>

        static public void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (obj)
            {
                //if (obj is Component) Debug.Log(Utils.GetHierarchy((obj as Component).gameObject), obj);
                //else if (obj is GameObject) Debug.Log(Utils.GetHierarchy(obj as GameObject), obj);
                //else Debug.Log("Hmm... " + obj.GetType(), obj);
                UnityEditor.EditorUtility.SetDirty(obj);
            }
#endif
        }

        /// <summary>
        /// Add a new child game object.
        /// </summary>

        static public GameObject AddChild(GameObject parent) { return AddChild(parent, true); }

        /// <summary>
        /// Add a new child game object.
        /// </summary>

        static public GameObject AddChild(GameObject parent, bool undo)
        {
            GameObject go = new GameObject();
#if UNITY_EDITOR
            if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        /// <summary>
        /// Instantiate an object and add it to the specified parent.
        /// </summary>

        static public GameObject AddChild(GameObject parent, GameObject prefab, bool keepPrefabLayer = false)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform, true);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;

                if (!keepPrefabLayer)
                {
                    go.layer = parent.layer;
                }
            }
            return go;
        }

        /// <summary>
        /// Helper function that recursively sets all children with widgets' game objects layers to the specified value.
        /// </summary>

        static public void SetChildLayer(Transform t, int layer)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                child.gameObject.layer = layer;
                SetChildLayer(child, layer);
            }
        }

        /// <summary>
        /// Add a child object to the specified parent and attaches the specified script to it.
        /// </summary>

        static public T AddChild<T>(GameObject parent) where T : Component
        {
            GameObject go = AddChild(parent);
            go.name = GetTypeName<T>();
            return go.AddComponent<T>();
        }

        /// <summary>
        /// Add a child object to the specified parent and attaches the specified script to it.
        /// </summary>

        static public T AddChild<T>(GameObject parent, bool undo) where T : Component
        {
            GameObject go = AddChild(parent, undo);
            go.name = GetTypeName<T>();
            return go.AddComponent<T>();
        }

        /// <summary>
        /// Get the rootmost object of the specified game object.
        /// </summary>

        static public GameObject GetRoot(GameObject go)
        {
            Transform t = go.transform;

            for (; ; )
            {
                Transform parent = t.parent;
                if (parent == null) break;
                t = parent;
            }
            return t.gameObject;
        }

        /// <summary>
        /// Finds the specified component on the game object or one of its parents.
        /// </summary>

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;
            // Commented out because apparently it causes Unity 4.5.3 to lag horribly:
            // http://www.tasharen.com/forum/index.php?topic=10882.0
            //#if UNITY_4_3
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
            T comp = go.GetComponent<T>();
#endif
            if (comp == null)
            {
                Transform t = go.transform.parent;

                while (t != null && comp == null)
                {
                    comp = t.gameObject.GetComponent<T>();
                    t = t.parent;
                }
            }
#if UNITY_FLASH
		return (T)comp;
#else
            return comp;
#endif
            //#else
            //		return go.GetComponentInParent<T>();
            //#endif
        }

        /// <summary>
        /// Finds the specified component on the game object or one of its parents.
        /// </summary>

        static public T FindInParents<T>(Transform trans) where T : Component
        {
            if (trans == null) return null;
#if UNITY_4_3
#if UNITY_FLASH
		object comp = trans.GetComponent<T>();
#else
		T comp = trans.GetComponent<T>();
#endif
		if (comp == null)
		{
			Transform t = trans.transform.parent;

			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
		}
#if UNITY_FLASH
		return (T)comp;
#else
		return comp;
#endif
#else
            return trans.GetComponentInParent<T>();
#endif
        }

        /// <summary>
        /// Destroy the specified object, immediately if in edit mode.
        /// </summary>

        static public void Destroy(UnityEngine.Object obj)
        {
            if (obj)
            {
                if (obj is Transform)
                {
                    Transform t = (obj as Transform);
                    GameObject go = t.gameObject;

                    if (Application.isPlaying)
                    {
                        t.parent = null;
                        UnityEngine.Object.Destroy(go);
                    }
                    else UnityEngine.Object.DestroyImmediate(go);
                }
                else if (obj is GameObject)
                {
                    GameObject go = obj as GameObject;
                    Transform t = go.transform;

                    if (Application.isPlaying)
                    {
                        t.parent = null;
                        UnityEngine.Object.Destroy(go);
                    }
                    else UnityEngine.Object.DestroyImmediate(go);
                }
                else if (Application.isPlaying) UnityEngine.Object.Destroy(obj);
                else UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Convenience extension that destroys all children of the transform.
        /// </summary>

        static public void DestroyChildren(this Transform t)
        {
            bool isPlaying = Application.isPlaying;

            while (t.childCount != 0)
            {
                Transform child = t.GetChild(0);

                if (isPlaying)
                {
                    child.parent = null;
                    UnityEngine.Object.Destroy(child.gameObject);
                }
                else UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// Destroy the specified object immediately, unless not in the editor, in which case the regular Destroy is used instead.
        /// </summary>

        static public void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (Application.isEditor) UnityEngine.Object.DestroyImmediate(obj);
                else UnityEngine.Object.Destroy(obj);
            }
        }

        /// <summary>
        /// Call the specified function on all objects in the scene.
        /// </summary>

        static public void Broadcast(string funcName)
        {
            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0, imax = gos.Length; i < imax; ++i) gos[i].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Call the specified function on all objects in the scene.
        /// </summary>

        static public void Broadcast(string funcName, object param)
        {
            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0, imax = gos.Length; i < imax; ++i) gos[i].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Determines whether the 'parent' contains a 'child' in its hierarchy.
        /// </summary>

        static public bool IsChild(Transform parent, Transform child)
        {
            if (parent == null || child == null) return false;

            while (child != null)
            {
                if (child == parent) return true;
                child = child.parent;
            }
            return false;
        }

        /// <summary>
        /// Activate the specified object and all of its children.
        /// </summary>

        static void Activate(Transform t) { Activate(t, false); }

        /// <summary>
        /// Activate the specified object and all of its children.
        /// </summary>

        static void Activate(Transform t, bool compatibilityMode)
        {
            SetActiveSelf(t.gameObject, true);

            if (compatibilityMode)
            {
                // If there is even a single enabled child, then we're using a Unity 4.0-based nested active state scheme.
                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    Transform child = t.GetChild(i);
                    if (child.gameObject.activeSelf) return;
                }

                // If this point is reached, then all the children are disabled, so we must be using a Unity 3.5-based active state scheme.
                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    Transform child = t.GetChild(i);
                    Activate(child, true);
                }
            }
        }

        /// <summary>
        /// Deactivate the specified object and all of its children.
        /// </summary>

        static void Deactivate(Transform t) { SetActiveSelf(t.gameObject, false); }

        /// <summary>
        /// SetActiveRecursively enables children before parents. This is a problem when a widget gets re-enabled
        /// and it tries to find a panel on its parent.
        /// </summary>

        static public void SetActive(GameObject go, bool state) { SetActive(go, state, true); }

        /// <summary>
        /// SetActiveRecursively enables children before parents. This is a problem when a widget gets re-enabled
        /// and it tries to find a panel on its parent.
        /// </summary>

        static public void SetActive(GameObject go, bool state, bool compatibilityMode)
        {
            if (go)
            {
                if (state)
                {
                    Activate(go.transform, compatibilityMode);
                }
                else Deactivate(go.transform);
            }
        }

        /// <summary>
        /// Activate or deactivate children of the specified game object without changing the active state of the object itself.
        /// </summary>

        static public void SetActiveChildren(GameObject go, bool state)
        {
            Transform t = go.transform;

            if (state)
            {
                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    Transform child = t.GetChild(i);
                    Activate(child);
                }
            }
            else
            {
                for (int i = 0, imax = t.childCount; i < imax; ++i)
                {
                    Transform child = t.GetChild(i);
                    Deactivate(child);
                }
            }
        }

        /// <summary>
        /// Helper function that returns whether the specified MonoBehaviour is active.
        /// </summary>

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        static public bool GetActive(Behaviour mb)
        {
            return mb && mb.enabled && mb.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Unity4 has changed GameObject.active to GameObject.activeself.
        /// </summary>

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        static public bool GetActive(GameObject go)
        {
            return go && go.activeInHierarchy;
        }

        /// <summary>
        /// Unity4 has changed GameObject.active to GameObject.SetActive.
        /// </summary>

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        static public void SetActiveSelf(GameObject go, bool state)
        {
            go.SetActive(state);
        }

        /// <summary>
        /// Recursively set the game object's layer.
        /// </summary>

        static public void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;

            Transform t = go.transform;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        /// <summary>
        /// Helper function used to make the vector use integer numbers.
        /// </summary>

        static public Vector3 Round(Vector3 v)
        {
            v.x = Mathf.Round(v.x);
            v.y = Mathf.Round(v.y);
            v.z = Mathf.Round(v.z);
            return v;
        }

        /// <summary>
        /// Save the specified binary data into the specified file.
        /// </summary>

        static public bool Save(string fileName, byte[] bytes)
        {
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1
		return false;
#else
            if (!Utils.fileAccess) return false;

            string path = Application.persistentDataPath + "/" + fileName;

            if (bytes == null)
            {
                if (File.Exists(path)) File.Delete(path);
                return true;
            }

            FileStream file = null;

            try
            {
                file = File.Create(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }

            file.Write(bytes, 0, bytes.Length);
            file.Close();
            return true;
#endif
        }

        /// <summary>
        /// Load all binary data from the specified file.
        /// </summary>

        static public byte[] Load(string fileName)
        {
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1
		return null;
#else
            if (!Utils.fileAccess) return null;

            string path = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            return null;
#endif
        }

        /// <summary>
        /// Extension for the game object that checks to see if the component already exists before adding a new one.
        /// If the component is already present it will be returned instead.
        /// </summary>

        static public T AddMissingComponent<T>(this GameObject go) where T : Component
        {
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
            T comp = go.GetComponent<T>();
#endif
            if (comp == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    RegisterUndo(go, "Add " + typeof(T));
#endif
                comp = go.AddComponent<T>();
            }
#if UNITY_FLASH
		return (T)comp;
#else
            return comp;
#endif
        }

        /// <summary>
        /// Convenience function that converts Class + Function combo into Class.Function representation.
        /// </summary>

        static public string GetFuncName(object obj, string method)
        {
            if (obj == null) return "<null>";
            string type = obj.GetType().ToString();
            int period = type.LastIndexOf('/');
            if (period > 0) type = type.Substring(period + 1);
            return string.IsNullOrEmpty(method) ? type : type + "/" + method;
        }
    }
}
