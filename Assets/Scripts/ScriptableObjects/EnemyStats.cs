using UnityEngine;

[CreateAssetMenu(fileName = "New enemy stats", menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float health;
    public float speed;
    public float attackRange;
    public float followRange;
    public float damage;
    public float bulletAttackRange;
    public Sprite sprite; // aspetto del nemico
    public GameObject bullet; // Bullet da sparare per attacco
    public float sizeMin; // dimensione
    public float sizeMax;
    public float attackRateMin;
    public float attackRateMax;
    public float bulletSpeedMin;
    public float bulletSpeedMax;
    public float bulletSizeMultiplierMin;
    public float bulletSizeMultiplierMax;
    public float degreeRotationPerSecondMin;
    public float degreeRotationPerSecondMax;

    // Per EnemyRange
    public float fleeDistance;

    public float xpToGive;
}
