using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    public int destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DestroyBallObject", destroyTime, 4);
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        Rigidbody rigid = GetComponent<Rigidbody>();
        if(rigid != null){
            GetComponent<Rigidbody>().AddForce(Physics.gravity * 2f, ForceMode.Acceleration);
        }
    }
    private void DestroyBallObject()
    {
        Destroy(this.gameObject);
    }
}
