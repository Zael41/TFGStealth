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
    public float detectionRange;

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
    SpawnController spawnController;
    public AudioSource[] audioSource;
    bool checkFootsteps;
    bool playFall;

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
        //audioSource = GetComponents<AudioSource>();
        StartCoroutine(Footsteps());
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && playFall)
        {
            audioSource[2].Play();
            playFall = false;
        }
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
            if (move == Vector3.zero) checkFootsteps = false;
            else checkFootsteps = true;
            SprintControl(move);
            CrouchControl();
            MovementControl(move);
            JumpControl();
            DetectionControl();
        }
        else if (spawnController == null)
        {
            spawnController = GameObject.Find("SpawnController").GetComponent<SpawnController>();
            SpawnController.keyItems = 0;
            spawnController.keyitemsText.text = SpawnController.keyItems + " / " + "18";
            SpawnController.itemsObtained = new bool[18];
            spawnController.ChangeScene("NorthEntrance", 2);
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
        Physics.SyncTransforms();
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
            audioSource[1].volume = 0.2f;
            audioSource[1].Play();
            StartCoroutine(FallTiming());
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void DetectionControl()
    {
        if (isSprinting) detectionRange = 4;
        else if (isCrouching) detectionRange = 1;
        else detectionRange = 2;
    }

    IEnumerator FallTiming()
    {
        yield return new WaitForSeconds(0.2f);
        playFall = true;
        yield break;
    }

    IEnumerator Footsteps()
    {
        while (true)
        {
            if (checkFootsteps && isGrounded)
            {
                //audioSource.clip = musicClips[0];
                audioSource[0].volume = 0.1f;
                audioSource[0].Play();
                if (isCrouching) yield return new WaitForSeconds(0.5f);
                else if (isSprinting) yield return new WaitForSeconds(0.3f);
                else yield return new WaitForSeconds(0.4f);
            }
            yield return null;
        }
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

    public bool CheckSprint()
    {
        return isSprinting;
    }

    public bool CheckCrouch()
    {
        return isCrouching;
    }
}
