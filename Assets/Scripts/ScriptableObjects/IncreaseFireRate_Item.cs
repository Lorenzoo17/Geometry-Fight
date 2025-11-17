using UnityEngine;

[CreateAssetMenu(fileName = "New increase fire rate", menuName = "ScriptableObjects/Items/IncreaseFireRate")]
public class IncreaseFireRate_Item : ItemData {
    public float rateReduction;
    public override void ItemEffect() {
        InGameUIEvents.Instance.ShowEffectText(this.effectName + " -" + rateReduction);

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Cerco player

        if (player != null) {
            if (player.TryGetComponent<PlayerAttack>(out PlayerAttack playerAttack)) {
                //playerAttack.UpdateFireRate(rateReduction); // Incremento fire rate (si riduce di quanto specificato)
                // In questo modo aumento solo per tot tempo
                playerAttack.ApplyTemporaryEffect(rateReduction, effectDuration, playerAttack.UpdateFireRate);
            }
        }
    }
}
