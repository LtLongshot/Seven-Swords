using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenSwords.CharacterCore;
using TMPro;


namespace SevenSwords
{
    public class OldCameraController : MonoBehaviour
    {
        private Vector3 cameraTarget = new Vector3(0, 0, 0);
        public GameObject playerObject;
        private Camera playerCamera;

        [SerializeField]
        private float screenSkin = 0.4f;  //this is the % for the screen skin

        const float slowLerp = 8f;
        private Vector2 vertBounds; //These bounds are for the initial box
        private Vector2 horzBounds;

        public float velThreshold = 2.5f;

        private float screenSkin2 = 0.10f;  //this is the % for the screen skin

        const float fastLerp = 4f;

        private float currentLerp = 1;

        private Vector2 vertBounds2;
        private Vector2 horzBounds2;

        public GameObject CameraPosDebug;

        private NewCharController charController;

        // Start is called before the first frame update
        void Start()
        {
            playerCamera = GetComponent<Camera>();
            if (playerCamera == null)
            {
                Debug.Log("No Camera component detected");
            }

            vertBounds = new Vector2(screenSkin, 1 - screenSkin);
            horzBounds = new Vector2(screenSkin, 1 - screenSkin);
            vertBounds2 = new Vector2(screenSkin2, 1 - screenSkin2);
            horzBounds2 = new Vector2(screenSkin2, 1 - screenSkin2);

            cameraTarget = playerCamera.WorldToViewportPoint(playerObject.transform.position);
            cameraTarget.z = 0;

            charController = playerObject.GetComponent<NewCharController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Mathf.Abs(charController._charVariables.velocity.x) < velThreshold)
            {

                if (playerCamera.WorldToViewportPoint(playerObject.transform.position).x < horzBounds.x || playerCamera.WorldToViewportPoint(playerObject.transform.position).x > horzBounds.y)
                {
                    currentLerp = (1f - Mathf.Abs(playerCamera.WorldToViewportPoint(playerObject.transform.position).x - 0.5f)) / slowLerp;
                    cameraTarget = playerCamera.WorldToViewportPoint(playerObject.transform.position);
                    cameraTarget.z = 0;
                    //currentLerp = 0.1f;
                    updatePosition(cameraTarget);
                }
                else
                {
                    currentLerp = (1f - Mathf.Abs(playerCamera.WorldToViewportPoint(playerObject.transform.position).x - 0.5f)) / slowLerp;
                    //if camera is at the target set to default
                    if (transform.position.x <= playerCamera.ViewportToWorldPoint(cameraTarget).x + 0.05 || transform.position.x >= playerCamera.ViewportToWorldPoint(cameraTarget).x + 0.05)
                    {
                        cameraTarget = new Vector3(0.5f, 0.5f, 0);
                    }
                    transform.position = playerCamera.ViewportToWorldPoint(cameraTarget);
                }
            }
            else
            {
                if (charController._charVariables.velocity.x > 0) //moving right
                {
                    cameraTarget = playerCamera.WorldToViewportPoint(playerObject.transform.position);
                    cameraTarget.x = cameraTarget.x + (0.5f - horzBounds2.x);
                    cameraTarget.z = 0;
                    currentLerp = (1f - Mathf.Abs(playerCamera.WorldToViewportPoint(playerObject.transform.position).x - 0.5f)) / fastLerp;
                    updatePosition(cameraTarget);
                }
                else if (charController._charVariables.velocity.x < 0) //moving left
                {
                    cameraTarget = playerCamera.WorldToViewportPoint(playerObject.transform.position);
                    cameraTarget.x = cameraTarget.x - (0.5f - horzBounds2.x);
                    cameraTarget.z = 0;

                    currentLerp = (1f - Mathf.Abs(playerCamera.WorldToViewportPoint(playerObject.transform.position).x - 0.5f)) / fastLerp;
                    updatePosition(cameraTarget);

                }
            }
            //transform.position = playerCamera.ViewportToWorldPoint(cameraTarget);
            //updatePosition(cameraTarget);

            //Target Debug
            CameraPosDebug.GetComponent<TextMeshProUGUI>().SetText("Cam Target Pos: " + cameraTarget);
        }

        public void updatePosition(Vector3 position)
        {
            float vel = 3;

            //transform.position = playerCamera.ViewportToWorldPoint(cameraTarget);
            transform.position = new Vector3(Mathf.SmoothStep(transform.position.x, playerCamera.ViewportToWorldPoint(cameraTarget).x, 10f * Time.deltaTime), transform.position.y, transform.position.z);
        }

        bool screenDetection()
        {
            return true;
        }
    }
}
