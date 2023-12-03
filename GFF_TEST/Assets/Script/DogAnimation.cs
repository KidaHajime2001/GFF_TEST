using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimation : MonoBehaviour
{

    enum ParameterName
    {
        MoveVecMagnitude,
        IsRotation,
        IsAttack,
    }
    Dictionary<ParameterName, string> parameterNameDic = new Dictionary<ParameterName, string>();


    [Tooltip("アニメーションの管理に必要")]
    Animator animController;
    [Tooltip("MoveController")]
    [SerializeField] DogController moveController;
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

        animController.SetBool(parameterNameDic[ParameterName.IsAttack], moveController.IsAttack());



    }
    /// <summary>
    /// パラメーター管理のためのenum型とパラメーター名を紐づけ
    /// </summary>
    void AddDic()
    {
        parameterNameDic.Add(ParameterName.MoveVecMagnitude, "MoveVecMagnitude");
        parameterNameDic.Add(ParameterName.IsRotation, "IsRotation");
        parameterNameDic.Add(ParameterName.IsAttack, "IsAttack");
    }
}
