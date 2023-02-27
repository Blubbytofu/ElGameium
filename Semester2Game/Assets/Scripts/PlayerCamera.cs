using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Camera primaryCamera;
        [SerializeField] private Camera secondaryCamera;

        private float mouseX;
        private float mouseY;
        private float xRotation;
        private float yRotation;

        [Header("Player Settings")]
        [SerializeField] private float xSensitivity = 2f;
        [SerializeField] private float ySensitivity = 2f;
        [SerializeField] private int FOV;
        [SerializeField] private int secondaryFOV;
        [HideInInspector] public float zoomFactor;

        [Header("Head Size")]
        [SerializeField] private float headRadius;
        public bool breathingWater { get; private set; }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            zoomFactor = 1;
        }

        private void Update()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, headRadius);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Water"))
                {
                    breathingWater = true;
                    break;
                }
                else
                {
                    breathingWater = false;
                }
            }

            primaryCamera.fieldOfView = FOV * (1.0f / zoomFactor);
            secondaryCamera.fieldOfView = secondaryFOV * (1.0f / zoomFactor);
        }

        private void LateUpdate()
        {
            playerCamera.position = cameraPosition.position;

            MouseLook();
        }

        private void MouseLook()
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * xSensitivity;
            xRotation -= mouseY * ySensitivity;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        public void SimulateRecoil(float x, float y)
        {
            yRotation += x;
            xRotation -= y;
        }

        public void UpdateAllFOV(int newFOV, int newSecondFOV)
        {
            FOV = newFOV;
            secondaryFOV = newSecondFOV;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, headRadius);
        }
    }
}