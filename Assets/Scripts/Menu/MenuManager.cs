using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private string gameSceneName;

    private void Start() {
        playButton.onClick.AddListener(() => {
            SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        });

        exitButton.onClick.AddListener(() => {
            Application.Quit(); // Esce dall'applicazione
        });
    }
}
