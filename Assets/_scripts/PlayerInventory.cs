using UnityEngine;
using PurrNet;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Hierarchy;
using TMPro;
using UnityEngine.UI;


public class PlayerInventory : NetworkBehaviour
{
    [Header("Iventory Settings")]
    public List<InventoryObject> inventoryObjects = new List<InventoryObject>();
    GameObject inventoryPanel; // panel ref
    Transform inventoryGrid; // ref to grid
    public GameObject crosshair;

    [SerializeField] GameObject inventoryItem; // item going into inventory
    [SerializeField] KeyCode iventoryButton = KeyCode.I;

    [Header("Pickup Settings")]
    [SerializeField] LayerMask pickupLayer;
    [SerializeField] float pickupDistance;
    [SerializeField] KeyCode pickupButton = KeyCode.E;

    Camera playerCamera; // camera ref 
    PlayerController playerController; // ref for player controls

    protected override void OnSpawned()
    {
        enabled = isOwner;

        playerCamera = Camera.main;
        inventoryPanel = GameObject.FindGameObjectWithTag("InventoryPanel");
        inventoryGrid = GameObject.FindGameObjectWithTag("InventoryGrid").transform;
        playerController = GetComponent<PlayerController>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        

        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(iventoryButton))
        {
            ToggleInventory();
        }
        if (Input.GetKeyDown(pickupButton))
        {
            Pickup();
        }
    }

    void ToggleInventory()
    {
        if (inventoryPanel.activeSelf)
        {
            crosshair.SetActive(true);
            inventoryPanel.SetActive(false);
            playerController.ChangeCursorLock();
        }

        else if (!inventoryPanel.activeSelf)
        {
            UpdateInventoryUI();

            crosshair.SetActive(false);
            inventoryPanel.SetActive(true);
            playerController.ChangeCursorLock();
        }
    }

    void Pickup()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupDistance, pickupLayer))
        {
            if (hit.transform.GetComponent<GroundItem>() == null)
            {
                return;
            }
            // if ray connects, add to inventory, and destroy the gameobject to remove it from the world
            AddToInventory(hit.transform.GetComponent<GroundItem>().itemScriptable);
            DespawnObject(hit.transform.gameObject);
        }
    }

    void DropItem(ItemScriptable itemScriptable)
    {
        foreach (InventoryObject invObj in inventoryObjects)
        {
            if (invObj.item != itemScriptable)
            {
                continue;
            }

            if (invObj.amount > 1)
            {
                invObj.amount--;
                DropItemRPC(invObj.item.itemPrefab, playerCamera.transform.position + playerCamera.transform.forward);
                UpdateInventoryUI();
                return;
            }
            if (invObj.amount <= 1)
            {
                inventoryObjects.Remove(invObj);
                DropItemRPC(invObj.item.itemPrefab, playerCamera.transform.position + playerCamera.transform.forward);
                UpdateInventoryUI();
                return;
            }
            
        }
    }
    [ObserversRpc]
    void DropItemObserver(GameObject prefab, Vector3 position)
    {
        GameObject drop = Instantiate(prefab, position, Quaternion.identity);
        if (drop)
            Instantiate(drop);
        
    }

    void DespawnObject(GameObject objToDespawn)
    {
        if (objToDespawn)
            Destroy(objToDespawn);
        
    }

    void DropItemRPC(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    void AddToInventory(ItemScriptable newItem)
    {
        // checking list for duplicates so it will add numeric value if multiples allowed
        foreach(InventoryObject invObj in inventoryObjects)
        {
           
            if (invObj.item == newItem)
            {
                // increment amount because item already exists in list
                invObj.amount++;
                return;
            }
        }

        inventoryObjects.Add(new InventoryObject() { item = newItem, amount = 1});
    }

    void UpdateInventoryUI()
    {
        foreach(Transform child  in inventoryGrid)
        {
            Destroy(child.gameObject);
        }
        foreach (InventoryObject invObj in inventoryObjects)
        {
            GameObject obj = Instantiate(inventoryItem, inventoryGrid);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{invObj.item.itemName} - {invObj.amount}";

            Button button = obj.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.onClick.AddListener(() => DropItem(invObj.item)); // Correct Unity UI syntax
            }
            else
            {
                Debug.LogError("Button component not found in the instantiated object.");
            }
        }
    }

    // Calls scriptable, and holds amount
    [System.Serializable]
    public class InventoryObject
    {
        public ItemScriptable item;
        public int amount; 
    }
}
