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
            return;
        }
        else if (other.gameObject.tag == "Untagged")
        {
            return;
        }
        else if (other.gameObject.tag == "Ice")
        {
            GetComponent<cshPlayerController>().speedMul = 1.5f;
            
            return;
        }
        else if(other.gameObject.tag == "pee")
        {
            GetComponent<cshPlayerController>().speedMul = 1f;
            
            return;
        }
        else if (other.gameObject.tag == "Lava")
        {
            GetComponent<cshPlayerController>().HP -= 20;
            return;
        }
        else
        {
            Debug.Log($"Triggered by: {other.name}");
            string name = other.name;
            if (name == "Line1") { rebo.Loopline++; }//한바퀴 돌면 루프라인 추가
            int i = int.Parse(name.Replace("Line", "")) - 1;

            Transform player = transform.parent;                    
            var prefab = heroCandidates[Random.Range(0, heroCandidates.Length)];

            Destroy(gameObject);                                     
            var newHero = Instantiate(prefab, lineSpawns[i].position, lineSpawns[i].rotation, player);
            rebo.Lastline = i;      
            newHero.name = "Hero";                                   
            FindObjectOfType<Follow1P>().targetpos = newHero.transform;                     
            var ui = GameObject.FindObjectOfType<cshUI>();
            if (ui) ui.player = newHero;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 트리거에서 벗어나면 기본 속도로 복귀
        GetComponent<cshPlayerController>().speedMul = 2f;
    }
    void Update() {
        if (GetComponent<cshPlayerController>().HP <= 0)
        {

            Transform player = transform.parent;                   
            var prefab = heroCandidates[Random.Range(0, heroCandidates.Length)];
            Destroy(gameObject);                                    
            var newHero = Instantiate(prefab, lineSpawns[rebo.Lastline].position, lineSpawns[rebo.Lastline].rotation, player);
            newHero.name = "Hero";                                   
            FindObjectOfType<Follow1P>().targetpos = newHero.transform;                     
            var ui = GameObject.FindObjectOfType<cshUI>();
            if (ui) ui.player = newHero;
        }
    }
}