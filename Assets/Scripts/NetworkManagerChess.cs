using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NetworkManagerChess : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject colorChoosingPanel;
    [SerializeField] private Button blackColor;
    [SerializeField] private Button whiteColor;
    [Header("Player Interation checks")]
    [SerializeField] private bool hasSelectedAColor = false;
    [SerializeField] private bool hasBothPlayersJoined = false;
    [SerializeField]
    private string creatorUid = "";
    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            creatorUid = (string)PhotonNetwork.CurrentRoom.CustomProperties["creatorId"];
            bool isCustomRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["isManuallyCreated"];
            bool isCreator = creatorUid == PhotonNetwork.LocalPlayer.UserId;

            if (isCreator && isCustomRoom)
            {
                colorChoosingPanel.SetActive(true);

                blackColor.onClick.AddListener(() => ChooseColor(true));
                whiteColor.onClick.AddListener(() => ChooseColor(false));
            }
            else
            {
                colorChoosingPanel.SetActive(false);
            }
        }
    }

    private void ChooseColor(bool isBlackCreator)
    {
        if (hasSelectedAColor)
            return;

        int creatorActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int joinedActor = 0;

        ExitGames.Client.Photon.Hashtable playercolors = new ExitGames.Client.Photon.Hashtable();
        if (isBlackCreator)
        {
            playercolors["black"] = creatorActor;
            playercolors["white"] = joinedActor;
        }
        else
        {
            playercolors["white"] = creatorActor;
            playercolors["black"] = joinedActor;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(playercolors);
        colorChoosingPanel.SetActive(false);
        hasSelectedAColor = true;
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            hasBothPlayersJoined = true;
        }

        if (hasBothPlayersJoined && hasSelectedAColor && (bool)PhotonNetwork.CurrentRoom.CustomProperties["isManuallyCreated"] == true)
        {
            ColorAssignToNewPlayer(PhotonNetwork.LocalPlayer);
            hasSelectedAColor = false;
        }

        if (hasBothPlayersJoined && (bool)PhotonNetwork.CurrentRoom.CustomProperties["isManuallyCreated"] == false)
        {
            AssignColorsRandomly();
            hasBothPlayersJoined = false;
        }
        else if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isManuallyCreated"] == false)
        {
            //Waiting for second player to join, do something like show a UI element
            Debug.Log("Waiting for second player to join...");
            return;
        }
        
        PlayerColorDisplay(PhotonNetwork.LocalPlayer);

    }

    public void PlayerColorDisplay(Player player)
    {

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("black") && props.ContainsKey("white"))
        {
            int blackPlayerActor = (int)props["black"];
            int whitePlayerActor = (int)props["white"];

            if (player.ActorNumber == blackPlayerActor)
            {
                Debug.Log("Player " + player.NickName + " is assigned the color Black.");
            }
            else if (player.ActorNumber == whitePlayerActor)
            {
                Debug.Log("Player " + player.NickName + " is assigned the color White.");
            }
            else
            {
                Debug.Log("Player " + player.NickName + " has not been assigned a color.");
            }
        }
        else
        {
            Debug.Log("Color properties are not set in the room.");
        }
    }

    public void ColorAssignToNewPlayer(Player newPlayer)
    {
        if (newPlayer.UserId == creatorUid)
        {
            return;
        }
        //NOTE: This is a copy, not a live reference to the mutable hashtable
        var props = PhotonNetwork.CurrentRoom.CustomProperties;


        if (props.ContainsKey("black") && props.ContainsKey("white"))
        {
            int blackPlayerActor = (int)props["black"];
            int whitePlayerActor = (int)props["white"];

            if (blackPlayerActor == 0 && newPlayer.ActorNumber != whitePlayerActor)
            {
                ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable();
                newProps["black"] = newPlayer.ActorNumber;
                PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);
            }
            else if (whitePlayerActor == 0 && newPlayer.ActorNumber != blackPlayerActor)
            {
                ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable();
                newProps["white"] = newPlayer.ActorNumber;
                PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);
            }
        }
        else
        {
            AssignColorsRandomly();
        }
    }

    private void AssignColorsRandomly()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        if (players.Count < 2)
            return;

        // Shuffle the player list to ensure randomness
        for (int i = 0; i < players.Count; i++)
        {
            Player temp = players[i];
            int randomIndex = Random.Range(i, players.Count);
            players[i] = players[randomIndex];
            players[randomIndex] = temp;
        }

        ExitGames.Client.Photon.Hashtable playerColors = new ExitGames.Client.Photon.Hashtable
        {
            { "black", players[0].ActorNumber },
            { "white", players[1].ActorNumber }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(playerColors);
    }
}
