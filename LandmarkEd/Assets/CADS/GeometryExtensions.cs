using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

namespace CADS {
   public sealed partial class Geometry {
      private const int MaxVertexCount = 65535;

      public List<Geometry> Split(int maxVertexCount = MaxVertexCount) {
         if (maxVertexCount == 0)
            throw new ArgumentOutOfRangeException("maxVertexCount");

         if (Indices.Length < maxVertexCount)
            return new List<Geometry> { this };

         var geometries = new List<Geometry>();

         for (var i = 0; i < Indices.Length; i += maxVertexCount) {
            var builder = new SubGeometryBuilder();

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

      private static List<Mesh> ToMeshes(ICollection<Geometry> geometries) {
         return geometries.Select(geometry => ToMesh(geometry)).ToList();
      }

      private static Mesh ToMesh(Geometry geometry) {
         var normals = geometry.Normals.Length > 0
            ? geometry.Normals
            : Calculator.CalculateNormals(geometry.Indices, geometry.Vertices);
 
         var mesh = new Mesh {
            vertices = geometry.Vertices,
            triangles = geometry.Indices,
            normals = normals,
            uv = geometry.UVs.Length > 0 ? geometry.UVs : Calculator.CalculateUVs(geometry.Vertices, normals, 10f)
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
