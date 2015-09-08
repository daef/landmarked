using System;
using System.IO;
using UnityEngine;

namespace Helper {
   internal sealed class StlBinaryReader {
      private const int HeaderSize = 80;
      private const int TriangleSize = sizeof (float)*9 + 3*sizeof (float) + sizeof (ushort);
      private const int PolygonVerticesCount = 3;

      private Vector3[] _vertices;
      private int[] _indices;

      private BinaryReader _stlBytes;

      public MeshData Read(byte[] bytes) {
         try {
            OpenStlBytes(bytes);
            FetchStlBytes();
         }
         finally {
            CloseStlBytes();
         }

         return new MeshData {Indices = _indices, Vertices = _vertices};
      }

      private void CloseStlBytes() {
         if (_stlBytes != null)
            _stlBytes.Close();
      }

      private void OpenStlBytes(byte[] bytes) {
         _stlBytes = new BinaryReader(new MemoryStream(bytes));
      }

      private void FetchStlBytes() {
         FetchHeader();
         FetchAllPolygons();
      }

      private void FetchHeader() {
         _stlBytes.ReadBytes(HeaderSize);
      }

      private void FetchAllPolygons() {
         var triangleCount = ReadTriangleCount();
         var vertexCount = triangleCount*3;

         _vertices = new Vector3[vertexCount];
         _indices = new int[vertexCount];

         for (var i = 0; i < triangleCount; ++i) {
            FetchPolygon(i);
            SkipAttribute();
         }
      }

      private void FetchPolygon(int index) {
         ReadVector3();

         for (var i = 0; i < PolygonVerticesCount; i++)
            AddVertex(ReadVector3(), index*PolygonVerticesCount + i);
      }

      private void AddVertex(Vector3 vertex, int index) {
         _indices[index] = index;
         _vertices[index] = vertex;
      }

      public static bool IsBinaryFormat(byte[] bytes) {
         var expectedFileSize = HeaderSize + sizeof (int) + ReadTriangleCount(bytes)*TriangleSize;
         return expectedFileSize == bytes.Length;
      }

      private static uint ReadTriangleCount(byte[] stlFileBytes) {
         return BitConverter.ToUInt32(stlFileBytes, HeaderSize);
      }

      private int ReadTriangleCount() {
         return _stlBytes.ReadInt32();
      }

      private void SkipAttribute() {
         _stlBytes.ReadUInt16();
      }

      private Vector3 ReadVector3() {
         return new Vector3(
            _stlBytes.ReadSingle(),
            _stlBytes.ReadSingle(),
            _stlBytes.ReadSingle());
      }
   }
}
