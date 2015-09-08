using System;

namespace Helper {
   public static class StlReader {
      public static MeshData Read(byte[] bytes) {
         if (bytes == null)
            throw new ArgumentNullException("bytes");

         return StlBinaryReader.IsBinaryFormat(bytes)
            ? new StlBinaryReader().Read(bytes)
            : new StlTextReader().Read(bytes);
      }
   }
}
