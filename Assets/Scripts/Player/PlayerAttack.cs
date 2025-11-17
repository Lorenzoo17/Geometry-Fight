using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPositionTransform; // transform che fa riferimento al punto in cui
    // istanziare i bullet
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletRange; // espresso come tempo dallo sparo
    [SerializeField] private float playerDamage;

    [SerializeField] private float fireRate = 0.2f;
    private float currentFireRate;

    private void Attack(Vector2 mouseDirection) {
        // Debug.Log(mouseDirection);
        // direzione di attacco -> posizione mouse - posizione punto di attacco (o posizione player)
        Vector2 attackDirection = mouseDirection; // inutile, solo per chiarezza

        // spawn proiettile in certa posizione (definita da transform)!
        GameObject bullet = Instantiate(bulletPrefab, attackPositionTransform.position, Quaternion.identity);
        // definizione direzione del proiettile in base a direzione del mouse e aggiornamneto del rigidbody
        // settaggio eventuali parametri del bullet accedendo al suo script
        // ...

        if (bullet.TryGetComponent<BaseBullet>(out BaseBullet bulletBehaviour)) { // se ha script base bullet o eventuale sottoclasse
            bulletBehaviour.SetUpBullet(bulletRange, attackDirection, playerDamage, bulletSpeed);
        }
        // Movimento del bullet gestito direttamente nell'update del bullet
    }

    private void Update() {
        if (this.GetComponent<PlayerController>().IsAttacking && currentFireRate <= Time.time) {
            Attack(this.GetComponent<PlayerController>().GetMouseDirection);
            currentFireRate = Time.time + fireRate;
        }
    }

    public void UpdatePlayerAttack(float value) {
        playerDamage += value;

        AvoidNegativeValue(playerDamage);
    }

    public void UpdatePlayerRange(float value) {
        bulletRange += value;

        AvoidNegativeValue(bulletRange);
    }

    public void UpdatePlayerAttackSpeed(float value) {
        bulletSpeed += value;

        AvoidNegativeValue(bulletSpeed);
    }

    public void UpdateFireRate(float value) {
        fireRate -= value; // Meno e' meglio e'

        AvoidNegativeValue(fireRate);
    }

    private void AvoidNegativeValue(float parameter) {
        if(parameter < 0) {
            parameter = 0;
        }
    }

    // In modo da richiamere i metodi in modo generico, vale solo per i metodi che accettano stesso argomento
    // Richiamato in sottoclassi di ItemData
    public void ApplyTemporaryEffect(float value, float duration, Action<float> method) {
        StartCoroutine(TemporaryEffect(value, duration, method));
    }

    private IEnumerator TemporaryEffect(float value, float duration, Action<float> method) {
        method.Invoke(value);
        yield return new WaitForSeconds(duration);
        method.Invoke(-value);
    }
}
