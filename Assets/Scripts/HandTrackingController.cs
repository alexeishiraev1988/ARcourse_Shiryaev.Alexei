using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class HandTrackingController : WebCamera
{
    [Header("Настройки детекции руки")]
    [SerializeField] private float _thresh = 100f;
    [SerializeField] private double _areaThresh = 3000;  // минимальная площадь — рука крупнее мусора
    [SerializeField] private bool _debug = false;

    [Header("Управление игроком")]
    [SerializeField] private Transform _player;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _deadZone = 0.1f; // зона нечувствительности в центре

    private Mat _processImage = new Mat();
    private Vector2 _handUV = new Vector2(0.5f, 0.5f); // позиция руки [0..1]
    private bool _handDetected = false;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        Mat image = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.CvtColor(image, _processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(_processImage, _processImage, _thresh, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(_processImage, out var contours, out var hierarchy,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        _handDetected = false;
        double maxArea = 0;
        Point[] bestContour = null;

        // ищем самый большой контур — это и есть рука
        foreach (var contour in contours)
        {
            double area = Cv2.ContourArea(contour);
            if (area > _areaThresh && area > maxArea)
            {
                maxArea = area;
                bestContour = contour;
            }
        }

        if (bestContour != null)
        {
            // находим центр контура
            var moments = Cv2.Moments(bestContour);
            if (moments.M00 > 0)
            {
                float cx = (float)(moments.M10 / moments.M00);
                float cy = (float)(moments.M01 / moments.M00);

                // нормализуем в [0..1]
                _handUV.x = cx / image.Width;
                _handUV.y = 1f - cy / image.Height;
                _handDetected = true;

                // рисуем точку для отладки
                if (_debug)
                    Cv2.Circle(image, new Point((int)cx, (int)cy), 15, new Scalar(0, 255, 0), 3);
            }
        }

        if (output != null)
            OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image, output);
        else
            output = OpenCvSharp.Unity.MatToTexture(_debug ? _processImage : image);

        return true;
    }

    void Update()
    {
        if (!_handDetected || _player == null) return;

        // переводим позицию руки в направление движения
        // центр экрана = (0.5, 0.5) = стоим на месте
        float dirX = _handUV.x - 0.5f;
        float dirZ = _handUV.y - 0.5f;

        // мёртвая зона — чтобы игрок не дёргался когда рука в центре
        if (Mathf.Abs(dirX) < _deadZone) dirX = 0;
        if (Mathf.Abs(dirZ) < _deadZone) dirZ = 0;

        Vector3 moveDir = new Vector3(dirX, 0, dirZ).normalized;
        _player.position += moveDir * _moveSpeed * Time.deltaTime;
    }
}
