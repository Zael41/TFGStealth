using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    public Transform player;
    PlayerMovement playerMovement;
    bool alreadySprinting;
    bool alreadyCrouching;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = player.GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.position;
        if (playerMovement.CheckSprint() && alreadySprinting == false)
        {
            transform.localScale = transform.localScale * 2f;
            alreadySprinting = true;
        }
        else if (playerMovement.CheckCrouch() && alreadyCrouching == false)
        {
            transform.localScale = transform.localScale * 0.5f;
            alreadyCrouching = true;
        }
        else if (!playerMovement.CheckSprint() && !playerMovement.CheckCrouch())
        {
            transform.localScale = new Vector3 (1f,1f,1f);
            alreadySprinting = false;
            alreadyCrouching = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, transform.up * 2f);
    }
}
