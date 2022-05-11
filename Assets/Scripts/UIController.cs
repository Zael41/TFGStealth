using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject loreText;
    public PlayerMovement playerScript;
    public MouseLook cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseLore()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerScript.enabled = true;
        cameraScript.enabled = true;
        loreText.SetActive(false);
    }
}
