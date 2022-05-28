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
    public GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = GameObject.Find("CrosshairCanvas");
        if (SpawnController.introPlayed == true)
        {
            playerScript.enabled = true;
            cameraScript.enabled = true;
            loreText.SetActive(false);
        }
        else
        {
            crosshair.SetActive(false);
        }
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
        SpawnController.introPlayed = true;
        crosshair.SetActive(true);
    }

    public void CloseGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
