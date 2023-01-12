using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KartGame.KartSystems;
using UnityEngine.EventSystems;
//using Photon.Pun.Demo.PunBasics;
//using Photon.Pun;





public class NitroScript : MonoBehaviour
{

    public static NitroScript instance;

    #region Private Fields

    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [Tooltip("UI Text to display Player's Nitro")]
    [SerializeField]
    private Text NitroTxt;

    [Tooltip("UI Slider to display Player's Nitro")]
    [SerializeField]
    public Slider playerNitroSlider;

    public GameObject fillNitroBar;

    public Sprite[] nitroIcon;
    public GameObject NitroStatus;

    float characterControllerHeight;

    Transform targetTransform;

    Renderer targetRenderer;

    CanvasGroup _canvasGroup;

    Vector3 targetPosition;

    #endregion

    #region MonoBehaviour Messages

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase
    /// </summary>
    ///

    void Awake()
    {

        fillNitroBar.GetComponent<Image>().gameObject.SetActive(false);
        _canvasGroup = this.GetComponent<CanvasGroup>();

        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        if (instance == null)
        {
            instance = GetComponent<NitroScript>();
        }

        if (playerNitroSlider != null)
        {
            playerNitroSlider.value = 0f;
        }
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// update the health slider to reflect the Player's health
    /// </summary>
    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (playerNitroSlider == null)
        {
            Destroy(this.gameObject);
            return;
        }



        // reset speed after out of nitro
        if (playerNitroSlider.value == 0)
        {
            ArcadeKart.Instance.baseStats.TopSpeed = 20;
        }

        // rotate nitro items
        //setAnimationForItems();
    }


    // update nitro icon
    public void UpdateNitroIcon()
    {
        if (Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
        {
            if (playerNitroSlider.value < 1f && playerNitroSlider.value >= 0f)
            {
                NitroStatus.GetComponent<Image>().sprite = nitroIcon[0];
            }
            else
            {
                NitroStatus.GetComponent<Image>().sprite = nitroIcon[1];
            }
        }
    }


    // update nitro bar
    public void BootNitroProcessBar()
    {

        if (playerNitroSlider != null)
        {
            // if take a nitro item then process bar will be add (0.2)

            if (playerNitroSlider.value <= 1f && playerNitroSlider.value >= 0f)
            {
                if (Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true && Photon.Pun.Demo.PunBasics.PlayerManager.instance.isPicked == true)
                {
                    fillNitroBar.GetComponent<Image>().gameObject.SetActive(true);
                    playerNitroSlider.value += Photon.Pun.Demo.PunBasics.PlayerManager.instance.nitroItem;
                    Photon.Pun.Demo.PunBasics.PlayerManager.instance.nitroValue = playerNitroSlider.value;
                }
                else
                {
                    playerNitroSlider.value += 0;

                }
            }

            if (!KeyboardInput.instance.isBootNitro && playerNitroSlider.value >= 1f)
            {
                AfterBootNitro();
            }
        }

    }




    // decrease nitro process bar after booting
    public void AfterBootNitro()
    {


        if (playerNitroSlider.value <= 1f && playerNitroSlider.value >= 0f)
        {
            if (KeyboardInput.instance.resetNitro && Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
            {
                playerNitroSlider.value -= 0.2f * Time.deltaTime;
                if (playerNitroSlider.value == 0f)
                {
                    NitroScript.instance.UpdateNitroIcon();
                    KeyboardInput.instance.resetNitro = false;
                    Photon.Pun.Demo.PunBasics.PlayerManager.instance.boostNitroVFX(false);
                    fillNitroBar.GetComponent<Image>().gameObject.SetActive(false);
                }
            }
            Photon.Pun.Demo.PunBasics.PlayerManager.instance.isNitroButtonClicked = false;
            Photon.Pun.Demo.PunBasics.PlayerManager.instance.nitroValue = 0f;
            Photon.Pun.Demo.PunBasics.PlayerManager.instance.isPicked = false;
        }
    }

    void LateUpdate()
    {

        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            if (Camera.main != null)
            {
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition);
            }
            else
            {
                Debug.LogError("Camera main is null");
            }
        }
    }




    #endregion



    #region Public Methods

    public void SetNitro()
    {

    }

    #endregion
}

