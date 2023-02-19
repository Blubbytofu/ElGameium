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
        private int vInput;
        private int hInput;

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
        private bool crouchInput;
        private bool groundedCrouch;

        void Start()
        {
            Physics.gravity = Physics.gravity.ReplaceField(newY: -gravityMagnitude);
        }


        void Update()
        {
            groundHitLength = transform.localScale.y + 0.1f;
            isGrounded = Physics.Raycast(orientation.position, -orientation.up, out groundHit, groundHitLength);

            GetInput();
        }

        private void FixedUpdate()
        {
            Crouching();

            if (isGrounded)
            {
                MoveGround();
                Jumping();
            }
            else
            {
                MoveAir();
            }
        }

        void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !jumpInput)
            {
                jumpInput = true;
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
                if (Physics.Raycast(orientation.position, -orientation.up, normalHeight))
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
                //transform.localScale = transform.localScale.ReplaceField(newY: Mathf.Lerp(transform.localScale.y, crouchHeight, crouchLerpSpeed * Time.deltaTime));
                playerCollider.height = Mathf.Lerp(playerCollider.height, 1, crouchLerpSpeed * Time.deltaTime);
                cameraPosition.localPosition = new Vector3(0, 0.22f, 0);

                if (groundedCrouch)
                {
                    playerRb.AddForce(groundedCrouchForce * -orientation.up, ForceMode.Impulse);
                    if (isGrounded)
                    {
                        groundedCrouch = false;
                    }
                }
            }
            else
            {
                //transform.localScale = transform.localScale.ReplaceField(newY: normalHeight);
                playerCollider.height = Mathf.Lerp(playerCollider.height, 2, crouchLerpSpeed * Time.deltaTime);
                cameraPosition.localPosition = new Vector3(0, 0.67f, 0);
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