using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected float health;
    protected float speed;
    protected float followRange; // Distanza dalla quale inizia a seguire il player
    protected float attackRange; // Distanza dalla quale inizia ad attaccare
    protected float damage;
    protected float attackRate; private float timeBtwAttack;
    protected float size;
    protected Sprite sprite;
    protected GameObject bullet;
    protected float bulletAttackRange;
    protected float bulletSpeed;
    protected float bulletSizeMultiplier;
    protected float degreeRotationPerSecond;

    [SerializeField] private EnemyStats stats; // ScriptableObject con le caratteristiche che vanno assegnate
    protected Transform target; // player
    protected Rigidbody2D rb;
    protected Vector2 movementDirection;

    [SerializeField] private GameObject deathTrail; // oggetto relativo al trail che si attiva in corrispondenza della morte

    public void SetStats(EnemyStats stats) {
        this.stats = stats;

        SetUpStats(); // Poi effettuo il setup delle stats
    }

    private void Awake() {
        SetUpStats(); // Set up tramite scriptableobjects -> cosi ho differenti tipi di nemici
        this.GetComponent<HealthSystem>().SetHealth(health);
    }

    private void SetUpStats() {
        if (stats != null) {
            this.health = stats.health;
            this.speed = stats.speed;
            this.attackRange = stats.attackRange;
            this.followRange = stats.followRange;
            this.damage = stats.damage;
            this.sprite = stats.sprite;
            this.bullet = stats.bullet;
            this.bulletAttackRange = stats.bulletAttackRange;
            // Da qui assegno quelli che sono casuali in un range predefinito
            this.bulletSpeed = UnityEngine.Random.Range(stats.bulletSpeedMin, stats.bulletSpeedMax);
            this.bulletSizeMultiplier = UnityEngine.Random.Range(stats.bulletSizeMultiplierMin, stats.bulletSizeMultiplierMax);
            this.attackRate = UnityEngine.Random.Range(stats.attackRateMin, stats.attackRateMax);
            this.size = UnityEngine.Random.Range(stats.sizeMin, stats.sizeMax);
            this.degreeRotationPerSecond = UnityEngine.Random.Range(stats.degreeRotationPerSecondMin, stats.degreeRotationPerSecondMax);
            
            this.timeBtwAttack = attackRate;

            this.GetComponent<HealthSystem>().SetXpToGive(stats.xpToGive); // Imposto esperienza da dare a player
        }
    }
    public void ScaleStats(float multiplier) {
        if (multiplier <= 0f) multiplier = 1f;

        health *= multiplier;                           
        damage *= Mathf.Lerp(1f, multiplier, 0.6f);     
        speed *= Mathf.Lerp(1f, multiplier, 0.3f);     


        followRange *= 1f + (multiplier - 1f) * 0.3f;
        attackRange *= 1f + (multiplier - 1f) * 0.2f;
        attackRate /= 1f + (multiplier - 1f) * 0.15f; 

        bulletAttackRange *= 1f + (multiplier - 1f) * 0.15f;
        bulletSpeed *= 1f + (multiplier - 1f) * 0.1f;
        bulletSizeMultiplier *= 1f + (multiplier - 1f) * 0.1f;

        GetComponent<HealthSystem>().SetHealth(health);
    }
    protected virtual void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = this.GetComponent<Rigidbody2D>();

        this.GetComponent<SpriteRenderer>().sprite = this.sprite; // Assegno lo sprite al nemico definito in EnemyStats
        transform.localScale = new Vector3(1, 1, 1) * size; // Assegno il size definito in EnemyStats (collider si adatta in automatico?)
    }

    // Update is called once per frame
    void Update()
    {
        if(TryGetComponent<HealthSystem>(out HealthSystem enemyHealth)) {
            if(enemyHealth.GetHealthPercentage() <= 0) {
                return;
            }
        }

        if (target != null) {
            EnemyBehaviour();
        }

        if(timeBtwAttack > 0) {
            timeBtwAttack -= Time.deltaTime;
        }

        ManageRotation();
    }

    private void ManageRotation() {
        // Rotazione attorno ad asse z locale di degreeRotationPerSecond al secondo (time.deltatime)
        transform.Rotate(new Vector3(0f, 0f, 1f), degreeRotationPerSecond * Time.deltaTime, Space.Self);
    }

    protected virtual void EnemyBehaviour() {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followRange) {
            Idle();
        }
        else if (distance > attackRange) {
            Movement();
        }
        else {
            Attack();
        }
    }

    protected virtual void Attack() {
        rb.linearVelocity = Vector2.zero;
        // Debug.Log("Attacking :" + target.gameObject.name);
        SpawnBullet();
    }

    protected void SpawnBullet() {
        if (bullet != null) { // Se bullet e' stato assegnato
            if (timeBtwAttack <= 0) {
                GameObject bulletSpawned = Instantiate(bullet, transform.position, Quaternion.identity);
                Vector2 attackDirection = (target.position - transform.position).normalized;
                // Faccio setup del bullet
                if (bulletSpawned.TryGetComponent<BaseBullet>(out BaseBullet baseBullet)) {
                    baseBullet.SetUpBullet(bulletAttackRange, attackDirection, damage, bulletSpeed, BaseBullet.BulletOWner.Enemy, bulletSizeMultiplier);
                }
                // Movimento del bullet gestito direttamente nell'update del bullet

                timeBtwAttack = attackRate;
            }
        }
    }

    protected virtual void Movement(bool inverted = false) {
        movementDirection = (target.position - transform.position).normalized;
        if (inverted) {
            movementDirection = -movementDirection;
        }
        rb.linearVelocity = movementDirection * speed; // Mi muovo verso il player
    }

    protected virtual void Idle() {
        movementDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // Per evitare di avere troppi nemici nella scena a seguito di grandi movimenti del player
        if (Vector2.Distance(transform.position, target.position) > (followRange * 3)) {
            this.GetComponent<HealthSystem>().TakeDamage(float.MaxValue); // Per farlo morire
        }
    }

    // Metodo richiamato in HealthSystem che gestisce la morte del nemico
    public void EnemyDeath() {
        // Spawn di oggetto random (prima spawno poi "lancio via")
        float spawnItemProbability = UnityEngine.Random.Range(0, 5); // 1 su 5 di probabilita'

        if (spawnItemProbability == 0) {
            ItemSpawner.Instance.SpawnItem(this.transform.position);
        }

        // Lo voglio lanciare via:
        // Direzione di lancio = direzione tra lui e il player
        Vector2 throwDirection = (transform.position - target.transform.position).normalized;

        if(rb != null) {
            rb.bodyType = RigidbodyType2D.Dynamic; // Setto da kinematic a dynamic

            float throwForce = 100f;
            rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        }

        if (deathTrail != null) {
            deathTrail.gameObject.SetActive(true);
        }

        // Distruzione gameobject
        float timeToBeDestroyed = 0.5f;
        Destroy(gameObject, timeToBeDestroyed);
    }
}
