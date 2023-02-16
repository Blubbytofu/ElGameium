using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject orientation;
    [SerializeField] private GameObject cameraPosition;
    [SerializeField] private GameObject playerCamera;

    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;

    public float xSensitivity = 2f;
    public float ySensitivity = 2f;

    private void Start()
    {

    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        playerCamera.transform.position = cameraPosition.transform.position;

        MouseLook();
    }

    private void MouseLook()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * xSensitivity;
        xRotation -= mouseY * ySensitivity;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
