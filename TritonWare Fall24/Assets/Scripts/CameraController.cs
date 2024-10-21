using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Camera mainCamera;        // Reference to the camera
    public float panSpeed = 20f;     // Speed at which the camera will pan
    public float edgeThreshold = 10f; // Distance from the screen edge to trigger panning
    public Vector2 minCameraPos;     // Minimum camera position (limits)
    public Vector2 maxCameraPos;     // Maximum camera position (limits)
    public float zoomSensitivity = 1f;
    public bool mouseControlsEnabled = true;

    private static Vector2 cameraTargetLocation;
    private static float zoomTargetOrthographicSize;

    private void Awake()
    {
        Instance = this;
        transform.position = new Vector3(MapManager.Instance.MapSize.x / 2f, MapManager.Instance.MapSize.y / 2f, transform.position.z);
        zoomTargetOrthographicSize = mainCamera.orthographicSize;   
        cameraTargetLocation = mainCamera.transform.position;
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 cameraPos = mainCamera.transform.position;

        Vector2 camMoveVector = Vector2.zero;

        // Get the width and height of the screen
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float screenAspect = screenWidth / screenHeight;
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        // KEYBOARD CONTROLS

        if (Input.GetKeyDown(KeyCode.F)) mouseControlsEnabled = !mouseControlsEnabled;

        if (Input.GetKey(KeyCode.W)) camMoveVector.y = 1;
        else if (Input.GetKey(KeyCode.S)) camMoveVector.y = -1;

        if (Input.GetKey(KeyCode.A)) camMoveVector.x = -1;
        else if (Input.GetKey(KeyCode.D)) camMoveVector.x = 1;

        if (mouseControlsEnabled && camMoveVector == Vector2.zero)
        {
            // MOUSE CONTROLS

            float mouseHorizontal = (mousePos.x / screenWidth - 0.5f) * 2;  // -1 to 1, centered at 0
            float mouseVertical = (mousePos.y / screenHeight - 0.5f) * 2;


            // Pan camera to the right if mouse is near the right edge
            if (mousePos.x >= screenWidth - edgeThreshold)
            {
                camMoveVector = new Vector2(1, mouseVertical);
            }

            // Pan camera to the left if mouse is near the left edge
            else if (mousePos.x <= edgeThreshold)
            {
                camMoveVector = new Vector2(-1, mouseVertical);
            }

            // Pan camera upwards if mouse is near the top edge
            else if (mousePos.y >= screenHeight - edgeThreshold)
            {
                camMoveVector = new Vector2(mouseHorizontal, 1);
            }

            // Pan camera downwards if mouse is near the bottom edge
            else if (mousePos.y <= edgeThreshold)
            {
                camMoveVector = new Vector2(mouseHorizontal, -1);
            }
            
            cameraTargetLocation += camMoveVector.normalized * Time.deltaTime * panSpeed;

            // Camera zoom
            Vector2 mouseScroll = Input.mouseScrollDelta;

            if(mouseScroll.magnitude > 0 ) {
                float newOrthographicSize = zoomTargetOrthographicSize - mouseScroll.y * zoomSensitivity;
                zoomTargetOrthographicSize = Math.Clamp(newOrthographicSize, 5, 25);

                float zoomMultiplier = (1 - zoomTargetOrthographicSize / mainCamera.orthographicSize);

                cameraTargetLocation += new Vector2((mouseHorizontal*cameraWidth/2f)*(zoomMultiplier), (mouseVertical*cameraHeight/2f)*(zoomMultiplier));
            
                mainCamera.orthographicSize = zoomTargetOrthographicSize;
            }            
        }


        cameraPos += (Vector3)(camMoveVector.normalized * panSpeed * Time.deltaTime);

        // Clamp the camera's position to stay within the specified bounds
        cameraHeight = mainCamera.orthographicSize * 2;
        cameraWidth = cameraHeight * screenAspect;
        Vector2 newMinCameraPos = new(minCameraPos.x + cameraWidth / 2f, minCameraPos.y + cameraHeight / 2f);
        Vector2 newMaxCameraPos = new(maxCameraPos.x - cameraWidth / 2f, maxCameraPos.y - cameraHeight / 2f);
        // cameraPos.x = Mathf.Clamp(cameraPos.x, newMinCameraPos.x, newMaxCameraPos.x);
        // cameraPos.y = Mathf.Clamp(cameraPos.y, newMinCameraPos.y, newMaxCameraPos.y);
        cameraPos.x = Mathf.Clamp(cameraTargetLocation.x, newMinCameraPos.x, newMaxCameraPos.x);
        cameraPos.y = Mathf.Clamp(cameraTargetLocation.y, newMinCameraPos.y, newMaxCameraPos.y);

        // Apply the new camera position
        mainCamera.transform.position = cameraPos;
    }
}
