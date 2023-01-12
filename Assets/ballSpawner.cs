using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour
{
    ObjectPooler instantiateObs;
    public Transform ballSpawnerObj;
    void Start()
    {
        instantiateObs = ObjectPooler.Instance;
    }

    void FixedUpdate()
    {
       StartCoroutine(SpawnBall());
    }

    IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(50f);
        instantiateObs.SpwanFromPool("PhysicsBall_Default", ballSpawnerObj.position, Quaternion.identity);
        Debug.Log("PhysicsBall_Default");
    }
}
