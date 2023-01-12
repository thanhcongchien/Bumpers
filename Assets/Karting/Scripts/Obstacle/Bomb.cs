using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject exp;
    public float exForce, radius;
    public AudioClip saw;    // Add  Audi Clip Here;

    //check collider with kart
    private void OnCollisionEnter(Collision other)
    {
        GameObject _exp = Instantiate(exp, transform.position, transform.rotation);
        if (saw)
        {
            AudioUtility.CreateSFX(saw, transform.position, AudioUtility.AudioGroups.Collision, 0f);
        }
        Destroy(_exp, 3);
        knockBack();
        Destroy(gameObject);

    }
    

    // knockback when the bomb hit the kart
    void knockBack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(exForce, transform.position, radius);

            }
        }
    }
}
