using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Assets;
using Helper;
using UnityEngine;
using Random = UnityEngine.Random;

public class FileHelper : MonoBehaviour {
   public class FileWrapper {
      public string FileName { get; set; }
      public byte[] Contents { get; set; }
   }

   [DllImport("__Internal")]
   private static extern void Alert(string msg);

   [DllImport("__Internal")]
   private static extern void CopyFileData(string filename, byte[] dst);

   [DllImport("__Internal")]
   private static extern int GetFileDataLength(string filename);

   [DllImport("__Internal")]
   private static extern void DownloadFile(string filename, byte[] ptr, int len);

   public Material StlMaterial;
   private string _stlName = "unnamed.stl";

   private void Start() {
      StartCoroutine(LoadSTL("suzanne.stl", Resources.Load<TextAsset>("suzanne").bytes));
   }

   private void Update() {
      //if (Input.GetMouseButtonDown(2)) {
      //   StartCoroutine(LoadSTL("sk.stl", File.ReadAllBytes(@"d:\tmp\sk.stl")));
      //   StartCoroutine(LoadPTS("sk.pts", File.ReadAllBytes(@"d:\tmp\sk.pts")));
      //}
   }

   public IEnumerator FileOpen(string filename) {
      var len = GetFileDataLength(filename);
      var data = new byte[len];
      CopyFileData(filename, data);

      var ext = filename.Substring(filename.LastIndexOf(".", StringComparison.InvariantCulture) + 1).ToLower();

      switch (ext) {
         case "pts":
            yield return StartCoroutine(LoadPTS(filename, data));
            break;
         case "stl":
            yield return StartCoroutine(LoadSTL(filename, data));
            break;
         case "txt":
            yield return StartCoroutine(LoadTXT(filename, data));
            break;
         default:
            Alert(string.Format("Unknown file extension: {0}.\nTry one of those: .stl .pts .txt", ext));
            break;
      }
   }

   private IEnumerator LoadTXT(string filename, byte[] data) {
      Alert(string.Format("{0}: {1}", filename, BitConverter.ToString(data)));
      return null;
   }

   private IEnumerator LoadPTS(string filename, byte[] data) {
      yield return null;
      GameObject.Find("UICanvas").BroadcastMessage("LoadPTS", new FileWrapper() { Contents = data, FileName = filename });
   }

   public void SavePTS(byte[] data) {
      DownloadFile(_stlName + ".pts", data, data.Length);
   }

   private IEnumerator LoadSTL(string filename, byte[] data) {
      _stlName = filename;
      var stl = GameObject.Find(".STL");

      // clean up
      foreach (Transform t in stl.transform)
         Destroy(t.gameObject);

      // does unity even care?
      GC.Collect();
      GC.WaitForPendingFinalizers();

      var gs = new List<GameObject>();
      var stlMeshData = StlReader.Read(data);
      stlMeshData.MergeVertices();
      var meshes = stlMeshData.ToMeshes();

      Bounds b = new Bounds();

      for (int i = 0; i < meshes.Count; i++) {
         var mesh = meshes[i];
         yield return null;

         var m = new GameObject("STL_" + i.ToString().PadLeft(4, '0'), typeof(MeshFilter), typeof(MeshRenderer));
         m.transform.parent = stl.transform;
         m.GetComponent<MeshFilter>().mesh = mesh;
         m.GetComponent<MeshRenderer>().sharedMaterial = StlMaterial;
         m.AddComponent<MeshCollider>().sharedMesh = mesh;

         gs.Add(m);

         if (i == 0)
            b = mesh.bounds;
         else
            b.Encapsulate(mesh.bounds);
      }

      Camera.main.SendMessage("NewBounds", b);
   }
}
