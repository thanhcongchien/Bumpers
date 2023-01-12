using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace KartGame.KartSystems
{

    public class KeyboardInput : BaseInput
    {
        public static KeyboardInput instance;
        public string Horizontal = "Horizontal";
        public string Vertical = "Vertical";
        public bool isBootNitro = false;
        public bool resetNitro = false;
        private bool isReady = false;
        public GameObject ownerKart;
        public GameObject NitroVFX;
        public GameObject[] DriffVFX;
        //reset pre position when the player was falled out road
        private Vector3 currentPos;

        private void Awake()
        {
            if (instance == null)
            {
                instance = GetComponent<KeyboardInput>();
            }
        }
        private void Start()
        {
            StartCoroutine(isReadyToStart());
            this.resetNitro = false;
            //currentPos = new Vector3(14.43f, 0.88f, 3f);
        }

        public override Vector2 GenerateInput()
        {
            if (isReady == true)
            {
                if (this.ownerKart.gameObject.GetComponent<PlayerManager>().isRotated == false)
                    return new Vector2
                    {
                        x = Input.GetAxis(Horizontal),
                        y = Input.GetAxis(Vertical)
                    };
            }
            return new Vector2 { };

        }


        void Update()
        {
            NitroButton();
            ResetPlayerPosition();
            if (NitroScript.instance != null)
            {
                if (resetNitro)
                {
                    NitroScript.instance.AfterBootNitro();
                }
            }
        }

        public void NitroButton()
        {
            if (PlayerManager.instance.isNitroButtonClicked == true && PlayerManager.instance.isLocalPlayer == true)
            {
                SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
                if (NitroScript.instance != null)
                {

                    isBootNitro = true;
                    if (NitroScript.instance.playerNitroSlider.value >= 1f)
                    {
                        if (this.ownerKart.gameObject.GetComponent<ArcadeKart>() != null && PlayerManager.instance.isLocalPlayer == true)
                        {

                            this.ownerKart.gameObject.GetComponent<ArcadeKart>().baseStats.TopSpeed += 50;
                            if (this.ownerKart.gameObject.GetComponent<ArcadeKart>().baseStats.TopSpeed > 20)
                            {
                                SoundManager.Instance.PlaySFX(SoundManager.NITRO_BOOST_SFX);
                                this.ownerKart.gameObject.GetComponent<PlayerManager>().boostNitroVFX(true);
                            }
                        }
                        resetNitro = true;
                    }
                }
            }
        }

        // After counting 3, 2, 1 then player can run
        public IEnumerator isReadyToStart()
        {
            yield return new WaitForSeconds(3f);
            this.isReady = true;
        }

        public void ResetPlayerPosition()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ownerKart.transform.position = currentPos;
            }
            StartCoroutine(GetCurrentPosition());
        }

        public IEnumerator GetCurrentPosition()
        {
            Vector3 prePosition = ownerKart.transform.position;
            yield return new WaitForSeconds(5.0f);
            currentPos = prePosition;
        }

        // not working
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.name == "GroundPlane")
            {
                if (this.isReady)
                {
                    ownerKart.transform.position = currentPos;
                    // new Vector3(14.43f, 0.88f, 3f);
                }

            }

        }
    }
}
