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

        public float maxClimbAngle = 70;
        public float maxDecendAngle = 70;

        //General Movement
        public Vector3 velocity = new Vector3(3, -3, 0);

        public bool isRight = true;

        public int floorMask = 1 << 8;
        public int enemyMask = 1 << 10;

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
            //Collision Sim
            updateRaycastOrigins();
            collisionInfo.Reset();
            //physics
            if (collisionInfo.grounded || collisionInfo.ceiling)
                _charVariables.velocity.y = 0;
            else
                _charVariables.velocity.y = -9.8f;

            if (collisionInfo.wall)
            {
                _charVariables.velocity.x = 0;
                Debug.Log("Wall");
            }
            else
            {
                _charVariables.velocity.x = 3f;
                Debug.Log("No Wall");
            }

            //if (_charVariables.velocity.y !=0)
            verticalCollisions(ref _charVariables.velocity);
            horizontalCollisions(ref _charVariables.velocity);
        }
        void Update()
        {
            //input and state mchn
            //Resolve movement

            resolveMovement();
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
            float rayLength = Mathf.Abs((velocity.y) + skinWidth)/5;

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
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    //detect grounded
                    if (collisionInfo.below && rayLength <= skinWidth + 0.001) //Skinwidth + buffer
                    {
                        collisionInfo.grounded = true;
                        //grounded
                        Debug.Log("Grounded");
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
                    velocity.x = (hit.distance - skinWidth) * directionX * 5;
                    if (rayLength <= skinWidth + 0.001)
                    {
                        collisionInfo.wall = true;
                    }
                    else
                    {
                        collisionInfo.wall = false;
                    }
                }
                else
                {
                    collisionInfo.wall = false;
                }
            }
        }

        #endregion

        #region Movement

        void resolveMovement()
        {
            //if(collisionInfo.left || collisionInfo.right)
            //    _charVariables.velocity.x = 0;
            transform.Translate(_charVariables.velocity * Time.deltaTime, Space.World);
        }
		#endregion
	}
}
