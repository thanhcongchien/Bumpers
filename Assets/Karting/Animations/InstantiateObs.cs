using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObs : MonoBehaviour
{
    public Transform chien;
    public Rigidbody ball;

    void Start()
    {
        InvokeRepeating("SpawnBall", 3, 3);
    }
    void SpawnBall()
    {
        Rigidbody ballObj;
        ballObj = Instantiate(ball, chien.position, chien.rotation);
    }

}
