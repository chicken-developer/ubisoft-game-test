﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenuPrefab;
    [SerializeField] private GameObject gameplayPrefab;
    [SerializeField] private GameObject lobbyPrefab;
    
    [SerializeField] private Text title;
    private WebSocket lobbyWS;
    private bool isGameHaveStarted;
    private int lobbySize = 1 ;
    private List<string> lobbyMessages;
    private List<PlayerDataSync> playersDataFromLobby;
    private static int playerIndex;
    public void JoinOrQuitLobby(string playerData = "null") // This will call from another scene to put player into lobby
    {
        Debug.Log("Enter join or quit lobby");
        if (lobbyWS == null)
        {
            lobbyWS = new WebSocket("ws://127.0.0.1:8087/?playerName=" + PlayerDataLocal.playerUserName);

        }
        lobbyWS.Connect ();
        lobbyWS.OnMessage += (sender, e) =>
        {
            Debug.Log("Receive new data: " + e.Data);
            var receiveData = new string(e.Data.ToCharArray());
            lobbyMessages.Add(receiveData);
        };
        //lobbyWS.Send("SPECIAL_REQUEST:JOIN"); // This server will return an address for player join match
    }

    public string StartGameFromLobby()
    {
        gameplayPrefab.SetActive(true);
        isGameHaveStarted = true;
        var indexMes = lobbyMessages[0];
        var currentPlayers = LobbyDataSync.Init(indexMes);
        var finalServerAddress = currentPlayers[currentPlayers.Count - 1].serverWillJoin;
        Debug.Log("Final server is: " + finalServerAddress);
        return finalServerAddress; //ws:192.168.220.129:8089/?playerName=thaocute&mapPosition=1_2
    }
    void LobbyUpdate()
    {
        var lastMessage = lobbyMessages[lobbyMessages.Count - 1];
        Debug.Log("Last message is: " + lastMessage);

        var currentLobbySize = lastMessage.Count(f => f == '{') / 2;
        Debug.Log("Lobby still update, current player is: " + currentLobbySize);
        title.text = currentLobbySize + " / " + lobbySize +" players join to lobby" + "\n You is player " + playerIndex;
        if (currentLobbySize >= lobbySize)
        {
            title.text = "READY TO START GAME BABIES..";
            Invoke("StartGameFromLobby", 2.0f);
        }
    }
    
    void Start()
    {
        inGameMenuPrefab.SetActive(false);
        isGameHaveStarted = false;
        lobbyMessages = new List<string>();
        Debug.Log("Lobby Start finished");
    }
    void FixedUpdate()
    {
        Debug.Log("Lobby Start update");

        if(!isGameHaveStarted)
            LobbyUpdate();
        
    }
}
