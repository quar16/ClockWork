using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLibrary : MonoBehaviour
{
    [System.Serializable]
    public struct ImageList
    {
        public Sprite[] Img;
    }
    public ImageList[] ShortImg;
    public ImageList[] LongImg;
}
