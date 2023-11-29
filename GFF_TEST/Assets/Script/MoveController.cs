using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MoveController : MonoBehaviour, PlayerAct.IPlayerActionActions
{

    [SerializeField]  Rigidbody rigidbody;
    Vector3 playerMoveVec=Vector3.zero;

    PlayerAct.PlayerActionActions input;
    void Awake()
    {
        // �C���v�b�g�𐶐����āA���g���R�[���o�b�N�Ƃ��ēo�^
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
    }
    // �C���v�b�g�̗L���E������
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();


    void OnDisable() => input.Disable();
    public void OnJump(InputAction.CallbackContext context)
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
        Vector3 moveForwardVec = context.ReadValue<Vector2>().y*Camera.main.transform.forward+context.ReadValue<Vector2>().x * Camera.main.transform.right;

        transform.rotation = Quaternion.LookRotation(moveForwardVec);
        Debug.Log(moveForwardVec);
        moveForwardVec*=10;
        rigidbody.AddForce(moveForwardVec,ForceMode.Force);
    }
}
