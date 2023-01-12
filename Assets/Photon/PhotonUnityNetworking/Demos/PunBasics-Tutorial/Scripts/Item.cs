using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class Item : MonoBehaviour

{
    public static Item instance;

    public string[] item_name = new string[] { "Banana", "Bomb", "Gun", "Smoke" };
    public string itemInBox;
    [Tooltip("Destroy the spawned spawnPrefabOnPickup gameobject after this delay time. Time is in seconds.")]
    public float destroySpawnPrefabDelay = 10;
    [Tooltip("Destroy this gameobject after collectDuration seconds")]
    public float collectDuration = 0f;

    public float repeateTime = 5f;
    

    void Awake(){
        if(instance == null){
            instance = GetComponent<Item>();
        }
    }

    void OnEnable()
    {
        int itemIndex = Random.Range(0, item_name.Length);
        itemInBox = item_name[itemIndex];
    }
    void OnCollect()
    {
        SoundManager.Instance.PlaySFX(SoundManager.ITEM_SELECTED_SFX);
        // Destroy(gameObject, collectDuration);
        this.gameObject.SetActive(false);
        InvokeRepeating("Repeat", repeateTime, 15f);
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager itemManager = other.gameObject.GetComponent<PlayerManager>();
            if (itemManager.GetComponent<PlayerManager>().start_select == false)
            {
                itemManager.GetComponent<PlayerManager>().start_select = true;
                itemManager.GetComponent<PlayerManager>().current_Item = itemInBox;

            }
            OnCollect();
        }
    }

    void Repeat(){
        this.gameObject.SetActive(true);
    }

}