using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;
    float xRotation = 0f;
    float yRotation = 0f;
    public float topClamp = -90f;
    public float bottomClamp = 90f; // Adjusted bottomClamp value

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp); // Adjusted the clamping logic

        yRotation += mouseX;

        // Apply the rotation to the GameObject's transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}