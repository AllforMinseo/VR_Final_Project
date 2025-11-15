using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshCollision : MonoBehaviour
{
    public float Hp;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //충돌시 총알지우기
            Destroy(collision.gameObject);
            if (gameObject.CompareTag("box"))
            {
                Hp--;
                if (Hp <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        
    }
}
