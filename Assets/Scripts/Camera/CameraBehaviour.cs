using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform followTarget; // che e' il player
    private Vector3 desiredPosition;
    [SerializeField] private float interpolationValue;
    [SerializeField] private float followOffsetZ;

    private float currentShakeTime; // Tempo di shake, che viene assegnato quando viene richiamato il metodo per lo shaking
    private float currentShakeAmount;

    private Vector2 shakeOffset; // offset calcolato per lo shake
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followTarget = GameObject.FindGameObjectWithTag("Player").transform; // si prende oggetto con tag player
    }

    private void Update() {
        ShakeBehaviour();
    }

    private void LateUpdate() {
        desiredPosition = Vector2.Lerp(transform.position, followTarget.position, Time.deltaTime * interpolationValue);

        transform.position = desiredPosition + new Vector3(0, 0, followOffsetZ) + (Vector3)shakeOffset; 
    }

    private void ShakeBehaviour() {
        if (currentShakeTime > 0) { // Se > 0 (quindi se viene eseguito MakeCameraShake)
            shakeOffset = Random.insideUnitSphere * currentShakeAmount;
            currentShakeTime -= Time.deltaTime;
        }
        else {
            currentShakeTime = 0;
            shakeOffset = Vector3.zero; // resetto offset
        }
    }

    public void MakeCameraShake(float shakeTime = 0.1f, float shakeAmount = 0.4f) {
        currentShakeTime = shakeTime;
        currentShakeAmount = shakeAmount;
    }
}
