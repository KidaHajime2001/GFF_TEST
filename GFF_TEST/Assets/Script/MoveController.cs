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
        // インプットを生成して、自身をコールバックとして登録
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
    }
    // インプットの有効・無効化
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();


    void OnDisable() => input.Disable();
    public void OnJump(InputAction.CallbackContext context)
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForwardVec = context.ReadValue<Vector2>().y*Camera.main.transform.forward+context.ReadValue<Vector2>().x * Camera.main.transform.right;

        transform.rotation = Quaternion.LookRotation(moveForwardVec);
        Debug.Log(moveForwardVec);
        moveForwardVec*=10;
        rigidbody.AddForce(moveForwardVec,ForceMode.Force);
    }
}
