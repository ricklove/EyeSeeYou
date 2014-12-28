using UnityEngine;
using System.Collections;

public class WebcamMotionDetector : MonoBehaviour
{
    public bool autoStart = false;
    public Vector2 lastMotionPosition;

    public MeshRenderer preview;
    public MeshRenderer lastSampled;
    public MeshRenderer sampled;
    public MeshRenderer diffed;
    public MeshRenderer diffWithCutoffPreview;
    public MeshRenderer motionClusters;

    private WebCamTexture _webCamTexture;
    private Color32[,] _lastSample;

    void Start()
    {
        if (autoStart)
        {
            StartCapturing();
        }
    }

    void Update()
    {
        if (_webCamTexture != null && _webCamTexture.isPlaying)
        {
            // Sample the image
            var sample = GetSample();
            var diff = GetDiff(sample, _lastSample);
            var diffWithCutoff = GetBinaryImage(diff, 50);
            var clusterPos = GetClusterPosition(diffWithCutoff);

            if (clusterPos != Vector2.zero)
            {
                lastMotionPosition = new Vector2(clusterPos.x / sample.GetLength(0), clusterPos.y / sample.GetLength(1));
            }

            Debug.Log("clusterPos: " + clusterPos);

            _lastSample = sample;



            // Show results
            if (preview != null)
            {
                preview.renderer.material.mainTexture = _webCamTexture;
            }

            RenderColorArray(sample, sampled);
            RenderColorArray(_lastSample, lastSampled);
            RenderColorArray(diff, diffed);
            RenderColorArray(diffWithCutoff, diffWithCutoffPreview);
            RenderPosition(clusterPos, new Vector2(sample.GetLength(0), sample.GetLength(1)), motionClusters);
        }
    }

    private Color32[,] GetBinaryImage(Color32[,] diff, int cutoff)
    {
        if (diff == null) return null;

        var width = diff.GetLength(0);
        var height = diff.GetLength(1);

        var binary = (Color32[,])diff.Clone();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var d = diff[i, j];
                var max = Mathf.Max(d.r, d.g, d.b);

                if (max > cutoff)
                {
                    binary[i, j] = new Color32(255, 255, 255, 255);
                }
                else
                {
                    binary[i, j] = new Color32(0, 0, 0, 255);
                }
            }
        }

        return binary;
    }

    private Vector2 GetClusterPosition(Color32[,] diff)
    {
        if (diff != null)
        {
            var totalPosX = 0.0f;
            var totalPosY = 0.0f;
            var totalValue = 0.0f;

            var width = diff.GetLength(0);
            var height = diff.GetLength(1);


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var d = diff[i, j];
                    var max = Mathf.Max(d.r, d.g, d.b);

                    if (max > 0)
                    {
                        var ratio = 255.0f / max;

                        var x = i * ratio;
                        var y = j * ratio;

                        totalValue += ratio;
                        totalPosX += x;
                        totalPosY += y;
                    }
                }
            }

            if (totalValue > 0)
            {
                return new Vector2(totalPosX / totalValue, totalPosY / totalValue);
            }
            else
            {
                return new Vector2();
            }
        }

        return new Vector2();
    }

    private static Color32[,] GetDiff(Color32[,] a, Color32[,] b)
    {
        if (a == null || b == null
            || a.GetLength(0) != b.GetLength(0)
            || a.GetLength(1) != b.GetLength(1)
            )
        {
            return null;
        }


        var diff = (Color32[,])a.Clone();

        var width = a.GetLength(0);
        var height = a.GetLength(1);


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var aVal = a[i, j];
                var bVal = b[i, j];

                var rDiff = (byte)Mathf.Abs(aVal.r - bVal.r);
                var gDiff = (byte)Mathf.Abs(aVal.g - bVal.g);
                var bDiff = (byte)Mathf.Abs(aVal.b - bVal.b);
                diff[i, j] = new Color32(rDiff, gDiff, bDiff, 255);
            }
        }

        return diff;
    }

    private void RenderPosition(Vector2 clusterPos, Vector2 max, MeshRenderer target)
    {
        if (target == null) return;

        var width = (int)max.x;
        var height = (int)max.y;

        Texture2D sTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        sTexture.SetPixels(GenerateEmptyColorBlock(width, height));
        if (clusterPos != Vector2.zero)
        {
            sTexture.SetPixel((int)clusterPos.x, (int)clusterPos.y, Color.white);
        }
        sTexture.Apply();
        target.renderer.material.mainTexture = sTexture;
    }

    private Color[] GenerateEmptyColorBlock(int width, int height)
    {
        return new Color[width * height];
    }

    private static void RenderColorArray(Color32[,] array, MeshRenderer target)
    {
        if (array != null && target != null)
        {
            Texture2D sTexture = new Texture2D((int)array.GetLength(0), (int)array.GetLength(1), TextureFormat.RGBA32, false);
            sTexture.SetPixels32(Flatten(array));
            sTexture.Apply();
            target.renderer.material.mainTexture = sTexture;
        }
    }

    private static Color32[] Flatten(Color32[,] sample)
    {
        var flat = new Color32[sample.GetLength(0) * sample.GetLength(1)];

        var width = sample.GetLength(0);
        var height = sample.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var iFlat = i + j * width;
                flat[iFlat] = sample[i, j];
            }
        }

        return flat;
    }

    private Color32[,] GetSample()
    {
        var sampleWidth = 16;
        var sampleHeight = 16;

        var sampleSize = sampleWidth * sampleHeight;

        var data = _webCamTexture.GetPixels32();

        var dataWidth = _webCamTexture.width;
        var dataHeight = _webCamTexture.height;

        var pixelsPerSampleWidth = dataWidth / sampleWidth;
        var pixelsPerSampleHeight = dataHeight / sampleHeight;

        var samples = new Color32[sampleWidth, sampleHeight];

        for (int i = 0; i < sampleWidth; i++)
        {
            for (int j = 0; j < sampleHeight; j++)
            {
                var iData = i * pixelsPerSampleWidth + (dataWidth * j * pixelsPerSampleHeight);
                var sample = data[iData];
                samples[i, j] = sample;
            }
        }

        return samples;
    }

    public void StartCapturing()
    {
        if (_webCamTexture == null)
        {
            _webCamTexture = new WebCamTexture(100, 100, 15);
            _webCamTexture.Play();
        }
    }
}
