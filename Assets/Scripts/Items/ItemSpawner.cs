using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public static ItemSpawner Instance;

    [Tooltip("Array di scriptableobjects relativi ai tipi di oggetti")]
    [SerializeField] private ItemData[] itemTypes;
    [SerializeField] private GameObject itemPrefab; // prefab dell'item
    [SerializeField] private float spawnRate;
    private float timeBtwSpawn;

    [Tooltip("Distanza massima dalla quale l'oggetto spawna rispetto al player")]
    [SerializeField] private float maxSpawnDistance;
    [Tooltip("Distanza minima dalla quale l'oggetto spawna rispetto al player")]
    [SerializeField] private float minSpawnDistance;
    private Transform player;

    [SerializeField] private int maxItemsInSceneAtOnce;
    private bool canSpawn; // Solo se 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeBtwSpawn = spawnRate;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBtwSpawn <= 0) {
            int numberOfItems = FindObjectsByType<ItemInteractable>(FindObjectsSortMode.None).Length;
            // Spawna solo se il numero di oggetti presenti e' inferiore a certo numero
            if(numberOfItems < maxItemsInSceneAtOnce) {
                SpawnItemRandomPosition();
                timeBtwSpawn = spawnRate;
            }
        }
        else {
            timeBtwSpawn -= Time.deltaTime;
        }
    }

    private void SpawnItemRandomPosition() {

        // Vedere EnemySpawner.cs per maggiore chiarezza!
        float distance = UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI); // angolo casuale da 0 a 2pi
        Vector3 spawnPosition = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        // Spawno oggetto
        SpawnItem(spawnPosition);
    }

    public void SpawnItem(Vector3 position) { // Richiamato anche alla morte dei nemici
        int rnd = UnityEngine.Random.Range(0, itemTypes.Length);
        ItemData itemDataChosen = itemTypes[rnd]; // Scelgo casualmente un tipo di oggetto
        Debug.Log(rnd + "; " + itemDataChosen.name);

        // spawno oggetto
        GameObject newItem = Instantiate(itemPrefab, position, Quaternion.identity);
        if (newItem.GetComponent<ItemInteractable>() == null) {
            newItem.AddComponent<ItemInteractable>();
        }

        newItem.GetComponent<ItemInteractable>().SetItemData(itemDataChosen);
    }
}
