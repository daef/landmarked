using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkBehaviour : MonoBehaviour {
   public List<Color> Colors;
   public GameObject MarkerTemplate;
   public GameObject Sub;
   public GameObject Add;
   public GameObject Save;
   public GameObject Clear;

   private readonly List<Marker> _markers = new List<Marker>();
   private GameObject _markerRoot;
   private GameObject _cursor;

   private Button _subBtn;
   private Button _addBtn;
   private Button _saveBtn;
   private Button _clearBtn;

   private Marker _focusedMarker;

   private Marker FocusedMarker {
      set {
         if (_focusedMarker != null)
            _focusedMarker.FocusTime = null;

         _focusedMarker = value;

         if (_focusedMarker != null)
            _focusedMarker.FocusTime = Time.fixedTime;
      }
   }

   private int _index;

   private void Start() {
      _markerRoot = new GameObject("markers");
      _cursor = GameObject.Find("Cursor");

      _subBtn = Sub.GetComponent<Button>();
      _addBtn = Add.GetComponent<Button>();
      _saveBtn = Save.GetComponent<Button>();
      _clearBtn = Clear.GetComponent<Button>();

      _subBtn.onClick.AddListener(() => BuildUi(_markers.Count - 1));
      _addBtn.onClick.AddListener(() => BuildUi(_markers.Count + 1));
      _clearBtn.onClick.AddListener(() => {
         _markers.ForEach(p => p.IsSet = false);
         SelectIndex(0);
         UpdateUi();
      });

      _saveBtn.onClick.AddListener(ExportPTS);

      BuildUi(Colors.Count/2);
      SelectIndex(0);
      UpdateUi();
   }

   public void CursorExit() {
      FocusedMarker = null;
      _cursor.SendMessage("SetVisible", false);
   }

   public void CursorEnter(RaycastHit hit) {
      var marker = _markers.FirstOrDefault(p => p.Mark == hit.collider.gameObject);
      FocusedMarker = marker;
      _cursor.SendMessage("SetVisible", marker == null);
   }

   public void CursorHit(RaycastHit hit) {
      var marker = _markers.FirstOrDefault(p => p.Mark == hit.collider.gameObject);

      if (marker == null) {
         var m = _markers[_index];
         m.IsSet = true;
         m.Mark.transform.position = hit.point;
         m.Mark.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
         var next = _markers.FindIndex(p => !p.IsSet);

         if (next > -1)
            SelectIndex(next);
      }
      else {
         marker.IsSet = false;
         SelectIndex(_markers.IndexOf(marker));
      }

      UpdateUi();
   }

   private void SelectIndex(int idx) {
      _index = idx;
      _cursor.SendMessage("SetColor", Colors[_index]);
   }

   private void BuildUi(int markerCount) {
      while (_markers.Count < markerCount) {
         var idx = _markers.Count;
         var selector = Instantiate(Sub);
         var btn = selector.GetComponent<Button>();
         btn.interactable = true;
         btn.onClick.AddListener(() => SelectIndex(idx));
         selector.transform.SetParent(transform);

         var trans = selector.GetComponent<RectTransform>();
         trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 42);
         trans.anchoredPosition = new Vector2(170 + 50*idx, 0);
         selector.GetComponent<Image>().color = Colors[idx];
         selector.name = "marker#" + (selector.GetComponentInChildren<Text>().text = (idx + 1).ToString());

         var mark = Instantiate(MarkerTemplate);
         mark.SetActive(false);
         mark.transform.SetParent(_markerRoot.transform);
         mark.GetComponent<Renderer>().material.color = Colors[idx];

         _markers.Add(new Marker() {Selector = selector, Mark = mark});
      }

      while (_markers.Count > markerCount) {
         var last = _markers.Last();
         last.Destroy();
         _markers.Remove(last);
      }

      if (_index >= _markers.Count) {
         var needed = _markers.FindIndex(p => !p.IsSet);
         if (needed < 0)
            SelectIndex(_markers.Count - 1);
         else
            SelectIndex(needed);
      }

      UpdateUi();
   }

   private void UpdateUi() {
      _subBtn.interactable = _markers.Count > 1;
      _addBtn.interactable = _markers.Count < Colors.Count;
      _clearBtn.interactable = _markers.Any(p => p.IsSet);
      _saveBtn.interactable = _markers.All(p => p.IsSet);

      for (int i = 0; i < _markers.Count; i++) {
         var m = _markers[i];
         var img = m.Selector.GetComponent<Image>();
         var c = img.color;
         c.a = m.IsSet ? 1f : .5f;
         img.color = c;
      }
   }

   private void Update() {
      var m = _markers[_index];
      var img = m.Selector.GetComponent<Image>();
      var c = img.color;
      c.a = (m.IsSet ? 1f : .5f) - (Mathf.Sin(Time.fixedTime*3f) + 1f)/4f;
      img.color = c;

      foreach (var marker in _markers.Where(p => p.IsSet)) {
         var scale = marker.Scale;
         marker.Mark.transform.localScale = (3f*marker.Mark.transform.localScale + new Vector3(scale, scale, scale))/4f;

         marker.SelectorTransform.localScale = marker.FocusTime.HasValue
            ? (3f*marker.Mark.transform.localScale + new Vector3(scale, scale, scale))/4f
            : marker.SelectorTransform.localScale = (marker.SelectorTransform.localScale + Vector3.one)/2f;
      }
   }

   private class PtsParseException : Exception {
      public PtsParseException(string m) : base(m) {}
   }

   public void LoadPTS(FileHelper.FileWrapper fileWrapper) {
      var lines = Encoding.UTF8.GetString(fileWrapper.Contents)
         .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

      if (lines[0] != "Version 1.0")
         throw new PtsParseException("unknown pts version");

      if (lines.Length < 2)
         throw new PtsParseException("invalid pts countExist");

      int cnt;
      if (!int.TryParse(lines[1], out cnt))
         throw new PtsParseException("invalid pts parseCount");

      if ((cnt + 2) != lines.Length)
         throw new PtsParseException("invalid pts countMismatch");

      if (cnt > 0 && cnt <= Colors.Count) {
         BuildUi(cnt);
         for (int i = 0; i < cnt; i++) {
            var fields = lines[i + 2].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            _markers[i].Mark.transform.position = new Vector3(
               float.Parse(fields[1]),
               float.Parse(fields[2]),
               float.Parse(fields[3]));
            _markers[i].IsSet = true;
         }
         UpdateUi();
      }
      else
         throw new PtsParseException("go'n'define more colors");
   }

   private void ExportPTS() {
      var sb = new StringBuilder();
      sb.AppendLine("Version 1.0");
      sb.AppendLine(_markers.Count.ToString());

      for (int i = 0; i < _markers.Count; i++) {
         var p = _markers[i].Mark.transform.position;
         sb.AppendLine(string.Format("S{0}  {1}  {2}  {3}",
            i.ToString().PadLeft(3, '0'),
            p.x.ToString(CultureInfo.InvariantCulture),
            p.y.ToString(CultureInfo.InvariantCulture),
            p.z.ToString(CultureInfo.InvariantCulture)));
      }

      var data = Encoding.ASCII.GetBytes(sb.ToString());
      GameObject.Find("FileHelper").SendMessage("SavePTS", data);
   }
}
