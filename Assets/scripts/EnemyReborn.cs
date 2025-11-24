using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReborn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject Enemy;
    public int LastLine = 0;
    public int LoopLine = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy == null) Enemy = GameObject.Find("Enemy");
    }
}
