using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltingIce : MonoBehaviour
{
    [Header("얼음이 다 녹는 시간")]
    public float meltTime = 15f;
    private float stayTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stayTime += Time.deltaTime;
        if (stayTime >= meltTime)
        {
            Debug.Log($"[Pinguin's ice]가 생성되고 {meltTime}초가 지나 녹아내렸습니다.");
            Destroy(gameObject);
        }
    }
}