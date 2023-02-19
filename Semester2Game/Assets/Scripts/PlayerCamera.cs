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

        private float mouseX;
        private float mouseY;
        private float xRotation;
        private float yRotation;

        [Header("Player Settings")]
        [SerializeField] private float xSensitivity = 2f;
        [SerializeField] private float ySensitivity = 2f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
    }
}