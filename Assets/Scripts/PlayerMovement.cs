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
    public float crouchMultiplier = 0.5f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public Camera cam;

    private float fovTargetNormal = 60f;
    private float fovTargetSprint = 80f;
    private float normalPlayerHeight = 1.75f;
    private float crouchedPlayerHeight = 0.5f;
    private float interiorScaleFactor = 1f;
    Vector3 velocity;
    Vector3 fixHeight;
    bool isGrounded;
    bool isSprinting;
    bool isCrouching;
    bool disabled;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "ExteriorScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            interiorScaleFactor = 0.8f;
        }
        isSprinting = false;
        isCrouching = false;
        EnemyNavMesh.OnGuardHasSpottedPlayer += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = Vector3.zero;
        if (!disabled)
        {
            move = transform.right * x + transform.forward * z;
            SprintControl(move);
            CrouchControl();
            MovementControl(move);
            JumpControl();
        }
        
        /*SprintControl(move);
        CrouchControl();
        MovementControl(move);
        JumpControl();*/
    }

    void CrouchControl()
    {
        if (Input.GetKeyDown("left ctrl"))
        {
            isCrouching = !isCrouching;
            fixHeight = new Vector3(0f, 0.625f * interiorScaleFactor, 0f);
            if (isCrouching) groundCheck.position += fixHeight;
            else groundCheck.position -= fixHeight;
        }

        if (isCrouching)
        {
            isSprinting = false;
            if (controller.height > crouchedPlayerHeight)
            {
                controller.height = Mathf.Lerp(controller.height, crouchedPlayerHeight, 10 * Time.deltaTime);
            }
        }
        else
        {
            if (controller.height < normalPlayerHeight)
            {
                controller.height = Mathf.Lerp(controller.height, normalPlayerHeight, 10 * Time.deltaTime);
            }
        }
    }

    void SprintControl(Vector3 move)
    {
        if (Input.GetKeyDown("left shift"))
        {
            isSprinting = !isSprinting;
        }

        if (move.x == 0 && move.z == 0)
        {
            isSprinting = false;
        }

        if (!isGrounded)
        {
            isSprinting = false;
        }
    }

    void MovementControl(Vector3 move)
    {
        if (isSprinting && isGrounded)
        {
            //cam.fieldOfView = 90f;
            if (cam.fieldOfView < fovTargetSprint)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTargetSprint, 10 * Time.deltaTime);
            }
            controller.Move(move * speed * Time.deltaTime * sprintMultiplier);
        }
        else if (isCrouching && isGrounded)
        {
            //cam.fieldOfView = 60f;
            if (cam.fieldOfView > fovTargetNormal)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTargetNormal, 10 * Time.deltaTime);
            }
            controller.Move(move * speed * Time.deltaTime * crouchMultiplier);
        }
        else
        {
            if (cam.fieldOfView > fovTargetNormal)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTargetNormal, 10 * Time.deltaTime);
            }
            controller.Move(move * speed * Time.deltaTime);
        }
    }

    void JumpControl()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Disable()
    {
        disabled = true;
    }

    public void TransitionDisable()
    {
        disabled = !disabled;
    }

    void OnDestroy()
    {
        EnemyNavMesh.OnGuardHasSpottedPlayer -= Disable;
    }
}
