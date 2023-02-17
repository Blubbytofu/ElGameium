using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody playerRb;
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private Transform orientation;

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
        private RaycastHit groundHit;
        [SerializeField] private float groundHitLength;
        public bool isGrounded { get; private set; }

        private int vInput;
        private int hInput;

        [Header("Jumping")]
        private bool jumpInput;
        [SerializeField] private float jumpForce;

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
            if ((Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space)) && isGrounded && !jumpInput)
            {
                jumpInput = true;
            }
            else
            {
                jumpInput = false;
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

            Vector3 moveD = vInput * orientation.transform.forward + hInput * orientation.transform.right;
            moveD.Normalize();

            float sideSpeed = Mathf.Sqrt(playerRb.velocity.x * playerRb.velocity.x + playerRb.velocity.z * playerRb.velocity.z);

            if (sideSpeed < maxAirVel)
            {
                playerRb.AddForce(airSpeed * moveD, ForceMode.VelocityChange);
            }
            else
            {
                playerRb.AddForce(backwardAirSpeed * -moveD, ForceMode.Impulse);
            }

            Debug.Log(sideSpeed);
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

            Vector3 moveD = vInput * orientation.transform.forward + hInput * orientation.transform.right;
            moveD = Vector3.ProjectOnPlane(moveD, groundHit.normal);
            moveD.Normalize();

            if (playerRb.velocity.magnitude < maxGroundedVel)
            {
                playerRb.velocity += (maxGroundedVel - playerRb.velocity.magnitude) * moveD;
            }

            Debug.Log(playerRb.velocity.magnitude);
        }

        private void Jumping()
        {
            if (jumpInput)
            {
                jumpInput = false;
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
                playerRb.AddForce(jumpForce * orientation.transform.up, ForceMode.Impulse);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orientation.position, orientation.position - new Vector3(0, groundHitLength, 0));
        }
    }
}