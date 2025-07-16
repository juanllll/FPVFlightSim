using PA_DronePack;
using UnityEditor;
using UnityEngine;

namespace PA_DronePackEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DroneControllerEx))]

    public class DroneControllerExEditor : Editor
    {
        private DroneControllerEx dcoScript;

        public void OnEnable()
        {
            dcoScript = (DroneControllerEx)base.target;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(dcoScript), typeof(DroneControllerEx), false);
            GUI.enabled = true;
            EditorGUILayout.LabelField("Movement Values", EditorStyles.boldLabel);
            dcoScript.forwardSpeed = EditorGUILayout.FloatField(new GUIContent("Forward Speed", "sets the drone's max forward speed"), dcoScript.forwardSpeed);
            dcoScript.backwardSpeed = EditorGUILayout.FloatField(new GUIContent("Backward Speed", "sets the drone's max backward speed"), dcoScript.backwardSpeed);
            GUILayout.Space(10f);
            dcoScript.rightSpeed = EditorGUILayout.FloatField(new GUIContent("Strafe Right Speed", "sets the drone's max right strafe speed"), dcoScript.rightSpeed);
            dcoScript.leftSpeed = EditorGUILayout.FloatField(new GUIContent("Strafe Left Speed", "sets the drone's max left strafe speed"), dcoScript.leftSpeed);
            GUILayout.Space(10f);
            dcoScript.riseSpeed = EditorGUILayout.FloatField(new GUIContent("Height Rise Speed", "sets the drone's max rise speed"), dcoScript.riseSpeed);
            dcoScript.lowerSpeed = EditorGUILayout.FloatField(new GUIContent("Height Lower Speed", "sets the drone's max lower speed"), dcoScript.lowerSpeed);
            GUILayout.Space(10f);
            dcoScript.acceleration = EditorGUILayout.Slider(new GUIContent("Acceleration", "how fast the drone speeds up"), dcoScript.acceleration, 0.1f, 1f);
            dcoScript.deceleration = EditorGUILayout.Slider(new GUIContent("Deceleration", "how fast the drone slows down"), dcoScript.deceleration, 0.1f, 1f);
            dcoScript.stability = EditorGUILayout.Slider(new GUIContent("Stability", "how eaisly the drone is affected by outside forces"), dcoScript.stability, 0f, 1f);
            dcoScript.turnSensitivty = EditorGUILayout.Slider(new GUIContent("Turn Sensitivity", "how fast the drone rotates"), dcoScript.turnSensitivty, 0.1f, 5f);
            GUILayout.Space(10f);
            dcoScript.motorOn = EditorGUILayout.Toggle(new GUIContent("Is Motor On?", "states whether or not the drone active on start"), dcoScript.motorOn);
            dcoScript.headless = EditorGUILayout.Toggle(new GUIContent("Use Headless Mode?", "makes the drone move relative to an external compass"), dcoScript.headless);
            if (dcoScript.headless)
            {
                dcoScript.compass = EditorGUILayout.ObjectField(new GUIContent("Headless Compass", "the external compass used to control the drone's flight direction"), dcoScript.compass, typeof(Transform), true) as Transform;
            }
            else
            {
                GUILayout.Space(18f);
            }

            GUILayout.Space(10f);
            EditorGUILayout.HelpBox("Download the Full version of this DronePack to Unlock more customization options!", MessageType.Info);
            EditorGUI.BeginDisabledGroup(disabled: true);
            EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("propellers"), true);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("propSpinSpeed"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("propStopSpeed"));
            if (dcoScript.headless)
            {
                EditorGUI.BeginDisabledGroup(disabled: true);
                EditorGUILayout.TextField(new GUIContent("Front Tilt", "tilt points disabled in headless mode"), "Tilt Points disabled in Headless mode...");
                EditorGUILayout.TextField(new GUIContent("Back Tilt", "tilt points disabled in headless mode"), "...");
                EditorGUILayout.TextField(new GUIContent("Right Tilt", "tilt points disabled in headless mode"), "...");
                EditorGUILayout.TextField(new GUIContent("Left Tilt", "tilt points disabled in headless mode"), "...");
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("frontTilt"));
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("backTilt"));
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("rightTilt"));
                EditorGUILayout.PropertyField(base.serializedObject.FindProperty("leftTilt"));
            }

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Collision Settings", EditorStyles.boldLabel);
            dcoScript.fallAfterCollision = EditorGUILayout.Toggle(new GUIContent("Fall After Collision?", "set whether or not the drone falls after a large impact"), dcoScript.fallAfterCollision);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("fallMinimumForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("sparkMinimumForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("sparkPrefab"));
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Sound Effects", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("flyingSound"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("sparkSound"));
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Read Only Variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("collisionMagnitude"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("liftForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("driveForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("strafeForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("turnForce"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("groundDistance"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("uprightAngleDistance"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("calPropSpeed"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("startRotation"), true);
            // EditorGUI.EndDisabledGroup();
            if (GUI.changed)
            {
                base.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(dcoScript);
                EditorUtility.SetDirty(dcoScript.gameObject);
            }
        }
    }
}