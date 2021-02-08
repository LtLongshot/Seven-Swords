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

        public float maxClimbAngle = 70;
        public float maxDecendAngle = 70;

        //General Movement
        public Vector3 velocity = new Vector3(0, 0, 0);

        public bool isRight = true;

        public Vector3 hitboxCreationPos = new Vector3(0.16f,0,0);
        public Vector3 hitboxCreationNeg = new Vector3(-0.16f, 0, 0);
    }

    public class CharController : MonoBehaviour
    {
        //LayerMasks
        private int floorMask = 1 << 8;

        public int enemyMask = 1 << 10;
        private int playerMask = 1 << 9;

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

                    if (collisionInfo.climbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }

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

            //multi slope check
            if (collisionInfo.climbingSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = (Mathf.Abs(velocity.x) + skinWidth)/20;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, floorMask);
                
                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if(slopeAngle != collisionInfo.slopeAngle)
                    {
                        velocity.x = (hit.distance - skinWidth * directionX);
                        collisionInfo.slopeAngle = slopeAngle;
                    }
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
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); //Slope movement
                    if (i == 0 && slopeAngle <= _moveVar.maxClimbAngle)
                    {
                        float distToSlopeStart = 0;
                        if(slopeAngle != collisionInfo.slopeAngleOld)
                        {
                            distToSlopeStart = hit.distance - skinWidth;
                            velocity.x -= distToSlopeStart * directionX;
                        }
                        ClimbSlope(ref velocity, slopeAngle);
                        //velocity.x += distToSlopeStart * directionX;
                    }

                    if (!collisionInfo.climbingSlope || slopeAngle > _moveVar.maxClimbAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        //vertical collision whist climbing slope
                        if (collisionInfo.climbingSlope)
                        {
                            velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }

                        //cleaner collision range for checks
                        if (rayLength <= 0.01f)
                        {
                            collisionInfo.left = directionX == -1;
                            collisionInfo.right = directionX == 1;
                        }
                    }
                }
            }
        }

        //collision info (where stuff is)
        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public float slopeAngle, slopeAngleOld;

            public bool descendingSlope;
            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;

                descendingSlope = false;

                slopeAngleOld = slopeAngle;

                slopeAngle = 0;
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

        void resolveMovement()
        {
            updateRaycastOrigins();
            collisionInfo.Reset();
            if(_moveVar.velocity.y < 0)
            {
                DescendSlope(ref _moveVar.velocity);
            }
            if (_moveVar.velocity.y != 0)
                verticalCollisions(ref _moveVar.velocity);
            if (_moveVar.velocity.x != 0)
                horizontalCollisions(ref _moveVar.velocity);
            IsRight();
            transform.Translate(_moveVar.velocity * Time.deltaTime, Space.World);
        }

        #region Movement Checks
        private float currentXSpeed;
        public float _currentXSpeed { get { return currentXSpeed; } }
        public void horizontalMove(float xVel)
        {
            currentXSpeed = xVel;
            //check current state
            if(!(_stateMachine.currentState is Walk)&&!(_stateMachine.currentState is Air)&&!(_stateMachine.currentState is ClimbSlope))
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

        void IsRight()
        {
            if (_moveVar.velocity.x > 0)
                _moveVar.isRight = true;
            else if (_moveVar.velocity.x < 0)
                _moveVar.isRight = false;
        }

        private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
        {
            _stateMachine.ChangeState(new ClimbSlope(this));
            float moveDistance = Mathf.Abs(velocity.x);
            float climbVelocityY= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            //slope jump check
            if (velocity.y <= climbVelocityY){
                velocity.y = climbVelocityY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                collisionInfo.below = true;
                collisionInfo.climbingSlope = true;
                collisionInfo.slopeAngle = slopeAngle;
            }
        }

        private void DescendSlope(ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, floorMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle !=0 && slopeAngle <= _moveVar.maxDecendAngle)
                {
                    if(Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if ((hit.distance - skinWidth) / 20 <= (Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))/200 ){
                            float moveDistance = Mathf.Abs(velocity.x);
                            float decVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                            velocity.y -= decVelocityY;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.below = true;
                        }
                    }
                }
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
            public float hitstun;
        }

        public GameObject HitboxCreationPoint;

        #region Player Specifics
        public void PlayerBasicAttack(HitboxData hitbox)
        {
            if (!(_stateMachine.currentState is PlayerAttack))
            {
                _stateMachine.ChangeState(new PlayerAttack(this, hitbox));
                _stateMachine.LockState(hitbox.hitboxLingeringTime);
            }
        }

        public Collider2D[] CreatePlayerHitbox(HitboxData hitbox)
        {
            //this is a lil jank for garbage collection
            //for right facing only ATM
            if(_moveVar.isRight)
                return Physics2D.OverlapBoxAll(transform.position + (_moveVar.hitboxCreationPos + new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize, 0, enemyMask);
            else
                return Physics2D.OverlapBoxAll(transform.position + (_moveVar.hitboxCreationNeg - new Vector3(hitbox.hitboxSize.x / 2, 0, 0)), hitbox.hitboxSize, 0, enemyMask);
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
            if(stateDebug != null)
            stateDebug.GetComponent<TextMeshProUGUI>().SetText("Current State: " + _stateMachine.currentState);
            if(velocityDebug != null)
            velocityDebug.GetComponent<TextMeshProUGUI>().SetText("Velocity: " + _moveVar.velocity);
            if(velocityDebug != null)
            gravityDebug.GetComponent<TextMeshProUGUI>().SetText("Gravity: " + _moveVar.gravity * Time.deltaTime);

        }
        #endregion
    }
}