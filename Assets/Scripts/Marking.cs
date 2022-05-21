using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marking : MonoBehaviour
{
    public LayerMask enemyMask;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50, enemyMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (Transform child in hit.transform)
                {
                    if (child.gameObject.layer == LayerMask.NameToLayer("Enemy")) child.gameObject.layer = LayerMask.NameToLayer("TaggedEnemy");
                }
                StartCoroutine(TagEnemy(hit.transform.gameObject));
            }
        }
    }

    IEnumerator TagEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(10f);
        foreach (Transform child in enemy.transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("TaggedEnemy")) child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        yield break;
    }
}
