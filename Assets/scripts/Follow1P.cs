using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Follow1P : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Vector3 firstLocalOffset = new Vector3(0f, 1.5f, 0.15f); // 1인칭 머리 높이쯤
    [SerializeField] Vector3 tpEuler = new Vector3(60f, 0f, 0f);
    [SerializeField] Vector3 offset = new Vector3(0, 20, -15);
    public bool isFirstPerson = false;
    public GameObject target;
    public GameObject playerinfo;
    public GameObject enemyinfo;
    public float smoothTime = 0.1f;
    public Transform targetpos;
    Vector3 _vel;

    float distance = 15f;              // 타겟과 카메라 거리
    float pitch = 50f;                 // 내려다보는 각도(고정)
    float yawSpeed = 20f;              // 초당 회전 속도(도)

    private float yaw;                        // 누적 Y 회전값
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) target = GameObject.Find("Hero");  // 새 Hero로 타깃 갱신
        targetpos = target.transform;
        
        if (playerinfo.GetComponent<StaticValue>().Loopline == 3 || enemyinfo.GetComponent<EnemyReborn>().LoopLine == 3) {
            //1인칭이였든 3인칭이였든 승리혹은 패배시
            yaw += yawSpeed * Time.unscaledDeltaTime;

            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 ofset = rot * new Vector3(0f, 0f, -distance);

            // 위치 고정 + 타겟 바라보기
            transform.position = targetpos.position + ofset;
            transform.LookAt(targetpos.position);
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
            isFirstPerson = !isFirstPerson;
        Vector3 wanted = targetpos.position + offset;

        if (isFirstPerson)
        {
            // 1인칭: 타겟 로컬 기준 firstLocalOffset 위치로, 타겟 바라보는 방향
            Vector3 wantedFP = targetpos.TransformPoint(firstLocalOffset);
            transform.position = Vector3.SmoothDamp(transform.position, wantedFP, ref _vel, smoothTime);
            transform.rotation = Quaternion.LookRotation(targetpos.forward, Vector3.up);
            switch (target.tag)
            {
                case "dog":
                    firstLocalOffset = new Vector3(0f, 1f, 0.1f);
                    break;

                case "Tiger":
                    firstLocalOffset = new Vector3(0f, 1.5f, 0.2f);
                    break;

                case "chicken":
                    firstLocalOffset = new Vector3(0f, 0.5f, 0.1f);
                    break;

                case "horse":
                    firstLocalOffset = new Vector3(0f, 2.0f, 0.15f);
                    break;

                case "deer":
                    firstLocalOffset = new Vector3(0f, 1.8f, 0.2f);
                    break;

                case "penguin":
                    firstLocalOffset = new Vector3(0f, 1.4f, -0.25f);
                    break;

                case "kitty":
                    firstLocalOffset = new Vector3(0f, 0.5f, -0.2f);
                    break;
            }
        }
        else  //3인칭일때
        {
                Vector3 wantedTP = new Vector3(targetpos.position.x, offset.y, targetpos.position.z + offset.z);
                transform.position = Vector3.SmoothDamp(transform.position, wantedTP, ref _vel, smoothTime);
                transform.rotation = Quaternion.Euler(tpEuler);
        }
        
    }
}
