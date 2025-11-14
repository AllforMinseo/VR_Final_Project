// PlayerControllerSimple.cs
using UnityEngine;

[RequireComponent(typeof(CharStats))]
public class cshPlayerController : MonoBehaviour
{
    [Header("래퍼런스")]
    public GameObject UI;
    public Skill skill;
    Animator animator;

    [Header("현재상황")]
    public float HP;
    public float skillTimer;
    public float speedMul;
    CharStats stats;

    [Header("카메라")]
    public GameObject cam;
    float LeftRight;  // 1인칭에서 쓸 고정 yaw(도)
    float turnSpeed = 90f; // A/D 회전 속도(도/초)
    void Awake()
    {
        stats = GetComponent<CharStats>();
        if (!UI) UI = GameObject.Find("UI");
        if (!animator) animator = GetComponent<Animator>();
    }

    void Start()
    {
        HP = stats.MaxHP;
        skillTimer = 10f;
        if (animator) animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        speedMul = 2f;                               // ★ 변경: 시작 기본값 세팅
        
    }

    void Update()
    {
        float dt = Time.deltaTime;
        skillTimer += dt;
        if (cam == null) cam = GameObject.FindWithTag("MainCamera");
        if (Time.timeScale == 0)
        {
            // 승패 처리/총알 정리는 필요 시 별도 이벤트로 빼도 됨
        }

        Vector3 dir = Vector3.zero;
        if (cam.GetComponent<Follow1P>().isFirstPerson)
        {
            
            // A/D로 '회전만' 제어 (돌기만 하고 이동축엔 반영 X)
            if (Input.GetKey(KeyCode.A)) LeftRight -= turnSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) LeftRight += turnSpeed * Time.deltaTime;

            // 안정적인 수평 forward/right 생성 
            Vector3 fwd = Quaternion.Euler(0f, LeftRight, 0f) * Vector3.forward;
            Vector3 right = new Vector3(fwd.z, 0f, -fwd.x); // 수평 우측

            if (Input.GetKey(KeyCode.W)) dir += fwd;
            if (Input.GetKey(KeyCode.S)) dir -= fwd;
            // 선택 스트레이프 (그대로 0.0f면 이동 없음)
            if (Input.GetKey(KeyCode.A)) dir -= right * 0.0f;
            if (Input.GetKey(KeyCode.D)) dir += right * 0.0f;

            dir = Vector3.ClampMagnitude(dir, 1f);

            // 1인칭 회전은 여기서 고정
            transform.rotation = Quaternion.Euler(0f, LeftRight, 0f);
        }
        else               // 3인칭: 월드 기준(기존 그대로)
        {
            if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
            if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) dir += Vector3.right;

        }

        if (Input.GetKeyDown(KeyCode.Space) && skillTimer >= stats.SkillCooltime)
        {
            skillTimer = 0f;
            skill.Cast();
            Invoke(nameof(RefreshSpeedMul), 4f); 
        }

        if (dir.sqrMagnitude > 1f) dir.Normalize();

        // ★ 3인칭일 때만 '이동 방향 바라보기' 적용
        if (!cam.GetComponent<Follow1P>().isFirstPerson && dir.sqrMagnitude > 0.01f)
        {
            var q = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.1f);
            animator.SetFloat("Vert", 1);
            if(speedMul==4) animator.SetFloat("State", 1);
        }
        else if(dir.sqrMagnitude < 0.01f) animator.SetFloat("Vert", 0);

        transform.Translate(dir * dt * stats.MoveSpeed * speedMul, Space.World);

    }

    public void RefreshSpeedMul() { speedMul = 2f; animator.SetFloat("State", 0); }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            HP = Mathf.Max(0, HP - 1f);
        }else if (col.gameObject.CompareTag("Tiger") && !gameObject.CompareTag("Tiger"))
        {
            HP = Mathf.Max(0, HP - 10f);
        }

    }
}
