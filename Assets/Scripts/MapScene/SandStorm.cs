using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStorm : MonoBehaviour
{
    float plus = 0.2f;
    float minus = 0;
    public SpriteRenderer sprite;
    float speed = 0;
    private void Start()
    {
        speed = Random.Range(0.01f, 0.03f);
        StartCoroutine(Storm());
    }
    private void Update()
    {
        transform.position += Vector3.right * speed;
    }

    IEnumerator Storm()
    {
        Color color = sprite.color;
        float a = Random.Range(0.1f, 0.5f);
        while (color.a < a)
        {
            color.a += Random.Range(0, 0.005f);
            sprite.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));

        while (color.a > 0)
        {
            color.a -= Random.Range(0, 0.01f);
            sprite.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
