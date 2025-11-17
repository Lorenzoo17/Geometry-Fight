using UnityEngine;

public class TargetTest : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 30;

    public void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
