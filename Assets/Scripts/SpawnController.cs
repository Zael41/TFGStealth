using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;
    public static string spawnObject;
    public static bool introPlayed;
    public static int sceneNumber;
    public static Animator animator;
    public static AudioSource audioSource;
    public static int keyItems;
    public static bool[] itemsObtained;

    public Transform[] transitions;
    private Transform nextTransition;
    public PlayerMovement playerMovement;
    public GameObject pauseMenu;
    public TMP_Text keyitemsText;
    public AudioClip[] musicClips;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
            itemsObtained = new bool[18];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenuScene")
        {
            if (!pauseMenu.activeInHierarchy)
            {
                PauseMenu();
            }
            else
            {
                ReturnButton();
            }
        }

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
        else if (sceneNumber == 2)
        {
            SceneManager.LoadScene("ExteriorScene");
        }
        else if (sceneNumber == 3)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public static void FadeToLevel()
    {
        animator.SetTrigger("FadeOut");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fade_Out")) animator.SetTrigger("FadeOut");
        if (sceneNumber == 3)
        {
            pauseMenu.SetActive(false);
            animator.SetTrigger("FadeOut");
            keyItems = 0;
            keyitemsText.text = keyItems + " / " + "18";
            audioSource.clip = musicClips[0];
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
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
            audioSource.clip = musicClips[2];
            audioSource.volume = 0.4f;
            audioSource.Play();
            int childIndex = 0;
            Transform keyItemsParent = GameObject.Find("KeyItems").GetComponent<Transform>();
            foreach (Transform child in keyItemsParent)
            {
                if (itemsObtained[childIndex] == true)
                {
                    Destroy(child.gameObject);
                }
                childIndex++;
            }
        }
        if (scene.name == "ExteriorScene")
        {
            GameObject escapeRoute = GameObject.Find("EscapeRoute");
            if (keyItems == 18)
            {
                escapeRoute.SetActive(true);
            }
            else
            {
                escapeRoute.SetActive(false);
            }
            audioSource.clip = musicClips[1];
            audioSource.volume = 0.2f;
            audioSource.Play();
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

    public void PauseMenu()
    {
        if (introPlayed)
        {
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReturnButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1.0f;
        spawnObject = null;
        introPlayed = false;
        sceneNumber = 3;
        FadeToLevel();
    }

    public void KeyItemGet(string name)
    {
        keyItems++;
        Debug.Log(keyItems);
        keyitemsText.text = keyItems + " / " + "18";
        int childIndex = 0;
        Transform keyItemsParent = GameObject.Find("KeyItems").GetComponent<Transform>();
        foreach (Transform child in keyItemsParent)
        {
            if (child.gameObject.name == name)
            {
                itemsObtained[childIndex] = true;
            }
            childIndex++;
        }
    }
}
