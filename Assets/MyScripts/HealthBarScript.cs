using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KartGame.KartSystems;
using Photon.Pun.Demo.PunBasics;

public class HealthBarScript : MonoBehaviour
{

    #region Private Fields

    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 40f, 0f);

    [Tooltip("UI Slider to display Player's Nitro")]
    [SerializeField]
    public Slider playerHealthSlider;

    public GameObject fillHealthBar;

    public Sprite[] nitroIcon;
    // public GameObject HealthStatus;

    float characterControllerHeight;


    Transform targetTransform;

    Renderer targetRenderer;

    CanvasGroup _canvasGroup;

    Vector3 targetPosition;
    public PlayerManager target;

    #endregion

    #region MonoBehaviour Messages
    void Awake()
    {

        // fillHealthBar.GetComponent<Image>().gameObject.SetActive(false);
        _canvasGroup = this.GetComponent<CanvasGroup>();

        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }


    // Update is called once per frame
    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (playerHealthSlider == null)
        {
            Destroy(this.gameObject);
            return;
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
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }
    }
    #endregion



    #region Public Methods

    /// <summary>
    /// Assigns a Player Target to Follow and represent.
    /// </summary>
    /// <param name="target">Target.</param>
    public void SetTarget(PlayerManager _target)
    {

        if (_target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for HealthBar.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();


        CharacterController _characterController = this.target.GetComponent<CharacterController>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }
    }


    #endregion
}
