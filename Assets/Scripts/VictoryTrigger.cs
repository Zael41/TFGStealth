using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    public GameObject victoryScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FirstPersonPlayer" && SpawnController.keyItems == 18)
        {
            Cursor.lockState = CursorLockMode.None;
            victoryScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
