using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Gun : MonoBehaviourPunCallbacks
    {

        public Transform gunTransform;
        public ParticleSystem ps;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //shoot
                    photonView.RPC("RPC_Shoot", RpcTarget.All);
                }
            }

        }


        [PunRPC]
        void RPC_Shoot()
        {
            //shoot
            ps.Play();
            Ray ray = new Ray(gunTransform.position, gunTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // we hit something
                // Check hit has Health Script we can take some value off health.
                var enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth)
                {
                    // we have hit an enemy-
                    enemyHealth.TakeDamage(20);
                }
            }
        }
    }
}
