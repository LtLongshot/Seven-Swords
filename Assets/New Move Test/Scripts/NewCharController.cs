﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.StateMchn;
using TMPro;
using SevenSwords.Utility;

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

        public float maxClimbAngle = 70; //intrinsic
        public float maxDecendAngle = 70; //intrinsic

        //General Movement
        public Vector3 velocity = new Vector3(0, 0, 0);

        public bool isRight = true;

        public int floorMask = 1 << 8;//intrinsic

        public bool hasJumped = false;

        public Vector3 frameIntialVel;
        public Vector3 hitboxCreationPos = new Vector3(0.16f,0,0);
        public Vector3 hitboxCreationNeg = new Vector3(-0.16f, 0, 0);
    }
    public class NewCharController : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();
        public StateMachine _stateMachine { get { return stateMachine; } }

        private CharVariables charVariables = new CharVariables();
        public CharVariables _charVariables { get { return charVariables; } set { charVariables = value; } }

        public CharacterManager manager; //maybe change to something else for inheretance

        // Start is called before the first frame update
        void Start()
        {
            collisions = GetComponent<BoxCollider2D>();
            calculateRaySpacing();
            _stateMachine.Start();
            _stateMachine.ChangeState(new Idle(this));
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

            if (_charVariables.velocity.y != 0)
                verticalCollisions(ref _charVariables.velocity);

            if (_charVariables.velocity.x != 0)
            horizontalCollisions(ref _charVariables.velocity);

            if (_charVariables.velocity.y < -0.0001f)
            {
                DescendSlope(ref _charVariables.velocity);
            }

            resolveMovement();
            DebugTools();
        }
        void Update()
        {
            _stateMachine.InputRead();
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

               // ceiling = grounded = false;

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
            float rayLength = Mathf.Abs((velocity.y)/10 + skinWidth);
            int groundCount = 0, ceilingCount = 0;

            for (int i = 0; i < verticalRayCount; i++)
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

                    //slope climbing collisions if there is a collision above while climbing slope
                    //TODO GET TO ACTUALLY WORKING
                    if (collisionInfo.climbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }


                    //detect grounded
                    if (collisionInfo.below && rayLength <= skinWidth + 0.01) //Skinwidth + buffer
                    {
                        groundCount++;
                    }
                    else if (collisionInfo.above && rayLength <= skinWidth + 0.01)
                    {
                        ceilingCount++;
                        Debug.Log("Celing");
                    }
                }
            }

            //Ground collision testing (need to refactor)
            if (groundCount > 0)
                collisionInfo.grounded = true;
            else
            {
                collisionInfo.grounded = false;
            }

                if (ceilingCount > 0)
                collisionInfo.ceiling = true;
            else
                collisionInfo.ceiling = false;
        }

        void horizontalCollisions (ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs((velocity.x)/10 + skinWidth) ;

            for (int i=0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _charVariables.floorMask);

                //debug
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (hit && velocity.x != 0f)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= _charVariables.maxClimbAngle)
                    {
                        float distanceToSlopeStart = 0;
                        if (slopeAngle != collisionInfo.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            velocity.x -= distanceToSlopeStart * directionX;
                            //Debug.Log(distanceToSlopeStart);
                        }
                        if(distanceToSlopeStart <= skinWidth && collisionInfo.grounded)
                        ClimbSlope(ref _charVariables.velocity, slopeAngle);
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    if ( slopeAngle > _charVariables.maxClimbAngle)//!collisionInfo.climbingSlope ||
                    {

                        rayLength = hit.distance;
                        collisionInfo.left = directionX == -1;
                        collisionInfo.right = directionX == 1;
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        if (collisionInfo.climbingSlope)
                            velocity.y = Mathf.Tan((collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x));

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

            //TODO FIX THIS IS DETECTING ANGLE CHANGES WHEN ON A SLOPE
            ////slope climb smoothing (for lower angles changing into larger angles)
            //if (collisionInfo.climbingSlope)
            //{
            //    rayLength = Mathf.Abs(velocity.x) + skinWidth;
            //    Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft) + Vector2.up * velocity.y;
            //    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _charVariables.floorMask);
            //    if (hit)
            //    {
            //        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            //        if (slopeAngle != collisionInfo.slopeAngle)
            //        {
            //            velocity.x = (hit.distance - skinWidth) * directionX;
            //            collisionInfo.slopeAngle = slopeAngle;
            //            Debug.Log("Angle Change");
            //        }
            //    }
            //}
        }

        void ClimbSlope(ref Vector3 velocity, float slopeAngle)
        {
            //Debug.Log("Climbing");
            float moveDistance = Mathf.Abs(velocity.x);
            float climbVelY = (Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance);
            if (velocity.y <= climbVelY)
            {
                velocity.y = climbVelY - (_charVariables.frameIntialVel.y * Time.deltaTime);
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                collisionInfo.below = true;
                collisionInfo.climbingSlope = true;
                collisionInfo.slopeAngle = slopeAngle;
            }
        }

        void DescendSlope(ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, _charVariables.floorMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= _charVariables.maxDecendAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if(hit.distance - skinWidth <= (Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))/10)
                        {
                            //Debug.Log("Descending Slope");
                            float moveDistance = Mathf.Abs(velocity.x);
                            float decVelY = (Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance);

                            velocity.x = (((Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance)+1f) * Mathf.Sign(velocity.x));
                            velocity.y -= decVelY;

                            collisionInfo.slopeAngle = slopeAngle;
                            collisionInfo.descendingSlope = true;
                            collisionInfo.below = true;
                            collisionInfo.grounded = true;
                        }
                    }
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

            if (!collisionInfo.climbingSlope || !collisionInfo.descendingSlope)
                transform.position += _charVariables.velocity * Time.deltaTime + accelleration * (Time.deltaTime * Time.deltaTime * 0.5f);
            else
                transform.position += _charVariables.velocity * Time.deltaTime;


            _charVariables.velocity += (accelleration * (Time.deltaTime/2)) * Time.deltaTime;
        }

		public void setJumpValues(float jumpHeight, float jumpApexTime)
        {
            _charVariables.gravity = -(jumpHeight) / Mathf.Pow(jumpApexTime, 2);
            _charVariables.jumpVel = Mathf.Abs(_charVariables.gravity) * jumpApexTime;
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
            if (stateDebug != null)
                stateDebug.GetComponent<TextMeshProUGUI>().SetText("Current State: " + _stateMachine.currentState);
            if (velocityDebug != null)
                velocityDebug.GetComponent<TextMeshProUGUI>().SetText("Velocity: " + _charVariables.velocity);
            if (velocityDebug != null)
                gravityDebug.GetComponent<TextMeshProUGUI>().SetText("Gravity: " + _charVariables.gravity * Time.deltaTime);

        }
        #endregion

    }
}
