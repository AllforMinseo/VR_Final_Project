using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cshUI : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject enemy;
    [SerializeField] public TextMeshProUGUI hptext;
    [SerializeField] public TextMeshProUGUI Timetext;
    [SerializeField] public TextMeshProUGUI Lose;
    [SerializeField] public TextMeshProUGUI Win;
    [SerializeField] public TextMeshProUGUI character;
    [SerializeField] public TextMeshProUGUI skilltxt;
    [SerializeField] public TextMeshProUGUI Looptxt;

    [SerializeField] public AudioClip win;
    [SerializeField] public AudioClip lose;
    public AudioSource audioSource;
    private bool once = true;

    public Image SKill;
    public Image HPbar;
    float gametime = 0;

    void Awake()
    {
        
        Win.gameObject.SetActive(false);
        Lose.gameObject.SetActive(false);
        once = true;

    }
    

    void Update()
    {
        if (player == null) player = GameObject.Find("Hero");
        if (enemy == null) enemy = GameObject.Find("Enemy");
        gametime += Time.deltaTime;

        if (player.GetComponent<cshPlayerController>().HP >= 0)//죽지않고 플레이중인경우
        {
            //hptext.text = "HP : + " + player.GetComponent<cshPlayerController>().HP;
            HPbar.fillAmount = player.GetComponent<cshPlayerController>().HP / player.GetComponent<CharStats>().MaxHP;
            SKill.fillAmount = player.GetComponent<cshPlayerController>().skillTimer / player.GetComponent<CharStats>().SkillCooltime;


        }
        if (player.transform.parent.GetComponent<StaticValue>().Loopline == 3 && once)// 승리시 시간멈추고 이김화면 표시
        {
            Win.gameObject.SetActive(true);
            audioSource.loop = false;
            audioSource.clip = win;
            audioSource.Play();
            Time.timeScale = 0f;
            once =false;
            hptext.gameObject.SetActive(false);
            HPbar.gameObject.SetActive(false);
            Timetext.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            Looptxt.gameObject.SetActive(false);
            skilltxt.gameObject.SetActive(false);
            SKill.gameObject.SetActive(false);
        }
        else if(enemy.transform.parent.GetComponent<EnemyReborn>().LoopLine == 3 && once)
        {
            Lose.gameObject.SetActive(true);
            audioSource.loop = false;
            audioSource.clip = lose;
            audioSource.Play();
            Time.timeScale = 0f;
            once = false;
            HPbar.gameObject.SetActive(false);
            hptext.gameObject.SetActive(false);
            Timetext.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            Looptxt.gameObject.SetActive(false);
            skilltxt.gameObject.SetActive(false);
            SKill.gameObject.SetActive(false);
        }
        Timetext.text = $"Time = {gametime.ToString("F2")}";
        hptext.text = $"체력 :        {player.GetComponent<cshPlayerController>().HP}";
        Looptxt.text = $"남은바퀴수 : {3-player.GetComponentInParent<StaticValue>().Loopline}\n적 남은바퀴수 : {3 - enemy.GetComponentInParent<EnemyReborn>().LoopLine}";
        switch (player.tag)
        {
            case "dog":
                character.text = $"캐릭터 : 강아지";
                skilltxt.text = $"특수능력: 화면상의 총알을 지워요";
                break;

            case "Tiger":
                character.text = $"캐릭터 : 호랑이";
                skilltxt.text = $"특수능력: 다른동물과 접촉시 \n먹어버리며 대쉬를 쓸 수 있어요";
                break;

            case "chicken":
                character.text = $"캐릭터 : 닭";
                skilltxt.text = $"특수능력: 오늘 낳은 신선한 달걀을 \n발사하고 포탑에 맞지않아요";
                break;

            case "horse":
                character.text = $"캐릭터 : 말";
                skilltxt.text = $"특수능력: 기본적으로 빠르며 \n대쉬를 쓸 수 있어요";
                break;

            case "deer":
                character.text = $"캐릭터 : 사슴";
                skilltxt.text = $"특수능력: 체력을 3 회복해요";
                break;

            case "penguin":
                character.text = $"캐릭터 : 펭귄";
                skilltxt.text = $"특수능력: 15초뒤 사라지는 \n얼음을 설치해요";
                break;

            case "kitty":
                character.text = $"캐릭터 : 새끼고양이";
                skilltxt.text = $"특수능력: 둔화를 거는 오줌을\n남기고 포탑에 맞지않아요";
                break;

        }
    }
}
