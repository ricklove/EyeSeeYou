using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LookAtMotionController : MonoBehaviour
{

    public GameObject eye;

    public int averageCount = 5;
    private List<Vector2> _lastPositions = new List<Vector2>();

    void Start()
    {

    }

    void Update()
    {

        if (eye != null)
        {
            var eyeController = eye.GetComponent<EyeController>();
            var motionDetector = GetComponent<WebcamMotionDetector>();

            var motionPos = motionDetector.lastMotionPosition;

            if (motionPos != Vector2.zero)
            {
                var motionScreenPos = new Vector2(
                    (1.0f - motionPos.x) * Screen.width,
                    motionPos.y * Screen.height
                    );

                _lastPositions.Add(motionScreenPos);

                while (_lastPositions.Count > Mathf.Max(1, averageCount))
                {
                    _lastPositions.RemoveAt(0);
                }

                var posX = _lastPositions.Average(p => p.x);
                var posY = _lastPositions.Average(p => p.y);
                var pos = new Vector2(posX, posY);

                Debug.Log("motionScreenPos: " + motionScreenPos);
                Debug.Log("average pos: " + pos);

                eyeController.setTargetScreenPosition(pos);

            }
        }
    }
}
