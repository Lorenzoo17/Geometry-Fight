using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletExplosionEffect;
    [SerializeField] private float raycastRange; // lunghezza o raggio del rayscast
    
    private float lifeTime;
    private Vector2 bulletDirection;
    private float bulletDamage;
    private float recoilValue; // Rinculo
    private float bulletSpeed;

    public enum BulletOWner {
        Player,
        Enemy
    }

    private BulletOWner owner;

    public void SetUpBullet(float lifeTime, Vector2 attackDirection, float damage, float speed, BulletOWner owner = BulletOWner.Player, float sizeMultiplier = 1f, float recoil=0.0f) { // richimato all'instantiate di un bullet
        this.lifeTime = lifeTime;
        this.bulletDirection = attackDirection;
        this.bulletDamage = damage; // Per ora semplicemente uguale all'attacco
        this.recoilValue = recoil;
        this.owner = owner;
        this.bulletSpeed = speed;

        // Quando si scala scala automaticamente anche il collider, quindi non ci sono problemi
        this.transform.localScale = this.transform.localScale * sizeMultiplier; // Scalo in base al sizeMultiplier
        // altri ...
        // Eventualmente passare enum per capire se player, enemy o ally

        Destroy(this.gameObject, lifeTime); // per distruggerlo dopo lifetime
    }

    private void Update() {
        // Gestisco movimento direttamente qui, con setup imposto come tale movimento deve avvenire (direzione e velocita')
        this.GetComponent<Rigidbody2D>().linearVelocity = bulletDirection * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Per evitare fuoco "amico"
        if ((other.CompareTag("Player") || other.CompareTag("Ally")) && this.owner == BulletOWner.Player) return;
        if (other.CompareTag("Enemy") && this.owner == BulletOWner.Enemy) return;

        if (other.TryGetComponent<IDamageable>(out IDamageable target)) {
            target.TakeDamage(bulletDamage);
            Debug.Log("Hit :" + other.gameObject.name);

            // magari non per tutti i tipi di proiettili
            DestructionBehaviour();
            Destroy(this.gameObject);
        }
    }

    private void DestructionBehaviour() {
        // Debug.Log("Bullet destroyed");
        GameObject effect = Instantiate(bulletExplosionEffect, transform.position, Quaternion.identity);

        // distruggere anche questo effetto -> fatto con script apposito sull'effetto
    }

    private void OnDestroy() { // Richiamato sul destroy
        DestructionBehaviour();
    }
}
