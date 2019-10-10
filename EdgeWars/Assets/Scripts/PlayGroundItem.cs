using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayGroundItem : MonoBehaviour
{
    GameObject playGroundController;
    PlayGround playGround;
    // Start is called before the first frame update
    void Start()
    {
        playGroundController = GameObject.Find("Main Camera");
        playGround = playGroundController.GetComponent<PlayGround>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        playGround.onItemMouseClick(this.gameObject);
    }
}
