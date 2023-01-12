using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{


    public List<Vector3> ListPostion;
    public object layerMask;

    public void Teleport()
    {
        if (ListPostion != null && Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
        {
            Photon.Pun.Demo.PunBasics.PlayerManager.instance.KartPlayer.transform.position = ListPostion[Random.Range(0, ListPostion.Count)];
            Debug.Log("new pos: " + Photon.Pun.Demo.PunBasics.PlayerManager.instance.KartPlayer.transform.position);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Teleport();
        }
    }

}
