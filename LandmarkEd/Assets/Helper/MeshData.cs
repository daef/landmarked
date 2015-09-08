using System;
using UnityEngine;

namespace Helper {
   public sealed partial class MeshData {
      public Vector3[] Vertices {
         get { return _vertices; }
         set {
            if (value == null)
               throw new ArgumentNullException("value");

            _vertices = new Vector3[value.Length];
            value.CopyTo(_vertices, 0);
         }
      }

      public int[] Indices {
         get { return _indices; }
         set {
            if (value == null)
               throw new ArgumentNullException("value");

            _indices = new int[value.Length];
            value.CopyTo(_indices, 0);
         }
      }

      public Vector3[] Normals {
         get { return _normals; }
         set {
            if (value == null)
               throw new ArgumentNullException("value");

            _normals = new Vector3[value.Length];
            value.CopyTo(_normals, 0);
         }
      }

      public Vector2[] UVs {
         get { return _uvs; }
         set {
            if (value == null)
               throw new ArgumentNullException("value");

            _uvs = new Vector2[value.Length];
            value.CopyTo(_uvs, 0);
         }
      }

      private Vector3[] _vertices = new Vector3[0];
      private int[] _indices = new int[0];
      private Vector3[] _normals = new Vector3[0];
      private Vector2[] _uvs = new Vector2[0];

      public bool IsEmpty() {
         return _indices.Length == 0 && _vertices.Length == 0;
      }
   }
}

