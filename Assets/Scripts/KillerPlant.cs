using System.Collections;
using UnityEngine;

public class KillerPlant : MonoBehaviour {
	//public variables(projectile prefab)

	public GameObject projectile;
	public float projectileForce;
	public float delayBetweenEachShot = 3f;
    public AudioClip greenProjectielSFX;


	//private variable
	private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;
	private bool canFire = true;

	void Awake () {
		_rigidbody = gameObject.GetComponent<Rigidbody2D> ();
        _audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

		fire ();
	}

	public void fire()
	{

		Vector3 localScale = _rigidbody.transform.localScale;
		Vector2 projectilePosition;
		GameObject myProjectile;

		if (localScale.x > 0 && canFire)
		{
			localScale.x = 1;
			projectilePosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y+1);
			myProjectile = Instantiate(projectile, projectilePosition, Quaternion.identity) as GameObject;
			myProjectile.GetComponent<Animator>().StartPlayback();
			myProjectile.GetComponent<Rigidbody2D>().AddForce(Vector2.left * projectileForce);
            PlaySound(greenProjectielSFX);
            myProjectile.transform.localScale = localScale;
			canFire = false;
			StartCoroutine(delay());


		}
		else if (localScale.x < 0 && canFire)
		{
			localScale.x = -1;
			projectilePosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y+1);
			myProjectile = Instantiate(projectile, projectilePosition, Quaternion.identity) as GameObject;
			myProjectile.GetComponent<Animator>().StartPlayback();
			myProjectile.GetComponent<Rigidbody2D>().AddForce(Vector2.right * projectileForce);
            PlaySound(greenProjectielSFX);
			myProjectile.transform.localScale = localScale;
			canFire = false;
			StartCoroutine(delay());



		}

	}

    void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    private IEnumerator delay()
	{
		if (canFire == false)
			yield return new WaitForSeconds(delayBetweenEachShot);
		canFire = true;
	}

}
