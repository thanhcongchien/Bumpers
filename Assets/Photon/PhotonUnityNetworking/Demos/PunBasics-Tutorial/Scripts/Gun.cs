using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GameObject GunObj;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 0.1f;
    public int bullletAmount = 5;
    public static Gun gunInstance;
    void Awake()
    {
        if (gunInstance == null)
        {
            gunInstance = GetComponent<Gun>();
        }
    }

    public void shootGun(){
        PlayerManager.instance.emitBullet();
    }

}
