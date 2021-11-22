using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;


[RequireComponent(typeof(Optional<PixelPerfectCamera>))]
public class CameraController : MonoBehaviour
{
    
    private Vector3 cameraTarget = new Vector3(0, 0, 0);
    private Vector3 cameraPos = new Vector3(0, 0, 0);
    public Optional<GameObject> targetObject;
    private Camera mainCamera;
    [SerializeField]
    private Optional<PixelPerfectCamera> pixelCamera;

    public Optional<Vector2> offset; //offset via viewport coords
    [Tooltip("Enable to directly target the targeted object with the targeting logic")]
    public bool directTarget = false; // Direct targeting no allowance for movement will lerp to position
    public float cameraSpeed = 10f;

    private Vector3 velocity = Vector3.zero;
    public GameObject velocityDebug;

    public float focusTime = 0.4f;


    //Bounds
    [SerializeField] [Tooltip("Is distance away from edge of screen in Viewport Space")]
    [Range(0,0.5f)]private float screenSkin = 0.4f;  //this is the % for the screen skin
    private Vector2 vertBounds; //These bounds are for the initial box (L,R)
    [SerializeField]
    private Vector2 horzBounds;
    [SerializeField]
    private Vector2 horzClamp;
    
    [Header("Zoom")]
    [Tooltip("Controls Zoom Level")]
    public int pixelsPerUnit = 192;
    [SerializeField]
    private Vector2 baseResolution = new Vector2(640, 360);

    [SerializeField]
    private Vector2 baseRatio;
    [SerializeField]
    private Vector2 baseRatioMulti;

    [SerializeField] 
    private Vector2 currentResolution;


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

        currentResolution = baseResolution;

        baseRatio = GetAspectRatio(Screen.width, Screen.height);

        baseRatioMulti = baseResolution / baseRatio;
    }

    private Vector2 GetAspectRatio(int aScreenWidth, int aScreenHeight)
    {
        float r = (float)aScreenWidth / (float)aScreenHeight;
        string _r = r.ToString("F2");
        string ratio = _r.Substring(0, 4);
        switch (ratio)
        {
            case "2.37":
            case "2.39":
                return (new Vector2(21,9));
            case "1.25":
                return (new Vector2(5, 4));
            case "1.33":
                return (new Vector2(4, 3));
            case "1.50":
                return (new Vector2(3, 2));
            case "1.60":
            case "1.56":
                return (new Vector2(16, 10));
            case "1.67":
            case "1.78":
            case "1.77":
                return (new Vector2(16, 9));
            case "0.67":
                return (new Vector2(2, 3));
            case "0.56":
                return (new Vector2(9, 16));
            default:
                return (Vector2.zero);
        }
    }



void FixedUpdate()
    {
        //PixelPerfect();
        if (targetObject.Enabled)
        {
            cameraTarget = new Vector3(0.5f, 0.5f, 0);
            boundCheck();
            //cameraTarget = mainCamera.WorldToViewportPoint(targetObject.Value.transform.position);
            cameraTarget.z = 0;

            if(offset.Enabled)
                cameraTarget += (Vector3)offset.Value;
        }

        if (Input.GetKeyDown("f"))
        {
            StartCoroutine(LResZoom(baseResolution*2.5f, 2));
        }

        if (Input.GetKeyDown("g"))
        {
            StartCoroutine(LResZoom(baseResolution / 3, 2));
        }

        cameraPos = Vector3.SmoothDamp((transform.position),
            (targetObject.Value.transform.position), ref velocity, focusTime);
        cameraPos.z = -10f;

        velocityDebug.GetComponent<TextMeshProUGUI>().SetText("Cam velocity: " + velocity);

        transform.position = cameraPos;
    }

    private void LateUpdate()
    {
        {
            //moveTowardsTarget(mainCamera.ViewportToWorldPoint(cameraTarget), cameraSpeed);

        }
    }

    void boundCheck()
    {
        if(directTarget)
        {
            if (targetObject.Enabled) {
                //check if within bounds
                if (mainCamera.WorldToViewportPoint(targetObject.Value.transform.position).x >= horzBounds.y)
                {
                    cameraTarget = mainCamera.WorldToViewportPoint(PixelPerfectClamp(targetObject.Value.transform.position, pixelsPerUnit));
                    cameraTarget.x -= screenSkin/2; // this is for bounds follow
                }else if(mainCamera.WorldToViewportPoint(targetObject.Value.transform.position).x <= horzBounds.x){
                    cameraTarget = mainCamera.WorldToViewportPoint(PixelPerfectClamp(targetObject.Value.transform.position, pixelsPerUnit));
                    cameraTarget.x += screenSkin / 2;
                }
                Debug.Log(cameraTarget.x);
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
        cameraPos = Vector3.SmoothDamp(
            (Vector3)PixelPerfectClamp(transform.position, pixelsPerUnit) + new Vector3(0, 0, transform.position.z), 
            (Vector3)PixelPerfectClamp(target, 64) + new Vector3(0, 0, transform.position.z), ref velocity, focusTime);
        //2nd best

        //cameraPos.x = Mathf.SmoothStep(PixelPerfectClamp(transform.position, 64).x, PixelPerfectClamp(target, 64).x, speed*Time.deltaTime);
        transform.position = (Vector3)PixelPerfectClamp(cameraPos, pixelsPerUnit) + new Vector3(0, 0, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(mainCamera.ViewportToWorldPoint(cameraTarget), 0.1f);   
    }

    #region Zoom
    IEnumerator LZoom(int targetPPU, float zoomTime)
    {
        int originalPPU = pixelsPerUnit;
        float t = 0f;
        while(pixelsPerUnit != targetPPU)
        {
            pixelsPerUnit = (int)Mathf.Lerp(originalPPU, targetPPU, t/zoomTime);
            t += Time.deltaTime;

            pixelCamera.Value.assetsPPU = pixelsPerUnit;

            yield return null;
        }
        yield return null;
    }

    IEnumerator LResZoom(Vector2 targetRes, float zoomTime)
    {
        Vector2 originalRes = currentResolution;
        float t = 0f;
        while (currentResolution != targetRes)
        {
            currentResolution = Vector2.Lerp(originalRes, targetRes, t / zoomTime);
            t += Time.deltaTime;

            //confirm that the resolution is usable
            if ((Mathf.RoundToInt(currentResolution.y / (int)baseRatio.y) * (int)baseRatio.y) %2 == 0)
            {
            pixelCamera.Value.refResolutionX = Mathf.RoundToInt(currentResolution.x / (int)baseRatio.x) * (int)baseRatio.x;
                pixelCamera.Value.refResolutionY = Mathf.RoundToInt(currentResolution.y / (int)baseRatio.y) * (int)baseRatio.y;
            }

            yield return null;
        }
        yield return null;
    }

    IEnumerator SLZoom(int targetZoom, float zoomTime)
    {
        float originalPPU = pixelsPerUnit;
        float t = 0f;
        while (pixelsPerUnit != targetZoom)
        {
            pixelsPerUnit = (int)Mathf.SmoothStep(originalPPU, targetZoom, t / zoomTime);
            t += Time.deltaTime;
            
            yield return null;
        }
        yield return null;
    }
	#endregion
}