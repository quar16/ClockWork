using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDot : MonoBehaviour
{
    public LineEnd lineEnd;
    public Vector3 start;
    public Vector3 end;
    public float i = 0;
    void Update()
    {
        start = lineEnd.start;
        transform.position = Vector3.Lerp(start, end, i);
        transform.position += Vector3.up * (-32 * Mathf.Pow((i - 0.5f), 2) + 4.5f);
        i += Time.deltaTime / 2;

        if (i > 0.95f)
        {
            Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
