using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public abstract class EnemyItself
    {
        public string name;
        public float maxhp, dmg, speed, maxspeed, gravity;
        public virtual void MovementUpdate(GameObject enemy, GameObject player)
        {
            GameObject ori = enemy.transform.Find("ori").gameObject;
            Rigidbody rb = enemy.GetComponent<Rigidbody>();

            float slerprate = 2f;
            Quaternion targetrot = Quaternion.LookRotation(player.transform.position - enemy.transform.position, Vector3.up);
            ori.transform.rotation = Quaternion.Slerp(ori.transform.rotation, targetrot, slerprate);

            Vector3 wishdir = (player.transform.position - enemy.transform.position).normalized;
            Vector3 velnoy = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //velnoy *= touchingsurface ? .9f : .95f;
            velnoy *= 0.95f; 
            Vector3 addspeed = wishdir * speed * Mathf.Clamp(maxspeed / velnoy.magnitude, 0f, 5f);
            velnoy += addspeed;


            rb.velocity = new Vector3(velnoy.x, rb.velocity.y - gravity * Time.fixedDeltaTime, velnoy.z);
        }
        public virtual void VisualUpdate(GameObject enemy)
        {
            
        }
    }
    public class Charles : EnemyItself
    {
        public Charles() {
            name = "Charles";
            maxhp = 10f; 
            dmg = 1f; 
            speed = 2f;
            maxspeed = 5f;
            gravity = 9f;
        }

    }
    EnemyItself enemyitself;
    void Start()
    {
        enemyitself = new Charles();
    }

    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        GameObject player = GameObject.Find("player");
        if (player != null)
        {
            enemyitself.MovementUpdate(this.gameObject, player);
        }
    }
}
