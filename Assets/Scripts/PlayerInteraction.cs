using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public LayerMask interactMask;
    public GameObject interactScreen;
    Camera cam;
    SpawnController spawnController;

    void Start()
    {
        cam = Camera.main;
        interactScreen.SetActive(false);
        spawnController = GameObject.Find("SpawnController").GetComponent<SpawnController>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2, interactMask))
        {
            ShowCanvas();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (SceneManager.GetActiveScene().name == "ExteriorScene")
                {
                    spawnController.ChangeScene(hit.transform.gameObject.name, 1);
                }
                else
                {
                    spawnController.ChangeScene(hit.transform.gameObject.name, 2);
                }
            }
        }
        else
        {
            interactScreen.SetActive(false);
        }
    }

    void ShowCanvas()
    {
        interactScreen.SetActive(true);
    }
}
