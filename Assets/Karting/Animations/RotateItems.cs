using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItems : MonoBehaviour
{
    
    [SerializeField] private RotationType rotationType = RotationType.Y_Direct;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // this.transform.Rotate(new Vector3(0f, 100f, 0f) * Time.deltaTime);
        if( rotationType == RotationType.X_Direct){
            rotateX();
        }else if( rotationType == RotationType.Y_Direct){
            rotateY();
        }else if( rotationType == RotationType.Z_Direct){
            rotateZ();
    }

    void rotateX(){
        
        this.transform.Rotate(new Vector3(100f, 0f, 0f) * Time.deltaTime);

    }
     void rotateY(){
       
        this.transform.Rotate(new Vector3(0f, 100f, 0f) * Time.deltaTime);

    }
    void rotateZ(){
        this.transform.Rotate(new Vector3(0f, 0f, 100f) * Time.deltaTime);

    }



}
}
public enum RotationType { X_Direct, Y_Direct, Z_Direct}
