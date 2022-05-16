using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            Connect();
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public void Play()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a room and failed");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room : "+ PhotonNetwork.CurrentRoom.Name + "- yay!");
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Leave()
    {
        StartCoroutine(Disconnect());
    }

    IEnumerator Disconnect()
    {
        PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(0);
        }
        while (PhotonNetwork.InRoom)
            yield return null;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("I left the room");
    }



}
