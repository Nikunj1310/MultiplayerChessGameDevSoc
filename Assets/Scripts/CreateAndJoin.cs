using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputCreate;
    [SerializeField] private TMP_InputField inputJoin;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(inputCreate.text);
        // TODO: cover players being able to create room with empty name
        // TODO: cover players being in same room but on different photon servers
        // TODO: cover Photon.Realtime, RoomOptions, TypedLobby, string[] expectedUsers
    }
    // TODO: Have a automated room ID generated for user to create room easier

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputJoin.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        PhotonNetwork.LoadLevel("Scenes/Game");
    }

    // TODO: Add a button for quick join, which joins a random room

    // TODO: Add error handling for room creation/joining failures, have a canvas that displays errors, and add a button what user can click to turn back to lobby, or close the canvas automatically after a few seconds
}
