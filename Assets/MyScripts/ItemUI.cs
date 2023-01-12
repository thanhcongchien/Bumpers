using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : TargetObject
{

    public Sprite[] itemIcon;
    Transform targetTransform;

    Renderer targetRenderer;

    CanvasGroup _canvasGroup;

    Vector3 targetPosition;

    public GameObject your_item;


    void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();

        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    void Update()
    {

        if (PlayerManager.instance.current_Item != "")
        {
            handOn_Item_UI(PlayerManager.instance.current_Item);
        }
        else
        {
            your_item.GetComponent<Image>().gameObject.SetActive(false);
        }
    }

    public void handOn_Item_UI(string current_Item_Handon)
    {
        if (your_item != null)
        {
            if (Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
            {
                if (current_Item_Handon != "")
                {
                    if (current_Item_Handon == "Banana")
                    {
                        your_item.GetComponent<Image>().gameObject.SetActive(true);
                        your_item.GetComponent<Image>().sprite = itemIcon[0];
                    }
                    else if (current_Item_Handon == "Bomb")
                    {
                        your_item.GetComponent<Image>().gameObject.SetActive(true);
                        your_item.GetComponent<Image>().sprite = itemIcon[1];
                    }
                    else if (current_Item_Handon == "Gun")
                    {
                        your_item.GetComponent<Image>().gameObject.SetActive(true);
                        your_item.GetComponent<Image>().sprite = itemIcon[2];
                    }
                    else if (current_Item_Handon == "Smoke")
                    {
                        your_item.GetComponent<Image>().gameObject.SetActive(true);
                        your_item.GetComponent<Image>().sprite = itemIcon[3];
                    }
                }
                else
                {
                    your_item.GetComponent<Image>().gameObject.SetActive(false);
                }
            }
        }

    }

}
