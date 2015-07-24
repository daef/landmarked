using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
   public float RotateSpeed = .23f;
   public float CameraPositioningSpeed = 5f;
   public float ZoomSensitivity = .14f;
   public float ZoomMin = 1f;
   public float ZoomMax = 800f;
   private float _zoom;
   private Camera _camera;
   private Vector3 _oldPos;
   private Vector3 _origin;
   private Vector3 _targetPosition;
   private Bounds _sceneBounds;
   private float _lerpingSince;

   private void Start() {
      _camera = GetComponent<Camera>();
      _origin = SceneCenter();
      _zoom = (_origin - _camera.transform.position).magnitude;
      _targetPosition = _origin - _camera.transform.forward*_zoom;
   }


   private void InitPositionLerp(Vector3 origin, float zoom) {
      var newPos = origin - _camera.transform.forward*(zoom);
      if ((newPos - _targetPosition).sqrMagnitude > 0.01) {
         _origin = origin;
         _zoom = zoom;
         _targetPosition = newPos;
         _lerpingSince = Time.fixedTime;
      }
   }

   public void NewBounds(Bounds b) {
      // see http://stackoverflow.com/questions/2866350/move-camera-to-fit-3d-scene
      // extends is already /2
      _sceneBounds = b;
      InitPositionLerp(b.center, b.extents.magnitude/Mathf.Tan(_camera.fieldOfView*Mathf.PI/180f) + b.extents.magnitude);
   }

   private Vector3 SceneCenter() {
      return _sceneBounds.center;
   }

   private void LateUpdate() {
      var pos = Input.mousePosition;

      #region smooth camera lerping

      if (!Mathf.Approximately(_lerpingSince, 0)) {
         var delta = (Time.fixedTime - _lerpingSince)*CameraPositioningSpeed;

         _camera.transform.position = Vector3.Lerp(
            _camera.transform.position,
            _targetPosition,
            delta);

         if (delta > 1)
            _lerpingSince = 0;
      }

      #endregion

      #region PAN

      if (Input.GetMouseButtonDown(1)) {
         RaycastHit hit;
         var cnt = Physics.Raycast(_camera.ScreenPointToRay(pos), out hit)
            ? hit.point
            : SceneCenter();
         InitPositionLerp(cnt, _zoom);
      }

      #endregion

      #region ZOOM

      var wheel = Input.mouseScrollDelta.y*ZoomSensitivity;
      if (!Mathf.Approximately(wheel, 0))
         InitPositionLerp(_origin, Mathf.Clamp(_zoom*(1f + wheel), ZoomMin, ZoomMax));

      #endregion

      #region ROTATE

      if (Input.GetMouseButton(1)) {
         var delta = pos - _oldPos;

         _camera.transform.RotateAround(_origin, _camera.transform.up, delta.x*RotateSpeed);
         _camera.transform.RotateAround(_origin, _camera.transform.right, -delta.y*RotateSpeed);

         InitPositionLerp(_origin, _zoom);
      }

      #endregion

      _oldPos = pos;
   }
}
