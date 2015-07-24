using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace CADS {
   /*
    * Used documentation: http://en.wikipedia.org/wiki/STL_%28file_format%29
    */

   internal sealed class StlTextReader {
      private const string KeyVertex = "vertex";
      private const string KeyNormal = "normal";
      private const int IndexOfVertex = 1;
      private const int IndexOfNormal = 2;
      private const int IndexOfKey = 0;

      private readonly List<Vector3> _vertices = new List<Vector3>();
      private readonly List<int> _indices = new List<int>();

      private StreamReader _stlBytes;

      public Geometry Read(byte[] bytes) {
         if (bytes == null)
            throw new ArgumentNullException("bytes");

         if (StlBinaryReader.IsBinaryFormat(bytes))
            throw new ArgumentException("bytes look binary, text reader panic");

         try {
            OpenStlBytes(bytes);
            FetchStlBytes();
         }
         finally {
            CloseStlBytes();
         }

         return new Geometry {Indices = _indices.ToArray(), Vertices = _vertices.ToArray()};
      }

      private void OpenStlBytes(byte[] bytes) {
         _stlBytes = new StreamReader(new MemoryStream(bytes));
      }

      private void CloseStlBytes() {
         if (_stlBytes != null)
            _stlBytes.Close();
      }

      private void FetchStlBytes() {
         FetchHeader();
         FetchAllPolygons();
      }

      private void FetchHeader() {
         _stlBytes.ReadLine();
      }

      private static string[] TokenizeLine(string line) {
         return line == null
            ? new string[0]
            : line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
      }

      private void FetchAllPolygons() {
         _vertices.Clear();
         _indices.Clear();

         while (!_stlBytes.EndOfStream) {
            var lineTokens = TokenizeLine(_stlBytes.ReadLine());
            if (lineTokens.Length == 0)
               continue;

            FetchLine(lineTokens);
         }
      }

      private void FetchLine(IList<string> lineTokens) {
         if (IsVertexLine(lineTokens))
            FetchVertex(lineTokens);
         else if (IsNormalLine(lineTokens))
            FetchNormal(lineTokens);
      }

      private static bool IsVertexLine(IList<string> lineTokens) {
         return lineTokens[IndexOfKey].Equals(KeyVertex);
      }

      private static bool IsNormalLine(IList<string> lineTokens) {
         return lineTokens[IndexOfKey].Equals(KeyNormal);
      }

      private void FetchNormal(IList<string> lineTokens) {
         const int polygonVerticesCount = 3;

         for (var i = 0; i < polygonVerticesCount; ++i) {
            ParseVector3(lineTokens, IndexOfNormal);
         }
      }

      private void FetchVertex(IList<string> lineTokens) {
         _indices.Add(_vertices.Count);
         _vertices.Add(ParseVector3(lineTokens, IndexOfVertex));
      }

      private static Vector3 ParseVector3(IList<string> items, int index) {
         return new Vector3(
            Single.Parse(items[index], CultureInfo.InvariantCulture),
            Single.Parse(items[index + 1], CultureInfo.InvariantCulture),
            Single.Parse(items[index + 2], CultureInfo.InvariantCulture));
      }
   }
}
