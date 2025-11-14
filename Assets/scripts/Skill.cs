// Skill.cs
using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum Kind
    {
        ClearTag,      // 특정 태그 전부 삭제(기존 동작)
        Heal,          // 체력 회복
        SpawnIce,   // 얼음 소환
        Dash, // 대쉬
        SpawnPool,
        Shoot,
        howling
    }

    [Header("공통")]
    public Kind kind = Kind.ClearTag;

    [Header("ClearTag")]
    public string tagToClear = "Bullet";

    [Header("Heal")]
    public float healAmount = 3f;

    [Header("SpawnIce")]
    public GameObject IceToSpawn;
    

    [Header("SpawnPool")]
    public GameObject PoolToSpawn;
    

    [Header("Shoot")]
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    Transform SpawnPos;

    [Header("Howling")]
    public Transform field;

    public void Cast()
    {
        switch (kind)
        {
            case Kind.ClearTag:
                DoClearTag();
                break;

            case Kind.Heal:
                DoHeal();
                break;

            case Kind.SpawnIce:
                DoSpawn();
                break;

            case Kind.Dash:
                Dash();
                break;

            case Kind.SpawnPool:
                DoSpawn2();
                break;
            case Kind.Shoot:
                Shoot();
                break;
        }
    }

    void DoClearTag()
    {
        var objs = GameObject.FindGameObjectsWithTag(tagToClear);
        foreach (var o in objs) Destroy(o);
    }

    void DoHeal()
    {
        // 네가 쓰는 컨트롤러에 HP/MaxHP가 있으니 그대로 활용
        var pc = GetComponent<cshPlayerController>();
        if (!pc) return;
        pc.HP = Mathf.Min(GetComponent<CharStats>().MaxHP, pc.HP + Mathf.Abs(healAmount));
    }

    void DoSpawn()
    {
        if (!IceToSpawn) return;
        Instantiate(IceToSpawn, transform.position - transform.forward * 5f, transform.rotation);
    }

    void Dash()
    {
        var pc = GetComponent<cshPlayerController>();
        pc.speedMul = 4f;
    }

    void DoSpawn2()
    {
        if (!PoolToSpawn) return;
        Instantiate(PoolToSpawn, transform.position - transform.forward * 5f, transform.rotation);
    }
    void Shoot() {

        GameObject prefabBullet = Instantiate(bullet, SpawnPos.position, transform.rotation);
        prefabBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 400f);
    }

}
