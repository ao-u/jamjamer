using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    void Start()
    {
        GenRoom(true);
        GenHallway();
    }

    private void FixedUpdate()
    {
        if (allrooms.Count > 4)
        {
            GameObject toberemoved = allrooms[0];
            allrooms.RemoveAt(0);
            Destroy(toberemoved);
        }
    }
    static int count = 0;
    static int localroomcount = 0;
    public static List<GameObject> allrooms = new List<GameObject>();
    public static void GenHallway()
    {
        if (GameObject.Find("spawnHallway") == null)
        {
            Director.LogTemp("NO HALLWAY SPAWNER IDIOT ??", Color.red);
        }
        GameObject spawner = GameObject.Find("spawnHallway");
        GameObject hwprefab = Resources.Load<GameObject>("prefabs/hallway" + Random.Range(1, 3));
        GameObject hw = Instantiate(hwprefab, spawner.transform.position, spawner.transform.rotation);
        hw.transform.name += count + "";
        count++;
        allrooms.Add(hw);
        Destroy(spawner);
    }
    public static void GenRoom(bool main)
    {
        
        
        if (GameObject.Find("spawnRoom") == null)
        {
            Director.LogTemp("NO ROOM SPAWNER IDIOT ??", Color.red);
        }
        GameObject spawner = GameObject.Find("spawnRoom");
        GameObject rprefab = Resources.Load<GameObject>("prefabs/room" + Random.Range(1, 3));
        if (main || localroomcount > Random.Range(6, 8))
        {
            rprefab = Resources.Load<GameObject>("prefabs/mainRoom");
            localroomcount = 0;
        }
        GameObject r = Instantiate(rprefab, spawner.transform.position, spawner.transform.rotation);
        if (count == 0)
        {
            GameObject hwprefab = Resources.Load<GameObject>("prefabs/hallway1");
            GameObject hw = Instantiate(hwprefab, r.transform.position, Quaternion.Euler(new Vector3(0f, 180f, 0f)));
            hw.transform.Find("doorholder1").Find("door1").GetComponent<Interactable>().index = "lockeddoor";
            hw.transform.Find("doorholder2").Find("door2").GetComponent<Interactable>().index = "lockeddoor";
            Destroy(hw.transform.Find("spawnRoom").gameObject);
            allrooms.Add(hw);
        }
        r.transform.name += count + "";
        count++;
        localroomcount++;
        allrooms.Add(r);
        Destroy(spawner);

        foreach (Transform c in r.transform)
        {
            //Debug.Log(c.name.ToString());
            if (c.tag == "itemspawnpoint")
            {
                if (Random.value < .9f) 
                {
                    GameObject itempref = Resources.Load<GameObject>("prefabs/item" + Random.Range(1, 5));
                    GameObject item = Instantiate(itempref, c.position, c.rotation);
                }
                
            }

        }
    }
}
