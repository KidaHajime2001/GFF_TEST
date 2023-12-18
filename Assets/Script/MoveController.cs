using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MoveController : MonoBehaviour, PlayerAct.IPlayerActionActions
{
    [Tooltip("�ړ��Ɏg��Rigidbody")]
    [SerializeField] new Rigidbody rigidbody;

    [Tooltip("�v���C���[�̈ړ��x�N�g��")]
    Vector3 velocity;

    [Tooltip("�v���C���[�̈ړ����x")]
    [SerializeField] float speed = 5;

    [Tooltip("InputAction�Őݒ肵���Ǝ���Action")]
    PlayerAct.PlayerActionActions input;

    //[Tooltip("GameObject��Transform")]
    //Transform _transform;

    [Tooltip("���X�ɉ�]�����邽�߂̖ڕWQuaternion")]
    Quaternion targetRot;

    [Tooltip("��]�̑��x")]
    float rotateStep = 1.0f;

    [Tooltip("�ǂ̊p�x�����]�I���Ƃ݂Ȃ����̒l")]
    float rotationAngle = 1f;

    [Tooltip("JumpPower")]
    float jumpPower = 10.0f;

    [Tooltip("jumpFlag")]
    bool jumpFlag = false;

    [Tooltip("groundCheckRadius")]
    float groundCheckRadius = 0.1f;

    [Tooltip("groundCheckOffsetY")]
    float groundCheckOffsetY = 0.0f;

    [Tooltip("groundCheckDistance")]
    float groundCheckDistance = 0.2f;

    [Tooltip("")]
    LayerMask groundLayers = 0;

    [Tooltip("")]
    RaycastHit hit;

    [Tooltip("")]
    float velocityY=0;


    void Awake()
    {
        // �C���v�b�g�𐶐����āA���g���R�[���o�b�N�Ƃ��ēo�^
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);

    }


    // �C���v�b�g�̗L���E������
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();

    private void Update()
    {
        if (CheckGroundStatus())
        {

            jumpFlag = false;
        }
        var speedPow=speed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateStep);
       // Debug.Log("rota"+transform.rotation.eulerAngles);
        //Debug.Log("Tage" + targetRot.eulerAngles);
        //if(transform.rotation.eulerAngles.y - targetRot.eulerAngles.y>= rotationAngle)
        //{
        //    speedPow = 0.5f;
        //}
        //�ړ������Ƒ��x�œ�����
        velocity.y = velocityY;
        rigidbody.velocity = velocity * speedPow;
        velocityY = 0;
        
    }


    void OnDisable() => input.Disable();
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpFlag = true;
        velocityY = jumpPower;

        Debug.Log(jumpFlag);



    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //���͂��x�N�g����
        velocity=new Vector3(context.ReadValue<Vector2>().x, 0,context.ReadValue<Vector2>().y).normalized;
        
        //���͂Ȃ��̑΍�
        if (velocity.magnitude <= 0.1f) velocity=Vector3.zero;
        
        //�ړ������։�]
        AdjustDirection(context.ReadValue<Vector2>());
        //Debug.Log(velocity);
    }

    private void AdjustDirection(Vector2 _inputVec)
    {

        var delta = new Vector3(_inputVec.x,0,_inputVec.y);
        if (delta.magnitude <= 0.1f)
        {
            delta = Vector3.zero; return;
        }

        // �i�s�����i�ړ��ʃx�N�g���j�Ɍ����悤�ȃN�H�[�^�j�I�����擾
        var rotation = Quaternion.LookRotation(delta, Vector3.up);
        // �I�u�W�F�N�g�̉�]�ɔ��f
        targetRot = rotation;
    }

    /// <summary>
    /// �A�j���[�V�����̃R���g���[���[�ɁA�L�����N�^�[�p�ړ��x�N�g���̑傫����n��
    /// </summary>
    /// <returns>�ړ��x�N�g���̑傫��/Velocity.magnitude</returns>
    public float GetMoveVectorMagnitude()
    {
        return velocity.magnitude;
    }
    public bool IsRotation()
    {
        if (Math.Abs(transform.rotation.eulerAngles.y - targetRot.eulerAngles.y) >= rotationAngle)
        {
            return true;
        }

        return false;
    }

    bool CheckGroundStatus()
    {
        
        return Physics.SphereCast(transform.position + groundCheckOffsetY * Vector3.up, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + groundCheckOffsetY * Vector3.up, groundCheckRadius);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}
