using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
       //flash screen

    [SerializeField] [Range(0f,3f)] float lerpTime;
    [SerializeField] Image _image;
    [SerializeField] Color[] _newColor;

    int colorIndex = 0;

    float t = 0f;
    int len;
    // Start is called before the first frame update
    void Start()
    {
         len = _newColor.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
        _image.color = Color.Lerp(_image.color,_newColor[colorIndex],lerpTime * Time.deltaTime);
        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
        if(t > .9f){
            t = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= _newColor.Length) ? 0 : colorIndex;
        }
    }

}