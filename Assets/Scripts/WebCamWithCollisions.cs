using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using UnityEngine.UI;

public class WebCamWithCollisions : WebCamera
{
    [SerializeField] private float _thresh;
    [SerializeField] private float _accuracy;
    [SerializeField] private float _areaThresh;
    [SerializeField] private bool _debug;
    [SerializeField] private PolygonCollider2D _collider;
    [SerializeField] private RawImage _surface;

    private Mat _processImage = new Mat();
    private Scalar LineColor = new Scalar(127, 127, 127);

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {

        Mat image = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.Flip(image, image, FlipMode.Y);
        Cv2.CvtColor(image, _processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(_processImage, _processImage, _thresh, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(_processImage, out var contours, out var hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

        _collider.pathCount = 0;

        float texW = input.width;
        float texH = input.height;
        RectTransform rt = _surface.rectTransform;
        float worldW = rt.rect.width;
        float worldH = rt.rect.height;

        foreach (var contour in contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, _accuracy, true);
            double area = Cv2.ContourArea(contour);

            if (area > _areaThresh)
            {
                for (int i = 1; i < points.Length; i++)
                {
                    Cv2.Line(_processImage, points[i - 1], points[i], LineColor, 2);
                }
                Cv2.Line(_processImage, points[points.Length - 1], points[0], LineColor, 2);
                _collider.pathCount++;

                Vector2[] vecPoints = new Vector2[points.Length];
                for (int i = 0; i < vecPoints.Length; i++)
                {
                    // vecPoints[i] = new Vector2(points[i].X, points[i].Y);

                    float nx = points[i].X / texW;
                    float ny = points[i].Y / texH;
                    float lx = (nx - 0.5f) * worldW;
                    float ly = (0.5f - ny) * worldH;
                    vecPoints[i] = new Vector2(lx, ly);

                }
                _collider.SetPath(_collider.pathCount - 1, vecPoints);
            }

        }

        if (output != null)
        {
            OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image, output);
        }
        else
        {
            output = OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image);
        }
        image.Dispose();
        return true;


    }

    private void OnDestroy()
    {
        _processImage?.Dispose();
    }

}
