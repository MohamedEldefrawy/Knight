using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //array to store waypoints
    public GameObject[] myWaypoints; // to define the movement waypoints

    public GameObject _player;
    public AudioClip EnemyHurtSFX;
    public AudioClip EnemyDieSFX;
    public AudioClip EnemyAttackSFX;
    public float waitAtWaypointTime = 1f; // how long to wait at a waypoint
    public bool loopWaypoints = true; // should it loop through the waypoints
    public float moveSpeed = 20;
    public float enemyHealth = 20;
    public static float hitPower = 20;
    public Transform sensor;


    private float distance;
    private bool ismoving = true;
    private Transform _transform;
    private Rigidbody2D _rigidbody2;
    private Animator _animator;
    private bool isAlive = true;

    [SerializeField]
    private int indexMyWayPoint;
    private float _vx = 0;
    private float moveTime;
    private AudioSource _audioSource;

    void Awake()
    {
        _transform = gameObject.GetComponent<Transform>();
        _rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _audioSource = gameObject.GetComponent<AudioSource>();

    }


    void Update()
    {
        //flip teh enemy towards the player when he get close enouph
        if (_player)
            //calculate the distance between the enemy and the player
            distance = Vector2.Distance(_player.transform.position, sensor.position);

        //fliping the enemy twordes the player by changing it's localScalef in x direction
        if (distance <= 8 && _player.transform.localScale.x > 0)
        {
            Vector3 localScale = _transform.localScale;
            localScale.x = 1;
            _transform.localScale = localScale;
            _animator.SetBool("Attack", true);
            ismoving = false;

        }
        else if (distance <= 8 && _player.transform.localScale.x < 0)
        {
            Vector3 localScale = _transform.localScale;
            localScale.x = -1;
            _transform.localScale = localScale;
            _animator.SetBool("Attack", true);
            ismoving = false;
            _animator.SetBool("Attack", true);
            ismoving = false;

        }
        else if (distance > 10)
        {
            _animator.SetBool("Attack", false);
            ismoving = true;

        }


        if (Time.time >= moveTime)
            EnemyMovment();
        else
        {
            _animator.SetBool("Run", false);
        }

    }


    //moving the enemy
    void EnemyMovment()
    {
        if (myWaypoints.Length != 0 && ismoving)
        {
            //flip face of eney to the waypoint based on previous movment
            flip(_vx);
            //determine the distance between waypoint and the enemy
            _vx = myWaypoints[indexMyWayPoint].transform.position.x - _transform.position.x;

            //if the enemy is close enouph to the waypoint make its trget the next waypoint
            if (Mathf.Abs(_vx) <= 1f)
            {
                //stop moving at the waypoint
                _rigidbody2.velocity = new Vector2(0, 0);
                indexMyWayPoint++;
                if (indexMyWayPoint >= myWaypoints.Length)
                {
                    if (loopWaypoints)
                        indexMyWayPoint = 0;
                    else
                        ismoving = false;
                }

                moveTime = Time.time + waitAtWaypointTime;

            }
            else
            {

                _animator.SetBool("Run", true);
                // Set the enemy's velocity to moveSpeed in the x direction.
                _rigidbody2.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody2.velocity.y);
            }
        }
    }

    // flip the enemy to face torward the direction he is moving in
    void flip(float x)
    {

        // get the current scale
        Vector3 localScale = _transform.localScale;

        if ((x > 0f) && (localScale.x < 0f))
            localScale.x *= -1;
        else if ((x < 0f) && (localScale.x > 0f))
            localScale.x *= -1;

        // update the scale
        _transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !KnightController.isSwordAttack)
        {
            if (enemyHealth > 0)
            {
                _animator.SetBool("Hurt", true);
                enemyHealth = KnightController.swordDamage(enemyHealth);
                PlaySound(EnemyHurtSFX);
                KnightController.isSwordAttack = true;
                StartCoroutine(resetAnimation());

            }

            else if (enemyHealth <= 0 && isAlive)
            {
                _animator.SetTrigger("Die");
                PlaySound(EnemyDieSFX);
                freezMotion();
                StartCoroutine(timeDelay());
            }
        }


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        // fire ball damage

        if (collision.gameObject.tag == "Projectile")
        {
            if (enemyHealth > 0)
            {
                _animator.SetBool("Hurt", true);
                enemyHealth = Projectile.projectileDamage(enemyHealth);
                PlaySound(EnemyHurtSFX);
                StartCoroutine(resetAnimation());
            }
            else if (enemyHealth <= 0 && isAlive)
            {

                freezMotion();
                _animator.SetTrigger("Die");
                PlaySound(EnemyDieSFX);
                freezMotion();
                StartCoroutine(timeDelay());
            }
        }

    }


    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(1.5f);
        isAlive = false;
        ismoving = false;
        Destroy(gameObject);
    }

    public static float enemyAttackDamage(float health)
    {
        health -= hitPower;
        return health;
    }

    void freezMotion()
    {
        _rigidbody2.isKinematic = true;
        _rigidbody2.simulated = false;
        ismoving = false;

    }

    IEnumerator resetAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        KnightController.isSwordAttack = false;
        _animator.SetBool("Hurt", false);
        ismoving = true;
    }

    void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }




}
