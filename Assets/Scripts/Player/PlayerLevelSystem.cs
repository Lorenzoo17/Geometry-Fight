using System;
using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    public event EventHandler OnLevelUp;
    public event EventHandler OnXpChange; 

    [SerializeField] private int currentLevel; // Livello corrente
    [SerializeField] private float currentXp;
    [SerializeField] private float xpRequiredForNextLevel;
    [SerializeField] private float xpGrowRate; // moltiplicatore XP richiesto per livello successivo

    private PlayerController playerController;
    private PlayerAttack playerAttack;

    public float GetCurrentXpPerc() { // Usato in InGameUIEvents
        return (currentXp / xpRequiredForNextLevel) * 100;
    }
    public int GetCurrentLevel => currentLevel;

    private void Start() {
        if(TryGetComponent<PlayerController>(out PlayerController pc)) {
            playerController = pc;
        }
        else {
            Debug.LogWarning("LEVEL UP - Script PlayerController not found!");
        }
        if (TryGetComponent<PlayerAttack>(out PlayerAttack pa)) {
            playerAttack = pa;
        }
        else {
            Debug.LogWarning("LEVEL UP - Script PlayerAttack not found!");
        }
    }

    public void AddExperience(float experienceGot) {
        currentXp += experienceGot;

        // Gestione multi livello -> per questo whilw
        while (currentXp >= xpRequiredForNextLevel) { // Se ho livellato
            currentXp -= xpRequiredForNextLevel; // resetto currentXp in base a esperienza in eccesso
            currentLevel++;
            StatsUpdateOnLevelUp(); // aggiorno statistiche
            xpRequiredForNextLevel *= xpGrowRate; // Incremento xp necessari per prossimo livello

            if(Camera.main.TryGetComponent<CameraBehaviour>(out CameraBehaviour camera)) {
                camera.MakeCameraShake(); // Camera shake
            }

            OnLevelUp?.Invoke(this, EventArgs.Empty); // invoco in modo da poter aggiornare UI (vedi InGameUIEvents)
        }


        OnXpChange?.Invoke(this, EventArgs.Empty);// ?.Invoke -> per evitare eccezioni se non e' stato aggiunto da nessuna parte
    }

    private void StatsUpdateOnLevelUp() { // Metodo che contiene le statistiche che aumentano al level up
        // NECESSARIO METTERE LIMIT A INCREMENTO STATS -> sarebbe da fare script PlayerStats
        // che le contiene -> con una mappa o array parallelo per i limiti

        float speedIncrease = 0.05f;
        float fireRateIncrease = 0.035f; // o 0.05
        float rangeIncrease = 0.1f;
        float attackSpeedIncrease = 0.1f;
        float damageIncrease = 0.2f;

        float multiplier = 1.1f;

        playerController.UpdatePlayerSpeed(multiplier * speedIncrease);
        playerAttack.UpdateFireRate(multiplier * fireRateIncrease);
        playerAttack.UpdatePlayerRange(multiplier * rangeIncrease);
        playerAttack.UpdatePlayerAttackSpeed(multiplier * attackSpeedIncrease);
        playerAttack.UpdatePlayerAttack(multiplier * damageIncrease);
    }
}
