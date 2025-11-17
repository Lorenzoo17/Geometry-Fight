using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.OnInteract(); // A seguito di interazione si esegue il metodo
        }
    }
}
