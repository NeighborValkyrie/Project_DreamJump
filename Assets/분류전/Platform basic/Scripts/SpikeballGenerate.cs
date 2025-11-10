using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeballGenerate : MonoBehaviour
{
    public  GameObject spikeball;
    public float generationInterval;
    public Vector2 rangeX;
    
    private float timer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        timer = generationInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = generationInterval;
            Vector3 pos = transform.position;
            pos.x = Random.Range(rangeX.x, rangeX.y);
            GameObject obj = Instantiate(spikeball, pos, Quaternion.identity);
            obj.SetActive(true);
        }
    }
}
