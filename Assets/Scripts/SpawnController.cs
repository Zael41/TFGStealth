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

    public Transform[] transitions;
    private Transform nextTransition;
    public PlayerMovement playerMovement;

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
        if (scene.name == "Floor1Scene")
        {
            transitions = GameObject.Find("InsideTransitions").GetComponentsInChildren<Transform>();
            playerMovement = GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>();
        }
    }

    public void Transition(Transform transitionStart)
    {
        string name = transitionStart.gameObject.name;
        string lastLetter = transitionStart.gameObject.name.Substring(transitionStart.gameObject.name.Length - 1);
        Debug.Log(lastLetter);
        foreach (Transform t in transitions)
        {
            if (t.gameObject.name.Substring(t.gameObject.name.Length - 1) == lastLetter && t.gameObject.name != name)
            {
                nextTransition = t;
            }
        }
        playerMovement.TransitionDisable();
        animator.SetTrigger("TransitionFade");
    }

    public void MovePlayer()
    {
        Debug.Log(nextTransition.gameObject.name);
        playerMovement.gameObject.transform.position = nextTransition.position;
        playerMovement.TransitionDisable();
        animator.SetTrigger("TransitionFade");
    }
}
