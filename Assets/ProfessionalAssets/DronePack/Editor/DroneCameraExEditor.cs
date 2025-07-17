using PA_DronePack;
using UnityEditor;
using UnityEngine;

namespace PA_DronePackEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DroneCameraEx))]
    public class DroneCameraExEditor : Editor
    {
        private DroneCameraEx dcScript;

        public void OnEnable()
        {
            dcScript = (DroneCameraEx)base.target;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(dcScript), typeof(DroneCameraEx), false);
            GUI.enabled = true;
            EditorGUILayout.LabelField("Main Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("cameraMode"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("followMode"));
            if (dcScript.followMode == DroneCameraEx.FollowMode.smooth)
            {
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("followSmoothing"));
            }

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("TPS Settings", EditorStyles.boldLabel);
            dcScript.findTarget = EditorGUILayout.Toggle("Auto Target?", dcScript.findTarget);
            if (!dcScript.findTarget)
            {
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("target"));
                GUILayout.Space(10f);
            }

            dcScript.autoPosition = EditorGUILayout.Toggle("Auto Position?", dcScript.autoPosition);
            if (!dcScript.autoPosition)
            {
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("height"));
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("distance"));
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("angle"));
                GUILayout.Space(10f);
            }

            dcScript.freeLook = EditorGUILayout.Toggle("Free Look?", dcScript.freeLook);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("xSensivity"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("ySensivity"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("invertYAxis"));
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("FPS Settings", EditorStyles.boldLabel);
            dcScript.findFPS = EditorGUILayout.Toggle("Auto Target?", dcScript.findFPS);
            if (!dcScript.findFPS)
            {
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("fpsPosition"));
                GUILayout.Space(10f);
            }

            dcScript.gyroscopeEnabled = EditorGUILayout.Toggle("Use Gyroscope?", dcScript.gyroscopeEnabled);
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("jitterRigidBodies"), true);
            if (GUI.changed)
            {
                base.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(dcScript);
                EditorUtility.SetDirty(dcScript.gameObject);
            }
        }
    }
}