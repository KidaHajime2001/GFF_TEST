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
    [Tooltip("キャラクターコントローラー")]
    [SerializeField] CharacterController characterController;

    [Tooltip("移動方向ベクトル")]
    Vector3 targetDirection;

    [Tooltip("移動速度")]
    float speed = 3.0f;

    [Tooltip("走りかどうかのフラグ")]
    bool runFlag;

    [Tooltip("設置判定の球状レイの半径")]
    [SerializeField] float groundCheckRadius = 0.1f;

    [Tooltip("")]
    [SerializeField] float groundCheckOffsetY = 0.0f;

    [Tooltip("")]
    [SerializeField] float groundCheckDistance = 0.2f;

    [Tooltip("レイヤーマスク：接地に使う")]
    [SerializeField] LayerMask groundLayers = 0;

    [Tooltip("レイのヒット")]
    RaycastHit hit;

    [Tooltip("徐々に回転させるための目標Quaternion")]
    Quaternion targetRot;

    PlayerAct.PlayerActionActions input;
    
    [Tooltip("ジャンプフラグ　false=ジャンプしていない,true=ジャンプしている")]
    bool jumpFlag;

    [Tooltip("落下速度")]
    float fallSpeed = 3.0f;

    [Tooltip("垂直の加速度")]
    float verticalVelocity;

    [Tooltip("ジャンプの力")]
    float jumpPower = 3.0f;

    float gravity = -5.0f;

    float animationBlend;


    float WALK_SPEED = 3.0f;
    float RUN_SPEED = 6.0f;


    void Awake()
    {
        // インプットを生成して、自身をコールバックとして登録
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
    }

    void OnEnable() => input.Enable();
    // インプットの有効・無効化
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
            Debug.Log("着地");
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
        //入力をベクトルへ
        targetDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;

        //入力なしの対策
        if (targetDirection.magnitude <= 0.1f) targetDirection = Vector3.zero;

        //移動方向へ回転
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

        // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
        var rotation = Quaternion.LookRotation(delta, Vector3.up);
        // オブジェクトの回転に反映
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
