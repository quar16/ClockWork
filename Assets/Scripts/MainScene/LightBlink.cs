using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlink : MonoBehaviour
{
    public Light light;
    bool blink = true;
    float max;
    float min;
    float frequency;
    public void Blink(float _min, float _max, float _frequency)
    {
        blink = true;
        min = _min;
        max = _max;
        frequency = _frequency;
        StartCoroutine(Blinking());
    }
    IEnumerator Blinking()
    {
        float l = 0;
        float fok = (min + max) / 2;
        float originRange = light.range;
        float originIntencity = light.intensity;
        while (blink)
        {
            l += Time.deltaTime * Random.Range(0.3f, 3.0f);

            light.range = originRange + Mathf.Sin(2 * Mathf.PI * l / frequency) * fok;
            light.intensity = originIntencity + Mathf.Sin(2 * Mathf.PI * l / frequency) * fok;

            yield return null;
            if (l > frequency)
            {
                l -= frequency;
                fok = Random.Range(min, max);
            }
        }
    }
}
