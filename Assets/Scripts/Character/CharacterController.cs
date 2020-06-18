using UnityEngine;
using System.Collections;
using System;

namespace Zeptolab
{
    [RequireComponent(typeof(SpaceCollisionsController))]
    public class CharacterController : MonoBehaviour
    {
        public GameObject CharacterModel;
        Animation _characterAnimation;
        //TODO: pass some of that character behaviour vars to the GameConfig ScriptableObject settings 
        //      in order to have the possibility of set different difficulties for each level

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
        private bool _gameEnded;

        private void Awake()
        {
            InputManager.Instance.OnUserTapDown += OnUserJump;
            InputManager.Instance.SetDirectionalInput += SetDirectionalInput;
            GamePlayManager.Instance.OnGameEnd += OnGameEnd;

            _characterAnimation = CharacterModel.GetComponent<Animation>();
        }

        private void OnGameEnd(bool win)
        {
            Debug.Log("OnGameEnd");
            _gameEnded = true;
            wallSliding = false;

            if (win)
            {
                _characterAnimation.Play(Constants.WinAnimationName);
            }
            else
            {
                _characterAnimation.Play(Constants.GameOverAnimationName);
            }
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
            if (_gameEnded) return;

            CalculateVelocity();
            HandleWallSliding();
            _spaceCollisionsController.Move(velocity * Time.deltaTime, directionalInput);

            if (!wallSliding)
            {
                Vector3 rotation = CharacterModel.transform.eulerAngles;
                float sign = Mathf.Sign(velocity.x);
                rotation.y = sign * 90;
                CharacterModel.transform.eulerAngles = rotation;
           
                SetDirectionalInput(sign*Vector3.right * RightSpeed);
            }

            if (_spaceCollisionsController.collisionsInfo.above || _spaceCollisionsController.collisionsInfo.below)
            {
                velocity.y = 0;
            }

            if(_spaceCollisionsController.collisionsInfo.right && _spaceCollisionsController.collisionsInfo.below || _spaceCollisionsController.collisionsInfo.left && _spaceCollisionsController.collisionsInfo.below)
            {
                velocity.x = 0;
                _characterAnimation.Play(Constants.IdleAnimationName);
            }else
            {
                _characterAnimation.Play(Constants.WalkAnimationName);
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
                Vector3 rotation = CharacterModel.transform.eulerAngles; 
                if (_spaceCollisionsController.collisionsInfo.left) 
                {
                    rotation.y = - 180;
                    _characterAnimation.Play(Constants.WallLeftAnimationName); 

                }else{
                    rotation.y = 180;
                    _characterAnimation.Play(Constants.WallRightAnimationName);
                }
                CharacterModel.transform.eulerAngles = rotation;

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
