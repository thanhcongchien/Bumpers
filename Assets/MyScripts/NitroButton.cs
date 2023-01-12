using System.Collections;
using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.UI;

public class NitroButton : MonoBehaviour
{
    public Button useNitroButton;
    // Start is called before the first frame update
    void Start()
    {
        useNitroButton.GetComponent<Button>();
        useNitroButton.onClick.AddListener(nitroOnClickButton);
    }

    public void nitroOnClickButton()
    {
        if (useNitroButton != null)
        {
            if (Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
            {
                Photon.Pun.Demo.PunBasics.PlayerManager.instance.OnClickNitrogen();
            }
        }
    }
}
