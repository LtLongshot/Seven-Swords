using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject cameraTarget;
    private Camera playerCamera;

    private float screenSkin = 30;  //this is the % for the screen skin

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
        {
            Debug.Log("No Camera component detected");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool screenDetection()
    {
        return true;
    }
}
