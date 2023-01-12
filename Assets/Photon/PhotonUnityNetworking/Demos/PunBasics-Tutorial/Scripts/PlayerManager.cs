// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

namespace Photon.Pun.Demo.PunBasics
{

#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The current nitro of our player")]
        public float Nitro = 1f;
        #endregion

        #region Private Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        [Tooltip("The Player's Nitro GameObject")]
        [SerializeField]
        private GameObject NitroObj;

        [Tooltip("The Player's Item GameObject")]
        [SerializeField]
        private GameObject ItemObj;

        [Tooltip("The Health GameObject to control")]
        [SerializeField]
        private GameObject HealthObj;
        public static PlayerManager instance;
        [Tooltip("The Nitro GameObject to control")]
        public List<GameObject> nitroUI;
        public GameObject NitroVFX;
        public GameObject[] DriffVFX;
        public bool isLocalPlayer = false;
        public float nitroItem = 0.2f;
        public float nitroValue = 0;
        public bool isNitroButtonClicked = false;

        [Tooltip("Bullet Object Instantiate")]
        public Transform bulletSpawnPoint;
        public GameObject bulletPrefab;
        public float bulletSpeed = 0.1f;
        public int bullletAmount = 5;

        //this.player
        public GameObject KartPlayer;


        // random skin of kart
        public Material[] materialList;
        public GameObject skinMaterialBody;
        public GameObject skinMaterialKart;
        private int index;

        private const byte COLOR_CHANGE = 1;
        //True, when the user is firing
        bool IsFiring;
        public float lerpTime = 1f;
        public bool isRotated = false;

        public bool hasitem = false; //true when player hits itembox

        public GameObject KartObj;

        // playerNode

        [SerializeField] GameObject playerNdoe;
        // item manager
        public bool start_select = false;
        public bool isPicked = false;
        public Sprite[] items_possible;
        public GameObject[] item_gameobjects;
        public GameObject your_item;

        [Header("ITEMS")]
        public GameObject Shell;
        public GameObject Banana;
        public GameObject Bomb;
        public GameObject Gun;
        public GameObject Smoke;
        public Transform shellSpawnPos;
        public Transform backshellPos; //also for bananas
        public Transform BananaSpawnPos;
        public Transform coinSpawnPos;
        public Transform bombSpawnPos;
        public Transform smokeSpawnPos;

        // [HideInInspector]
        public int item_index = 0;
        // [HideInInspector]
        public int tripleItemCount = 3;
        public bool isSmoke = false;
        public int smokeCount = 1;
        // [HideInInspector]
        public string current_Item;
        public bool isUsedItem = false;

        // health progress when recieved damage
        public bool isDamageRecieved = false;
        public float baseDamage = 1;

        public float damageRecieved;
        public float healthPlayer = 1f;
        public Slider healthSlider;

        public Rigidbody rigidPlayerBody;


        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        /// 

        private void OnEnable()
        {
            // int randomSkin = Random.Range(0, materialList.Length - 1);
            // Debug.Log("index: " + randomSkin);
            // object[] datas = new object[] { randomSkin };
            // RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            // PhotonNetwork.RaiseEvent(COLOR_CHANGE, datas, raiseEventOptions, SendOptions.SendReliable);
        }




        public void Awake()
        {

            if (instance == null)
            {
                instance = GetComponent<PlayerManager>();
            }

            if (this.beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                isLocalPlayer = true;
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);


        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            // Create the UI
            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            // this.nitroItem = 0f;

            //create the nitro UI
            if (this.NitroObj != null)
            {
                if (photonView.IsMine)
                {
                    GameObject _uiNitro = Instantiate(this.NitroObj);
                    _uiNitro.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                }

            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> NitroObj reference on player Prefab.", this);
            }
            //create the Item UI
            if (this.ItemObj != null)
            {
                if (photonView.IsMine)
                {
                    GameObject _uiItem = Instantiate(this.ItemObj);
                    _uiItem.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                }

            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> ItemObj reference on player Prefab.", this);
            }

            //create the Health UI
            if (this.HealthObj != null)
            {
                if (photonView.IsMine)
                {
                    // GameObject _healthItem = Instantiate(this.HealthObj);
                    // _healthItem.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                    photonView.RPC("createHealthBarUI", RpcTarget.AllViaServer);
                }

            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> HealthObj reference on player Prefab.", this);
            }

            if (photonView.IsMine)
            {
                playerNdoe.GetComponent<MeshRenderer>().material = materialList[10];
            }
            // PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;


#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }


        public override void OnDisable()
        {
            // PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
            // Always call the base to remove callbacks
            base.OnDisable();

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }
        private void NetworkingClient_EventReceived(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == COLOR_CHANGE)
            {
                object[] datas = (object[])photonEvent.CustomData;
                int randomskin = (int)datas[0];
                if (materialList.Length > 0)
                {
                    var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
                    foreach (var view in photonViews)
                    {
                        var player = view.gameObject;
                        Debug.Log("PLAYER: " + player);
                        //Objects in the scene don't have an owner, its means view.owner will be null
                        GameObject skinMaterialBody = player.transform.Find("KartSuspension/Kart/Kart_Body").gameObject;
                        skinMaterialBody.GetComponent<SkinnedMeshRenderer>().material = materialList[randomskin];
                        Debug.Log("randomskin :" + randomskin);

                    }

                }

            }

        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        public void Update()
        {
            photonView.RPC("HealthProcessBar", RpcTarget.AllViaServer);
            if (start_select) //this ensures item select process does not begin until player has used up curret item
            {



                // if (current_Item == "Gun")
                // {
                //     photonView.RPC("getGun", RpcTarget.AllViaServer);
                // }

            }
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();
                isLocalPlayer = true;
                if (this.Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
            else
            {
                isLocalPlayer = false;
            }

            if (this.beams != null && this.IsFiring != this.beams.activeInHierarchy)
            {
                this.beams.SetActive(this.IsFiring);
            }


            if (isRotated == true)
            {
                photonView.RPC("rotateBanana", RpcTarget.AllViaServer);
            }

        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }


            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            this.Health -= 0.1f;
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are interesting the player
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {

            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }

            // check colider with nitro items
            if (photonView.IsMine)
            {
                if (other.gameObject.name == "Nitro")
                {
                    this.isPicked = true;
                    this.nitroItem = 0.2f;

                }
            }
            if (other.gameObject.name == "Banana Peel(Clone)")
            {
                this.isRotated = true;
            }
            if (other.gameObject.name == "BulletBillPlayer(Clone)")
            {
                photonView.RPC("isDamaged", RpcTarget.AllViaServer, true);
                photonView.RPC("dameRecipe", RpcTarget.AllViaServer, 0.1f);
            }

            if(other.gameObject.name == "Itembox"){
                Item item = other.gameObject.GetComponent<Item>();
                if(item.itemInBox == "Gun"){;
                    photonView.RPC("getGun", RpcTarget.AllViaServer);
                }
            }


            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            this.Health -= 0.1f * Time.deltaTime;
        }
        [PunRPC]
        public void createHealthBarUI()
        {
            GameObject _healthItem = Instantiate(this.HealthObj);
            healthSlider = _healthItem.GetComponent<Slider>();
            healthSlider.value = 1f;
            _healthItem.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        [PunRPC]
        IEnumerator waitForRotateBanana()
        {
            yield return new WaitForSeconds(2.0f);
            this.KartObj.transform.localRotation = Quaternion.identity;
            this.KartObj.transform.GetChild(1).gameObject.transform.localRotation = Quaternion.identity;
            Debug.Log("ROTATED" + this.KartObj.transform.GetChild(1).gameObject);
            this.isRotated = false;
        }
        public void RandomSkinKart()
        {
            if (materialList.Length > 0)
            {
                index = Random.Range(0, materialList.Length - 1);
                this.skinMaterialBody.GetComponent<SkinnedMeshRenderer>().material = materialList[index];
                this.skinMaterialKart.GetComponent<SkinnedMeshRenderer>().material = materialList[index];
                Debug.Log("index: " + index);
            }
        }

        [PunRPC]
        void rotateBanana()
        {
            KartObj.transform.Rotate(Vector3.up, Time.deltaTime * lerpTime);
            resetRotation();
        }

        void resetRotation()
        {
            photonView.RPC("waitForRotateBanana", RpcTarget.AllViaServer);
        }


        // enable Nitro VFX 


        public void boostNitroVFX(bool isNitro)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("nitroVFX", RpcTarget.AllViaServer, isNitro);
            }
        }

        [PunRPC]
        public void nitroVFX(bool nitro)
        {

            NitroVFX.gameObject.SetActive(nitro);
            foreach (GameObject driff in DriffVFX)
            {
                driff.gameObject.SetActive(nitro);
            }

        }


        public void OnClickNitrogen()
        {
            if (PlayerManager.instance.nitroValue >= 1f)
            {
                this.isNitroButtonClicked = true;
            }
        }
        // health bar 
        [PunRPC]
        public void HealthProcessBar()
        {
            if (healthSlider != null)
            {
                // if recieve a damage from weapon(gun/bomb) then health process bar will be minus
                if (healthSlider.value > 0f)
                {
                    // if (Photon.Pun.Demo.PunBasics.PlayerManager.instance.isLocalPlayer == true)
                    // {
                    //playerHealthSlider.value += 0.2f;
                    if (isDamageRecieved == true)
                    {
                        healthSlider.value -= this.damageRecieved;
                        this.damageRecieved = 0f;
                        this.healthPlayer = healthSlider.value;
                        photonView.RPC("isDamaged", RpcTarget.AllViaServer, false);
                    }
                    else
                    {
                        healthSlider.value -= 0;
                        this.healthPlayer -= 0;
                    }

                    // }
                }
                else
                {
                    healthSlider.transform.GetChild(1).gameObject.SetActive(false);

                }

            }

        }

        // weapon

        public void ReadyToUseItem()
        {
            if (start_select) //this ensures item select process does not begin until player has used up curret item
            {
                if (current_Item == "Banana")
                {
                    // StartCoroutine(spawnBanana(1));
                    photonView.RPC("spawnBanana", RpcTarget.All, 1);
                }
                else if (current_Item == "Bomb")
                {
                    // StartCoroutine(spawnBomb(-1));
                    photonView.RPC("spawnBomb", RpcTarget.All, -1);
                }
                else if (current_Item == "Smoke")
                {
                    // StartCoroutine(spawnSmoke());
                    photonView.RPC("spawnSmoke", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        IEnumerator spawnBanana(int direction)
        {
            GameObject clone;
            if (direction == 1)//forward
            {
                if (tripleItemCount > 0)
                {
                    yield return new WaitForSeconds(0.1f);
                    clone = Instantiate(Banana, BananaSpawnPos.position, BananaSpawnPos.rotation);
                    // clone.GetComponent<Banana>().Banana_thrown(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z * 200);
                    // clone.GetComponent<Banana>().whoThrewBanana = gameObject.name;
                    tripleItemCount--;
                    SoundManager.Instance.PlaySFX(SoundManager.ITEM_SFX);
                    used_Item_Done();
                }

            }
        }

        [PunRPC]
        IEnumerator spawnBomb(int direction)
        {
            GameObject clone;
            if (direction == -1)//forward
            {
                yield return new WaitForSeconds(0.1f);

                clone = Instantiate(Bomb, bombSpawnPos.position, bombSpawnPos.rotation);
                tripleItemCount--;
                SoundManager.Instance.PlaySFX(SoundManager.ITEM_SFX);
                used_Item_Done();
            }
        }

        [PunRPC]
        IEnumerator getGun()
        {
            yield return new WaitForSeconds(0.1f);
            Gun.gameObject.SetActive(true);
        }

        [PunRPC]
        IEnumerator spawnSmoke()
        {
            GameObject clone;
            yield return new WaitForSeconds(0.1f);
            clone = Instantiate(Smoke, smokeSpawnPos.position, smokeSpawnPos.rotation);
            isSmoke = false;
            smokeCount--;
            SoundManager.Instance.PlaySFX(SoundManager.ITEM_SFX);
            used_Item_Done();
        }


        void used_Item_Done()
        {
            if (tripleItemCount == 0 || smokeCount == 0)
            {
                start_select = false;
                current_Item = "";
                tripleItemCount = 3;
                smokeCount = 1;
            }
        }




        public void emitBullet()
        {
            // if (photonView.IsMine)
            // {
            photonView.RPC("InstantiateBullet", RpcTarget.AllViaServer);
            // }
        }

        [PunRPC]
        public void InstantiateBullet()
        {
            var bulllet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bulllet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.up * bulletSpeed;
            bullletAmount--;
            SoundManager.Instance.PlaySFX(SoundManager.SHOOT_SFX);
            if (bullletAmount <= 0)
            {
                start_select = false;
                current_Item = "";
                Gun.gameObject.SetActive(false);
                bullletAmount = 5;
                Debug.Log("gun is disable");
            }
        }



        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.name == "Bomb-Omb(Clone)")
            {
                photonView.RPC("isDamaged", RpcTarget.AllViaServer, true);
                // damage of bomb is 0.3
                // dameRecipe(0.3f);
                photonView.RPC("dameRecipe", RpcTarget.AllViaServer, 0.3f);
            }

        }

        [PunRPC]
        void isDamaged(bool isDamaged)
        {
            this.isDamageRecieved = isDamaged;
        }

        [PunRPC]
        public void dameRecipe(float dameLevel)
        {
            damageRecieved = baseDamage * dameLevel;
            Debug.Log("damageRecieved: " + damageRecieved);
        }



#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            //GameObject _uiGo = Instantiate(this.playerUiPrefab);
            //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);



            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }


        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // we don't want to fire when we interact with UI buttons for example. IsPointerOverGameObject really means IsPointerOver*UI*GameObject
                // notice we don't use on on GetbuttonUp() few lines down, because one can mouse down, move over a UI element and release, which would lead to not lower the isFiring Flag.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(this.Health);
                stream.SendNext(rigidPlayerBody.position);
                stream.SendNext(rigidPlayerBody.rotation);
                stream.SendNext(rigidPlayerBody.velocity);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                rigidPlayerBody.position = (Vector3)stream.ReceiveNext();
                rigidPlayerBody.rotation = (Quaternion)stream.ReceiveNext();
                rigidPlayerBody.velocity = (Vector3)stream.ReceiveNext();

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                rigidPlayerBody.position += rigidPlayerBody.velocity * lag;
            }
        }

        #endregion
    }
}