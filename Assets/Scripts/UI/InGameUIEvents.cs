using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUIEvents : MonoBehaviour {

    public static InGameUIEvents Instance;

    private UIDocument uiDocument;
    private ProgressBar healthBar;
    private ProgressBar staminaBar;
    private GameObject player;
    private HealthSystem playerHealthSystem;
    private Label effectText;
    private ProgressBar levelBar;
    private Label levelText;
    private PlayerLevelSystem playerLevelSystem;
    [SerializeField] private Color effectTextColor;
    [SerializeField] private float effectTextSize;
    [SerializeField] private float effectTextShowTime;
    private Coroutine ActiveEffectTextCoroutine;

    private VisualElement effectIconsContainer; // barra con le icone relative agli effetti attivi al momento

    [Header("UIelements names")]
    [SerializeField] private string healthBar_name = "HealthBar";
    [SerializeField] private string staminaBar_name = "StaminaBar";
    [SerializeField] private string effectText_name = "EffectText";
    [SerializeField] private string effectIconsContainerName = "EffectIconsContainer";
    [SerializeField] private string levelBar_name = "LevelBar";
    [SerializeField] private string levelText_name = "LevelText";

    private void Awake() {
        uiDocument = GetComponent<UIDocument>();
        healthBar = uiDocument.rootVisualElement.Q(healthBar_name) as ProgressBar;
        staminaBar = uiDocument.rootVisualElement.Q(staminaBar_name) as ProgressBar;
        effectText = uiDocument.rootVisualElement.Q(effectText_name) as Label;
        effectIconsContainer = uiDocument.rootVisualElement.Q<VisualElement>(effectIconsContainerName);
        levelBar = uiDocument.rootVisualElement.Q(levelBar_name) as ProgressBar;
        levelText = uiDocument.rootVisualElement.Q(levelText_name) as Label;

        Instance = this;
    }

    private void Start() {
        // Debug per controllare corretto funzionamento
        // if (healthBar != null) {
        //     Debug.Log(healthBar.name);
        // }
        float startingValue = 100f;
        healthBar.value = startingValue; // Imposto valore iniziale a 100

        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        
        if (player != null && player.TryGetComponent<HealthSystem>(out HealthSystem playerHealth)) {
            // Se il player e' stato trovato correttamente
            playerHealthSystem = playerHealth;
            playerHealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged; // Quando richiamo l'evento in HealhSystem, voglio che anche questo metodo
                                                                                // venga aggiunto
            playerLevelSystem = player.GetComponent<PlayerLevelSystem>();
            playerLevelSystem.OnLevelUp += InGameUIEvents_OnLevelUp;
            playerLevelSystem.OnXpChange += InGameUIEvents_OnXpChange;
        }

        effectText.style.color = new Color(effectTextColor.r, effectTextColor.g, effectTextColor.b, 0);
    }

    private void Update() {
        if (player != null && player.TryGetComponent<PlayerController>(out PlayerController playerController)) {
            if (staminaBar != null) {
                staminaBar.value = playerController.GetTimeBtwDashPerc();
            }
        }
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
        if(healthBar != null) {
            healthBar.value = playerHealthSystem.GetHealthPercentage();
        }
    }

    private void InGameUIEvents_OnXpChange(object sender, EventArgs e) {
        if(levelBar != null && player != null) {
            levelBar.value = playerLevelSystem.GetCurrentXpPerc();
        }
    }

    private void InGameUIEvents_OnLevelUp(object sender, EventArgs e) {
        if (levelText != null && player != null) {
            levelText.text = "LEVEL " + playerLevelSystem.GetCurrentLevel.ToString();
            // Aggiorno anche barra
            InGameUIEvents_OnXpChange(sender, e);
        }
    }

    // NECESSARIO RIMUOVERE ICONE QUANDO EFFETTO FINISCE!
    public void AddEffectIcon(Sprite sprite, Color color, float effectDuration) {
        if (sprite == null || effectDuration <= 0) return; // Se ad esempio e' effetto di cura (in realta' messo esplicitamente in ItemInteractable), manco mostro l'icona
        var icon = new VisualElement(); // creo nuova icon da aggiungere
        icon.AddToClassList("effect-icon"); // le assegno la classe uss

        icon.style.backgroundImage = new StyleBackground(sprite.texture); // metto come background lo sprite
        icon.style.unityBackgroundImageTintColor = color; // Assegno il colore corretto all'immagine di background
        effectIconsContainer.Add(icon);// Aggiungo nuovo elemento

        StartCoroutine(RemoveIcon(icon, effectDuration)); // Rimuovo l'icona dopo effect duration    
    }

    public void ShowEffectText(string effectName) { 
        // In modo che non si buggi quando si prendono piu' oggetti di seguito
        if (ActiveEffectTextCoroutine != null)  // Se e' gia' presente una coroutine (un effetto sta gia' venendo visualizzato)
            StopCoroutine(ActiveEffectTextCoroutine); // fermo la coroutine corrente

        // mostro il nuovo effetto
        ActiveEffectTextCoroutine = StartCoroutine(ShowEffectTextTemporary(effectName));
    }

    // In IEnumerator le parti prima e dopo WaitForSeconds vengono eseguite solo 1 volta!
    // Quindi per farle eseguire per piu' frame e' necessario usare ad esempio un while
    private IEnumerator ShowEffectTextTemporary(string effectName) {
        effectText.text = effectName; // Cambio testo
        // Quando il fade della scritta deve durare
        float elapsed = 0f;
        float fadeDuration = 0.5f;

        Color startColor = new Color(effectTextColor.r, effectTextColor.g, effectTextColor.b, 0);
        Color endColor = effectTextColor;

        while (elapsed < fadeDuration) {
            effectText.style.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration); // Interpolazione del colore
            elapsed += Time.deltaTime;
            yield return null; // aspetta frame successivo
        }

        yield return new WaitForSeconds(effectTextShowTime);

        // Lo riporto "invisibile"
        elapsed = 0f;
        while (elapsed < fadeDuration) {
            effectText.style.color = Color.Lerp(endColor, startColor, elapsed / fadeDuration); // Interpolazione del colore
            elapsed += Time.deltaTime;
            yield return null; // aspetta frame successivo
        }

        effectText.style.color = startColor; // Per sicurezza
        ActiveEffectTextCoroutine = null; // Si setta a null per sicurezza, ma non necessario
    }

    private IEnumerator RemoveIcon(VisualElement iconToRemove, float duration) {
        yield return new WaitForSeconds(duration); // Si aspetta duration

        if (effectIconsContainer.Contains(iconToRemove)) {
            effectIconsContainer.Remove(iconToRemove); // Si rimuove l'icona
        }
    }
}
