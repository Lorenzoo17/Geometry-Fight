using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private string menuSceneName;
    [SerializeField] private GameObject victoryUI;

    [SerializeField] private int levelToReach;

    private Transform player;
    private HealthSystem playerHealthSystem;
    private PlayerLevelSystem playerLevelSystem;

    private PlayerInputActions inputActions;

    private void Awake() {
        inputActions = new PlayerInputActions();

        inputActions.UI.Enable();
        inputActions.UI.Restart.performed += Restart_performed;
        inputActions.UI.Escape.performed += Escape_performed;
        inputActions.UI.Menu.performed += Menu_performed;
    }

    private void Start() {
        Instance = this;

        PauseUI.SetActive(false);
        GameOverUI.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null) {
            if (player.TryGetComponent<HealthSystem>(out HealthSystem playerHealth)) {
                playerHealthSystem = playerHealth;
                playerHealthSystem.OnHealthChanged += GameController_OnHealthChanged_Player; // Sovrascrittura
            }

            if (player.TryGetComponent<PlayerLevelSystem>(out PlayerLevelSystem playerLevel)) {
                playerLevelSystem = playerLevel;
                playerLevelSystem.OnLevelUp += PlayerLevelSystem_OnLevelUp;
            }
        }
    }

    private void PlayerLevelSystem_OnLevelUp(object sender, EventArgs e) {
        if (playerLevelSystem.GetCurrentLevel >= levelToReach) { // Se raggiungo livello "levelToReach"
            victoryUI.SetActive(true); // Attivo finestra di vittoria

            Time.timeScale = 0; // Fermo il gioco
        }
    }


    // In modo da evitare controllo ad ogni frame
    private void GameController_OnHealthChanged_Player(object sender, EventArgs e) {
        if (playerHealthSystem != null) {
            Debug.Log("check");
            if (playerHealthSystem.GetCurrentHealth() <= 0) { // Se player morto
                Debug.Log("Morto");
                GameOverUI.SetActive(true);

                Time.timeScale = 0; // metto in pausa il gioco
            }
        }
    }


    private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (PauseUI.activeSelf) {
            PauseUI.SetActive(false); // Tolgo finestra di pausa

            Time.timeScale = 1; // Faccio ripartire gioco
        }
        else {
            if (GameOverUI.activeSelf) return; // Non posso aprire pausa se c'e' gameover

            PauseUI.SetActive(true);

            Time.timeScale = 0; // Blocco il gioco
        }
    }

    private void Restart_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (PauseUI.activeSelf) { // Se sono in pausa
            PauseUI.SetActive(false); // Tolgo finestra di pausa

            Time.timeScale = 1; // Faccio ripartire gioco
        }
        if (GameOverUI.activeSelf || victoryUI.activeSelf) { // Se sono in gameOver
            // Ricarico questa scena
            Time.timeScale = 1;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    private void Menu_performed(InputAction.CallbackContext context) {
        Time.timeScale = 1;

        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }

    private void OnDisable() {
        inputActions.UI.Disable();
    }
}
