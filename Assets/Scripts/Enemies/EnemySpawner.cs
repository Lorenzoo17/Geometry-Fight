using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Array di gameobjects relativi alle possibili stats dei nemici")]
    [SerializeField] private GameObject[] enemiesType;
    [SerializeField] private float initialEnemySpawnRate;
    private float enemySpawnRate;
    private float timeBtwSpawn;
    private Transform playerReference;
    private PlayerLevelSystem playerLevelSystem;

    [Tooltip("Numero base di nemici spawnati per volta al livello 1")]
    [SerializeField] private int initialEnemiesToSpawnAtOnce = 1;
    private int enemiesToSpawnAtOnce;

    [Tooltip("Distanza massima dalla quale il nemico spawna rispetto al player")]
    [SerializeField] private float maxSpawnDistance;
    [Tooltip("Distanza minima dalla quale il nemico spawna rispetto al player")]
    [SerializeField] private float minSpawnDistance;

    [SerializeField] private GameObject spawnEffect;

    public void UpdateEnemySpawnRate(float value) { // Per modificare dall'esterno l'enemySpawnRate
        this.enemySpawnRate += value; //
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("Player").transform;
        playerLevelSystem = playerReference.GetComponent<PlayerLevelSystem>();
        playerLevelSystem.OnLevelUp += PlayerLevelSystem_OnLevelUp;

        enemiesToSpawnAtOnce = initialEnemiesToSpawnAtOnce;
        enemySpawnRate = initialEnemySpawnRate;
    }

    private void PlayerLevelSystem_OnLevelUp(object sender, System.EventArgs e) { // Quando il player sale di livello si sottoscrive l'evento con questo
        int level = playerLevelSystem.GetCurrentLevel;
        UpdateSpawnParameters(level); // Aggiorno parametri dello spawn rate
    }

    private void UpdateSpawnParameters(int level) {
        // Livello massimo del player 10 !

        // Si scala spawnrate in base a livello
        float minSpawnRate = 1f; // minimo un nemico ogni secondo
        float maxSpawnRate = initialEnemySpawnRate;
        enemySpawnRate = Mathf.Lerp(maxSpawnRate, minSpawnRate, (level - 1) / 9f);

        // Si scala numero massimo di nemici in base a spawnrate
        int minEnemies = initialEnemiesToSpawnAtOnce;
        int maxEnemies = 4; // Massimo 4 nemici alla volta
        enemiesToSpawnAtOnce = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, (level - 1) / 9f));
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBtwSpawn <= 0) {
            EnemySpawn();
            timeBtwSpawn = enemySpawnRate;
        }
        else {
            timeBtwSpawn -= Time.deltaTime;
        }
    }

    private void EnemySpawn() {
        int enemiesToSpawn = Random.Range(1, enemiesToSpawnAtOnce); // In modo da non spawnare sempre il numero massimo

        for (int i = 0; i < enemiesToSpawn; i++) {
            Vector3 playerPosition = playerReference.position;
            // Al fine di evitare di far spawnare il nemico troppo vicino al player
            // Non posso semplicemente fare un random.range da -distance,distance --> perche potrei beccare
            // esattamente la posizione del player.
            // Quindi: definisco un angolo (uso come origine la posizione del player)
            // Trovo la distanza di spawn, da min a max
            // calcolo il punto (x, y) -> (cos(angle), sin(angle))
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            float angle = Random.Range(0f, 2 * Mathf.PI); // angolo casuale da 0 a 2pi

            // ottengo il punto unitario (cos(), sin()) e lo moltiplico per distance
            // Sommo a playerPosition in quanto voglio quello come origine
            Vector3 spawnPosition = playerPosition + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            // estraggo casualmente il nemico da spawnare
            int rnd = Random.Range(0, enemiesType.Length);
            GameObject type = enemiesType[rnd];
            // Spawno
            GameObject newEnemy = Instantiate(type, spawnPosition, Quaternion.identity);

            // Moltiplicatore per le statistiche dei nemici in base all'avanzamento del player
            float multiplier = 1f + (playerLevelSystem.GetCurrentLevel - 1) * 0.10f;

            newEnemy.GetComponent<Enemy>().ScaleStats(multiplier); // Scalo le stats in base al multiplier

            // Debug.Log(newEnemy.name + "Spawned at position : " + spawnPosition.ToString());

            // Effect spawn nella posizione del nuovo nemico
            if (spawnEffect != null) {
                GameObject effect = Instantiate(spawnEffect, newEnemy.transform.position, Quaternion.identity);
            }
        }
    }
}
