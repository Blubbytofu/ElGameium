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
        [SerializeField] private Transform orientation;
        [SerializeField] private PlayerCamera playerCameraScript;

        [Header("Grounded")]
        [SerializeField] private float maxGroundedVel;
        [SerializeField] private float groundDrag;

        [Header("In The Air")]
        [SerializeField] private float gravityMagnitude;
        [SerializeField] private float maxAirVel;
        [SerializeField] private float airSpeed;
        [SerializeField] private float airDrag;
        [SerializeField] private float backwardAirSpeed;
        [SerializeField] private float bHopMultiplier;
        private bool bHopping;

        [Header("Environment Detection")]
        [SerializeField] private float groundHitLength;
        private RaycastHit groundHit;
        public bool isGrounded { get; private set; }

        private int vInput;
        private int hInput;

        [Header("Jumping")]
        [SerializeField] private float jumpForce;
        private bool jumpInput;

        void Start()
        {
            Physics.gravity = new Vector3(0, -gravityMagnitude, 0);
        }


        void Update()
        {
            isGrounded = Physics.Raycast(orientation.position, -orientation.up, out groundHit, groundHitLength);

            GetInput();
        }

        private void FixedUpdate()
        {
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
            if ( Input.GetKeyDown(KeyCode.Space) && isGrounded && !jumpInput)
            {
                jumpInput = true;
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

            if ((playerCameraScript.mouseX > 0 && hInput > 0 && vInput == 0)
            || (playerCameraScript.mouseX < 0 && hInput < 0 && vInput == 0))
            {
                //bHopping = true;
            }
            else
            {
                //bHopping = false;
            }

            float sideSpeed = Mathf.Sqrt(playerRb.velocity.x * playerRb.velocity.x + playerRb.velocity.z * playerRb.velocity.z);

            if (bHopping)
            {
                Vector3 moveD = orientation.forward + hInput * orientation.right;
                moveD.Normalize();

                if (sideSpeed < maxAirVel * bHopMultiplier)
                {
                    playerRb.AddForce(airSpeed * moveD, ForceMode.VelocityChange);
                }
                else
                {
                    playerRb.AddForce(backwardAirSpeed * -moveD, ForceMode.Impulse);
                }
            }
            else
            {
                Vector3 moveD = vInput * orientation.forward + hInput * orientation.right;
                moveD.Normalize();

                if (sideSpeed < maxAirVel)
                {
                    playerRb.AddForce(airSpeed * moveD, ForceMode.VelocityChange);
                }
                else
                {
                    playerRb.AddForce(backwardAirSpeed * -moveD, ForceMode.Impulse);
                }
            }

            //Debug.Log(sideSpeed);
        }

        private void MoveGround()
        {
            playerRb.useGravity = false;
            if (jumpInput)
            {
                playerRb.drag = airDrag;
            }
            else
            {
                playerRb.drag = groundDrag;
            }

            Vector3 moveD = vInput * orientation.forward + hInput * orientation.right;
            moveD = Vector3.ProjectOnPlane(moveD, groundHit.normal);
            moveD.Normalize();

            if (playerRb.velocity.magnitude < maxGroundedVel)
            {
                playerRb.velocity += (maxGroundedVel - playerRb.velocity.magnitude) * moveD;
            }

            //Debug.Log(playerRb.velocity.magnitude);
        }

        private void Jumping()
        {
            if (jumpInput)
            {
                jumpInput = false;
                playerRb.velocity.ReplaceField(newY: 0);
                playerRb.AddForce(jumpForce * orientation.up, ForceMode.Impulse);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orientation.position, orientation.position - new Vector3(0, groundHitLength, 0));
        }
    }
}