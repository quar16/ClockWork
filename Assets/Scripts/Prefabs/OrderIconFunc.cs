using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderIconFunc : MonoBehaviour
{
    public AllManager manager;

    public int myOrder;
    public int orderTestInt;
    public FlowManager.TYPE type;
    public Image image;
    public Image face;
    public Text text;
    
    float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if(manager.flow.nowOrder != orderTestInt)
        {
                timer = 0;
        }

        image.rectTransform.localPosition = Vector2.Lerp(image.rectTransform.localPosition, new Vector2(600, 300 + 100 * (manager.flow.nowOrder - myOrder)), timer);
        
        orderTestInt = manager.flow.nowOrder;
    }
}
