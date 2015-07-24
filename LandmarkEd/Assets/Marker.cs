using UnityEngine;

class Marker {
   private float _birthday;

   private GameObject _selector;
   public GameObject Selector {
      get { return _selector; }
      set {
         _selector = value;
         SelectorTransform = _selector.GetComponent<RectTransform>();
      }
   }

   public RectTransform SelectorTransform { get; private set; }

   public GameObject Mark { get; set; }

   public float? FocusTime { get; set; }

   public float Scale {
      get {
         var age = Time.fixedTime - _birthday;
         var scale = age > 7 ? 1 : Mathf.Sin(age * 1.5f) * 3f / (age * age + 1) + age * age / (2 + age * age);
         if (FocusTime.HasValue)
            scale = (scale + 3f) / 3f;
         return scale;
      }
   }

   public bool IsSet {
      get { return Mark.activeSelf; }
      set {
         Mark.SetActive(value);
         if (value)
            _birthday = Time.fixedTime;
      }
   }

   public void Destroy() {
      Object.DestroyObject(Selector);
      Object.DestroyObject(Mark);
   }
}
