using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomReplacer : MonoBehaviour
{
    [Header("교체할 Enemy 프리팹 후보들")]
    [SerializeField] private GameObject[] enemyCandidates;

    [Header("라인별 스폰 위치 (Line1→0 ... Line4→3)")]
    [SerializeField] private Transform[] lineSpawns;

    private EnemyReborn rebo;

    void Awake()
    {
        var enemy = transform.parent;
        rebo = enemy ? enemy.GetComponent<EnemyReborn>() : null;
        if (!rebo && enemy)
            rebo = enemy.gameObject.AddComponent<EnemyReborn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var ec = GetComponent<EnemyController>();

        if (other.CompareTag("poison"))
        {
            ec.HP -= 2;
            return;
        }

        if (other.CompareTag("pee"))
        {
            ec.speedMul = 1f;
            ec.Invoke(nameof(EnemyController.ResetSpeed), 2f);
            return;
        }

        // 여기부터는 Line 충돌 처리
        if (other.CompareTag("Line"))
        {
            string name = other.name;
            int i = int.Parse(name.Replace("Line", "")) - 1;

            Transform enemyRoot = transform.parent;
            var prefab = enemyCandidates[Random.Range(0, enemyCandidates.Length)];

            Destroy(gameObject);

            var newEnemy = Instantiate(
                prefab,
                lineSpawns[i].position,
                lineSpawns[i].rotation,
                enemyRoot
            );

            rebo.LastLine = i;

            //라인 번호를 EnemyController로 전달 -> enemy가 죽어도 죽은 라인의 레드존을 다시 향하도록
            newEnemy.GetComponent<EnemyController>().currentLine = i;

            newEnemy.name = "Enemy";
        }
    }

    void Update()
    {
        var ec = GetComponent<EnemyController>();

        if (ec && ec.HP <= 0)
        {
            Transform enemyRoot = transform.parent;
            var prefab = enemyCandidates[Random.Range(0, enemyCandidates.Length)];

            Destroy(gameObject);

            var newEnemy = Instantiate(
                prefab,
                lineSpawns[rebo.LastLine].position,
                lineSpawns[rebo.LastLine].rotation,
                enemyRoot
            );

            newEnemy.GetComponent<EnemyController>().currentLine = rebo.LastLine;

            newEnemy.name = "Enemy";
        }
    }
}
