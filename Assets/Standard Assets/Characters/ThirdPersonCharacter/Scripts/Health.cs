using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Health : MonoBehaviourPunCallbacks, IPunObservable
    {
        public int health;
        // Start is called before the first frame update
        void Start()
        {

        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //sync heatlh
            if (stream.IsWriting)
            {
                // we are writing
                stream.SendNext(health);
            }
            else
            { // we are reading
                health = (int)stream.ReceiveNext();
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
        }

        IEnumerator Respawn()
        {
            health = 100;
            GetComponent<ThirdPersonUserControl>().enabled = false;
            transform.position = new Vector3(553, 5, 496);
            yield return new WaitForSeconds(1);
            GetComponent<ThirdPersonUserControl>().enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (health <= 0)
            {
                StartCoroutine(Respawn());
            }
        }
    }
}