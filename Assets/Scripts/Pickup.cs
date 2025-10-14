using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Item item; // L'objet que ce pickup représente

    private void OnTriggerStay(Collider other)
    {
        // Vérifie si c'est le joueur
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.K))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && item != null)
            {
                inventory.AddItem(item); // Ajoute à l'inventaire
                Destroy(gameObject); // Supprime l'objet de la scène
            }
        }
    }
}
