using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : TargetObject
{
    [Header("PickupObject")]

    [Tooltip("Destroy the spawned spawnPrefabOnPickup gameobject after this delay time. Time is in seconds.")]
    public float destroySpawnPrefabDelay = 10;

    [Tooltip("Destroy this gameobject after collectDuration seconds")]
    public float collectDuration = 0f;
    public float life = 3;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, life);
    }

    void OnCollect()
    {

        if (CollectSound)
        {
            AudioUtility.CreateSFX(CollectSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
        }
        Objective.OnUnregisterPickup(this);

        TimeManager.OnAdjustTime(TimeGained);
    }

    void OnTriggerEnter(Collider other)
    {
            OnCollect();
    }

}
