using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using TMPro;
using SevenSwords.StateMchn;

namespace SevenSwords.CharacterCore
{
    [SerializeField]
    public class CharacterVariables
    {

        //Jumping stuff
        public float gravity = -9f;
        public float distToGround = 0.02f;
        public float gravTime = 0.1f;

        public float jumpVel = 20;

        //General Movement
        public Vector3 velocity = new Vector3(0, 0, 0);
    }

    public class CharController : MonoBehaviour
    {
        //Player controller init
        //later move to character input system to organise buffering
        public int playerID = 0;
        public Rewired.Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(playerID) : null; } }

        //LayerMasks
        private int floorMask = 1 << 8;

        private StateMachine stateMachine = new StateMachine();
        public StateMachine _stateMachine { get { return stateMachine; } }

        private CharacterVariables characterVariables = new CharacterVariables();
        public CharacterVariables _moveVar { get { return characterVariables; } set { characterVariables = value; } }


        // Start is called before the first frame update
        void Start()
        {
            _stateMachine.ChangeState(new Idle(this));
            collisions = GetComponent<BoxCollider2D>();
            calculateRaySpacing();
        }

        #region Collision
        //Collision
        private BoxCollider2D collisions;
        RaycastOrigins raycastOrigins;
        const float skinWidth = 0.015f;

        public int horizontalRayCount = 4;
        public int verticalRayCount = 4;

        float horizontalRaySpacing;
        float verticalRaySpacing;

        public CollisionInfo collisionInfo;

        struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        void verticalCollisions(ref Vector3 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs((velocity.y) + skinWidth) / 20;

            int groundedCheck = 0;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, floorMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green);

                if (hit)
                {
                    velocity.y = (((hit.distance - skinWidth) * directionY) * 20);
                    rayLength = hit.distance;

                    //cleaner collision range for checks
                    if (rayLength <= 0.02f)
                    {
                        collisionInfo.below = directionY == -1;
                        collisionInfo.above = directionY == 1;
                    }
                }
                //checks how many rays are not touching the ground when in the air
                else if (!hit && _stateMachine.currentState.ToString() != "Air")
                {
                    groundedCheck++;

                }
            }
            //GROUND CHECK
            if (groundedCheck == verticalRayCount)
                _stateMachine.ChangeState(new Air(this));
        }

        //Collisions for horizontal surfaces
        void horizontalCollisions(ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) / 20 + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, floorMask);

                //debug
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (hit)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX * 20;
                    rayLength = hit.distance;

                    //cleaner collision range for checks
                    if (rayLength <= 0.01f)
                    {
                        collisionInfo.left = directionX == -1;
                        collisionInfo.right = directionX == 1;
                    }
                }
            }
        }

        //collision info (where stuff is)
        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public void Reset()
            {
                above = below = false;
                left = right = false;
            }
        }

        //update where the raycasts start
        void updateRaycastOrigins()
        {
            Bounds bounds = collisions.bounds;
            bounds.Expand(skinWidth * -2);

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        //spaacing of ray from eachother
        void calculateRaySpacing()
        {
            Bounds bounds = collisions.bounds;
            bounds.Expand(skinWidth * -2);

            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        #endregion


        //input checks done in here
        private void Update()
        {
            _stateMachine.Update();
            resolveMovement();

            DebugTools();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

        }

        void resolveMovement()
        {
            updateRaycastOrigins();
            collisionInfo.Reset();
            if (_moveVar.velocity.y != 0)
                verticalCollisions(ref _moveVar.velocity);
            if (_moveVar.velocity.x != 0)
                horizontalCollisions(ref _moveVar.velocity);
            transform.Translate(_moveVar.velocity * Time.deltaTime, Space.World);
        }

        #region Movement Checks
        private float currentXSpeed;
        public float _currentXSpeed { get { return currentXSpeed; } }
        public void horizontalMove(float xVel)
        {
            currentXSpeed = xVel;
            //check current state
            if(!(_stateMachine.currentState is Walk)&&!(_stateMachine.currentState is Air))
                _stateMachine.ChangeState(new Walk(this, _currentXSpeed));
        }

        public void setJumpValues(float jumpHeight, float jumpApexTime)
        {
            _moveVar.gravity = -(jumpHeight * 5) / Mathf.Pow(jumpApexTime, 2);
            _moveVar.jumpVel = Mathf.Abs(_moveVar.gravity) * jumpApexTime;
        }

        public void checkIdle()
        {
            if (!(_stateMachine.currentState is Idle)&&!(_stateMachine.currentState is Air))
                _stateMachine.ChangeState(new Idle(this));
        }

        public void Jump()
        {
            if (!(_stateMachine.currentState is Air))
            {
                _moveVar.velocity.y = _moveVar.jumpVel;
                _stateMachine.ChangeState(new Air(this));
            }
        }

        #endregion

        public enum BladeColour { white, green, red, blue };

        public struct HitboxData
        {
            public float damage;
            public float hitboxCreationTime;
            public float hitboxLingeringTime; //Time from frame it is changed to so creation time + Lingering time
            public Vector2 hitboxSize;
            public BladeColour colour;
        }

        #region Player Specifics
        public void PlayerBasicAttack(HitboxData hitbox)
        {
            _stateMachine.ChangeState(new PlayerAttack(this, hitbox));
            _stateMachine.LockState(hitbox.hitboxLingeringTime);
        }
        #endregion

        #region Debug Tools
        /// <summary>
        /// DEBUG TOOLS AND VARIABLES FOR UI LINKING
        /// </summary>

        public GameObject stateDebug;
        public GameObject velocityDebug;
        public GameObject gravityDebug;

        public void DebugTools()
        {
            stateDebug.GetComponent<TextMeshProUGUI>().SetText("Current State: " + _stateMachine.currentState);
            velocityDebug.GetComponent<TextMeshProUGUI>().SetText("Velocity: " + _moveVar.velocity);
            gravityDebug.GetComponent<TextMeshProUGUI>().SetText("Gravity: " + _moveVar.gravity * Time.deltaTime);

        }
        #endregion
    }
}