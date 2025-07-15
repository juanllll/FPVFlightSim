using PA_DronePack;
using UnityEditor;
using UnityEngine;

namespace PA_DronePackEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DroneAxisInputEx))]
    public class DroneAxisInputExEditor : Editor
    {
        private DroneAxisInputEx daiScript;

        public void OnEnable()
        {
            daiScript = (DroneAxisInputEx)base.target;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(daiScript), typeof(DroneAxisInputEx), false);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("inputType"));
            GUILayout.Space(10f);
            if (daiScript.inputType == DroneAxisInputEx.InputType.Custom)
            {
                daiScript.UpdateInput();
                EditorGUILayout.LabelField("Input Axis", EditorStyles.boldLabel);
                daiScript._forwardBackward = EditorGUILayout.TextField("Forward & Backward", daiScript._forwardBackward);
                daiScript._strafeLeftRight = EditorGUILayout.TextField("Strafe Left & Right", daiScript._strafeLeftRight);
                daiScript._riseLower = EditorGUILayout.TextField("Rise & Lower", daiScript._riseLower);
                daiScript._turn = EditorGUILayout.TextField("Turn", daiScript._turn);
                GUILayout.Space(10f);
                daiScript._cameraRiseLower = EditorGUILayout.TextField("Camera Rise & Lower", daiScript._cameraRiseLower);
                daiScript._cameraTurn = EditorGUILayout.TextField("Camera Turn", daiScript._cameraTurn);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Input Axis / Button / Keycode", EditorStyles.boldLabel);
                daiScript._toggleMotor = EditorGUILayout.TextField("Toggle Motor", daiScript._toggleMotor);
                daiScript._toggleCameraMode = EditorGUILayout.TextField("Change Camera Mode", daiScript._toggleCameraMode);
                daiScript._toggleCameraGyro = EditorGUILayout.TextField("Toggle Camera Gyro", daiScript._toggleCameraGyro);
                daiScript._toggleFollowMode = EditorGUILayout.TextField("Change Follow Mode", daiScript._toggleFollowMode);
                daiScript._cameraFreeLook = EditorGUILayout.TextField("Toggle FreeLook", daiScript._cameraFreeLook);
                daiScript._toggleHeadless = EditorGUILayout.TextField("Toggle Headless Mode", daiScript._toggleHeadless);
            }
            else
            {
                daiScript.UpdateInput();
                EditorGUILayout.LabelField("Input Axis", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(disabled: true);
                daiScript.forwardBackward = EditorGUILayout.TextField("Forward & Backward", daiScript.forwardBackward);
                daiScript.strafeLeftRight = EditorGUILayout.TextField("Strafe Left & Right", daiScript.strafeLeftRight);
                daiScript.riseLower = EditorGUILayout.TextField("Rise & Lower", daiScript.riseLower);
                daiScript.turn = EditorGUILayout.TextField("Turn", daiScript.turn);
                GUILayout.Space(10f);
                daiScript.cameraRiseLower = EditorGUILayout.TextField("Camera Rise & Lower", daiScript.cameraRiseLower);
                daiScript.cameraTurn = EditorGUILayout.TextField("Camera Turn", daiScript.cameraTurn);
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Input Axis / Button / Keycode", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(disabled: true);
                daiScript.toggleMotor = EditorGUILayout.TextField("Toggle Motor", daiScript.toggleMotor);
                daiScript.toggleCameraMode = EditorGUILayout.TextField("Change Camera Mode", daiScript.toggleCameraMode);
                daiScript.toggleCameraGyro = EditorGUILayout.TextField("Toggle Camera Gyro", daiScript.toggleCameraGyro);
                daiScript.toggleFollowMode = EditorGUILayout.TextField("Change Follow Mode", daiScript.toggleFollowMode);
                daiScript.cameraFreeLook = EditorGUILayout.TextField("Toggle FreeLook", daiScript.cameraFreeLook);
                daiScript.toggleHeadless = EditorGUILayout.TextField("Toggle Headless Mode", daiScript.toggleHeadless);
                EditorGUI.EndDisabledGroup();
            }

            if (GUI.changed)
            {
                base.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(daiScript);
                EditorUtility.SetDirty(daiScript.gameObject);
            }
        }
    }
}