using UnityEngine;

[CreateAssetMenu(fileName = "New shield", menuName = "ScriptableObjects/Items/Shield")]
public class SpawnShield_Item : ItemData {

    public GameObject shieldPrefab;
    public float shieldSize;
    // public float invincibilityTime;
    public float shieldHealth;
    public float contactDamage;

    // Qui uso direttamente effectDuration cosi, non c'e' bisogno di richiamare le coroutine
    public override void ItemEffect() {
        InGameUIEvents.Instance.ShowEffectText(this.effectName + " +" + effectDuration + " s");

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Cerco player

        if (player != null) {
            if(player.GetComponentInChildren<ShieldBehaviour>() != null) {// Se ha gia' uno scudo
                GameObject playerCurrentShield = player.GetComponentInChildren<ShieldBehaviour>().gameObject;

                Destroy(playerCurrentShield); // Distruggo lo scudo corrente (per spawnare poi quello nuovo)
            }

            // Spawno lo scudo
            GameObject shiedObject = Instantiate(shieldPrefab, player.transform.position, Quaternion.identity);

            if (shiedObject.TryGetComponent<ShieldBehaviour>(out ShieldBehaviour shield)) {
                shield.SetShieldStats(shieldHealth, shieldSize, contactDamage, player.transform);
            }

            Destroy(shiedObject, effectDuration); // Distruggo lo scudo dopo shieldDuration
        }
    }
}
