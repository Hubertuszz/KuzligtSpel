using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{

    #region Private Fields

    [Tooltip("UI Text to display Players Name")]
    [SerializeField]
    private Text playerNameText;

    [Tooltip("Ui Slider to display Players Health")]
    [SerializeField]
    private Slider playerHealthSlider;

    private PlayerManager target;
    private PhotonView m_PhotonView;

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    #endregion

    #region Public Fields
    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    #endregion

    #region MonoBehaviour Callbacks

    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = target.Health;
        }

        if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    #endregion

    #region Public Methods

    void Awake()
    {
        this.m_PhotonView = GetComponent<PhotonView>();
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void SetTarget(PlayerManager _target)
    {
        if(_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        target = _target;
        if(playerNameText != null)
        {
            playerNameText.text = this.target.m_PhotonView.Owner.NickName;
        }

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();
        if(characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }

    void LateUpdate()
    {
        if(targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        if(targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.targetTransform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

    #endregion

}
