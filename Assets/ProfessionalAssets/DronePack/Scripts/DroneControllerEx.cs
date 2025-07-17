using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace PA_DronePack
{

    public class DroneControllerEx : MonoBehaviour
    {
        [Tooltip("sets the drone's max forward speed")]
        public float forwardSpeed = 7f;

        [Tooltip("sets the drone's max backward speed")]
        public float backwardSpeed = 5f;

        [Tooltip("sets the drone's max left strafe speed")]
        public float rightSpeed = 5f;

        [Tooltip("sets the drone's max right strafe speed")]
        public float leftSpeed = 5f;

        [Tooltip("sets the drone's max rise speed")]
        public float riseSpeed = 5f;

        [Tooltip("sets the drone's max lower speed")]
        public float lowerSpeed = 5f;

        [Tooltip("how fast the drone speeds up")]
        public float acceleration = 0.5f;

        [Tooltip("how fast the drone slows down")]
        public float deceleration = 0.2f;

        [Tooltip("how eaisly the drone is affected by outside forces")]
        public float stability = 0.1f;

        [Tooltip("how fast the drone rotates")]
        public float turnSensitivty = 2f;

        [Tooltip("states whether or not the drone active on start")]
        public bool motorOn = true;

        [Tooltip("makes the drone move relative to an external compass")]
        public bool headless;

        [Tooltip("the external compass used to control the drone's flight direction")]
        public Transform compass;

        [Tooltip("assign drone's propellers to this array")]
        [HideInInspector]
        public List<GameObject> propellers;

        [Tooltip("set propellers max spin speed")]
        [HideInInspector]
        public float propSpinSpeed = 50f;

        [Tooltip("how fast the propellers slow down")]
        [HideInInspector]
        [Range(0f, 1f)]
        public float propStopSpeed = 1f;

        [Tooltip("the transform/location used to tilt the drone forward")]
        [HideInInspector]
        public Transform frontTilt;

        [Tooltip("the transform/location used to tilt the drone backward")]
        [HideInInspector]
        public Transform backTilt;

        [Tooltip("the transform/location used to tilt the drone right")]
        [HideInInspector]
        public Transform rightTilt;

        [Tooltip("the transform/location used to tilt the drone left")]
        [HideInInspector]
        public Transform leftTilt;

        [Tooltip("set whether or not the drone falls after a large impact")]
        [HideInInspector]
        public bool fallAfterCollision = true;

        [Tooltip("sets the min. collision force used to drop the drone")]
        [HideInInspector]
        public float fallMinimumForce = 6f;

        [Tooltip("sets the min. collision force used to create a spark")]
        [HideInInspector]
        public float sparkMinimumForce = 1f;

        [Tooltip("the spark particle/object spawned on a collision")]
        [HideInInspector]
        public GameObject sparkPrefab;

        [Tooltip("audio clip played during flight")]
        [HideInInspector]
        public AudioSource flyingSound;

        [Tooltip("audio clip played on collision")]
        [HideInInspector]
        public AudioSource sparkSound;

        [Tooltip("displays the collision force of the last impact")]
        [HideInInspector]
        public float collisionMagnitude;

        [Tooltip("displays the current force lifting up the drone")]
        [HideInInspector]
        public float liftForce;

        [Tooltip("displays the current force driving the drone")]
        [HideInInspector]
        public float driveForce;

        [Tooltip("displays the current force strafing the drone")]
        [HideInInspector]
        public float strafeForce;

        [Tooltip("displays the current force turning the drone")]
        [HideInInspector]
        public float turnForce;

        [Tooltip("displays the drone's distance from the ground")]
        [HideInInspector]
        public float groundDistance = float.PositiveInfinity;

        [Tooltip("displays the drones distance from being upright")]
        [HideInInspector]
        public float uprightAngleDistance;

        [Tooltip("displays the current propeller speed")]
        [HideInInspector]
        public float calPropSpeed;

        [Tooltip("displays drone's starting position")]
        [HideInInspector]
        public Vector3 startPosition;

        [Tooltip("displays the drone's rotational position")]
        [HideInInspector]
        public Quaternion startRotation;

        [Tooltip("states whether or not the drone active on start")]
        //motor fault
        public bool motorFault = false;
        public float motorFaultTorqueMagnitude = 100f;
        public Vector3 motorFaultTorqueDirection = new Vector3(0.1f, -0.03f, 0.08f);
        public Vector3 currentTorqueDirection = new Vector3(0.1f, -0.1f, 0.1f);
        public float motorFaultDuration = 0.3f;
        public float motorRecoveryTime = 2f;
        private bool isMotorFaultRunning = false;
        public bool spinPropeller = true;
        private Coroutine motorFaultCoroutine;

        //gust
        public bool gust = false;
        public float gustMagnitude = 100f;
        public float gustDuration = 0.3f;
        public Vector3 gustDirection = new Vector3(0, -1f, 0);
        private Coroutine gustCoroutine;


        public Rigidbody rigidBody;

        private RaycastHit hit;

        private Collider coll;

        private float driveInput;

        private float strafeInput;

        private float liftInput;

        private float _drag;

        private float _angularDrag;

        private bool _gravity;

        private void Awake()
        {
            coll = GetComponent<Collider>();
            rigidBody = GetComponent<Rigidbody>();
            startPosition = base.transform.position;
            startRotation = base.transform.rotation;
            _gravity = rigidBody.useGravity;
            _drag = rigidBody.drag;
            _angularDrag = rigidBody.angularDrag;

            groundDistance = float.PositiveInfinity;
        }

        private void Start()
        {
            MeshRenderer component = GetComponent<MeshRenderer>();
            if ((bool)component)
            {
                UnityEngine.Object.Destroy(component);
            }

            Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>(includeInactive: true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (!(componentsInChildren[i] == base.transform) && (!componentsInChildren[i].GetComponent<ObjectID>() || componentsInChildren[i].GetComponent<ObjectID>().id != "uQ3mR7quTHTw2aAy"))
                {
                    base.enabled = false;
                    base.hideFlags = HideFlags.NotEditable;
                    Debug.LogError("ERROR: Editing " + base.name + "'s prefab is NOT allowed!\n", base.gameObject);
                    Debug.LogWarning("Disabling PA_DroneController script...\n<color=#0057af>(Unlock the Full version of the drone pack to customize drones!)</color>", base.gameObject);
                    return;
                }
            }

            if (headless && !compass)
            {
                Debug.LogError("no headless compassed assinged! please asign a compass in order to use headless mode!");
                headless = false;
            }
        }

        private void Update()
        {
            uprightAngleDistance = (1f - base.transform.up.y) * 0.5f;
            uprightAngleDistance = (((double)uprightAngleDistance < 0.001) ? 0f : uprightAngleDistance);
            if (Physics.Raycast(base.transform.position, Vector3.down, out hit, float.PositiveInfinity))
            {
                groundDistance = hit.distance;
            }

            calPropSpeed = (motorOn ? propSpinSpeed : (calPropSpeed * (1f - propStopSpeed / 2f)));
            for (int i = 0; i < propellers.Count; i++)
            {
                if (i == 0 && spinPropeller == false)
                {
                    propellers[i].transform.Rotate(0f, 0f, 0f);
                }
                else
                {
                    propellers[i].transform.Rotate(0f, 0f, calPropSpeed);
                }
            }

            if ((bool)flyingSound)
            {
                flyingSound.volume = calPropSpeed / propSpinSpeed;
                flyingSound.pitch = 1f + liftForce * 0.02f;
            }
        }

        private void FixedUpdate()
        {
            if (motorOn)
            {
                rigidBody.useGravity = false;
                rigidBody.drag = 0f;
                rigidBody.angularDrag = 0f;
                if (headless)
                {
                    if (!compass)
                    {
                        Debug.LogError("no headless compassed assinged! please asign a compass in order to use headless mode!");
                        headless = false;
                    }

                    if (groundDistance > 0.2f && (driveInput != 0f || strafeInput != 0f))
                    {
                        rigidBody.AddForceAtPosition(Vector3.down, rigidBody.position + rigidBody.velocity.normalized * 0.5f, ForceMode.Acceleration);
                    }

                    Vector3 direction = compass.InverseTransformDirection(rigidBody.velocity);
                    direction.z = ((driveInput != 0f) ? Mathf.Lerp(direction.z, driveInput, acceleration * 0.3f) : Mathf.Lerp(direction.z, driveInput, deceleration * 0.2f));
                    driveForce = ((Mathf.Abs(direction.z) > 0.01f) ? direction.z : 0f);
                    direction.x = ((strafeInput != 0f) ? Mathf.Lerp(direction.x, strafeInput, acceleration * 0.3f) : Mathf.Lerp(direction.x, strafeInput, deceleration * 0.2f));
                    strafeForce = ((Mathf.Abs(direction.x) > 0.01f) ? direction.x : 0f);
                    rigidBody.velocity = compass.TransformDirection(direction);
                }
                else
                {
                    if (groundDistance > 0.2f)
                    {
                        if (driveInput > 0f)
                        {
                            rigidBody.AddForceAtPosition(Vector3.down * (Mathf.Abs(driveInput) * 0.3f), frontTilt.position, ForceMode.Acceleration);
                        }

                        if (driveInput < 0f)
                        {
                            rigidBody.AddForceAtPosition(Vector3.down * (Mathf.Abs(driveInput) * 0.3f), backTilt.position, ForceMode.Acceleration);
                        }

                        if (strafeInput > 0f)
                        {
                            rigidBody.AddForceAtPosition(Vector3.down * (Mathf.Abs(strafeInput) * 0.3f), rightTilt.position, ForceMode.Acceleration);
                        }

                        if (strafeInput < 0f)
                        {
                            rigidBody.AddForceAtPosition(Vector3.down * (Mathf.Abs(strafeInput) * 0.3f), leftTilt.position, ForceMode.Acceleration);
                        }
                    }

                    Vector3 direction2 = base.transform.InverseTransformDirection(rigidBody.velocity);
                    direction2.z = ((driveInput != 0f) ? Mathf.Lerp(direction2.z, driveInput, acceleration * 0.3f) : Mathf.Lerp(direction2.z, driveInput, deceleration * 0.2f));
                    driveForce = ((Mathf.Abs(direction2.z) > 0.01f) ? direction2.z : 0f);
                    direction2.x = ((strafeInput != 0f) ? Mathf.Lerp(direction2.x, strafeInput, acceleration * 0.3f) : Mathf.Lerp(direction2.x, strafeInput, deceleration * 0.2f));
                    strafeForce = ((Mathf.Abs(direction2.x) > 0.01f) ? direction2.x : 0f);
                    rigidBody.velocity = base.transform.TransformDirection(direction2);
                }
                if (motorFault)
                {
                    acceleration = 2f;
                    if (!isMotorFaultRunning)
                    {
                        isMotorFaultRunning = true;
                        motorFaultCoroutine = StartCoroutine(SimulateRepeatingMotorFault());
                        Debug.Log("Motor fault sequence initiated from FixedUpdate.");
                    }
                }
                if (gust)
                {
                    gustCoroutine = StartCoroutine(SimulateGust());
                    gust = false;
                }
                liftForce = ((liftInput != 0f) ? Mathf.Lerp(liftForce, liftInput, acceleration * 0.2f) : Mathf.Lerp(liftForce, liftInput, deceleration * 0.3f));
                liftForce = ((Mathf.Abs(liftForce) > 0.01f) ? liftForce : 0f);
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, liftForce, rigidBody.velocity.z);
                rigidBody.angularVelocity *= 1f - Mathf.Clamp(InputMagnitude(), 0.2f, 1f) * stability;
                Quaternion quaternion = Quaternion.FromToRotation(base.transform.up, Vector3.up);
                rigidBody.AddTorque(new Vector3(quaternion.x, 0f, quaternion.z) * 100f, ForceMode.Acceleration);
                rigidBody.angularVelocity = new Vector3(rigidBody.angularVelocity.x, turnForce, rigidBody.angularVelocity.z);
            }
            else
            {
                rigidBody.useGravity = _gravity;
                rigidBody.drag = _drag;
                rigidBody.angularDrag = _angularDrag;
            }
        }

        private IEnumerator SimulateRepeatingMotorFault()
        {
            while (motorFault)
            {
                // --- FAULT ACTIVE PHASE ---
                Debug.Log("Motor fault ACTIVE!");
                float currentFaultTime = 0f;
                spinPropeller = false;
                var index = 0;
                while (currentFaultTime < motorFaultDuration)
                {
                    currentTorqueDirection = motorFaultTorqueDirection * motorFaultTorqueMagnitude;
                    Debug.Log($"============= [{index++}] {motorFaultTorqueDirection} / {motorFaultTorqueMagnitude}");
                    rigidBody.AddRelativeTorque(motorFaultTorqueDirection * motorFaultTorqueMagnitude, ForceMode.Acceleration);
                    currentFaultTime += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                spinPropeller = true;
                // --- RECOVERY PHASE ---
                Debug.Log("Motor fault RECOVERING...");
                yield return new WaitForSeconds(motorRecoveryTime); // Wait for the recovery time
            }
            isMotorFaultRunning = false;
            Debug.Log("Motor fault coroutine finished.");
        }
        private IEnumerator SimulateGust()
        {
            float timer = 0f;
            Vector3 forceVector = gustDirection.normalized * gustMagnitude;

            while (timer < gustDuration)
            {
                rigidBody.AddForce(forceVector, ForceMode.Force);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        public void MotorFault()
        {
            motorFault = true;
        }

        private void OnCollisionEnter(Collision newObject)
        {
            collisionMagnitude = newObject.relativeVelocity.magnitude;
            if (collisionMagnitude > sparkMinimumForce)
            {
                SpawnSparkPrefab(newObject.contacts[0].point);
                if ((bool)sparkSound)
                {
                    sparkSound.pitch = collisionMagnitude * 0.1f;
                    sparkSound.PlayOneShot(sparkSound.clip, collisionMagnitude * 0.05f);
                }
            }

            if (collisionMagnitude > fallMinimumForce && fallAfterCollision)
            {
                motorOn = false;
            }
        }

        private void OnCollisionStay(Collision newObject)
        {
            if (groundDistance < coll.bounds.extents.y + 0.15f)
            {
                liftForce = Mathf.Clamp(liftForce, 0f, float.PositiveInfinity);
            }
        }

        public void ToggleMotor()
        {
            motorOn = !motorOn;
        }

        public void ToggleHeadless()
        {
            headless = !headless;
        }

        public void DriveInput(float input)
        {
            if (input > 0f)
            {
                driveInput = input * forwardSpeed;
            }
            else if (input < 0f)
            {
                driveInput = input * backwardSpeed;
            }
            else
            {
                driveInput = 0f;
            }
        }

        public void StrafeInput(float input)
        {
            if (input > 0f)
            {
                strafeInput = input * rightSpeed;
            }
            else if (input < 0f)
            {
                strafeInput = input * leftSpeed;
            }
            else
            {
                strafeInput = 0f;
            }
        }

        public void LiftInput(float input)
        {
            if (input > 0f)
            {
                liftInput = input * riseSpeed;
                motorOn = true;
            }
            else if (input < 0f)
            {
                liftInput = input * lowerSpeed;
            }
            else
            {
                liftInput = 0f;
            }
        }

        public void TurnInput(float input)
        {
            turnForce = input * turnSensitivty;
        }

        public void ResetDronePosition()
        {
            rigidBody.position = startPosition;
            rigidBody.rotation = startRotation;
            rigidBody.velocity = Vector3.zero;
        }

        public void SpawnSparkPrefab(Vector3 position)
        {
            GameObject obj = UnityEngine.Object.Instantiate(sparkPrefab, position, Quaternion.identity);
            ParticleSystem.MainModule main = obj.GetComponent<ParticleSystem>().main;
            UnityEngine.Object.Destroy(obj, main.duration + main.startLifetime.constantMax);
        }

        public void AdjustLift(float value)
        {
            riseSpeed = value;
            lowerSpeed = value;
        }

        public void AdjustSpeed(float value)
        {
            forwardSpeed = value;
            backwardSpeed = value;
        }

        public void AdjustStrafe(float value)
        {
            rightSpeed = value;
            leftSpeed = value;
        }

        public void AdjustTurn(float value)
        {
            turnSensitivty = value;
        }

        public void AdjustAccel(float value)
        {
            acceleration = value;
        }

        public void AdjustDecel(float value)
        {
            deceleration = value;
        }

        public void AdjustStable(float value)
        {
            stability = value;
        }

        public void ToggleFall(bool state)
        {
            fallAfterCollision = !fallAfterCollision;
        }

        public void ChangeFlightAudio(AudioClip newClip)
        {
            flyingSound.clip = newClip;
            flyingSound.enabled = false;
            flyingSound.enabled = true;
        }

        public void ChangeImpactAudio(AudioClip newClip)
        {
            sparkSound.clip = newClip;
            sparkSound.enabled = false;
            sparkSound.enabled = true;
        }

        private float InputMagnitude()
        {
            return (Mathf.Abs(driveInput) + Mathf.Abs(strafeInput) + Mathf.Abs(liftInput)) / 3f;
        }

        private int TextureHash(Texture2D _texture)
        {
            int num = 0;
            try
            {
                Color[] pixels = _texture.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    Color color = pixels[i];
                    num += Mathf.FloorToInt(color.r + color.g + color.b);
                }
            }
            catch
            {
                Texture2D texture2D = new Texture2D(_texture.width, _texture.height, _texture.format, _texture.mipmapCount > 1);
                texture2D.LoadRawTextureData(_texture.GetRawTextureData());
                texture2D.Apply();
                Color[] pixels = texture2D.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    Color color2 = pixels[i];
                    num += Mathf.FloorToInt(color2.r + color2.g + color2.b);
                }
            }

            return num;
        }
    }
}