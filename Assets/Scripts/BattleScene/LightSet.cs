using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSet : MonoBehaviour
{
    public Light asd;
    float t = 0;
    int d = -1;
    public float max;
    public float min;
    public float deg;
    private void Update()
    {
        t = Time.deltaTime * d * deg;

        if (asd.intensity > max)
        {
            d = -1;
        }
        if (asd.intensity < min)
        {
            d = 1;
        }

        asd.intensity += t;//9.5 ~ 5
    }
}
