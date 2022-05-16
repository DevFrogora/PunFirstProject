using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviourPunCallbacks, IPunObservable
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        public Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        public Gun gun;
        public RigBuilder rigBuilder;
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                //m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            if(!photonView.IsMine)
            {
                GetComponentInChildren<Camera>().enabled = false;
                GetComponentInChildren<AudioListener>().enabled = false;
            }
            gun = GetComponent<Gun>();
           rigBuilder =  GetComponent<RigBuilder>();
        }


        private void Update()
        {
            if (photonView.IsMine)
            {
                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
                Look();
            }

        }
        [Range(0.5f,5f)]
        public float mouseSensitivity = 2f;
        [Range(0.5f, 5f)]
        public float maxPitch = 85f;
        [Range(0.5f, 5f)]
        public float minPitch = -85f;
        [Range(0.5f, 5f)]
        public float pitch = 0f;
        void Look()
        {
            float xInput = Input.GetAxis("Mouse X") * mouseSensitivity;
            float yInput = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(0, xInput, 0);
            pitch -= yInput;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rot = Quaternion.Euler(pitch, 0, 0);
            m_Cam.localRotation = rot;
            //gun.gameObject.transform.localRotation = rot;
            //StartCoroutine(AutoUpdateTheRigBuilder());
        }

        IEnumerator AutoUpdateTheRigBuilder()
        {
            
            yield return new WaitForSeconds(0.2f);
            //GetComponent<Animator>().WriteDefaultValues();
            rigBuilder.gameObject.SetActive(false);
            rigBuilder.gameObject.SetActive(true);
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                // read inputs
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
                bool crouch = Input.GetKey(KeyCode.C);

                // calculate move direction to pass to character
                if (m_Cam != null)
                {
                    // calculate camera relative direction to move:
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = v * m_CamForward + h * m_Cam.right;
                }
                else
                {
                    // we use world-relative directions in the case of no main camera
                    m_Move = v * Vector3.forward + h * Vector3.right;
                }
#if !MOBILE_INPUT
                // walk speed multiplier
                if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

                // pass all parameters to the character control script
                m_Character.Move(m_Move, crouch, m_Jump);
                m_Jump = false;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
           if(stream.IsWriting)
            {
                stream.SendNext(pitch);
            }
            else
            {
                pitch = (float) stream.ReceiveNext();
                Quaternion rot = Quaternion.Euler(pitch, 0, 0);
                m_Cam.localRotation = rot;
            }
        }
    }
}
