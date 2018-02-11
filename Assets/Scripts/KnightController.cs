using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KnightController : MonoBehaviour
{
    //private variales
    private Animator _animator;
    private Transform _transform;
    private Rigidbody2D _rigidbody2;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;


    //player conditions 
    private bool isGround = true;
    private bool isRunning;
    private bool canDoubleJump = true;
    private bool canFire = true;
    private bool facingLeft = true;
    private bool isAlive = true;

    public static bool isSwordAttack = false;

    //Vertices
    private float _vx;
    private float _vy;

    //public variables

    public LayerMask whatIsGround;
    public Transform groundCheak;
    public float movmentSpeed = 10;
    public float jumpingPower = 10f;
    public static float swordPower = 5f;
    public static float playerHealth = 100;

    //projectiel stuff
    public GameObject projectile;
    public float projectileForce = 10;
    private Animator _animatorProjectile;
    // Delay between firing
    public float delay = 5f;

    //firePotion stuff
    public GameObject firePotion;
    public float fireAbiltyDuration = 20;
    public GameObject fireAbiltyCanavas;
    public Text fireAbiltyCoolDownText;

    //wining warning 
    public GameObject winingCanavas;

    public Text winningWarningText;

    //Vanish stuff
    public GameObject vanishPotion;
    public float vanishAbiltyDuration = 20;
    public GameObject vanishAbiltyCanvas;
    public Text vanishAbiltyText;

    //helthpotion stuff
    public GameObject playerHealthpotion;
    private bool healthPotionTaken = false;

    //respawnLocation
    [HideInInspector]
    public static Transform respawnLocation;
    public Transform gameStartLocation;

    //Audio Clips
    public AudioClip deathSFX;
    public AudioClip hurtSFX;
    public AudioClip _GemSFX;
    public AudioClip jumpSFX;
    public AudioClip swordAttacSFX;
    public AudioClip mainMusicSFX;
    public AudioClip fireBallSFX;


    // Use this for initialization
    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _transform = gameObject.GetComponent<Transform>();
        _rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        winingCanavas.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //cheak if the player is ground or not
        isGround = Physics2D.Linecast(_transform.position, groundCheak.position, whatIsGround);
        _vx = Input.GetAxisRaw("Horizontal");
        _vy = _rigidbody2.velocity.y;


        _animator.SetBool("Grounded", isGround);

        if (isGround)
        {
            canDoubleJump = true;
        }

        if (_vx == 0)
            isRunning = false;
        else
        {
            isRunning = true;

        }


        _animator.SetBool("Running", isRunning);
        _rigidbody2.velocity = new Vector2(_vx * movmentSpeed, _vy);

        _animator.SetBool("Grounded", isGround);
        if (Input.GetButtonDown("Jump") && isGround)
        {

            Jump();
            canDoubleJump = true;
        }


        if (Input.GetButtonDown("Jump") && canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }





        //this condition to make the fire abilty allowed for specific time
        if (firePotion == null)
        {

            fireAbiltyCanavas.SetActive(true);
            fireAbiltyCoolDownText.text = ((int)fireAbiltyDuration).ToString();
            fireAbiltyDuration -= Time.deltaTime;

        }
        //to make the UI of time disapper after the time finish
        if (fireAbiltyDuration <= 0)
        {
            fireAbiltyCanavas.SetActive(false);
        }

        //fire magic happens here
        if (Input.GetKey(KeyCode.Tab) && canFire && firePotion == null && fireAbiltyDuration > 0)
        {
            canFire = false;
            fire();
            StartCoroutine(timeDelayforFire());
        }

        //this conditions make vanshing abilty allowed for certain time
        if (vanishPotion == null)
        {

            vanishAbiltyCanvas.SetActive(true);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            vanishAbiltyText.text = ((int)vanishAbiltyDuration).ToString();
            vanishAbiltyDuration -= Time.deltaTime;

        }
        //to make the UI of time disapper after the time finish
        if (vanishAbiltyDuration <= 0)
        {
            vanishAbiltyCanvas.SetActive(false);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            gameObject.layer = 8;
        }
        // vanish magic happens here
        if (vanishPotion == null && vanishAbiltyDuration > 0)
        {
            vanish();
        }

        if (playerHealthpotion == null && !healthPotionTaken)
        {
            restoreHealth();
            healthPotionTaken = true;
        }


        //making sword attack action
        if (Input.GetKey(KeyCode.E))
        {
            swordAttack();
        }

    }

    void LateUpdate()
    {
        Vector3 localScale = _transform.localScale;
        //to make the face of player the same of the key pressed
        if (_vx > 0)
        {
            facingLeft = true;
        }
        else if (_vx < 0)
        {
            facingLeft = false;
        }

        if ((localScale.x > 0 && facingLeft) || (localScale.x < 0 && !facingLeft))
        {
            localScale.x *= -1;
        }
        _transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (playerHealth > 0)
            {
                _animator.SetBool("Hurt", true);
                playerHealth = Enemy.enemyAttackDamage(playerHealth);
                if (GameManger.gm)
                    GameManger.gm.reduceHealth(playerHealth);
                PlaySound(hurtSFX);
                StartCoroutine(resetAnimationAfterHurt());
            }
            else if (playerHealth <= 0)
            {
                if (GameManger.gm)
                    GameManger.gm.reduceHealth(0);
                isAlive = false;
                _animator.SetTrigger("Dead");
                PlaySound(deathSFX);
                freezMotion();
                isRunning = false;
                if (GameManger.gm)
                    GameManger.gm.ResetGame();
                StartCoroutine(resetAnimationAfterDead());

            }
        }

        if (collision.gameObject.tag == "DeathZone")
        {
            fallDeath();
            if (GameManger.gm)
                GameManger.gm.reduceHealth(0);
            isAlive = false;
            _animator.SetTrigger("Dead");
            freezMotion();
            isRunning = false;
            if (GameManger.gm)
                GameManger.gm.ResetGame();
            StartCoroutine(resetAnimationAfterDead());

        }
        if (!isAlive)
        {
            StartCoroutine(resetAnimationAfterDead());
        }
        if (collision.gameObject.tag == "GreenProjectile")
        {

            if (playerHealth > 0)
            {
                _animator.SetBool("Hurt", true);
                PlaySound(hurtSFX);
                playerHealth = Projectile.projectileDamage(playerHealth);
                if (GameManger.gm)
                    GameManger.gm.reduceHealth(Projectile.projectileDamage(playerHealth));
                StartCoroutine(resetAnimationAfterHurt());
            }
            else if (playerHealth <= 0)
            {
                isAlive = false;
                _animator.SetTrigger("Dead");
                PlaySound(deathSFX);
                freezMotion();
                isRunning = false;
                StartCoroutine(resetAnimationAfterDead());
                if (GameManger.gm)
                    GameManger.gm.ResetGame();

            }
        }

    }




    private void Jump()
    {
        PlaySound(jumpSFX);
        _vy = 0;
        _rigidbody2.velocity = new Vector2(0, jumpingPower);
    }

    public void restoreHealth()
    {
        if (GameManger.gm)
            GameManger.gm.AddHealth();
    }


    public void vanish()
    {
        gameObject.layer = 11;
    }

    public void fire()
    {

        Vector3 localScale = _rigidbody2.transform.localScale;
        Vector2 projectilePosition;
        GameObject myProjectile;

        if (localScale.x > 0)
        {
            localScale.x = 1;
            projectilePosition = new Vector2(gameObject.transform.position.x - 2f, gameObject.transform.position.y);
            myProjectile = Instantiate(projectile, projectilePosition, Quaternion.identity) as GameObject;
            myProjectile.GetComponent<Animator>().SetBool("Fire", true);
            AudioSource myProjectileAudioSource = myProjectile.GetComponent<AudioSource>();
            myProjectileAudioSource.PlayOneShot(fireBallSFX);
            myProjectile.GetComponent<Rigidbody2D>().AddForce(Vector2.left * projectileForce);
            myProjectileAudioSource.PlayOneShot(fireBallSFX);
            myProjectile.transform.localScale = localScale;


        }
        else if (localScale.x < 0)
        {
            localScale.x = -1;
            projectilePosition = new Vector2(gameObject.transform.position.x + 2f, gameObject.transform.position.y);
            myProjectile = Instantiate(projectile, projectilePosition, Quaternion.identity) as GameObject;
            myProjectile.GetComponent<Animator>().SetBool("Fire", true);
            AudioSource myProjectileAudioSource = myProjectile.GetComponent<AudioSource>();
            myProjectile.GetComponent<Rigidbody2D>().AddForce(Vector2.right * projectileForce);
            myProjectileAudioSource.PlayOneShot(fireBallSFX);
            myProjectile.transform.localScale = localScale;


        }

    }

    //method that make player attack with his sword , 
    public void swordAttack()
    {
        _animator.SetBool("SwordAttack", true);
        PlaySound(swordAttacSFX);
        StartCoroutine(resetAnimation());

    }
    //method that allow the health of player to be reduced when get attacked by the sword
    public static float swordDamage(float health)
    {
        health -= swordPower;
        return health;
    }

    //method that helps to accsess the score editing when want to add score from the gameManger class 
    public void CollectGems(int amount)
    {
        PlaySound(_GemSFX);
        if (GameManger.gm)
            GameManger.gm.AddPoints(amount);
    }

    //killing player when fall into death zone

    private void fallDeath()
    {
        playerHealth = DeathZone.kill(playerHealth);
        PlaySound(deathSFX);
    }

    public void victory()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            freezMotion();
            _animator.SetTrigger("Victory");
            if (GameManger.gm) // do the game manager level compete stuff, if it is available
                GameManger.gm.LevelCompete();
        }
        else
        {
            winingCanavas.SetActive(true);
            StartCoroutine(winningWarningTimeDelay());

        }

    }

    //waiting a certain time then disable winingWarningCanvas
    IEnumerator winningWarningTimeDelay()
    {
        yield return new WaitForSeconds(5f);
        winingCanavas.SetActive(false);
    }

    //waiting time between each throwed projectile
    IEnumerator timeDelayforFire()
    {
        if (canFire == false)
            yield return new WaitForSeconds(delay);
        canFire = true;
    }

    //a simple method to play any clip


    //to stop the sword attack animation from playing
    IEnumerator resetAnimation()
    {
        yield return new WaitForSeconds(1f);
        _animator.SetBool("SwordAttack", false);
        _animator.SetBool("Grounded", true);
    }

    //to stop the hurt animation from playing
    IEnumerator resetAnimationAfterHurt()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("Hurt", false);
    }

    IEnumerator resetAnimationAfterDead()
    {
        yield return new WaitForSeconds(2f);

        isAlive = true;
        _animator.Rebind();
        if (CheakPoint.cheakPointReached)
            respawn(respawnLocation.position);
        else
            respawn(gameStartLocation.position);

    }

    //to destroy the game object after weaiting ane desired secounds
    IEnumerator destroyObjectAFterWaiting()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    //stop player from moving when he died
    void freezMotion()
    {
        isRunning = false;
        _rigidbody2.isKinematic = true;
        _rigidbody2.simulated = false;

    }

    void unFreezMotion()
    {
        isRunning = true;
        _rigidbody2.isKinematic = false;
        _rigidbody2.simulated = true;
    }

    public void respawn(Vector3 spawnLocation)
    {
        unFreezMotion();
        playerHealth = 100;
        _transform.parent = null;
        _transform.position = spawnLocation;
    }

    //simple method to make playing clip much easier
    void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

}

