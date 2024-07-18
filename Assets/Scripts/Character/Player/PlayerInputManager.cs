using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EB
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;

        public PlayerManager player;

        // think about goals in steps
        // move character based on those values

        PlayerControls playerControls;

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraHorizontalInput;
        public float cameraVerticalInput;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float horizontalInput;
        public float verticalInput;
        [SerializeField] public float moveAmout;

        [Header("Player Actions Input")]
        [SerializeField] bool dodgeInput = false;

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

            // When the scene changes, run this logic
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // if we are loading into our world scene, enabling our player controls
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            // otherwise must be at the main menu, disbale our player controls
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
                //playerControls.PlayerActions.Dodge.performed += instance => dodgeInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // if we destroy object, unsub from this event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        // if we minimise or lower the window, stop adjusting inputs
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
        }

        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // returns the absoulte number, (meaning number without the negative sign, so its always positive)
            moveAmout = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            // we clamp the values, so they are 0, 0.5 or 1 (optional)
            if (moveAmout <= 0.5 && moveAmout > 0)
            {
                moveAmout = 0.5f;
            }
            else if (moveAmout > 0.5 && moveAmout <= 1)
            {
                moveAmout = 1;
            }

            // Why do we pass 0 on the horizontal? because we only want non-strafing movement
            // we use the horizontal when we are strafing or locked on

            if (player == null)
            {
                return;
            }

            // if we are not locked on, only use the move amout
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmout);

            // if we are locked on pass the horizontal movement as well
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        private void HandleDodgeinput()
        {

        }
    }
}

