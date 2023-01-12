﻿using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;
        [Header("Map Selection")]
        [SerializeField] Image mapImg;
        [SerializeField] Text mapTxt;
        public int currentMap;
        public string[] Map_List;
        public string[] Scene_List;
        public Sprite[] mapImgList;
        public Button buttonLeftMap;
        public Button buttonRighttMap;
        
        #region UNITY

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();

            PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions { MaxPlayers = 4 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();


            SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
            buttonLeftMap.gameObject.SetActive(CheckPlayerCanSelectMap());
            buttonRighttMap.gameObject.SetActive(CheckPlayerCanSelectMap());

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
            SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            //check min and max players in a room(2-4)
            if ((int.Parse(MaxPlayersInputField.text) >= 2 && int.Parse(MaxPlayersInputField.text) <= 4))
            {
                byte maxPlayers;
                byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
                maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);
                RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

                PhotonNetwork.CreateRoom(roomName, options, null);
                Debug.Log("create room a successfully!!!");
                SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            }
            else
            {
                Debug.Log("cannot create a room !!!");
                SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            }


        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
            SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
        }

        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
            SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
        }

        // public void OnClickChooseMap(int next)
        // {
        //     currentMap += next;

        //     if (currentMap > 0)
        //     {
        //         currentMap = Map_List.Length - 1;

        //         if (currentMap == 1)
        //         {
        //             mapTxt.text = "Paradise Island";
        //             mapImg.sprite = mapImgList[currentMap];
        //             SoundManager.Instance.PlaySFX(SoundManager.OPEN_SFX);
        //         }
        //     }
        //     else if (currentMap < Map_List.Length - 1)
        //     {
        //         currentMap = 0;

        //         if (currentMap == 0)
        //         {
        //             mapTxt.text = "Future City";
        //             mapImg.sprite = mapImgList[currentMap];
        //             SoundManager.Instance.PlaySFX(SoundManager.OPEN_SFX);
        //         }
        //     }
        // }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            //PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
            // if (currentMap == 0)
            // {
            //     PhotonNetwork.LoadLevel("MainScene");
            //     SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            // }
            // else
            // {
            //     PhotonNetwork.LoadLevel("MainScene1");
            //     SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            // }
            if(this.currentMap != null){
                PhotonNetwork.LoadLevel(Scene_List[this.currentMap]);
                SoundManager.Instance.PlaySFX(SoundManager.CLICK_SFX);
            }

        }



        // emit chnaged-map image for all players to see

         public void emitPreviosMapImage(){
            photonView.RPC("OnClickPreviousMapButtonClicked", RpcTarget.AllViaServer, 1);
        }
        public void emitNextMapImage(){
            photonView.RPC("OnClickNextMapButtonClicked", RpcTarget.All,1);
        }
        

        
        void selectMap(int map){
            map = this.currentMap;
            mapTxt.text = Map_List[map];
            mapImg.sprite = mapImgList[currentMap];
            
        }

        [PunRPC]
        private void OnClickNextMapButtonClicked(int next){
            if(this.currentMap < Map_List.Length - 1){
                this.currentMap += next;    
                selectMap(currentMap);
            }
        }
        
        [PunRPC]
        private void OnClickPreviousMapButtonClicked(int previous){
             if(currentMap >= previous){
                this.currentMap -= previous;    
                selectMap(currentMap);
             }
        }
        


        private bool CheckPlayerCanSelectMap(){
            if(!PhotonNetwork.IsMasterClient){
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList){
                object isPlayerChangeMap;
                if(p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_CHANGED_MAP, out isPlayerChangeMap)){
                    if(!(bool)isPlayerChangeMap){
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }
    }
}