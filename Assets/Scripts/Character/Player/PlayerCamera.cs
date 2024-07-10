using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        // change these to tweak camers performance
        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1; // the bigger this number, the longer for the camer to reach its position during movement
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30; // the lowest point you are able to look down
        [SerializeField] float maximumPivot = 60; // the highest point you are able to look up

        // just displays camera values
        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                // follow the player
                HandleFollowTarget();

                // rotate around the player
                HandleRotations();

                // collide with objects
            }

        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraposition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraposition;
        }

        private void HandleRotations()
        {
            // if locked on, force rotation towards target
            // else rotate reguarly

            // normal rotation
            // rotate left and right based on horizontal movement on the right joystick
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            // rotate up and doen based on the vertical momvement on the right joystick
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // clamp the up and down look angle between a min and max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // rotate this gameobject left and right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // rotate this gameobject up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }
}

