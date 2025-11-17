using UnityEngine;

public class EnemyRange : Enemy
{
    [Tooltip("Valore che viene diviso a Attack range! Rappresenta distanza dalla quale il nemico inizia a fuggire")]
    [SerializeField] private float fleeDistanceDivider;
    private float startSpeed;
    private float fleeSpeed;

    protected override void Start() {
        base.Start();
        startSpeed = this.speed;
        fleeSpeed = startSpeed / 2f; // FleeSpeed -> dimezzata rispetto alla startSpeed
    }
    protected override void EnemyBehaviour() {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followRange) {
            Idle();
        }
        else if (distance > attackRange) {
            Movement();
        }
        else {
            this.Attack(); // Attacca a prescindere

            if (distance < (attackRange / fleeDistanceDivider)) { // Se rientra in flow distance
                speed = fleeSpeed; // Dimezza la velocita'
                Movement(true); // Si muove, ma in direzione inversa rispetto al player
            }
            else {
                speed = startSpeed;
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    protected override void Attack() {
        // Non si ferma quindi
        SpawnBullet();
    }
}
