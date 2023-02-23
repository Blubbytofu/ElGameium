using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

namespace PlayerObject
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody playerRb;
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private Transform orientation;
        [SerializeField] private LayerMask environmentMask;

        [Header("Walking")]
        [SerializeField] private float walkingVelMultiplier;
        private bool walkingInput;
        public int vInput { get; private set; }
        public int hInput { get; private set; }

        [Header("Grounded")]
        [SerializeField] private float maxGroundedVel;
        [SerializeField] private float groundDrag;

        [Header("In The Air")]
        [SerializeField] private float gravityMagnitude;
        [SerializeField] private float maxAirVel;
        [SerializeField] private float airSpeed;
        [SerializeField] private float airDrag;
        [SerializeField] private float backwardAirSpeed;

        [Header("Environment Detection")]
        [SerializeField] private float groundHitLength;
        private RaycastHit groundHit;
        [SerializeField] private bool isGrounded;

        [Header("Jumping")]
        [SerializeField] private float jumpForce;
        private bool jumpInput;

        [Header("Crouching")]
        [SerializeField] private float crouchHeight;
        [SerializeField] private float normalHeight;
        [SerializeField] private float groundedCrouchForce;
        [SerializeField] private float crouchSpeedMultiplier;
        [SerializeField] private float crouchLerpSpeed;
        [SerializeField] private float headClearanceRadius;
        [SerializeField] private bool crouchInput;
        [SerializeField] private bool groundedCrouch;

        private enum MovementState
        {
            GROUNDED,
            AIR,
            LADDER,
            WATER,
            NOCLIP,
            STUNNED
        }

        private MovementState movementState;

        private void Start()
        {
            Physics.gravity = Physics.gravity.ReplaceField(newY: -gravityMagnitude);
        }


        private void Update()
        {
            groundHitLength = playerCollider.height * 0.5f * transform.localScale.y + 0.1f;
            isGrounded = Physics.Raycast(orientation.position, -orientation.up, out groundHit, groundHitLength);

            GetInput();

            DetermineMovementState();
        }

        private void FixedUpdate()
        {
            SwitchMovementState();
        }

        private void DetermineMovementState()
        {
            if (isGrounded)
            {
                movementState = MovementState.GROUNDED;
            }
            else
            {
                movementState = MovementState.AIR;
            }
        }

        private void SwitchMovementState()
        {
            switch (movementState)
            {
                case MovementState.GROUNDED:
                    MoveGround();
                    Jumping();
                    Crouching();
                    break;
                case MovementState.AIR:
                    MoveAir();
                    Crouching();
                    break;
                case MovementState.LADDER:
                    break;
                case MovementState.WATER:
                    break;
                case MovementState.STUNNED:
                    break;
            }
        }

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !jumpInput)
            {
                jumpInput = true;
            }

            if (Input.GetKey(KeyCode.Space) && !jumpInput && groundedCrouch)
            {
                groundedCrouch = false;
                jumpInput = true;
                Jumping();
            }

            if (Input.GetKey(KeyCode.LeftShift) && !crouchInput)
            {
                walkingInput = true;
            }
            else
            {
                walkingInput = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (isGrounded)
                {
                    groundedCrouch = true;
                }

                crouchInput = true;
            }

            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (!Physics.CheckSphere(orientation.position + normalHeight * orientation.up, headClearanceRadius, environmentMask))
                {
                    crouchInput = false;
                    groundedCrouch = false;
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                vInput = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                vInput = -1;
            }
            else
            {
                vInput = 0;
            }

            if (Input.GetKey(KeyCode.D))
            {
                hInput = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                hInput = -1;
            }
            else
            {
                hInput = 0;
            }
        }

        private void MoveAir()
        {
            playerRb.useGravity = true;
            playerRb.drag = airDrag;

            float sideSpeed = Mathf.Sqrt(playerRb.velocity.x * playerRb.velocity.x + playerRb.velocity.z * playerRb.velocity.z);

            float maxVel = walkingInput ? maxAirVel * walkingVelMultiplier : (crouchInput ? maxAirVel * crouchSpeedMultiplier : maxAirVel);

            Vector3 moveD = vInput * orientation.forward + hInput * orientation.right;
            moveD.Normalize();

            if (sideSpeed < maxVel)
            {
                playerRb.AddForce(airSpeed * moveD, ForceMode.VelocityChange);
            }
            else
            {
                playerRb.AddForce(backwardAirSpeed * -moveD, ForceMode.Impulse);
            }

            //Debug.Log(sideSpeed);
        }

        private void MoveGround()
        {
            playerRb.useGravity = false;
            playerRb.drag = groundDrag;

            Vector3 moveD = vInput * orientation.forward + hInput * orientation.right;
            moveD = Vector3.ProjectOnPlane(moveD, groundHit.normal);
            moveD.Normalize();

            float maxVel = walkingInput ? maxGroundedVel * walkingVelMultiplier : (crouchInput ? maxGroundedVel * crouchSpeedMultiplier : maxGroundedVel);

            if (playerRb.velocity.magnitude < maxVel)
            {
                playerRb.velocity += (maxVel - playerRb.velocity.magnitude) * moveD;
            }

            //Debug.Log(playerRb.velocity.magnitude);
        }

        private void Jumping()
        {
            if (jumpInput)
            {
                jumpInput = false;
                playerRb.velocity = playerRb.velocity.ReplaceField(newY: 0);

                playerRb.AddForce(jumpForce * orientation.up, ForceMode.Impulse);
            }
        }

        private void Crouching()
        {
            if (crouchInput)
            {
                playerCollider.height = Mathf.Lerp(playerCollider.height, 1, crouchLerpSpeed * Time.deltaTime);
                cameraPosition.localPosition = cameraPosition.localPosition.ReplaceField(newY: Mathf.Lerp(cameraPosition.localPosition.y, 0.22f, crouchLerpSpeed * Time.deltaTime));

                if (groundedCrouch)
                {
                    if (playerCollider.height < 1.01f)
                    {
                        groundedCrouch = false;
                    }
                    else if(!isGrounded)
                    {
                        playerRb.AddForce(groundedCrouchForce * -orientation.up, ForceMode.Impulse);
                    }
                }
            }
            else
            {
                playerCollider.height = Mathf.Lerp(playerCollider.height, 2, crouchLerpSpeed * Time.deltaTime);
                cameraPosition.localPosition = cameraPosition.localPosition.ReplaceField(newY: Mathf.Lerp(cameraPosition.localPosition.y, 0.67f, crouchLerpSpeed * Time.deltaTime));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orientation.position, orientation.position - new Vector3(0, groundHitLength, 0));
            Gizmos.DrawWireSphere(orientation.position + normalHeight * orientation.up, headClearanceRadius);
        }
    }
}