using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour
{
    public float depth = 0.1f;
    private Vector2 targetScreenPosition = Vector2.zero;

    public void setTargetScreenPosition( Vector2 target)
    {
        targetScreenPosition = target;
        Debug.Log("Set targetScreenPosition: " + targetScreenPosition);
    }

    void Start()
    {

    }

    void Update()
    {
        var screenPos = new Vector2();
        var worldPos = new Vector3();


        Debug.Log("targetScreenPosition: " + targetScreenPosition);
        Debug.Log("Input.mousePosition: " + Input.mousePosition);


        if (targetScreenPosition != Vector2.zero)
        {
            screenPos = targetScreenPosition;

            Debug.Log("Using targetScreenPosition: " + screenPos);
        }
        else
        {
            // Detect input location
            // mousePosition works with both touch and mouse
            Input.simulateMouseWithTouches = true;
            var screenPos3D = Input.mousePosition;
            screenPos = new Vector2(screenPos3D.x, screenPos3D.y);

            Debug.Log("Using inputPosition: " + screenPos);
        }

        //Debug.Log("screenPos: " + screenPos);

        if (screenPos != Vector2.zero)
        {
            var screenOffset = new Vector3(screenPos.x, screenPos.y, depth);
             worldPos = Camera.main.ScreenToWorldPoint(screenOffset);

            // Show the world pos as a gadget
            //Debug.Log("worldPos: " + worldPos);
            Debug.DrawLine(new Vector3(), worldPos);
        }

        // Rotate the eye
        if (worldPos != Vector3.zero)
        {
            transform.LookAt(worldPos);
        }
    }
}
