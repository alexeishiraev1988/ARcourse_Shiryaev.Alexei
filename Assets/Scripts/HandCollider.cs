using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class HandCollider : WebCamera
{
    [Header("Детекция руки")]
    [SerializeField] private float _thresh = 100f;
    [SerializeField] private double _areaThresh = 3000;
    [SerializeField] private bool _debug = false;
    [SerializeField] private double _areaMax = 200000; // максимальная площадь

    [Header("3D объект руки")]
    [SerializeField] private Transform _handObject; // пустой GameObject с коллайдером
    [SerializeField] private float _handZ = 0f;     // Z позиция руки в мире

    private Mat _processImage = new Mat();
    private Vector3 _handWorldPos;
    private bool _handDetected = false;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        //UnityEngine.Debug.Log("ProcessTexture called, input size: " + input.width + "x" + input.height);

        Mat image = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.CvtColor(image, _processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(_processImage, _processImage, _thresh, 255, ThresholdTypes.Binary);
        Cv2.FindContours(_processImage, out var contours, out var hierarchy,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        _handDetected = false;
        double maxArea = 0;
        Point[] bestContour = null;

        foreach (var contour in contours)
        {
            double area = Cv2.ContourArea(contour);
            //UnityEngine.Debug.Log("Contour area: " + area);
            if (area > _areaThresh && area < _areaMax && area > maxArea)
            {
                maxArea = area;
                bestContour = contour;
            }
        }

        if (bestContour != null)
        {
            var moments = Cv2.Moments(bestContour);
            //UnityEngine.Debug.Log("M00: " + moments.M00 + " cx: " + moments.M10 / moments.M00);
            if (moments.M00 > 0)
            {
                float cx = (float)(moments.M10 / moments.M00);
                float cy = (float)(moments.M01 / moments.M00);

                // используем тот же метод что и для маркера
                Vector3 pos2D = GetWorldPositionFromTexturePixel(cx, image.Height - cy, image.Width, image.Height);
                _handWorldPos = new Vector3(pos2D.x, pos2D.y, _handZ);
                _handDetected = true;

                if (_debug)
                    Cv2.Circle(image, new Point((int)cx, (int)cy), 15, new Scalar(0, 255, 0), 3);
                Cv2.Circle(_processImage, new Point((int)cx, (int)cy), 15, new Scalar(128), 3);
            }
        }

        if (output != null)
            OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image, output);
        else
            output = OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image);

        return true;
    }

    void LateUpdate()
    {
        if (_handObject == null) return;
        UnityEngine.Debug.Log("HandDetected: " + _handDetected + " pos: " + _handWorldPos);
        if (_handDetected)
        {
            _handObject.position = _handWorldPos;
            _handObject.gameObject.SetActive(true);
        }
        else
        {
            _handObject.gameObject.SetActive(false);
        }
    }

    private Vector3 GetWorldPositionFromTexturePixel(float x, float y, float width, float height)
    {
        float uvX = x / width;
        float uvY = y / height;
        Vector2 localPoint;
        localPoint.x = (uvX - 0.5f) * _surfaceRectTransform.rect.width;
        localPoint.y = (uvY - 0.5f) * _surfaceRectTransform.rect.height;
        return _surfaceRectTransform.TransformPoint(localPoint);
    }
}
