using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    
    private Vector3 cameraTarget = new Vector3(0, 0, 0);
    private Vector3 cameraPos = new Vector3(0, 0, 0);
    public Optional<GameObject> targetObject;
    private Camera mainCamera;
    [SerializeField]
    private Optional<PixelPerfectCamera> pixelCamera;

    public Vector2 offset;
    public bool directTarget = false; // Direct targeting no allowance for movement will lerp to position
    public float cameraSpeed = 1f;

    //Bounds
    [SerializeField]
    private float screenSkin = 0.4f;  //this is the % for the screen skin
    private Vector2 vertBounds; //These bounds are for the initial box (L,R)
    private Vector2 horzBounds;
    [SerializeField]
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private float focusTime= 0f;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.Log("No Camera component detected, please attach camera component");
        }
        cameraTarget = mainCamera.WorldToViewportPoint(targetObject.Value.transform.position);
        cameraTarget.z = 0;


        vertBounds = new Vector2(screenSkin, 1 - screenSkin);
        horzBounds = new Vector2(screenSkin, 1 - screenSkin);
    }


    void LateUpdate()
    {
        if (targetObject.Enabled)
        {
            cameraTarget = mainCamera.WorldToViewportPoint(targetObject.Value.transform.position);
            cameraTarget.z = 0;
            cameraTarget += (Vector3)offset;
            //velocity = targetObject.Value.GetComponent<NewCharController>()._charVariables.velocity;
            moveTowardsTarget(mainCamera.ViewportToWorldPoint(cameraTarget), cameraSpeed);
        }
    }

    void smoothingCheck()
    {
        if(!directTarget)
        {
            //check if within bounds
        }
    }

    private Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit));

        return vectorInPixels / pixelsPerUnit;
    }

    //Move camera towards specified point
    private void moveTowardsTarget(Vector3 target, float speed)
    {
        cameraPos = Vector3.SmoothDamp(
            (Vector3)PixelPerfectClamp(transform.position, 64) + new Vector3(0, 0, transform.position.z), 
            (Vector3)PixelPerfectClamp(target, 64) + new Vector3(0, 0, transform.position.z), ref velocity, focusTime);
        transform.position = cameraPos;
        //transform.position = target;
    }

}