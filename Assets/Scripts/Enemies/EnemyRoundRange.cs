using UnityEngine;

public class EnemyRoundRange : Enemy
{
    // [Tooltip("Valore che viene diviso a followRange! Distanza dalla quale inizia a girare attorno al player")]
    // [SerializeField] private float roundDistanceDivider;
    [SerializeField] private float RotationAroundPlayerSpeedMultiplier = 2f;
    private float roundDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start() {
        base.Start();

        roundDistance = attackRange; // direttamente pari all'attack range (?)
    }

    protected override void EnemyBehaviour() {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followRange) {
            Idle();
        }
        else if(distance <= followRange && distance > roundDistance) {
            Movement();
        }
        else if(distance <= roundDistance) {
            RoundMovement();
        }

        if(distance <= attackRange) {
            this.Attack();
        }
    }

    protected override void Attack() {
        // Non si ferma quindi
        SpawnBullet();
    }

    private void RoundMovement() {
        // moto circolare --> direzione tangente a circonferenza e perpendicolare alla direzione del raggio
        // il raggio in questo caso e' la distanza tra player e nemico
        Vector3 radiusDirection = (target.position - transform.position).normalized;
        Vector3 tangentDirection = Vector3.Cross(radiusDirection, Vector3.forward); // in 2D e' il forward che esce dal piano, in 3d sarebbe vector3.up

        // Debug.Log(tangentDirection);

        rb.linearVelocity = tangentDirection * RotationAroundPlayerSpeedMultiplier * speed;
    }
}
