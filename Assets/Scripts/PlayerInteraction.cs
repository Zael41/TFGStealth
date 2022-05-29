using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public LayerMask interactMask;
    public GameObject interactScreen;
    public GameObject pickupScreen;
    public GameObject laptopScreen;
    Camera cam;
    SpawnController spawnController;
    InsideTransitions insideTransitions;

    void Start()
    {
        cam = Camera.main;
        interactScreen.SetActive(false);
        spawnController = GameObject.Find("SpawnController").GetComponent<SpawnController>();
        if (SceneManager.GetActiveScene().name == "Floor1Scene") insideTransitions = GameObject.Find("InsideTransitions").GetComponent<InsideTransitions>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2, interactMask))
        {
            ShowCanvas(hit);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit.collider.CompareTag("SceneMove"))
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
                else if (hit.collider.CompareTag("InsideMove"))
                {
                    spawnController.Transition(hit.transform);
                }
                else if (hit.collider.CompareTag("KeyItem"))
                {
                    Destroy(hit.transform.gameObject);
                    spawnController.KeyItemGet(hit.transform.gameObject.name);
                }
                else if (hit.collider.CompareTag("Laptop"))
                {
                    spawnController.MarkItems();
                }
            }
        }
        else
        {
            interactScreen.SetActive(false);
            pickupScreen.SetActive(false);
            laptopScreen.SetActive(false);
        }
    }

    void ShowCanvas(RaycastHit hit)
    {
        if (hit.transform.gameObject.tag == "KeyItem") pickupScreen.SetActive(true);
        else if (hit.transform.gameObject.tag == "Laptop") laptopScreen.SetActive(true);
        else interactScreen.SetActive(true);
    }
}
