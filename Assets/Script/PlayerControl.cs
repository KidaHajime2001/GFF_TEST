using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Windows;
public class PlayerControl : MonoBehaviour, PlayerAct.IPlayerActionActions
{
    [Tooltip("�L�����N�^�[�R���g���[���[")]
    [SerializeField] CharacterController characterController;

    [Tooltip("�ړ������x�N�g��")]
    Vector3 targetDirection;

    [Tooltip("�ړ����x")]
    float speed = 3.0f;

    [Tooltip("���肩�ǂ����̃t���O")]
    bool runFlag;

    [Tooltip("�ݒu����̋��󃌃C�̔��a")]
    [SerializeField] float groundCheckRadius = 0.1f;

    [Tooltip("")]
    [SerializeField] float groundCheckOffsetY = 0.0f;

    [Tooltip("")]
    [SerializeField] float groundCheckDistance = 0.2f;

    [Tooltip("���C���[�}�X�N�F�ڒn�Ɏg��")]
    [SerializeField] LayerMask groundLayers = 0;

    [Tooltip("���C�̃q�b�g")]
    RaycastHit hit;

    [Tooltip("���X�ɉ�]�����邽�߂̖ڕWQuaternion")]
    Quaternion targetRot;

    PlayerAct.PlayerActionActions input;
    
    [Tooltip("�W�����v�t���O�@false=�W�����v���Ă��Ȃ�,true=�W�����v���Ă���")]
    bool jumpFlag;

    [Tooltip("�������x")]
    float fallSpeed = 3.0f;

    [Tooltip("�����̉����x")]
    float verticalVelocity;

    [Tooltip("�W�����v�̗�")]
    float jumpPower = 3.0f;

    float gravity = -5.0f;

    float animationBlend;


    float WALK_SPEED = 3.0f;
    float RUN_SPEED = 6.0f;


    void Awake()
    {
        // �C���v�b�g�𐶐����āA���g���R�[���o�b�N�Ƃ��ēo�^
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
    }

    void OnEnable() => input.Enable();
    // �C���v�b�g�̗L���E������
    void OnDestroy() => input.Disable();
    void OnDisable() => input.Disable();
    void Update()
    {
        characterController.Move(targetDirection.normalized * (speed * Time.deltaTime)+
            new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime
            );

        CalculationGravity();
        //Debug.Log("velo"+ verticalVelocity);
        if(CheckGroundStatus())
        {
            Debug.Log("���n");
            jumpFlag = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("jump");
        if(!jumpFlag)
        {
            verticalVelocity = jumpPower;
            jumpFlag = true;

        }

    }
    void CalculationGravity()
    {
        if(!CheckGroundStatus())
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -fallSpeed;
            }
        }

        verticalVelocity += gravity * Time.deltaTime;


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(targetDirection.normalized);
        //���͂��x�N�g����
        targetDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;

        //���͂Ȃ��̑΍�
        if (targetDirection.magnitude <= 0.1f) targetDirection = Vector3.zero;

        //�ړ������։�]
        AdjustDirection(context.ReadValue<Vector2>());

    }
    void Move()
    {
        //float targetSpeed = ;


    }


    private void AdjustDirection(Vector2 _inputVec)
    {

        var delta = new Vector3(_inputVec.x, 0, _inputVec.y);
        if (delta.magnitude <= 0.1f)
        {
            delta = Vector3.zero; return;
        }

        // �i�s�����i�ړ��ʃx�N�g���j�Ɍ����悤�ȃN�H�[�^�j�I�����擾
        var rotation = Quaternion.LookRotation(delta, Vector3.up);
        // �I�u�W�F�N�g�̉�]�ɔ��f
        targetRot = rotation;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            runFlag = true;
        }
        else if(context.canceled)
        {
            runFlag= false;
        }
        

    }
    bool CheckGroundStatus()
    {
        return Physics.CheckSphere(transform.position + groundCheckOffsetY * Vector3.up,
            groundCheckRadius,
            groundLayers);
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = CheckGroundStatus() ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position + groundCheckOffsetY * Vector3.up, groundCheckRadius);
    }
}
