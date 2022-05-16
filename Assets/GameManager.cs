using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject pauseCanvas;
    public bool paused = false;

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(553, 5, 496), Quaternion.identity);
        SetPaused();
    }

    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            SetPaused();
        }
    }

    void SetPaused()
    {
        //set the canvas 
        pauseCanvas.SetActive(paused);
        // set the cursor lock
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        // set the cursor visible
        Cursor.visible = paused;
    }

}
