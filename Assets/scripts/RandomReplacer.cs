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
            string name = other.name;                 //라인을 넘으면 gameObject가 삭제될것이므로
            if (name == "Line1") { rebo.Loopline++; } // 라인정보는 부모객체의 스크립트 변수에 저장
            int i = int.Parse(name.Replace("Line", "")) - 1;  // 라인정보를 인덱스로 바꾸기기

            Transform player = transform.parent;
            var prefab = heroCandidates[Random.Range(0, heroCandidates.Length)];

            Destroy(gameObject);    //자가삭제후 새 동물 랜덤생성                                 
            var newHero = Instantiate(prefab, lineSpawns[i].position, lineSpawns[i].rotation, player);
            rebo.Lastline = i;      //제일 최근 돌았던 라인정보도 부모객체에 저장장
            newHero.name = "Hero";    //주인공 이름은 반드시 고정
            FindObjectOfType<Follow1P>().targetpos = newHero.transform; //갈곳잃은 UI와 카메라도 다시 붙임
            var ui = GameObject.FindObjectOfType<cshUI>(); 
            if (ui) ui.player = newHero;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice" || other.gameObject.tag == "pee")
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