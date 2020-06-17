using UnityEngine;
using System.Collections;
using System;

namespace Zeptolab
{
    [RequireComponent(typeof(SpaceCollisionsController))]
    public class CharacterController : MonoBehaviour
    {
        public GameObject CharacterModel;
        public float RightSpeed;
        public float maxJumpHeight = 4;
        public float timeToJumpApex = .4f;
        float accelerationTimeAirborne = .2f;
        float accelerationTimeGrounded = .1f;
        float moveSpeed = 6;

        public Vector2 wallJumpClimb;
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;

        public float wallSlideSpeedMax = 3;
        public float wallStickTime = .25f;
        float timeToWallUnstick ;

        float gravity;
        float maxJumpVelocity;
        float minJumpVelocity;
        Vector3 velocity;
        float velocityXSmoothing;

        private SpaceCollisionsController _spaceCollisionsController;

        Vector2 directionalInput;
        bool wallSliding;
        int wallDirX;

        private void Awake()
        {
            InputManager.Instance.OnUserTapDown += OnUserJump;
            InputManager.Instance.SetDirectionalInput += SetDirectionalInput;
        }

        void Start()
        {
            _spaceCollisionsController = GetComponent<SpaceCollisionsController>();

            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

            SetDirectionalInput(Vector3.right*RightSpeed);
        }

        private void FixedUpdate()
        {
            CalculateVelocity();
            HandleWallSliding();

            _spaceCollisionsController.Move(velocity * Time.deltaTime, directionalInput);
            Vector3 rotation = CharacterModel.transform.eulerAngles;
            rotation.y = Mathf.Sign(velocity.x)*90;
            CharacterModel.transform.eulerAngles = rotation;
            if (_spaceCollisionsController.collisionsInfo.above || _spaceCollisionsController.collisionsInfo.below)
            {
                velocity.y = 0;
            }
        }

        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }

        public void OnUserJump()
        {
            if (wallSliding)
            {
                if (wallDirX == directionalInput.x )
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if(directionalInput.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (_spaceCollisionsController.collisionsInfo.below)
            {
                velocity.y = maxJumpVelocity;
            }
        }

        private void HandleWallSliding()
        {
            wallDirX = (_spaceCollisionsController.collisionsInfo.left) ? -1 : 1;
            wallSliding = false;
            if ((_spaceCollisionsController.collisionsInfo.left || _spaceCollisionsController.collisionsInfo.right) && !_spaceCollisionsController.collisionsInfo.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (directionalInput.x != wallDirX && directionalInput.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }

            }

        }

        void CalculateVelocity()
        {
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (_spaceCollisionsController.collisionsInfo.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }

        private void OnDestroy()
        {
            InputManager.Instance.OnUserTapDown -= OnUserJump;
        }
    }
}
