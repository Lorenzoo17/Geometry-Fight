using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input actions
    private PlayerInputActions playerInputActions;

    // Richiamate da altri scripts
    public UnityAction<Vector2> playerAttackAction; // Viene usata in modo da non dover gestire in PlayerAttack la lettura degli input!

    private Rigidbody2D rb;

    // Movement
    private Vector2 movement;
    [SerializeField] private float playerSpeed;
    private Vector2 mousePosition;
    private Vector2 mouseDirection;

    [SerializeField] private float dashDistance = 1f;
    [SerializeField] private float dashDuration = 0.2f; // influenza solo l'animazione del lerping
    [SerializeField] private float dashCooldownTime = 1f;
    private Vector2 dashDirection;
    private Vector2 dashTargetPosition;
    private float dashElapsedTime;
    private float timeBtwDash;
    private bool isDashing;
    private bool canDash;

    // dash effect
    private float dashEffectSpawnRate = 0.01f;
    private float timeBtwDashEffect;

    [SerializeField] private float rotationOffset;

    // Attack
    private bool isAttacking;

    #region Get and Set

    public bool IsAttacking => isAttacking; // Richiamato in PlayerAttack
    public Vector2 GetMouseDirection => mouseDirection; // Richiamato in PlayerAttack

    public float GetTimeBtwDashPerc() { // Richiamato per la staminaBar in InGameUIEvents.cs -> visto che cambia ad ogni 
        // frame non ha senso usare un evento come per la vita
        // in modo da avere timeBtwDash tra 0 e 100
        return (dashCooldownTime - timeBtwDash) / dashCooldownTime * 100.0f;
    }

    #endregion

    #region ItemsRelated
    public void UpdatePlayerSpeed(float value) {
        playerSpeed += value; // Usato in IncreaseSpeed_Item
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
    #endregion

    #region Awake, Start, Update
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable(); // Si abilita l'input action relativo al player 

        // Movement
        playerInputActions.Player.Movement.performed += Movement_performed; // Al verificarsi dell'evento movement
        // si richiama in automatico la funzione riportata
        playerInputActions.Player.Movement.canceled += Movement_canceled;

        // Mouse movement
        playerInputActions.Player.MousePosition.performed += MousePosition_performed;

        // Mouse attack
        playerInputActions.Player.Attack.started += obj => isAttacking = true;
        playerInputActions.Player.Attack.canceled += obj => isAttacking = false;

        // Dash
        playerInputActions.Player.Dash.performed += _ => StartDash();
    }

    private void Update() {
        // Rotazione del player sulla base della posizione del mouse (attorno ad asse z)
        ManagePlayerRotation();

        if (!canDash) { // Da quando premo il pulsante
            timeBtwDash -= Time.deltaTime;
            if (timeBtwDash <= 0)
                canDash = true;
        }

        if (isDashing) { // Se sta effettuando il dash
            if(timeBtwDashEffect <= 0) {
                DashEffectSpawn(); // spawno effetto
                timeBtwDashEffect = dashEffectSpawnRate;
            }
            else {
                timeBtwDashEffect -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate() { // ad ogni frame "della fisica"
        // rb.linearVelocity = movement * playerSpeed;
        if(isDashing) {
            dashElapsedTime += Time.fixedDeltaTime;
            float t = dashElapsedTime / dashDuration; // Controllo il lerping tramite la durata del dash, 

            // Destinazione --> da posizione attuale a dove punta movement (scalato per la distanza del dash)
            Vector2 dashTargetPosition = rb.position + dashDirection * dashDistance;
            // Per avere cambio di posizione smooth (in questo caso non ho problemi di collisioni)
            Vector2 desiredPosition = Vector2.Lerp(rb.position, dashTargetPosition, t);

            rb.MovePosition(desiredPosition); // Mi muovo nel

            if (t >= 1f) { // quando il lerp e' finito imposto a false dashing
                isDashing = false;
            }

            return; // in modo che, se sto dashando, il codice di MovePosition qui sotto non venga eseguito
            // per evitare movimenti insieme al dash
        }
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * playerSpeed);
    }

    #endregion

    // Metodi
    #region private useful methods 
    private void StartDash() {
        if (canDash) { // Se il cooldown e' terminato
            canDash = false; // non puo' dashare
            isDashing = true; // inizio del dash
            dashElapsedTime = 0f;
            timeBtwDash = dashCooldownTime; // resetto cooldown

            // Imposto direzione qui in modo che non possa essere aggiornata duraente il lerp
            dashDirection = movement != Vector2.zero ? movement.normalized : mouseDirection;
            dashTargetPosition = rb.position + dashDirection * dashDistance; // posizione finale
        }
    }

    private void ManagePlayerRotation() {
        // Conversione posizione mouse da schermo a mondo
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        // Calcolo direzione tra il player e il mouse
        Vector2 direction = (worldMousePos - transform.position).normalized;
        mouseDirection = direction;

        float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
        // Applica rotazione al player
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    private void DashEffectSpawn() {
        SpriteRenderer playerRenderer = this.GetComponentInChildren<SpriteRenderer>();
        if (playerRenderer != null) {
            GameObject dashEffect = new GameObject("dashEffect"); // cosi' c'e' anche lo spawn
            dashEffect.AddComponent<SpriteRenderer>();
            dashEffect.GetComponent<SpriteRenderer>().sprite = playerRenderer.sprite;

            float dashEffectAlpha = 0.05f;
            dashEffect.GetComponent<SpriteRenderer>().color = new Color(playerRenderer.color.r,
                playerRenderer.color.g, playerRenderer.color.b, dashEffectAlpha);

            dashEffect.AddComponent<DestroyAfterTime>();
            dashEffect.GetComponent<DestroyAfterTime>().SetLifeTime(0.5f);
            dashEffect.transform.position = transform.position;
            dashEffect.transform.rotation = transform.rotation;

            // Per sicurezza nel rendering
            dashEffect.GetComponent<SpriteRenderer>().sortingLayerID = playerRenderer.sortingLayerID;
            dashEffect.GetComponent<SpriteRenderer>().sortingOrder = playerRenderer.sortingOrder - 1; // dietro il player
        }
    }
    #endregion

    // CallBacks input system
    #region callback for input system
    private void Movement_canceled(InputAction.CallbackContext obj) { // quando non viene premuto un tasto relativo al movimento, si resetta
        movement = Vector2.zero;
    }

    private void Movement_performed(InputAction.CallbackContext obj) {
        movement = obj.ReadValue<Vector2>();
    }

    private void MousePosition_performed(InputAction.CallbackContext obj) {
        mousePosition = obj.ReadValue<Vector2>();
    }

    private void OnDisable() {
        playerInputActions.Player.Disable();
    }
    #endregion
}
