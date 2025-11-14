using System.Collections;
using UnityEngine;




public class RandomReplacer : MonoBehaviour
{
    [Header("교체할 Hero 프리팹 후보들")]
    [SerializeField] private GameObject[] heroCandidates;

    [Header("라인 이름별 스폰 위치 매핑 (line1→0, line2→1 ...)")]
    [SerializeField] private Transform[] lineSpawns;
    private StaticValue rebo;


    void Awake()
    {
        var player = transform.parent;
        rebo = player ? player.GetComponent<StaticValue>() : null;
        if (!rebo && player)
            rebo = player.gameObject.AddComponent<StaticValue>(); // 부모(Player)에 없으면 자동 부착
    }
    private void OnTriggerEnter(Collider other)
    {
        var pc = GetComponent<cshPlayerController>();
        if (other.gameObject.tag == "poison")
        {
            GetComponent<cshPlayerController>().HP -= 2;
            
        }
        else if(other.gameObject.tag == "pee")
        {
            GetComponent<cshPlayerController>().speedMul = 1f;
            pc.Invoke(nameof(cshPlayerController.RefreshSpeedMul), 2f);
        }
        else
        {
            Debug.Log($"Triggered by: {other.name}");
            string name = other.name;
            if (name == "Line1") { rebo.Loopline++; }//한바퀴 돌면 루프라인 추가
            int i = int.Parse(name.Replace("Line", "")) - 1;

            Transform player = transform.parent;                    // Player 보관
            var prefab = heroCandidates[Random.Range(0, heroCandidates.Length)];

            Destroy(gameObject);                                     // 나(Hero) 삭제
            var newHero = Instantiate(prefab, lineSpawns[i].position, lineSpawns[i].rotation, player);
            rebo.Lastline = i;      //마지막까지 돈 라인 기억하기
            newHero.name = "Hero";                                   // 이름 유지
            FindObjectOfType<Follow1P>().target = newHero.transform;                     // 카메라 타깃 갱신
            var ui = GameObject.FindObjectOfType<cshUI>();
            if (ui) ui.player = newHero;
        }
    }
    void Update() {
        if (GetComponent<cshPlayerController>().HP <= 0)
        {

            Transform player = transform.parent;                    // Player 보관
            var prefab = heroCandidates[Random.Range(0, heroCandidates.Length)];
            Destroy(gameObject);                                     // 나(Hero) 삭제
            var newHero = Instantiate(prefab, lineSpawns[rebo.Lastline].position, lineSpawns[rebo.Lastline].rotation, player);
            newHero.name = "Hero";                                   // 이름 유지
            FindObjectOfType<Follow1P>().target = newHero.transform;                     // 카메라 타깃 갱신
            var ui = GameObject.FindObjectOfType<cshUI>();
            if (ui) ui.player = newHero;
        }
    }
}