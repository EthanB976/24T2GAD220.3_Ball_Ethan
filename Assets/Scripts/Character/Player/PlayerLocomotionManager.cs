using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmout;

        private Vector3 targetRotationDirection;
        private Vector3 moveDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float rotationSpeed = 15;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (playerManager.IsOwner)
            {
                playerManager.characterNetworkManager.verticalMovement.Value = verticalMovement;
                playerManager.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                playerManager.characterNetworkManager.networkMoveAmout.Value = moveAmout;
            }
            else
            {
                verticalMovement = playerManager.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = playerManager.characterNetworkManager.horizontalMovement.Value;
                moveAmout = playerManager.characterNetworkManager.networkMoveAmout.Value;

                // if not locked on, pass move amout
                playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmout);
            }
        }
        public void HandleAllMovement()
        {
            // ground movement
            HandleGroundedMovement();
            HandleRotation();

            // aerial movement
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmout = PlayerInputManager.instance.moveAmout;

            //clamo the movements
        }

        private void HandleGroundedMovement()
        {
            GetMovementValues();

            // our movement direction is based on our camera direction and our movement inputs
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (PlayerInputManager.instance.moveAmout > 0.5f)
            {
                // move at running speed
                playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);

            }
            else if (PlayerInputManager.instance.moveAmout >= 0.5f)
            {
                // move at walking speed
                playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;

           
        }
    }
}

