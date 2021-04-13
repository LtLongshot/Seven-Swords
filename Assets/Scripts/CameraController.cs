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

    public Optional<Vector2> offset; //offset via viewport coords
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
    private Vector2 horzClamp;

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

        horzClamp = new Vector2(horzBounds.x - screenSkin / 2, horzBounds.y + screenSkin / 2);
    }


    void FixedUpdate()
    {
        if (targetObject.Enabled)
        {
            cameraTarget = new Vector3(0.5f, 0.5f, 0);
            boundCheck();
            //cameraTarget = mainCamera.WorldToViewportPoint(targetObject.Value.transform.position);
            cameraTarget.z = 0;

            if(offset.Enabled)
                cameraTarget += (Vector3)offset.Value;
            moveTowardsTarget(mainCamera.ViewportToWorldPoint(cameraTarget), cameraSpeed);
        }
    }

    void boundCheck()
    {
        if(directTarget)
        {
            if (targetObject.Enabled) {
                //check if within bounds
                Debug.Log(mainCamera.WorldToViewportPoint(targetObject.Value.transform.position).x);

                if (mainCamera.WorldToViewportPoint(targetObject.Value.transform.position).x >= horzBounds.y)
                {
                    cameraTarget = mainCamera.WorldToViewportPoint(PixelPerfectClamp(targetObject.Value.transform.position, 64));
                    cameraTarget.x -= 0.5f-screenSkin; // this is for bounds follow
                }else if(mainCamera.WorldToViewportPoint(targetObject.Value.transform.position).x <= horzBounds.x){
                    cameraTarget = mainCamera.WorldToViewportPoint(PixelPerfectClamp(targetObject.Value.transform.position, 64));
                    cameraTarget.x += 0.5f-screenSkin;
                }
            }
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
        //cameraPos = Vector3.SmoothDamp(
        //    (Vector3)PixelPerfectClamp(transform.position, 64) + new Vector3(0, 0, transform.position.z), 
        //    (Vector3)PixelPerfectClamp(target, 64) + new Vector3(0, 0, transform.position.z), ref velocity, focusTime);
        //2nd best


        //cameraPos = Vector3.Lerp((Vector3)PixelPerfectClamp(transform.position, 64) + new Vector3(0, 0, transform.position.z),
        //(Vector3)PixelPerfectClamp(target, 64) + new Vector3(0, 0, transform.position.z), speed*Time.deltaTime);

        cameraPos.x = Mathf.SmoothStep(PixelPerfectClamp(transform.position, 64).x, PixelPerfectClamp(target, 64).x, speed * Time.deltaTime);

        //cameraPos.x = Mathf.MoveTowards(PixelPerfectClamp(transform.position, 64).x, PixelPerfectClamp(target, 64).x, speed*Time.deltaTime);
        //This might be it but in Fixed update? (doesn't really work lacks behind the player)

        //cameraPos = target;
        transform.position = (Vector3)PixelPerfectClamp(cameraPos,64) + new Vector3(0, 0, transform.position.z);
        //transform.position = target;
    }

}