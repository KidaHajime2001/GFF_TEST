using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ParameterName
{
    MoveVecMagnitude,
    Speed,
}

public class AnimationController : MonoBehaviour
{
    
    Dictionary<ParameterName,string> parameterNameDic = new Dictionary<ParameterName,string>();


    [Tooltip("�A�j���[�V�����̊Ǘ��ɕK�v")]
    Animator animController;
    [Tooltip("MoveController")]
    [SerializeField] MoveController moveController;
    // Start is called before the first frame update
    void Start()
    {

        AddDic();

    }

    // Update is called once per frame
    void Update()
    {
        animController.SetFloat(
            parameterNameDic[ParameterName.MoveVecMagnitude],
            moveController.GetMoveVectorMagnitude()
            );


    }
    /// <summary>
    /// �p�����[�^�[�Ǘ��̂��߂�enum�^�ƃp�����[�^�[����R�Â�
    /// </summary>
    void AddDic()
    {
        parameterNameDic.Add(ParameterName.MoveVecMagnitude,"MoveVecMagnitude");
        parameterNameDic.Add(ParameterName.Speed, "Speed");
    }

    //public void SetParameterFloat(ParameterName _pN,)
    //{
    //    animController.SetFloat()
    //}
    //public void SetParameterBool(ParameterName _pN)
    //{
    //    animController.s
    //}

}
