using UnityEngine;

public class DeathZone : MonoBehaviour {

	public static float damage = 10 ;
	public static bool isTrap = false ;

	// Use this for initialization

	public static float kill(float target)
	{
		if (!isTrap) {
			target -= damage;
			return target;
		} else {
			target -= 1000;
			return target;
		}

	}
}
