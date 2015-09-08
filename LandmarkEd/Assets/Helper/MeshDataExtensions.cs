using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

namespace Helper {
   public sealed partial class MeshData {
      private const int MaxVertexCount = 65535;

      public List<MeshData> Split(int maxVertexCount = MaxVertexCount) {
         if (maxVertexCount == 0)
            throw new ArgumentOutOfRangeException("maxVertexCount");

         if (Indices.Length < maxVertexCount)
            return new List<MeshData> { this };

         var geometries = new List<MeshData>();

         for (var i = 0; i < Indices.Length; i += maxVertexCount) {
            var builder = new SubMeshDataBuilder();

            for (var j = 0; j < maxVertexCount && (i + j) < Indices.Length; j++) {
               var vIndex = Indices[i + j];
               builder.AddVertex(vIndex, Vertices[vIndex]);

               if (vIndex < Normals.Length)
                  builder.AddNormal(Normals[vIndex]);

               if (vIndex < UVs.Length)
                  builder.AddUv(UVs[vIndex]);
            }

            geometries.Add(builder.Build());
         }

         return geometries;
      }

      public List<Mesh> ToMeshes() {
         return ToMeshes(Split());
      }

      private static List<Mesh> ToMeshes(ICollection<MeshData> geometries) {
         return geometries.Select(meshData => ToMesh(meshData)).ToList();
      }

      private static Mesh ToMesh(MeshData meshData) {
         var normals = meshData.Normals.Length > 0
            ? meshData.Normals
            : Calculator.CalculateNormals(meshData.Indices, meshData.Vertices);
 
         var mesh = new Mesh {
            vertices = meshData.Vertices,
            triangles = meshData.Indices,
            normals = normals,
            uv = meshData.UVs.Length > 0 ? meshData.UVs : Calculator.CalculateUVs(meshData.Vertices, normals, 10f)
         };

         mesh.RecalculateBounds();
         return mesh;
      }

      public int MergeVertices() {
         var vertexMap = new Dictionary<Vector3, int>();
         var mergedIndices = new List<int>();
         var mergedVertices = new List<Vector3>();
         var mergedUVs = new List<Vector2>();

         foreach (var index in Indices) {
            var vertex = Vertices[index];
            var mergedIndex = mergedVertices.Count;

            if (vertexMap.ContainsKey(vertex)) {
               mergedIndex = vertexMap[vertex];
            } else {
               vertexMap.Add(vertex, mergedIndex);

               mergedVertices.Add(vertex);

               if (index < UVs.Length)
                  mergedUVs.Add(UVs[index]);
            }

            mergedIndices.Add(mergedIndex);
         }

         var mergedVerticesCount = Vertices.Length - mergedVertices.Count;

         // no need to copy here -> assing to backing field instead of using setter
         _indices = mergedIndices.ToArray();
         _vertices = mergedVertices.ToArray();
         _normals = new Vector3[0];
         _uvs = mergedUVs.ToArray();

         return mergedVerticesCount;
      }
   }
}
