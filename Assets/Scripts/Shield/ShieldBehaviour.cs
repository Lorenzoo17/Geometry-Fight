using UnityEngine;

public class ShieldBehaviour : MonoBehaviour, IDamageable{

    [SerializeField] private float shieldHealth;
    private float shieldCurrentHealth;

    private SpriteRenderer spriteRenderer;
    private float initialColorAlpha;
    private float shieldContactDamage;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColorAlpha = spriteRenderer.color.a;

        shieldCurrentHealth = shieldHealth;
    }

    public void SetShieldStats(float health, float size, float contactDamage, Transform parent) { // Richiamato in SpawnShield_Item.cs
        shieldHealth = health;
        shieldCurrentHealth = shieldHealth;
        shieldContactDamage = contactDamage;

        transform.localScale = transform.localScale * size;
        transform.SetParent(parent);
    }

    public void TakeDamage(float damage) {
        shieldCurrentHealth -= damage; // tolgo vita

        if(shieldCurrentHealth <= 0) {
            Destroy(gameObject); // Distruggo lo scudo
        }
        else { // Aggiorno visual (riduco gradualmente l'alpha dello scudo)
            float newAlpha = (shieldCurrentHealth / shieldHealth) * initialColorAlpha;
            spriteRenderer.color =
                new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) { // Se e' nemico
            if(other.TryGetComponent<IDamageable>(out IDamageable target)) {
                target.TakeDamage(shieldContactDamage); // Danno a contatto

                float damageAfterContact = 10f; // Danno autoinflitto a seguito del contatto
                this.TakeDamage(damageAfterContact);
            }
        }
    }
}
