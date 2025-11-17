using UnityEngine;

[CreateAssetMenu(fileName = "New increase speed item", menuName = "ScriptableObjects/Items/IncreaseSpeed")]
public class IncreaseSpeed_Item : ItemData {
    public float speedToAdd;
    public override void ItemEffect() {
        InGameUIEvents.Instance.ShowEffectText(this.effectName + " +" + speedToAdd);

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Cerco player

        if (player != null) {
            if (player.TryGetComponent<PlayerController>(out PlayerController controller)) {
                // controller.UpdatePlayerSpeed(speedToAdd); // Incremento velocita'
                // Per aumentare velocita' temporaneamente
                controller.ApplyTemporaryEffect(speedToAdd, effectDuration, controller.UpdatePlayerSpeed);
            }
        }
    }
}
