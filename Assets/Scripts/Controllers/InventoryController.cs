using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryController : MonoBehaviour
{

    private GameController gameController;

    [Header("SLOTS")]
    public Button[] slots;

    [Header("ICON")]
    public Image[] iconItems;

    [Header("ITEMS")]
    public List<GameObject> items;
    public List<GameObject> itemsLoaded;

    [Header("POWER")]
    public TextMeshProUGUI quantityHealth;
    public TextMeshProUGUI quantityMana;
    public int qHealth;
    public int qMana;

    [Header("ARROWS")]
    public TextMeshProUGUI quantityArrowA;
    public TextMeshProUGUI quantityArrowB;
    public TextMeshProUGUI quantityArrowC;
    public int qArrowA;
    public int qArrowB;
    public int qArrowC;

    void Start()
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
    }

    public void loadInventory()
    {
        cleanOldInventory();

        foreach (Button button in slots) // Reset all buttons interactables
        {
            button.interactable = false;
        }

        foreach (Image image in iconItems) // Disable all images
        {
            image.gameObject.SetActive(false);
        }

        quantityHealth.text = "x0";
        quantityMana.text = "x0";

        quantityArrowA.text = "x0";
        quantityArrowB.text = "x0";
        quantityArrowC.text = "x0";

        loadInventoryFromPlayerItems();
    }

    void loadInventoryFromPlayerItems()
    {
        int id = 0; // id of item

        foreach (GameObject item in items)
        {
            GameObject itemTemp = Instantiate(item);

            itemsLoaded.Add(itemTemp);

            slots[id].GetComponent<SlotController>().objectSlot = itemTemp; // Set item at slot index by id
            slots[id].interactable = true; // Enable slot interaction

            ItemController itemController = itemTemp.GetComponent<ItemController>(); // Get sprite at slot index

            iconItems[id].sprite = gameController.weaponImages[itemController.idItem]; // Set sprite at slot index
            iconItems[id].gameObject.SetActive(true); // Enable it

            id++; // Increment slot index
        }
    }

    public void cleanOldInventory()
    {
        foreach (GameObject item in itemsLoaded)
        {
            Destroy(item);
        }

        itemsLoaded.Clear();
    }
}
