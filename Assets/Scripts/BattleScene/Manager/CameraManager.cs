using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public IEnumerator Shake(float time, float power = 1)
    {
        float l = 0;
        Vector3 save = transform.position;
        while (l < time)
        {
            float shake = time - l;
            shake *= power;
            l += Time.deltaTime;
            transform.position = save + new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), Random.Range(-shake, shake));
            yield return null;
        }
        transform.position = save;
    }

    public IEnumerator bip(float time, float power = 1)
    {
        float l = 0;
        Vector3 save = transform.position;
        while (l < time)
        {
            float shake = power;
            l += Time.deltaTime;
            transform.position = save + new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), Random.Range(-shake, shake));
            yield return null;
        }
        transform.position = save;
    }
}
