using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleWidthCamera : MonoBehaviour
{

    public int targetWitdh = 320;
    public float pixelsToUnits = 100;

    // Update is called once per frame
    void Update()
    {
        int height = Mathf.RoundToInt(targetWitdh / (float)Screen.width * Screen.height);
        Camera.main.orthographicSize = height / pixelsToUnits / 2;
    }
}
