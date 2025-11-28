// Skill.cs
using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum Kind
    {
        ClearTag,      // bullet삭제
        Heal,          // 체력 회복
        SpawnIce,   // 얼음 소환
        Dash,   // 대쉬
        SpawnPool, //pee 소환
        Shoot //egg 소환
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
        //컨트롤러에 HP/MaxHP가 있으니 그대로 활용
        var pc = GetComponent<cshPlayerController>();
        var ec = GetComponent<EnemyController>();
        if (gameObject.name == "Enemy")
            ec.HP = Mathf.Min(GetComponent<CharStats>().MaxHP, ec.HP + Mathf.Abs(healAmount));
        else
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
        var ec = GetComponent<EnemyController>();
        if (gameObject.name == "Enemy")
            ec.speedMul = 4f;
        else
            pc.speedMul = 4f;
    }

    void DoSpawn2()
    {
        if (!PoolToSpawn) return;
        Instantiate(PoolToSpawn, transform.position - transform.forward * 5f, transform.rotation);
    }
    void Shoot() {

        GameObject prefabBullet = Instantiate(bullet, SpawnPos.position, transform.rotation);
        prefabBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 500f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Ice"))
        {
            var pc = GetComponent<cshPlayerController>();
            var ec = GetComponent<EnemyController>();
            if (gameObject.name == "Enemy")
                ec.speedMul = 1.5f;
            else
                pc.speedMul = 1.5f;
        }
    }

}
