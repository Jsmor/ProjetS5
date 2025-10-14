using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // à assigner dans l'Inspector (le component Inventory du Player)
    public Slot[] slots;        // 10 éléments assignés dans l'Inspector (Slot_0 -> Slot_9)

    /// <summary>
    /// Met à jour graphiquement les slots. Appelle cette méthode depuis Inventory quand quelque chose change.
    /// Cette version est très tolérante et logge les erreurs pour te permettre de debuger.
    /// </summary>
    public void UpdateGraphics()
    {
        try
        {
            Debug.Log("[InventoryUI] UpdateGraphics start");

            if (inventory == null)
            {
                Debug.LogError("[InventoryUI] inventory reference is null ! Assign it in the Inspector.");
                return;
            }

            if (inventory.inventory == null)
            {
                Debug.LogError("[InventoryUI] inventory.inventory is null !");
                return;
            }

            if (slots == null || slots.Length == 0)
            {
                Debug.LogError("[InventoryUI] slots array is null or empty! Assign slot elements in the Inspector.");
                return;
            }

            int count = Mathf.Min(slots.Length, inventory.inventory.Length);

            for (int i = 0; i < count; i++)
            {
                // defensive checks per-slot
                if (slots[i] == null)
                {
                    Debug.LogWarning($"[InventoryUI] slots[{i}] is null.");
                    continue;
                }

                // icon
                if (slots[i].icon == null)
                {
                    Debug.LogWarning($"[InventoryUI] slots[{i}].icon is null.");
                }
                else
                {
                    var item = inventory.inventory[i];
                    slots[i].icon.enabled = (item != null);
                    if (item != null)
                        slots[i].icon.sprite = item.icon;
                }

                // highlight (on active le GameObject du highlight si sélection)
                if (slots[i].highlight == null)
                {
                    Debug.LogWarning($"[InventoryUI] slots[{i}].highlight is null.");
                }
                else
                {
                    bool shouldHighlight = (i == inventory.GetSelectedIndex());
                    // Utilise SetActive sur le GameObject (plus fiable pour l'UI)
                    if (slots[i].highlight.gameObject.activeSelf != shouldHighlight)
                        slots[i].highlight.gameObject.SetActive(shouldHighlight);
                }
            }

            // si slots.Length > inventory.inventory.Length, désactive highlights restants proprement
            for (int i = count; i < slots.Length; i++)
            {
                if (slots[i] != null && slots[i].highlight != null && slots[i].highlight.gameObject.activeSelf)
                    slots[i].highlight.gameObject.SetActive(false);
                if (slots[i] != null && slots[i].icon != null && slots[i].icon.enabled)
                    slots[i].icon.enabled = false;
            }

            Debug.Log("[InventoryUI] UpdateGraphics done");
        }
        catch (Exception ex)
        {
            Debug.LogError("[InventoryUI] Exception dans UpdateGraphics : " + ex + "\nVérifie que Inventory et tous les éléments Slot sont assignés.");
        }
    }
}
