using System;
using System.Linq;
using UnityEngine;

namespace PA_DronePack
{
    public class DroneAxisInputEx : MonoBehaviour
    {
        public enum InputType
        {
            Desktop,
            Gamepad,
            OpenVR,
            Custom
        }

        public InputType inputType;

        private InputType? _inputType;

        public string forwardBackward;

        public string _forwardBackward;

        public string strafeLeftRight;

        public string _strafeLeftRight;

        public string riseLower;

        public string _riseLower;

        public string turn;

        public string _turn;

        public string toggleMotor;

        public string _toggleMotor;

        public string toggleCameraMode;

        public string _toggleCameraMode;

        public string toggleCameraGyro;

        public string _toggleCameraGyro;

        public string toggleFollowMode;

        public string _toggleFollowMode;

        public string toggleHeadless;

        public string _toggleHeadless;

        public string cameraRiseLower;

        public string _cameraRiseLower;

        public string cameraTurn;

        public string _cameraTurn;

        public string cameraFreeLook;

        public string _cameraFreeLook;

        public DroneControllerEx dcoScript;

        public DroneCameraEx dcScript;

        private bool toggleMotorIsKey;

        private bool toggleCameraModeIsKey;

        private bool toggleCameraGyroIsKey;

        private bool toggleFollowModeIsKey;

        private bool cameraFreeLookIsKey;

        private bool toggleHeadlessLookIsKey;

        private string[] keys = new string[77]
        {
            "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10",
            "f11", "f12", "f13", "f14", "f15", "0", "1", "2", "3", "4",
            "5", "6", "7", "8", "9", "a", "b", "c", "d", "e",
            "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y",
            "z", "backspace", "delete", "tab", "clear", "return", "pause", "escape", "space", "up",
            "down", "right", "left", "insert", "home", "end", "pageup", "pagedown", "numlock", "capslock",
            "scroll ock", "rightshift", "leftshift", "rightctrl", "leftctrl", "rightalt", "leftalt"
        };

        private void Awake()
        {
            dcoScript = GetComponent<DroneControllerEx>();
            dcScript = UnityEngine.Object.FindObjectOfType<DroneCameraEx>();
            UpdateInput();
        }

        private void Update()
        {
            if (_inputType != inputType)
            {
                UpdateInput();
                _inputType = inputType;
            }

            if (forwardBackward != "")
            {
                dcoScript.DriveInput(Input.GetAxisRaw(forwardBackward));
            }

            if (strafeLeftRight != "")
            {
                dcoScript.StrafeInput(Input.GetAxisRaw(strafeLeftRight));
            }

            if (riseLower != "")
            {
                dcoScript.LiftInput(Input.GetAxisRaw(riseLower));
            }

            if (turn != "")
            {
                dcoScript.TurnInput(Input.GetAxisRaw(turn));
            }

            if (cameraRiseLower != "" && (bool)dcScript)
            {
                dcScript.LiftInput(Input.GetAxisRaw(cameraRiseLower));
            }

            if (cameraTurn != "" && (bool)dcScript)
            {
                dcScript.TurnInput(Input.GetAxisRaw(cameraTurn));
            }

            if (toggleMotor != "")
            {
                if (toggleMotorIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), toggleMotor)))
                    {
                        dcoScript.ToggleMotor();
                    }
                }
                else if (Input.GetButtonDown(toggleMotor))
                {
                    dcoScript.ToggleMotor();
                }
            }

            if (toggleHeadless != "")
            {
                if (toggleHeadlessLookIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), toggleHeadless)))
                    {
                        dcoScript.ToggleHeadless();
                    }
                }
                else if (Input.GetButtonDown(toggleHeadless))
                {
                    dcoScript.ToggleHeadless();
                }
            }

            if (toggleCameraMode != "" && (bool)dcScript)
            {
                if (toggleCameraModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), toggleCameraMode)))
                    {
                        dcScript.ChangeCameraMode();
                    }
                }
                else if (Input.GetButtonDown(toggleCameraMode))
                {
                    dcScript.ChangeCameraMode();
                }
            }

            if (toggleCameraGyro != "" && (bool)dcScript)
            {
                if (toggleCameraGyroIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), toggleCameraGyro)))
                    {
                        dcScript.ChangeGyroscope();
                    }
                }
                else if (Input.GetButtonDown(toggleCameraGyro))
                {
                    dcScript.ChangeGyroscope();
                }
            }

            if (toggleFollowMode != "" && (bool)dcScript)
            {
                if (toggleFollowModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), toggleFollowMode)))
                    {
                        dcScript.ChangeFollowMode();
                    }
                }
                else if (Input.GetButtonDown(toggleFollowMode))
                {
                    dcScript.ChangeFollowMode();
                }
            }

            if (!(cameraFreeLook != "") || !dcScript)
            {
                return;
            }

            if (cameraFreeLookIsKey)
            {
                if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), cameraFreeLook)))
                {
                    dcScript.CanFreeLook(state: true);
                }

                if (Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), cameraFreeLook)))
                {
                    dcScript.CanFreeLook(state: false);
                }
            }
            else
            {
                if (Input.GetButtonDown(cameraFreeLook))
                {
                    dcScript.CanFreeLook(state: true);
                }

                if (Input.GetButtonUp(cameraFreeLook))
                {
                    dcScript.CanFreeLook(state: false);
                }
            }
        }

        private void ParseKeys()
        {
            toggleMotorIsKey = keys.Contains(toggleMotor.ToLower());
            toggleCameraModeIsKey = keys.Contains(toggleCameraMode.ToLower());
            toggleCameraGyroIsKey = keys.Contains(toggleCameraGyro.ToLower());
            toggleFollowModeIsKey = keys.Contains(toggleFollowMode.ToLower());
            cameraFreeLookIsKey = keys.Contains(cameraFreeLook.ToLower());
            toggleHeadlessLookIsKey = keys.Contains(toggleHeadless.ToLower());
        }

        public void UpdateInput()
        {
            if (inputType == InputType.Desktop)
            {
                forwardBackward = "Vertical";
                strafeLeftRight = "Horizontal";
                riseLower = "Lift";
                turn = "FD_Turn";
                cameraRiseLower = "Mouse Y";
                cameraTurn = "Mouse X";
                toggleMotor = "Z";
                toggleCameraMode = "C";
                toggleCameraGyro = "G";
                toggleFollowMode = "F";
                cameraFreeLook = "LeftAlt";
                toggleHeadless = "H";
            }

            if (inputType == InputType.Gamepad)
            {
                forwardBackward = "GP SecondaryJoystick Y";
                strafeLeftRight = "GP SecondaryJoystick X";
                riseLower = "GP PrimaryJoystick Y";
                turn = "GP PrimaryJoystick X";
                cameraRiseLower = "GP DPad Y";
                cameraTurn = "GP PrimaryJoystick X";
                toggleMotor = "GP Button 0";
                toggleCameraMode = "GP Button 1";
                toggleCameraGyro = "GP Button 2";
                toggleFollowMode = "GP Button 3";
                cameraFreeLook = "GP Button 5";
                toggleHeadless = "GP Button 6";
            }

            if (inputType == InputType.OpenVR)
            {
                forwardBackward = "OVR RightJoystick Y";
                strafeLeftRight = "OVR RightJoystick X";
                riseLower = "OVR LeftJoystick Y";
                turn = "OVR LeftJoystick X";
                cameraRiseLower = "";
                cameraTurn = "OVR LeftJoystick X";
                toggleMotor = "OVR RightButton 0";
                toggleCameraMode = "C";
                toggleCameraGyro = "G";
                toggleFollowMode = "F";
                cameraFreeLook = "OVR RightTrigger";
                toggleHeadless = "OVR RightGrip";
            }

            if (inputType == InputType.Custom)
            {
                forwardBackward = _forwardBackward;
                strafeLeftRight = _strafeLeftRight;
                riseLower = _riseLower;
                turn = _turn;
                toggleMotor = _toggleMotor;
                toggleCameraMode = _toggleCameraMode;
                toggleCameraGyro = _toggleCameraGyro;
                toggleFollowMode = _toggleFollowMode;
                cameraRiseLower = _cameraRiseLower;
                cameraTurn = _cameraTurn;
                cameraFreeLook = _cameraFreeLook;
                toggleHeadless = _toggleHeadless;
            }

            ParseKeys();
        }
    }
}