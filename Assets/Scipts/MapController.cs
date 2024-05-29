using UnityEngine;

namespace Mapbox.Examples
{
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MapController : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 20)]
        public float _panSpeed = 1.0f;

        [SerializeField]
        float _zoomSpeed = 0.1f;

        [SerializeField]
        public Camera _referenceCamera;

        [SerializeField]
        AbstractMap _mapManager;

        [SerializeField]
        bool _useDegreeMethod;

        private Vector3 _origin;
        private Vector3 _mousePosition;
        private Vector3 _mousePositionPrevious;
        private bool _shouldDrag;
        private bool _isInitialized = false;
        private Plane _groundPlane = new Plane(Vector3.up, 0);
        private bool _dragStartedOnUI = false;

        //�� ���� Ȯ�� ����
        private bool zooming = false;

        private Coroutine pinchCoroutine;

        // Ÿ�� ĳ��
        private Dictionary<Vector2d, GameObject> tileCache = new Dictionary<Vector2d, GameObject>();

        //----����-----------------------
        private Vector2d _initialCenterLatitudeLongitude;
        private float _initialZoom;
        //----����-----------------------

        void Awake()
        {
            if (null == _referenceCamera)
            {
                _referenceCamera = GetComponent<Camera>();
                if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
            }

            _mapManager.OnInitialized += () =>
            {
                _isInitialized = true;
                //----����-----------------------
                SaveInitialValues(); // �ʱⰪ ����
                //----����-----------------------
            };
        }

        //----����-----------------------
        private void SaveInitialValues()
        {
            _initialCenterLatitudeLongitude = _mapManager.CenterLatitudeLongitude;
            _initialZoom = _mapManager.Zoom;
        }

        public void ResetToInitialValues()
        {
            if (_isInitialized)
            {
                _mapManager.UpdateMap(_initialCenterLatitudeLongitude, _initialZoom);
            }
        }
        //----����-----------------------

        void Update()
        {
            if (Input.touchCount == 2 && !EventSystem.current.IsPointerOverGameObject())
            {
                if (pinchCoroutine == null)
                {
                    pinchCoroutine = StartCoroutine(ProcessPinchGesture());
                }
            }
            else
            {
                if (pinchCoroutine != null)
                {
                    StopCoroutine(pinchCoroutine);
                    pinchCoroutine = null;
                }
            }
        }

        IEnumerator ProcessPinchGesture()
        {

            float lastDistance = 0f;
            while (true)
            {
                if (Input.touchCount != 2)
                {
                    yield break;
                }

                Vector2 touch0Pos = Input.GetTouch(0).position;
                Vector2 touch1Pos = Input.GetTouch(1).position;
                float distance = Vector2.Distance(touch0Pos, touch1Pos);

                if (lastDistance == 0)
                {
                    lastDistance = distance;
                }

                float zoomFactor = (distance - lastDistance) * _zoomSpeed * Time.deltaTime;
                ZoomMapUsingTouchOrMouse(zoomFactor);

                lastDistance = distance;

                yield return null;
            }
        }



        private void LateUpdate()
        {
            if (!_isInitialized) { return; }

            if (!_dragStartedOnUI)
            {
                if (Input.touchSupported && Input.touchCount > 0)
                {
                    HandleTouch();
                }
                else
                {
                    HandleMouseAndKeyBoard();
                }
            }

            //�� ������ �������� ǥ��
            zooming = false;
        }

        //PC ����
        void HandleMouseAndKeyBoard()
        {
            // zoom
            float scrollDelta = 0.0f;
            scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            ZoomMapUsingTouchOrMouse(scrollDelta);


            //pan keyboard -> ���� �̵�
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");

            PanMapUsingKeyBoard(xMove, zMove);


            //pan mouse
            PanMapUsingTouchOrMouse();

        }

        void HandleTouch()
        {
            float zoomFactor = 0.0f;
            //pinch to zoom.
            switch (Input.touchCount)
            {
                case 1:
                    {
                        PanMapUsingTouchOrMouse();
                    }
                    break;
                case 2:
                    {
                        // Store both touches.
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        // Find the position in the previous frame of each touch.
                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        // Find the magnitude of the vector (the distance) between the touches in each frame.
                        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                        // Find the difference in the distances between each frame.
                        zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
                    }
                    ZoomMapUsingTouchOrMouse(zoomFactor);
                    break;
                default:
                    break;
            }
        }

        void ZoomMapUsingTouchOrMouse(float zoomFactor)
        {
            // �� ���� ������ ǥ��
            zooming = true;

            float newZoom = _mapManager.Zoom + zoomFactor;

            // �� ������ 1���� 18 ���̷� ����
            newZoom = Mathf.Clamp(newZoom,10, 18);

            // �� ������ 10���� 18 ���̷� �����ϰ�, ��踦 ����� ��� �ƹ� �۾��� ���� ����
            if (newZoom < 10 || newZoom > 18)
            {
                return;
            }

            // ���� �� ������ ���Ͽ� ���� �Ǵ� �����ϴ� ��쿡�� ���� ������Ʈ
            if (newZoom != _mapManager.Zoom)
            {
                _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, newZoom);
            }
        }

        void PanMapUsingKeyBoard(float xMove, float zMove)
        {
            if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
            {
                // Get the number of degrees in a tile at the current zoom level.
                // Divide it by the tile width in pixels ( 256 in our case)
                // to get degrees represented by each pixel.
                // Keyboard offset is in pixels, therefore multiply the factor with the offset to move the center.
                float factor = _panSpeed * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));

                var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);

                UpdateMapWithCaching(latitudeLongitude, _mapManager.Zoom);
            }
        }

        void PanMapUsingTouchOrMouse()
        {
            if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject())
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.deltaPosition * _panSpeed * Time.deltaTime;
                    Vector3 forward = transform.forward;
                    forward.y = 0; // y �������� �̵����� �ʵ��� ��
                    Vector3 right = transform.right;

                    Vector3 move = forward * delta.y + right * delta.x;
                    transform.position += move;

                    Vector2d newCenterLatitudeLongitude = _mapManager.CenterLatitudeLongitude + Conversions.MetersToLatLon(new Vector2d(move.x, move.z));
                    UpdateMapWithCaching(newCenterLatitudeLongitude, _mapManager.Zoom);
                }
            }
        }

        void UpdateMapWithCaching(Vector2d newCenterLatitudeLongitude, float zoom)
        {
            // �ֺ� Ÿ���� �켱������ ���� ������
            var prefetchTiles = GetSurroundingTiles(newCenterLatitudeLongitude);

            foreach (var tilePos in prefetchTiles)
            {
                if (!tileCache.ContainsKey(tilePos))
                {
                    // ĳ�ÿ� Ÿ���� ������ ���� �ε��ϰ� ĳ�ÿ� ����
                    LoadTile(tilePos, zoom);
                }
            }

            // ���� ��ġ�� Ÿ�� ������Ʈ
            _mapManager.UpdateMap(newCenterLatitudeLongitude, zoom);
        }

        // �ֺ� Ÿ���� �켱������ ���� �������� �޼���
        IEnumerable<Vector2d> GetSurroundingTiles(Vector2d centerTile)
        {
            List<Vector2d> tiles = new List<Vector2d>();
            int range = 1; // ������Ī ���� ����

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    tiles.Add(new Vector2d(centerTile.x + x, centerTile.y + y));
                }
            }

            // ���� ��ġ�� ����� Ÿ�Ϻ��� �켱������ ��ȯ
            tiles.Sort((a, b) => Vector2d.Distance(a, centerTile).CompareTo(Vector2d.Distance(b, centerTile)));
            return tiles;
        }

        // ���� Ÿ�� �ε� �޼���
        void LoadTile(Vector2d tilePos, float zoom)
        {
            // ���� Ÿ�� �ε� ���� ����
            // ����: Ÿ���� �ε��ϰ� ĳ�ÿ� �߰�
            GameObject tile = new GameObject("Tile_" + tilePos.x + "_" + tilePos.y);
            tileCache[tilePos] = tile;
        }

        void UseMeterConversion()
        {
            if (Input.GetMouseButtonUp(1))
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                var pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                var latlongDelta = _mapManager.WorldToGeoPosition(pos);
                Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y);
            }

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
                            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        void UseDegreeConversion()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            // Get the number of degrees in a tile at the current zoom level.
                            // Divide it by the tile width in pixels ( 256 in our case)
                            // to get degrees represented by each pixel.
                            // Mouse offset is in pixels, therefore multiply the factor with the offset to move the center.
                            float factor = _panSpeed * Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;

                            var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor);
                            _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        private Vector3 getGroundPlaneHitPoint(Ray ray)
        {
            float distance;
            if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
            return ray.GetPoint(distance);
        }
    }
}
