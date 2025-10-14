using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Item[] inventory = new Item[10];       // inventaire de 10 cases 
    private int selectedIndex = 0;
    public InventoryUI UI;

    void Update()
    {
        HandleScroll();
        DisplaySelectedItem();
        HandleDrop();
    }

    // Ajoute un objet dans le premier slot vide
    public void AddItem(Item newItem)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = newItem;
                Debug.Log(newItem.itemName + " ajouté à l'inventaire !");
                return;
            }
        }
        Debug.Log("Inventaire plein !");
    }

    // Gérer la molette pour changer de slot
    void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            selectedIndex++;
            if (selectedIndex >= inventory.Length) selectedIndex = 0;
            if (UI != null) UI.UpdateGraphics();
        }
        else if (scroll < 0f)
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = inventory.Length - 1;
            if (UI != null) UI.UpdateGraphics();
        }
    }


    // Affiche dans la console le slot sélectionné
    void DisplaySelectedItem()
    {
        if (inventory[selectedIndex] != null)
        {
            Debug.Log("Slot sélectionné : " + selectedIndex + " -> " + inventory[selectedIndex].itemName);
        }
        else
        {
            Debug.Log("Slot sélectionné : " + selectedIndex + " -> vide");
        }
    }

    // Permet de récupérer l'index sélectionné depuis d'autres scripts
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    // Permet de récupérer l'item sélectionné depuis d'autres scripts
    public Item GetSelectedItem()
    {
        return inventory[selectedIndex];
    }

    void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Item currentItem = GetSelectedItem();
            if (currentItem != null && currentItem.prefab != null)
            {
                // Instancie le prefab devant le joueur
                Vector3 dropPosition = transform.position + transform.forward * 2f; // 2 unités devant
                Instantiate(currentItem.prefab, dropPosition, Quaternion.identity);

                // Retire l'objet de l'inventaire
                inventory[selectedIndex] = null;
                Debug.Log(currentItem.itemName + " a été relâché !");
                
                // Met à jour l'UI
                UI.UpdateGraphics();
            }
            else
            {
                Debug.Log("Aucun objet à relâcher !");
            }
        }
    }
}