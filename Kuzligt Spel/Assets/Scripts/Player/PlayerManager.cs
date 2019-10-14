using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

using Photon.Pun;


public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsFiring);
        }
        else
        {
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion

    #region Public Fields
    [Tooltip("The current Healght of our player")]
    public float Health = 1f;

    [Tooltip("The local player instance. Use this to know if th elocal player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    #endregion

    #region Private Fields

    [Tooltip("The Beams GameObject to control")]
    [SerializeField]
    private GameObject beams;
    public PhotonView m_PhotonView;

    [Tooltip("The Players UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;

    bool IsFiring;

    #endregion

    #region Private Methods

    #endregion

    #region MonoBehaviour CallBacks

    #if !UNITY_5_4_OR_NEWER

        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
    #endif

    

    void CalledOnLevelWasLoaded(int level)
    {
        GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        if(Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_PhotonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }
        Health -= 0.1f;
    }

    void OnTriggerStay(Collider other)
    {
        if (!m_PhotonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }
        Health -= 0.1f * Time.deltaTime;
    }

    void Awake()
    {

        
        this.m_PhotonView = GetComponent<PhotonView>();

        if (m_PhotonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);

        if (beams == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }
    }

    void Start()
    {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

        if(PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red></a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        if(_cameraWork != null)
        {
            if (m_PhotonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a/></Color> Camera Work Component on playerPrefab.", this);
        }

        
    }


    // Update is called once per frame
    void Update()
    {
        if (m_PhotonView.IsMine)
        {
            ProcessInputs();
            if(Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }

        if(beams != null && IsFiring != beams.activeSelf)
        {
            beams.SetActive(IsFiring);
        }
    }

#endregion

    #region Custom

    void ProcessInputs()
    {
        if (m_PhotonView.IsMine)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }
    }

#endregion


}
