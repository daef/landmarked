using UnityEngine;
using UnityEngine.EventSystems;

public class CursorBehaviour : MonoBehaviour {
   private GameObject _cursor;
   private GameObject _ui;
   private MeshRenderer _cursorRenderer;

   private void Awake() {
      _ui = GameObject.Find("UICanvas");
      _cursor = GameObject.Find("Cursor");
      _cursorRenderer = _cursor.GetComponent<MeshRenderer>();
   }

   public void SetColor(Color c) {
      _cursorRenderer.material.color = c;
   }

   public void SetVisible(bool visible) {
      _cursorRenderer.enabled = visible;
   }

   private GameObject _lastHoverObject;

   private void Update() {
      if (_cursorRenderer == null) {
         _cursor = GameObject.Find("Cursor");
         _cursorRenderer = _cursor.GetComponent<MeshRenderer>();
      }

      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)) {
         var hitobject = hit.collider.gameObject;

         if (_lastHoverObject != hitobject)
            _ui.BroadcastMessage("CursorEnter", hit);

         _lastHoverObject = hitobject;

         if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            _ui.BroadcastMessage("CursorHit", hit);

         if (_cursorRenderer.enabled) {
            _cursor.transform.position = hit.point;
            _cursor.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
         }
      }
      else if (_lastHoverObject != null) {
         _ui.BroadcastMessage("CursorExit");
         _lastHoverObject = null;
      }
   }
}
