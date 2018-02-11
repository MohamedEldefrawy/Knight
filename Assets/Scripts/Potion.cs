using UnityEngine;

public class Potion : MonoBehaviour
{

    public GameObject PotionExplosion;
    

    private Animator _animator;
    private bool taken = false;

    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!taken && collision.gameObject.tag == "Player")
        {

            if (PotionExplosion)
                Instantiate(PotionExplosion, gameObject.transform.position, Quaternion.identity);

            if (_animator)
                _animator.StartPlayback();

        }
        Destroy(gameObject);
    }
}
