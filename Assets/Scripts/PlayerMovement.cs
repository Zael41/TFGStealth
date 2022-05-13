using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float sprintMultiplier = 1.5f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public Camera cam;

    private float fovTargetNormal = 60f;
    private float fovTargetSprint = 80f;
    Vector3 velocity;
    bool isGrounded;
    bool isSprinting;
    bool isCrouching;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "ExteriorScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        isSprinting = false;
        isCrouching = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown("left shift"))
        {
            isSprinting = !isSprinting;
        }

        if (Input.GetKeyDown("left ctrl"))
        {
            isCrouching = !isCrouching;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (isSprinting)
        {
            //cam.fieldOfView = 90f;
            if (cam.fieldOfView < fovTargetSprint)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTargetSprint, 10 * Time.deltaTime);
            }
            controller.Move(move * speed * Time.deltaTime * sprintMultiplier);
        }
        else
        {
            //cam.fieldOfView = 60f;
            if (cam.fieldOfView > fovTargetNormal)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTargetNormal, 10 * Time.deltaTime);
            }
            controller.Move(move * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
