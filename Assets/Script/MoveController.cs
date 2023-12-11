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
    [Tooltip("移動に使うRigidbody")]
    [SerializeField] new Rigidbody rigidbody;

    [Tooltip("プレイヤーの移動ベクトル")]
    Vector3 velocity;

    [Tooltip("プレイヤーの移動速度")]
    [SerializeField] float speed = 5;

    [Tooltip("InputActionで設定した独自のAction")]
    PlayerAct.PlayerActionActions input;

    //[Tooltip("GameObjectのTransform")]
    //Transform _transform;

    [Tooltip("徐々に回転させるための目標Quaternion")]
    Quaternion targetRot;

    [Tooltip("回転の速度")]
    float rotateStep = 1.0f;

    [Tooltip("どの角度から回転終わりとみなすかの値")]
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
        // インプットを生成して、自身をコールバックとして登録
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);

    }


    // インプットの有効・無効化
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
        //移動方向と速度で動かす
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
        //入力をベクトルへ
        velocity=new Vector3(context.ReadValue<Vector2>().x, 0,context.ReadValue<Vector2>().y).normalized;
        
        //入力なしの対策
        if (velocity.magnitude <= 0.1f) velocity=Vector3.zero;
        
        //移動方向へ回転
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

        // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
        var rotation = Quaternion.LookRotation(delta, Vector3.up);
        // オブジェクトの回転に反映
        targetRot = rotation;
    }

    /// <summary>
    /// アニメーションのコントローラーに、キャラクター用移動ベクトルの大きさを渡す
    /// </summary>
    /// <returns>移動ベクトルの大きさ/Velocity.magnitude</returns>
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
}
