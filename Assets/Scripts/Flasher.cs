using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{

    public float waitTime;

    private bool isActive = true;
    // Use this for initialization


    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {

            StartCoroutine(ActiveTime());

        }
        else
        {
            StartCoroutine(notActiveTime());

        }
    }

    IEnumerator ActiveTime()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(waitTime);
        isActive = false;

    }

    IEnumerator notActiveTime()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        isActive = true;

    }

}
