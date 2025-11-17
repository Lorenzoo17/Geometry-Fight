using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "New restore health", menuName = "ScriptableObjects/Items/RestoreHealth")]
public class RestoreHealth_Item : ItemData {

    public float healthToRestore;
    public override void ItemEffect() {
        InGameUIEvents.Instance.ShowEffectText(this.effectName + " +" + healthToRestore);

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Cerco player

        if(player != null) {
            if(player.TryGetComponent<HealthSystem>(out HealthSystem playerHealth)) {
                playerHealth.RestoreHealth(healthToRestore); // Ripristino la vita
            }
        }
    }
}
