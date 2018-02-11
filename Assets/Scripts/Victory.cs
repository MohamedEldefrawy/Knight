using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour {
    public bool taken = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Player") && (!taken))
        {
            // mark as taken so doesn't get taken multiple times
            taken = true;

            // if explosion prefab is provide, then instantiate it
          
            // do the player victory thing
            other.gameObject.GetComponent<KnightController>().victory();

            // destroy the victory gameobject
            DestroyObject(this.gameObject);
        }
    }
}
