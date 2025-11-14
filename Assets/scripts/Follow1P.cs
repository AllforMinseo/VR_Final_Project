using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow1P : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 firstLocalOffset = new Vector3(0f, 1.5f, 0.15f); // 1인칭 머리 높이쯤
    [SerializeField] Vector3 tpEuler = new Vector3(60f, 0f, 0f);
    [SerializeField] Vector3 offset = new Vector3(0, 20, -15);
    public bool isFirstPerson = false;
    public Transform target;
    public GameObject playerinfo;
    public float smoothTime = 0.1f;
    Vector3 _vel;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) target = GameObject.Find("Hero")?.transform;  // 새 Hero로 타깃 갱신
        // 위치만 부드럽게 따라옴
        Vector3 wanted = target.position + offset;
        if (Input.GetKeyDown(KeyCode.T))
            isFirstPerson = !isFirstPerson;

        if (isFirstPerson)
        {
            // 1인칭: 타겟 로컬 기준 firstLocalOffset 위치로, 타겟 바라보는 방향
            Vector3 wantedFP = target.TransformPoint(firstLocalOffset);
            transform.position = Vector3.SmoothDamp(transform.position, wantedFP, ref _vel, smoothTime);
            transform.rotation = Quaternion.LookRotation(target.forward, Vector3.up);
        }
        else
        {
            // 3인칭: 기존 offset 따라가기 
            Vector3 wantedTP = new Vector3(target.position.x, offset.y, target.position.z+ offset.z);
            transform.position = Vector3.SmoothDamp(transform.position, wantedTP, ref _vel, smoothTime);
            transform.rotation = Quaternion.Euler(tpEuler);
        }
        
        if (playerinfo.GetComponent<StaticValue>().Loopline == 1) transform.position = new Vector3(22.5f, 30, -15f); //승리시 카메라 이동

    }
}
