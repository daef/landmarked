using System.Collections.Generic;
using UnityEngine;

namespace Helper {
   internal sealed class SubMeshDataBuilder {
      private readonly List<Vector3> _vertices = new List<Vector3>();
      private readonly List<Vector3> _normals = new List<Vector3>();
      private readonly List<Vector2> _uvs = new List<Vector2>();
      private readonly List<int> _indices = new List<int>();
      private readonly Dictionary<int, Vector3> _sourceIndexMap = new Dictionary<int, Vector3>();
      private readonly Dictionary<int, int> _internalIndexMap = new Dictionary<int, int>();

      public MeshData Build() {
         return new MeshData {
            Indices = _indices.ToArray(),
            Vertices = _vertices.ToArray(),
            Normals = _normals.ToArray(),
            UVs = _uvs.ToArray()
         };
      }

      public void AddVertex(int index, Vector3 vertex) {
         int internalIndex;

         if (!_sourceIndexMap.ContainsKey(index)) {
            internalIndex = _vertices.Count;

            _internalIndexMap.Add(index, internalIndex);
            _sourceIndexMap.Add(index, vertex);
            _vertices.Add(vertex);
         } else
            internalIndex = _internalIndexMap[index];

         _indices.Add(internalIndex);
      }

      public void AddNormal(Vector3 normal) {
         _normals.Add(normal);
      }

      public void AddUv(Vector2 uv) {
         _uvs.Add(uv);
      }
   }
}
