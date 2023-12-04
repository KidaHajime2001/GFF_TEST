using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MoveController : MonoBehaviour, PlayerAct.IPlayerActionActions
{
    [Tooltip("�ړ��Ɏg��Rigidbody")]
    [SerializeField]new  Rigidbody rigidbody;

    [Tooltip("�v���C���[�̈ړ��x�N�g��")]
    Vector3 velocity;

    [Tooltip("�v���C���[�̈ړ����x")]
    [SerializeField]float speed = 5;

    [Tooltip("InputAction�Őݒ肵���Ǝ���Action")]
    PlayerAct.PlayerActionActions input;

    //[Tooltip("GameObject��Transform")]
    //Transform _transform;

    [Tooltip("���X�ɉ�]�����邽�߂̖ڕWQuaternion")]
    Quaternion targetRot;

    [Tooltip("��]�̑��x")]
    float rotateStep = 0.1f;

    [Tooltip("�ǂ̊p�x�����]�I���Ƃ݂Ȃ����̒l")]
    float rotationAngle=1f;

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
        var speedPow=speed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateStep);
       // Debug.Log("rota"+transform.rotation.eulerAngles);
        Debug.Log("Tage" + targetRot.eulerAngles);
        if(transform.rotation.eulerAngles.y - targetRot.eulerAngles.y>= rotationAngle)
        {
            speedPow = 0.5f;
        }
        //�ړ������Ƒ��x�œ�����
        rigidbody.velocity = velocity * speedPow;
    }


    void OnDisable() => input.Disable();
    public void OnJump(InputAction.CallbackContext context)
    {
        
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
}
