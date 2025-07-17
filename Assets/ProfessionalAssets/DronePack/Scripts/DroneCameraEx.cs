using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{

    public class DroneCameraEx : MonoBehaviour
    {
        public enum CameraMode
        {
            thirdPerson,
            firstPerson
        }

        public enum FollowMode
        {
            firm,
            smooth
        }

        public CameraMode cameraMode;

        public FollowMode followMode;

        public DroneControllerEx target;

        public Transform fpsPosition;

        [Range(0.03f, 0.5f)]
        public float followSmoothing = 0.1f;

        [Range(0f, 7f)]
        public float xSensivity = 2f;

        [Range(0f, 7f)]
        public float ySensivity;

        public float height = 0.5f;

        public float distance = 1.5f;

        public float angle = 19f;

        public bool findTarget = true;

        public bool autoPosition = true;

        public bool freeLook;

        public bool findFPS = true;

        public bool gyroscopeEnabled;

        public bool invertYAxis;

        public List<Rigidbody> jitterRigidBodies;

        private Vector3 velocity;

        private float fpsMinAngle = -20f;

        private float fpsMaxAngle = 70f;

        private float angleV;

        private float scrollForce;

        private float turnForce;

        private float liftForce;

        private float targetRot;

        private void Start()
        {
            if (findTarget)
            {
                target = UnityEngine.Object.FindObjectOfType<DroneControllerEx>();
                if (target == null)
                {
                    Debug.LogWarning("PA_DroneCamera could not find a drone target", base.gameObject);
                }
            }

            if (findFPS)
            {
                fpsPosition = GameObject.Find("FPSView").transform;
                if (fpsPosition == null)
                {
                    Debug.LogWarning("PA_DroneCamera could not find an FPS position", base.gameObject);
                }
            }

            if (autoPosition && (bool)target)
            {
                float num = Mathf.Abs(target.transform.position.x - base.transform.position.x);
                float num2 = Mathf.Abs(target.transform.position.z - base.transform.position.z);
                distance = ((num > num2) ? num : num2);
                height = Mathf.Abs(target.transform.position.y - base.transform.position.y);
                angle = base.transform.eulerAngles.x;
            }
            else if (!target)
            {
                Debug.LogError("ERROR: PA_DroneCamera is missing a target");
            }
        }

        private void Update()
        {
            if (freeLook && cameraMode == CameraMode.thirdPerson)
            {
                target.TurnInput(0f);
            }

            scrollForce = Mathf.Clamp(scrollForce + (0f - Input.GetAxis("Mouse ScrollWheel")) * 10f, fpsMinAngle, fpsMaxAngle);
        }

        private void LateUpdate()
        {
            if (freeLook && cameraMode == CameraMode.thirdPerson)
            {
                target.TurnInput(0f);
            }

            if ((bool)target && followMode == FollowMode.firm && cameraMode == CameraMode.thirdPerson)
            {
                if (target.rigidBody.interpolation != RigidbodyInterpolation.Interpolate && !GetComponent<Camera>().targetTexture)
                {
                    target.rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
                }

                height += ((angle > -60f && angle < 60f) ? (liftForce * 0.03f) : 0f);
                angle = Mathf.Clamp(angle + liftForce, -60f, 60f);
                targetRot = (freeLook ? (targetRot + turnForce * xSensivity) : target.transform.eulerAngles.y);
                base.transform.position = target.transform.position - Quaternion.Euler(0f, targetRot, 0f) * Vector3.forward * distance + new Vector3(0f, height, 0f);
                base.transform.rotation = Quaternion.Euler(angle, targetRot, 0f);
                foreach (Rigidbody jitterRigidBody in jitterRigidBodies)
                {
                    if (jitterRigidBody.interpolation != RigidbodyInterpolation.Interpolate && !GetComponent<Camera>().targetTexture)
                    {
                        jitterRigidBody.interpolation = RigidbodyInterpolation.Interpolate;
                    }
                }
            }

            if (!target || !fpsPosition || cameraMode != CameraMode.firstPerson || (bool)GetComponent<Camera>().targetTexture)
            {
                return;
            }

            target.rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            base.transform.position = fpsPosition.position;
            base.transform.rotation = ((!gyroscopeEnabled) ? Quaternion.Euler(target.transform.rotation.eulerAngles.x + scrollForce, fpsPosition.rotation.eulerAngles.y, fpsPosition.rotation.eulerAngles.z) : Quaternion.Euler(scrollForce, fpsPosition.rotation.eulerAngles.y, 0f));
            fpsPosition.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, target.transform.rotation.eulerAngles.y, target.transform.rotation.eulerAngles.z);
            foreach (Rigidbody jitterRigidBody2 in jitterRigidBodies)
            {
                jitterRigidBody2.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        private void FixedUpdate()
        {
            if (freeLook && cameraMode == CameraMode.thirdPerson)
            {
                target.TurnInput(0f);
            }

            if (!target || followMode != FollowMode.smooth || cameraMode != CameraMode.thirdPerson)
            {
                return;
            }

            if (target.rigidBody.interpolation != RigidbodyInterpolation.None && !GetComponent<Camera>().targetTexture)
            {
                target.rigidBody.interpolation = RigidbodyInterpolation.None;
            }

            height += ((angle > -60f && angle < 60f) ? (liftForce * 0.03f) : 0f);
            angle = Mathf.Clamp(angle + liftForce, -60f, 60f);
            targetRot = (freeLook ? (targetRot + turnForce * xSensivity) : target.transform.eulerAngles.y);
            float y = Mathf.SmoothDampAngle(base.transform.eulerAngles.y, targetRot, ref angleV, followSmoothing * Time.fixedDeltaTime * 60f);
            Vector3 vector = target.transform.position - Quaternion.Euler(0f, y, 0f) * Vector3.forward * distance + new Vector3(0f, height, 0f);
            base.transform.position = Vector3.SmoothDamp(base.transform.position, vector, ref velocity, followSmoothing * Time.fixedDeltaTime * 60f);
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(angle, targetRot, 0f), followSmoothing * Time.fixedDeltaTime * 60f);
            foreach (Rigidbody jitterRigidBody in jitterRigidBodies)
            {
                if (jitterRigidBody.interpolation != RigidbodyInterpolation.None && !GetComponent<Camera>().targetTexture)
                {
                    jitterRigidBody.interpolation = RigidbodyInterpolation.None;
                }
            }
        }

        public void ChangeCameraMode()
        {
            cameraMode = ((cameraMode != CameraMode.firstPerson) ? CameraMode.firstPerson : CameraMode.thirdPerson);
        }

        public void ChangeFollowMode()
        {
            followMode = ((followMode != FollowMode.smooth) ? FollowMode.smooth : FollowMode.firm);
        }

        public void ChangeGyroscope()
        {
            gyroscopeEnabled = !gyroscopeEnabled;
        }

        public void LiftInput(float input)
        {
            liftForce = ((!invertYAxis) ? (input * ySensivity) : ((0f - input) * ySensivity));
        }

        public void TurnInput(float input)
        {
            turnForce = input * xSensivity;
        }

        public void CanFreeLook(bool state)
        {
            freeLook = state;
        }

        public void XSensitivity(float value)
        {
            xSensivity = value;
        }

        public void YSensitivity(float value)
        {
            ySensivity = value;
        }

        public void InvertYAxis(bool state)
        {
            invertYAxis = state;
        }
    }
}