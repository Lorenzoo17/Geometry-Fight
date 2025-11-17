using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HealthSystem : MonoBehaviour, IDamageable {
    [SerializeField] private float health;
    [SerializeField] private SpriteRenderer entityRenderer;

    // evento per gestione cambio vita
    public EventHandler OnHealthChanged;

    [Tooltip("Colore che assume dopo i danni")]
    [SerializeField] private Color32 damageColor;
    [Tooltip("Tempo necessario per tornare a colore originale")]
    [SerializeField] private float colorInterpolationValueAfterDamage;
    private Color startingColor;
    [Tooltip("Se non si vuole invincibilita' si lascia a 0")]
    [SerializeField] private float invincibiltyTime;
    private float currentInvicibilityTime;
    private bool invincible;

    private float currentHealth;

    // usato solo per nemici
    private float xpToGiveAfterDeath = 0.0f;

    public void SetXpToGive(float value) { // Richiamato in Enemy.cs
        xpToGiveAfterDeath = value;
    }

    public float GetHealthPercentage() {
        return (currentHealth / health) * 100;
    }

    public float GetCurrentHealth() => currentHealth;

    public void InitializeHealthStats(float health, float invincTime) {
        this.health = health;
        currentHealth = health;
        invincibiltyTime = invincTime;
        currentInvicibilityTime = invincibiltyTime;
    }

    private void Start() {
        this.currentHealth = health;
        startingColor = entityRenderer.color;

        currentInvicibilityTime = invincibiltyTime;
    }

    private void Update() {
        // se colore diverso da originale -> interpolazione lineare
        if (entityRenderer.color != startingColor) {
            entityRenderer.color = Color.Lerp(entityRenderer.color, startingColor, colorInterpolationValueAfterDamage * Time.deltaTime);
        }

        // Gestione invincibilita'
        if (invincible) { // Se e' invincibile (appena stato colpito)
            if(currentInvicibilityTime <= 0) { // Se il tempo di invincibilita' e' finito
                currentInvicibilityTime = invincibiltyTime; // resetto timer
                invincible = false; // non piu' invincibile
            }
            else {
                currentInvicibilityTime -= Time.deltaTime;
            }
        }
    }

    public void SetHealth(float value) {
        this.health = value;
    }

    public void RestoreHealth(float valueToAdd) {
        if (currentHealth + valueToAdd < health)
            this.currentHealth += valueToAdd;
        else
            this.currentHealth = health;

        // Richiamo evento per aggiornamento interfaccia -> vedere in InGameUIEvents
        if (OnHealthChanged != null) {
            OnHealthChanged(this, EventArgs.Empty);
        }
    }

    public void TakeDamage(float damage) { // Implementazione dell'interfaccia IDamageable
        if (!invincible) { // Se non e' invincibile
            currentHealth -= damage;
            invincible = true; // Invincibile

            // Richiamo evento per aggiornamento interfaccia -> vedere in InGameUIEvents
            // Anche se viene richiamato da enemy non va ad intaccare player in quanto in "InGameUIEvents.cs"
            // riferisco solamente al HealthSystem del player!
            if (OnHealthChanged != null) {
                OnHealthChanged(this, EventArgs.Empty);
            }

            // Some damage effect

            // interpolazione colore da base a rosso
            entityRenderer.color = damageColor;

            if (currentHealth <= 0) {
                Debug.Log(gameObject.name + " is dead");
                if (gameObject.GetComponent<Enemy>() != null) {
                    // Cerco player nella scena e aggiungo esperienza
                    var player = GameObject.FindGameObjectWithTag("Player").transform;
                    if (player != null) {
                        player.GetComponent<PlayerLevelSystem>().AddExperience(xpToGiveAfterDeath);
                    }

                    gameObject.GetComponent<Enemy>().EnemyDeath(); // Cosa fare alla morte e' specificato qui
                }
                else if (gameObject.GetComponent<PlayerController>() == null) { // se non e' il player
                    Destroy(gameObject);
                }
            }
        }
    }
}
