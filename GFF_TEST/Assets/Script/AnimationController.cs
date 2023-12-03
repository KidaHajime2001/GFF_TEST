using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    enum ParameterName
    {
        MoveVecMagnitude,
        IsRotation,
    }
    Dictionary<ParameterName,string> parameterNameDic = new Dictionary<ParameterName,string>();


    [Tooltip("�A�j���[�V�����̊Ǘ��ɕK�v")]
    Animator animController;
    [Tooltip("MoveController")]
    [SerializeField] MoveController moveController;
    // Start is called before the first frame update
    void Start()
    {

        AddDic();

        animController = GetComponent<Animator>();
        
        

    }

    // Update is called once per frame
    void Update()
    {
        animController.SetFloat(
            parameterNameDic[ParameterName.MoveVecMagnitude],
            moveController.GetMoveVectorMagnitude()
            );

        animController.SetBool(parameterNameDic[ParameterName.IsRotation], moveController.IsRotation());

    }
    /// <summary>
    /// �p�����[�^�[�Ǘ��̂��߂�enum�^�ƃp�����[�^�[����R�Â�
    /// </summary>
    void AddDic()
    {
        parameterNameDic.Add(ParameterName.MoveVecMagnitude,"MoveVecMagnitude");
        parameterNameDic.Add(ParameterName.IsRotation, "IsRotation");
    }
}
