using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideTransitions : MonoBehaviour
{
    private Animator animator;
    public Transform[] transitions;
    private Transform nextTransition;
    public PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        animator = GameObject.Find("SpawnController").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Transition(Transform transitionStart)
    {
        Debug.Log("we rollin");
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
        animator.SetTrigger("FadeOut");
    }

    public void MovePlayer()
    {
        playerMovement.gameObject.transform.position = nextTransition.position;
        playerMovement.TransitionDisable();
        animator.SetTrigger("FadeOut");
    }
}
