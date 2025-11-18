using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharStats))]
public class EnemyController : MonoBehaviour
{
    [Header("스킬 & 스탯")]
    public Skill skill;

    [Header("현재 상황")]
    public float HP;
    public float skillTimer;
    public float speedMul;

    [Header("라인별 Redzone (Line1→0 ... Line4→3)")]
    public Transform[] redzones;

    [HideInInspector] public int currentLine = 0;

    [Header("회피 설정")]
    public float detectDistance = 5f;       // 위협 감지 거리
    public float avoidDuration = 1.0f;      // 회피 유지 시간
    public float sideCheckDistance = 2.0f;  // 좌우 벽체크 거리

    private float avoidTimer = 0;
    private Vector3 avoidDirection = Vector3.zero;

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
    }

    void Update()
    {
        float dt = Time.deltaTime;
        skillTimer += dt;

        AutoSkillCast();

        // 회피 중이면 회피 이동
        if (avoidTimer > 0)
        {
            avoidTimer -= dt;
            MoveAvoid(dt);
        }
        else
        {
            // 위협 감지 → 회피 시작
            if (DetectThreat())
                StartAvoid();

            // 기본 이동
            AutoMoveRedzone(dt);
        }
    }

    void AutoSkillCast()
    {
        if (skillTimer >= stats.SkillCooltime)
        {
            skillTimer = 0;
            skill.Cast();
        }
    }


    //=====================
    // 회피 감지
    //=====================
    bool DetectThreat()
    {
        LayerMask layer = LayerMask.GetMask("Threat");
        // "Threat"는 bullet/pee/poison이 속한 Layer로 설정해야 한다.
        // Layer 대신 Tag 기반으로 바꾸고 싶으면 말해줘.

        return Physics.Raycast(transform.position, transform.forward, detectDistance, layer);
    }


    //=====================
    // 회피 시작
    //=====================
    void StartAvoid()
    {
        avoidTimer = avoidDuration;

        // 좌우 방향 체크
        bool leftBlocked = Physics.Raycast(transform.position, -transform.right, sideCheckDistance);
        bool rightBlocked = Physics.Raycast(transform.position, transform.right, sideCheckDistance);

        // 이동 가능한 방향 선택
        if (!leftBlocked && rightBlocked)
            avoidDirection = -transform.right;     // 왼쪽만 가능
        else if (leftBlocked && !rightBlocked)
            avoidDirection = transform.right;      // 오른쪽만 가능
        else if (!leftBlocked && !rightBlocked)
            avoidDirection = (Random.value > 0.5f ? transform.right : -transform.right); // 둘다 가능 → 랜덤
        else
            avoidDirection = -transform.forward;   // 양쪽 다 막힘 → 뒤로 회피
    }


    //=====================
    // 회피 이동
    //=====================
    void MoveAvoid(float dt)
    {
        transform.Translate(avoidDirection * dt * stats.MoveSpeed * speedMul, Space.World);
    }


    //=====================
    // 원래 Redzone 방향으로 이동
    //=====================
    void AutoMoveRedzone(float dt)
    {
        if (redzones == null || redzones.Length == 0) return;
        if (currentLine < 0 || currentLine >= redzones.Length) return;

        Transform target = redzones[currentLine];
        if (!target) return;

        Vector3 dir = (target.position - transform.position);
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);
            animator.SetFloat("Vert", 1, 0.2f,Time.deltaTime);
            if (speedMul == 2) animator.SetFloat("State", 0, 0.2f, Time.deltaTime);
            else if (speedMul == 4) animator.SetFloat("State", 1, 0.2f, Time.deltaTime);
        }

        transform.Translate(dir.normalized * dt * stats.MoveSpeed * speedMul, Space.World);
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
