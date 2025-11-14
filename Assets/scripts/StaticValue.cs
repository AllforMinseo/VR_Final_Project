using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticValue : MonoBehaviour
{   
    [SerializeField] public GameObject player;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public int Lastline = 0;   // 여기 저장해두고 계속 씀
    public int Loopline = 0;   // 여기 저장해두고 계속 씀
    // Update is called once per frame
    void Update()
    {
        if (player == null) player = GameObject.Find("Hero");
    }
}
