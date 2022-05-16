using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene (string position, int sceneIndex)
    {
        if (sceneIndex == 1)
        {
            SceneManager.LoadScene("Floor1Scene");
        }
        else
        {
            SceneManager.LoadScene("ExteriorScene");
        }
        Transform player = GameObject.Find("FirstPersonPlayer").GetComponent<Transform>();
        Transform spawn = GameObject.Find(position).GetComponent<Transform>();
        player.position = spawn.position;
    }
}
