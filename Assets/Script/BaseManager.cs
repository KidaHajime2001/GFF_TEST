using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [Tooltip("ÉXÉeÅ[ÉWÇPÇÃìyë‰Çìoò^")]
    [SerializeField] List<GameObject> firstBases = new List<GameObject>();
    [Tooltip("ÉXÉeÅ[ÉWÇQÇÃìyë‰Çìoò^")]
    [SerializeField] List<GameObject> secondBases = new List<GameObject>();
    [Tooltip("ÉXÉeÅ[ÉWÇRÇÃìyë‰Çìoò^")]
    [SerializeField] List<GameObject> thirdBases = new List<GameObject>();

    int nowStage=0;

    Dictionary<int,List<GameObject>> nowStageBases = new Dictionary<int,List<GameObject>>();
    private void Start()
    {
        nowStageBases.Add(0,firstBases);
        nowStageBases.Add(1,secondBases);
        nowStageBases.Add(2,thirdBases);
    }
    private void Update()
    {
        Debug.Log("nowStage:"+nowStage);
        if (Input.GetKeyDown(KeyCode.I))
        {
            nowStage++;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            nowStage--;
        }
    }
    public Vector3 GetNearBasesPosition(Vector3 _searcherPosition)
    {
        
        float minDis=0;
        float dis=0;
        int keyNum =0;
        int i = 0;
        foreach (var item in nowStageBases[nowStage])
        {
            dis = Vector3.Distance(item.gameObject.transform.position, _searcherPosition);
            if (i==0)
            {
                minDis = dis;
            }
            else if(minDis<=dis)
            {
                keyNum = i;
            }
            i++;
        }
        return nowStageBases[nowStage][keyNum].gameObject.transform.position;
    }



}
