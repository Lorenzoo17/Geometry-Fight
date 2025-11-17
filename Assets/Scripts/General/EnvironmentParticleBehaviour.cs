using UnityEngine;

public class EnvironmentParticleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject particleToSpawn;
    [SerializeField] private float spawnRate;
    private Transform targetTransform;
    private float currentSpawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        try {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }catch(System.Exception) {
            targetTransform = transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSpawnTime <= 0) {

            // Spawno nella posizone del player
            GameObject effect = Instantiate(particleToSpawn, targetTransform.position, Quaternion.identity);
            currentSpawnTime = spawnRate;
        }
        else {
            currentSpawnTime -= Time.deltaTime;
        }
    }
}
