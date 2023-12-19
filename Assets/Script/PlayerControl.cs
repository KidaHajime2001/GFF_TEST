using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
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


    [Tooltip("垂直の加速度")]
    float verticalVelocity;

    [Tooltip("ジャンプの力")]
    public float jumpPower = 7.0f;

    //[Tooltip("落下速度")]
    //public float fallSpeed = 5.0f;

    [Tooltip("重力")]
    public float gravity = -10.0f;

    [Tooltip("下限落下速度")]
    public float LIMIT_FALL_SPEED = -10.0f;

    [Tooltip("落下速度")]
    float animationBlend;


    float WALK_SPEED = 30.0f;
    float RUN_SPEED = 100.0f;
    float SPEED_CHANGE_RATE = 10.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    private float rotationVelocity;

    [Tooltip("アニメーションの管理に必要")]
    Animator animator;

    int animIDSpeed;
    int animIDGround;
    int animIDJump;
    int animIDFreeFall;

    float JUMP_COOL_TIME = 0.5f;
    float coolTimeCountStart;
    bool jumpCoolTimeFlag=false;

    float JETPACK_POWER=14;
    bool jetFlag=false;
    [SerializeField] private ParticleSystem particle;
    //[SerializeField] GameObject particlePos;
    void Awake()
    {
        // インプットを生成して、自身をコールバックとして登録
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
        animator = GetComponent<Animator>();
        SetAnimationID();
    }

    void OnEnable() => input.Enable();
    // インプットの有効・無効化
    void OnDestroy() => input.Disable();
    void OnDisable() => input.Disable();
    void Update()
    {
        if(jetFlag)
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
        

        Move();
        CalculationGravity();
        //Debug.Log("velo"+ verticalVelocity);
        if (CheckGroundStatus())
        {
            //Debug.Log("着地");
            
            if(jumpFlag)
            {
                jumpCoolTimeFlag = true;
                coolTimeCountStart = Time.time;
            }
            jumpFlag = false;
            if(animator)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
                animator.SetBool(animIDGround, true);
            }

        }
        else
        {
            animator.SetBool(animIDGround, false);
            animator.SetBool(animIDFreeFall, true);
        }
        if (jumpCoolTimeFlag&&JUMP_COOL_TIME<Time.time-coolTimeCountStart)
        {
            jumpCoolTimeFlag = false;
        }
    }

    void SetAnimationID()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDGround = Animator.StringToHash("Ground");
        animIDJump = Animator.StringToHash("Jump");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(!jumpFlag&&!jumpCoolTimeFlag&&context.started)
        {
            verticalVelocity = jumpPower;
            jumpFlag = true;
            if (animator)
            {
                animator.SetBool(animIDJump, true);
            }
        }
        if (context.performed)
        {
            jetFlag = true;
            Debug.Log("長押し" + jetFlag);
        }
        if (context.canceled && jetFlag)
        {
            jetFlag = false;
            Debug.Log("長押し終わり" + jetFlag);
        }

    }
    void CalculationGravity()
    {
        var jet= jetFlag ? JETPACK_POWER : 0;
        verticalVelocity += (gravity +jet) * Time.deltaTime;
        Debug.Log("verticalVelocity:" + verticalVelocity);
        if (!CheckGroundStatus())
        {
            
            if (verticalVelocity < LIMIT_FALL_SPEED)
            {
                verticalVelocity = LIMIT_FALL_SPEED;
            }
        }



    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //入力をベクトルへ
        targetDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;
        

        //入力なしの対策
        if (targetDirection.magnitude <= 0.1f) targetDirection = Vector3.zero;

        
        //if(context.canceled)
        //{
        //    Debug.Log(context.ReadValue<Vector2>());
        //}


    }
    void Move()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //Debug.Log("CameraF:"+cameraForward+"<INPUT:"+targetDirection+">");

        var moveVec = cameraForward * targetDirection.z + Camera.main.transform.right * targetDirection.x;
        
        
        float targetSpeed = runFlag ? RUN_SPEED : WALK_SPEED;
        float coef= jetFlag ? 5:1;
        
        targetSpeed *= coef;
        if (targetDirection == Vector3.zero)
        {
            targetSpeed = 0;
        }
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SPEED_CHANGE_RATE);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;

        }
        else
        {
            speed = targetSpeed;
        }
        
        animationBlend = Mathf.Lerp(animationBlend,targetSpeed,Time.deltaTime * SPEED_CHANGE_RATE);
        if (animator)
        {

            //Debug.Log("Speed:" + animationBlend);
            animator.SetFloat(animIDSpeed, animationBlend);
        }
        //移動方向へ回転
        AdjustDirection(moveVec);



        characterController.Move(moveVec.normalized * (speed * Time.deltaTime) +
            new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime
            );

        //Debug.Log("cameraF:" + cameraVec);
    }


    private void AdjustDirection(Vector3 _moveVec)
    {
        if (targetDirection != Vector3.zero)
        {
            var targetRotation = Mathf.Atan2(_moveVec.x,_moveVec.z) * Mathf.Rad2Deg/* +Camera.main.transform.eulerAngles.y*/;
            float mRotation= Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, mRotation, 0.0f);

        }

      
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
