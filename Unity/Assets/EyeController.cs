using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour
{

    public float depth = 0.1f;

    void Start()
    {

    }

    void Update()
    {
        // Detect input location
        // mousePosition works with both touch and mouse
        Input.simulateMouseWithTouches = true;
        var screenPos3D = Input.mousePosition;
        var screenPos = new Vector2(screenPos3D.x, screenPos3D.y);
        var worldPos = new Vector3();

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
