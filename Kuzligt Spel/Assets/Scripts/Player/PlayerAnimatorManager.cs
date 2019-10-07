using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class PlayerAnimatorManager : MonoBehaviour
{
    #region Private Fields

    [SerializeField]
    private float directionDampTime = 0.25f;

    #endregion

    #region MonoBehavior Callbacks

    private Animator animator;
    private PhotonView m_PhotonView;

    void Awake()
    {
        this.m_PhotonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_PhotonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.IsName("Base Layer.Run"))
        {
            if (Input.GetButtonDown("Fire2"))
            {
                animator.SetTrigger("Jump");
            }
        }
        if (!animator)
        {
            return;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(v < 0)
        {
            v = 0;
        }
        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }

    #endregion
}
