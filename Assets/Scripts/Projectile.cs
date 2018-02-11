using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Update is called once per frame   
    public static float damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public static float projectileDamage(float health)
    {
        health -= damage;
        return health;
    }


}
