using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI inputCreate;
    [SerializeField] private TMP_InputField inputJoin;
    [SerializeField]
    RoomOptions roomOptions = new RoomOptions()
    {
        MaxPlayers = 2,
        IsVisible = false,
        IsOpen = true,
        PlayerTtl = 60000, // 1 minute
        EmptyRoomTtl = 60000 // 1 minute
    };
    [SerializeField] private TextMeshProUGUI roomJoinOrCreateErrorMessage;
    void Start()
    {
        inputCreate.text = GenerateRandomRoomName();
    }
    //  automated room ID generated for user to create room easier
    public void GenerateRandomRoomID()
    {
        inputCreate.text = GenerateRandomRoomName();
    }

    public void CreateRoom()
    {


        PhotonNetwork.CreateRoom(inputCreate.text, roomOptions);
        Debug.Log("Creating room: " + inputCreate.text);

    }

    private string GenerateRandomRoomName(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder result = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[Random.Range(0, chars.Length)]);
        }
        return "Room_" + result.ToString();
    }

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
    public void QuickJoinRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, null, null, null, roomOptions);
    }

    // TODO: Add error handling for room creation/joining failures, have a canvas that displays errors, and add a button what user can click to turn back to lobby, or close the canvas automatically after a few seconds
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
        roomJoinOrCreateErrorMessage.text = "Room creation failed: " + message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room joining failed: " + message);
        roomJoinOrCreateErrorMessage.text = "Room joining failed: " + message;
    }

}
