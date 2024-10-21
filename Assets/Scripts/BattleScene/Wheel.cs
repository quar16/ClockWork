using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    float angle = 0;
    float speed = 0;
    // Update is called once per frame
    void Update()
    {
        speed += Time.deltaTime * Random.Range(0.1f, 0.5f);
        angle += speed * 0.2f;
        gameObject.transform.rotation = Quaternion.Euler(0, 225, angle);
        if (speed > Random.Range(4.5f, 6.0f))
        {
            speed = Random.Range(-0.2f, -0.1f);
        }
    }
}
