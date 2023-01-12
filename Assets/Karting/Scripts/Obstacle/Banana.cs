using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : TargetObject
{
    public float collectDuration = 0;

    void OnCollect(){
        if (CollectSound)
        {
            AudioUtility.CreateSFX(CollectSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
        }

        Destroy(gameObject, collectDuration);
    }


    void OnTriggerEnter(Collider other){
        if ((layerMask.value & 1 << other.gameObject.layer) > 0 && other.gameObject.CompareTag("Player"))
        {
            OnCollect();
        }
    }

}