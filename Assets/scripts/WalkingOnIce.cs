using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingOnIce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Ice"))
        {
            var pc = GetComponent<cshPlayerController>();
            var ec = GetComponent<EnemyController>();
            if (gameObject.name == "Enemy")
                ec.speedMul += 5f;
            else
                pc.speedMul += 5f;
        }
    }
}
