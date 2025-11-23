using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharStats))]
public class EnemyController : MonoBehaviour
{
    [Header("스킬 & 스탯")]
    public Skill skill;
    public float HP;
    public float skillTimer;
    public float speedMul;

    [Header("라인별 Redzone (Line1→0 ... Line4→3)")]
    public Transform[] redzones;
    public int currentLine = 0;

    [Header("라인 정체 시 사망 처리할 정체 시간")]
    public float stayTimeLimit = 30f; //30초 이상 머무르면 사망
    private float stayTimer = 0f;
    private int lastLine;

    [Header("기본 회피 범위 설정")]
    public float detectDistance = 5f;
    public float avoidDuration = 1.0f;
    public float sideCheckDistance = 2.0f;

    [Header("Raycast 회피 세부 설정")]
    public float RayOffset = 0.5f;
    public float RayLength = 3.5f;
    public int obstacleRayDepth = 3;
    public int minEscapeRayCount = 5; // 이동 가능 레이가 이 정도 이상이어야 방향을 정함

    private float avoidTimer = 0;
    private Vector3 avoidDirection = Vector3.zero;

    //Tuple이 안되서 대신 사용할 클래스
    private class RayInfo
    {
        public Ray ray;
        public bool hit;
    }

    private List<RayInfo> listRays = new List<RayInfo>();
    private Vector3 detectDir;
    private bool isObstacleDetected = false;

    CharStats stats;
    Animator animator;

    void Awake()
    {
        stats = GetComponent<CharStats>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        HP = stats.MaxHP;
        skillTimer = 0;
        speedMul = 2f;
        Invoke(nameof(ResetSpeed), 4f);
        lastLine = currentLine;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        skillTimer += dt;

        AutoSkillCast();
        DetectObstacle();

        if (avoidTimer > 0)
        {
            avoidTimer -= dt;
            MoveAvoid(dt);
        }
        else if (isObstacleDetected)
        {
            ApplyObstacleAvoid();
        }
        else
        {
            if (DetectThreat())
                StartSimpleAvoid();

            AutoMoveRedzone(dt);
        }

        //라인에 특정 시간 이상 머무를 경우 kill itself
        if (currentLine == lastLine)
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayTimeLimit)
            {
                Debug.Log($"[Enemy]가 {stayTimeLimit}초 넘게 라인 {currentLine+1}에 머물러 사망하였습니다.");
                HP=0;
            }
        }
        else
        {
            //라인 변경되면 타이머 초기화
            stayTimer = 0f;
            lastLine = currentLine;
        }

    }

    //위협물 & 장애물 감지 -> rotate & avoid

    void DetectObstacle()
    {
        listRays.Clear();
        isObstacleDetected = false;

        Vector3[] baseDirs = new Vector3[] { transform.right, -transform.right, transform.forward };

        // 기본 3 방향 레이
        foreach (var dir in baseDirs)
            CastRay(dir);

        // 재귀로 중간 방향 추가
        for (int i = 0; i < 2; i++)
            RecursiveRay(baseDirs[i], baseDirs[2], 0);
    }

    void CastRay(Vector3 dir)
    {
        Ray ray = new Ray(transform.position + Vector3.up * RayOffset, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RayLength))
        {
            isObstacleDetected = true;
            Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.red);
            listRays.Add(new RayInfo { ray = ray, hit = true });
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.green);
            listRays.Add(new RayInfo { ray = ray, hit = false });
        }
    }

    void RecursiveRay(Vector3 dir1, Vector3 dir2, int depth)
    {
        if (depth > obstacleRayDepth) return;

        Vector3 mid = (dir1 + dir2).normalized;
        CastRay(mid);

        // 깊이 증분 후 좌/우로 다시 분기
        RecursiveRay(dir1, mid, depth + 1);
        RecursiveRay(dir2, mid, depth + 1);
    }

    void ApplyObstacleAvoid()
    {
        var nonHitDirs = listRays
                        .Where(x => !x.hit)
                        .Select(x => x.ray.direction)
                        .ToList();

        if (nonHitDirs.Count >= minEscapeRayCount)
        {
            detectDir = Vector3.zero;
            foreach (var d in nonHitDirs)
                detectDir += d;

            detectDir /= nonHitDirs.Count;
            detectDir.y = 0;

            avoidDirection = detectDir.normalized;
            avoidTimer = avoidDuration;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(detectDir), Time.deltaTime * 3f);
        }
    }

    //Threat layer인 것만 피하게 함
    bool DetectThreat()
    {
        LayerMask layer = LayerMask.GetMask("Threat");
        return Physics.Raycast(transform.position, transform.forward, detectDistance, layer);
    }

    //raycast로 안 잡히는 거 피하기(ex. pee...)
    void StartSimpleAvoid()
    {
        avoidTimer = avoidDuration;

        bool leftBlocked = Physics.Raycast(transform.position, -transform.right, sideCheckDistance);
        bool rightBlocked = Physics.Raycast(transform.position, transform.right, sideCheckDistance);

        if (!leftBlocked && rightBlocked)
            avoidDirection = -transform.right;
        else if (leftBlocked && !rightBlocked)
            avoidDirection = transform.right;
        else if (!leftBlocked && !rightBlocked)
            avoidDirection = Random.value > 0.5f ? transform.right : -transform.right;
        else
            avoidDirection = -transform.forward;
    }

    void MoveAvoid(float dt)
    {
        transform.Translate(avoidDirection * dt * stats.MoveSpeed * speedMul, Space.World);
    }

    // 기존 Redzone 경로 이동

    void AutoMoveRedzone(float dt)
    {
        if (redzones == null || redzones.Length == 0) return;
        if (currentLine < 0 || currentLine >= redzones.Length) return;

        Transform target = redzones[currentLine];
        if (!target) return;

        //그냥 redzone을 pos로 넣으면 중앙 좌표로만 향함
        //redzone이 바라보는 방향으로 이동
        Vector3 dir = target.forward;  // redzone 방향
        dir.y = 0;

        //만약 벽이나 장애물 때문에 target 쪽이 막혀있으면 target.position 기반 보정
        if (Physics.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Wall")))
        {
            dir = (target.position - transform.position).normalized;
            dir.y = 0;
        }

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);
            animator.SetFloat("Vert", 1, 0.2f, Time.deltaTime);

            if (speedMul == 2) animator.SetFloat("State", 0, 0.2f, Time.deltaTime);
            else if (speedMul == 4) animator.SetFloat("State", 1, 0.2f, Time.deltaTime);
        }

        transform.Translate(dir * dt * stats.MoveSpeed * speedMul, Space.World);
    }


    // 스킬, 충돌 처리

    void AutoSkillCast()
    {
        if (skillTimer >= stats.SkillCooltime)
        {
            skillTimer = 0;
            skill.Cast();
        }
    }

    public void ResetSpeed() => speedMul = 2f;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            HP = Mathf.Max(HP - 1, 0);
        }
    }
}
