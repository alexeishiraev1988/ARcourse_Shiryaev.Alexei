using OpenCvSharp.Demo;
using System.Diagnostics;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using System;

public class MarkerWebCamMovement : WebCamera
{
    public event EventHandler <Vector3> MarkerDetected;
    [SerializeField] private bool _debug;
    [SerializeField] private Transform _debugTarget;
    [SerializeField] private int _targetMarker;


    private WebCamTexture _webCamTexture;

    [SerializeField] private Transform _gameRoot;
    [SerializeField] private float _markerSize = 0.05f; // размер маркера в метрах

    private Mat _processImage = new Mat();
    private Mat _grayImage = new Mat();
    private DetectorParameters _detectorParameters;
    private Dictionary _dictionary;
    private Point2f[][] _corners = new Point2f[0][];
    private int[] _ids = new int[0];
    private  Point2f[][] _rejectedImgPoints = new Point2f[0][];


    private Mat _cameraMatrix;
    private Mat _distCoeffs;

    void Start()
    {
        _webCamTexture = new WebCamTexture();

        _webCamTexture.Play();
    }

    protected override void Awake()
    {
        base.Awake();
        _detectorParameters = DetectorParameters.Create();
        _dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);

        _cameraMatrix = Mat.Eye(3, 3, MatType.CV_64FC1);
        _cameraMatrix.Set(0, 0, 1000);
        _cameraMatrix.Set(1, 1, 1000);
        _cameraMatrix.Set(0, 2, Screen.width / 2);
        _cameraMatrix.Set(1, 2, Screen.height / 2);

        _distCoeffs = new Mat(5, 1, MatType.CV_64FC1, Scalar.All(0));
    }
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        Mat image = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.CvtColor(image, _grayImage, ColorConversionCodes.BGR2GRAY);

        // Детекция маркеров
        Point2f[][] corners;
        int[] ids;
        Point2f[][] rejectedImgPoints;

        CvAruco.DetectMarkers(
            _grayImage,
            _dictionary,
            out corners,
            out ids,
            _detectorParameters,
            out rejectedImgPoints
        );

        if (ids.Length > 0)
        {
            _ids = ids;
            _corners = corners;
            _rejectedImgPoints = rejectedImgPoints;
            UnityEngine.Debug.Log("Detected ID: " + ids[0]);
        }

        if (_ids.Length > 0 && _corners.Length > 0)
        {
            if (_ids[0] == _targetMarker && _corners[0].Length == 4)
            {
                // Середина маркера
                var newX = _corners[0][0].X + (_corners[0][1].X - _corners[0][0].X) / 2;
                var newY = _corners[0][0].Y + (_corners[0][3].Y - _corners[0][0].Y) / 2;
                var coords = GetWorldPositionFromTexturePixel(newX, image.Height - newY, image.Width, image.Height);

                if (_debug)
                {
                    Cv2.Circle(image, new Point(newX, newY), 10, new Scalar(0, 0, 127), 2);
                    _debugTarget.position = coords;
                }

                MarkerDetected?.Invoke(this, coords);

                // 3D координаты маркера в локальной системе
                Point3f[] markerCorners3D = new Point3f[]
                {
                new Point3f(-_markerSize/2,  _markerSize/2, 0),
                new Point3f( _markerSize/2,  _markerSize/2, 0),
                new Point3f( _markerSize/2, -_markerSize/2, 0),
                new Point3f(-_markerSize/2, -_markerSize/2, 0)
                };

                Mat rvec = new Mat();
                Mat tvec = new Mat();

                // SolvePnP с оберткой InputArray
                Cv2.SolvePnP(
                    InputArray.Create(markerCorners3D),
                    InputArray.Create(_corners[0]),
                    _cameraMatrix,
                    _distCoeffs,
                    rvec,
                    tvec
                );
                UnityEngine.Debug.Log(tvec.At<double>(2));

                // Преобразуем вращение в матрицу и quaternion
                Mat rotMat = new Mat();
                Cv2.Rodrigues(rvec, rotMat);

                float scaleFactor = 1000f; // масштаб, чтобы объект был видим
                Vector3 position = new Vector3(
                    (float)tvec.At<double>(0),
                    -(float)tvec.At<double>(1),
                    (float)tvec.At<double>(2)
                ) * scaleFactor;

                _gameRoot.position = position;

                Quaternion rotation = ConvertRotationMatrixToQuaternion(rotMat);

                // Устанавливаем позицию и вращение игры
                if (_gameRoot != null)
                {
                    _gameRoot.localPosition = position;
                    _gameRoot.localRotation = rotation;
                    _gameRoot.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Если маркер не найден
            if (_gameRoot != null)
                _gameRoot.gameObject.SetActive(false);
        }

        // Отображаем маркеры для отладки
        if (_debug && _ids != null && _ids.Length > 0)
        {
            CvAruco.DrawDetectedMarkers(image, _corners, _ids);
        }

        // Обновляем текстуру
        if (output != null)
            OpenCvSharp.Unity.MatToTexture(image, output);
        else
            output = OpenCvSharp.Unity.MatToTexture(image);

        return true;
    }
    private Vector3 GetWorldPositionFromTexturePixel(float x, float y, float width, float height)
    {
        float uvX = x / width;
        float uvY = y / height;
        return GetWorldPositionFromUV(new Vector2(uvX, uvY));
    }
    
    private Vector3 GetWorldPositionFromUV(Vector2 uv)
    {
        Vector2 localPoint;
        localPoint.x = (uv.x - 0.5f) * _surfaceRectTransform.rect.width;
        localPoint.y = (uv.y - 0.5f) * _surfaceRectTransform.rect.height;

        return _surfaceRectTransform.TransformPoint(localPoint);

    }

    private Quaternion ConvertRotationMatrixToQuaternion(Mat R)
    {
        // R — 3x3 double
        Matrix4x4 m = new Matrix4x4();
        m.SetColumn(0, new Vector4((float)R.At<double>(0, 0), -(float)R.At<double>(1, 0), (float)R.At<double>(2, 0), 0));
        m.SetColumn(1, new Vector4((float)R.At<double>(0, 1), -(float)R.At<double>(1, 1), (float)R.At<double>(2, 1), 0));
        m.SetColumn(2, new Vector4((float)R.At<double>(0, 2), -(float)R.At<double>(1, 2), (float)R.At<double>(2, 2), 0));
        m.SetColumn(3, new Vector4(0, 0, 0, 1));
        return m.rotation;
    }

}
