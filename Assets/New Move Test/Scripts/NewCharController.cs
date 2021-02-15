using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.StateMchn;

namespace SevenSwords.CharacterCore
{
    [SerializeField]
    public class CharVariables
    {
        //Jumping stuff
        public float distToGround = 0.02f;
        public float gravTime = 0.1f;

        public float jumpVel = 20;

        public float gravity = -9.8f;

        public float maxClimbAngle = 70;
        public float maxDecendAngle = 70;

        //General Movement
        public Vector3 velocity = new Vector3(0, 0, 0);

        public bool isRight = true;

        public int floorMask = 1 << 8;
        public int enemyMask = 1 << 10;


        public Vector3 frameIntialVel;
    }
    public class NewCharController : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();
        public StateMachine _stateMachine { get { return stateMachine; } }

        private CharVariables charVariables = new CharVariables();
        public CharVariables _charVariables { get { return charVariables; } set { charVariables = value; } }

        // Start is called before the first frame update
        void Start()
        {
            collisions = GetComponent<BoxCollider2D>();
            calculateRaySpacing();
        }
        // Update is called once per frame

        private void FixedUpdate()
        {
            _charVariables.frameIntialVel = _charVariables.velocity;
            //Collision Sim
            updateRaycastOrigins();
            collisionInfo.Reset();

            //inputs and state mchn
            _stateMachine.Update();

            //if (_charVariables.velocity.y !=0)
            verticalCollisions(ref _charVariables.velocity);
            if(_charVariables.velocity.x != 0)
            horizontalCollisions(ref _charVariables.velocity);
            Debug.Log(collisionInfo.grounded);
            resolveMovement();
        }
        void Update()
        {

            
        }

        #region Collisions
        private BoxCollider2D collisions;
        RaycastOrigins raycastOrigins;
        struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        const float skinWidth = 0.05f;
        float verticalRaySpacing;
        float horizontalRaySpacing;

        public CollisionInfo collisionInfo;

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public float slopeAngle, slopeAngleOld;

            public bool grounded;
            public bool ceiling;

            public bool wall;

            public bool descendingSlope;

            public float distToWall;
            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;

                descendingSlope = false;

                slopeAngleOld = slopeAngle;

                slopeAngle = 0;

                distToWall = 10;//make it max speed
            }
        }

        public int verticalRayCount = 4;
        public int horizontalRayCount = 4;
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

        void verticalCollisions(ref Vector3 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs((velocity.y) + skinWidth)/10;

            for (int i=0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _charVariables.floorMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green);

                if (hit)
                {
                    rayLength = hit.distance;

                    collisionInfo.below = directionY == -1;
                    collisionInfo.above = directionY == 1;
                    velocity.y = (hit.distance - skinWidth) * directionY * 10;
                    //detect grounded
                    if (collisionInfo.below && rayLength <= skinWidth + 0.001) //Skinwidth + buffer
                    {
                        collisionInfo.grounded = true;
                        //grounded
                    }
                    else if(collisionInfo.above && rayLength <= skinWidth + 0.001)
                    {
                        collisionInfo.ceiling = true;
                        Debug.Log("Celing");
                    }
                    else
                    {
                        collisionInfo.grounded = false;
                        collisionInfo.ceiling = false;
                    }

                }
                else
                {
                    collisionInfo.ceiling = false;
                    collisionInfo.grounded = false;
                }
            }
        }

        void horizontalCollisions (ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs((velocity.x) + skinWidth) / 5;

            for (int i=0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _charVariables.floorMask);

                //debug
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (hit)
                {
                    rayLength = hit.distance;
                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                    velocity.x = (hit.distance - skinWidth) * directionX*4;
                    if (rayLength <= skinWidth + 0.001)
                    {
                        collisionInfo.wall = true;
                    }
                    else
                    {
                        collisionInfo.wall = false;
                    }
                    collisionInfo.distToWall = hit.distance;
                }
                else
                {
                    collisionInfo.wall = false;
                }
            }
        }

        #endregion

        #region Movement
        private float currentXSpeed;
        public float _currentXSpeed { get { return currentXSpeed; } }
        void resolveMovement()
        {
            Vector3 accelleration;
            accelleration = ((_charVariables.frameIntialVel - _charVariables.velocity) / Time.deltaTime);

            transform.position += Time.deltaTime * (_charVariables.velocity + Time.deltaTime * accelleration / 2);

            //_charVariables.velocity += accelleration * (Time.deltaTime);
        }

        public void horizontalMove(float xVel)
        {
            currentXSpeed = xVel;
            if(!(_stateMachine.currentState is Walk) &&!(_stateMachine.currentState is Air))
            {
                _stateMachine.ChangeState(new Walk(this, _currentXSpeed));
            }
        }

        public void checkIdle()
        {
            if(!(_stateMachine.currentState is Idle) && !(_stateMachine.currentState is Air))
            {
                _stateMachine.ChangeState(new Idle(this));
            }
        }

        public void setJumpValues(float jumpHeight, float jumpApexTime)
        {
            _charVariables.gravity = -(jumpHeight) / Mathf.Pow(jumpApexTime, 2);
            _charVariables.jumpVel = Mathf.Abs(_charVariables.gravity) * jumpApexTime;
        }

        public void jump()
        {
            if (!(_stateMachine.currentState is Air))
            {
                _charVariables.velocity.y = _charVariables.jumpVel;
                _stateMachine.ChangeState(new Air(this));
            }
        }
        #endregion
    }
}
