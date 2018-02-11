using UnityEngine;

public class GemsManger : MonoBehaviour
{
    public int value = 1;
    AudioSource _audioSource;

    private bool taken = false;

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !taken)
        {
            taken = true;
            other.gameObject.GetComponent<KnightController>().CollectGems(value);
        }
        Destroy(gameObject);
    }

   
}
