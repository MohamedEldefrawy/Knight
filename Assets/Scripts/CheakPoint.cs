using System.Collections;
using UnityEngine;

public class CheakPoint : MonoBehaviour
{

    public static bool cheakPointReached = false;
    public GameObject campFireCanavas;
    public AudioClip campfireSFX;
    AudioSource _audioSource;

    Animator _animator;
    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _audioSource = gameObject.GetComponent<AudioSource>();

    }
    // Use this for initialization

    void OnTriggerEnter2D(Collider2D collider)

    {
        campFireCanavas.SetActive(true);
        if (collider.gameObject.tag == "Player")
        {
            PlaySound(campfireSFX);
            cheakPointReached = true;
            _animator.SetBool("Reached", true);
            KnightController.respawnLocation = gameObject.transform;
            StartCoroutine(textDisableDelay());
        }
    }

    IEnumerator textDisableDelay()
    {
        yield return new WaitForSeconds(2f);
        campFireCanavas.SetActive(false);

    }

    //simple method to make music much easier
    void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}
