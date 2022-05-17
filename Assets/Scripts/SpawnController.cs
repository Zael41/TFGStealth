using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;
    public static string spawnObject;
    public static bool introPlayed;
    public static int sceneNumber;
    public static Animator animator;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene (string position, int sceneIndex)
    {
        spawnObject = position;
        sceneNumber = sceneIndex;
        introPlayed = true;
        FadeToLevel();
    }

    public void LoadScenes()
    {
        if (sceneNumber == 1)
        {
            SceneManager.LoadScene("Floor1Scene");
        }
        else
        {
            SceneManager.LoadScene("ExteriorScene");
        }
    }

    public static void FadeToLevel()
    {
        animator.SetTrigger("FadeOut");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade_Out")) animator.SetTrigger("FadeOut");
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name != "MainMenuScene")
        {
            Transform player = GameObject.Find("FirstPersonPlayer").GetComponent<Transform>();
            if (spawnObject != null)
            {
                Transform spawn = GameObject.Find(spawnObject).GetComponent<Transform>();
                player.position = spawn.position;
            }
        }
    }
}
