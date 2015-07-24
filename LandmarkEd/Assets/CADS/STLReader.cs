using System;

namespace CADS {
   public static class StlReader {
      public static Geometry Read(byte[] bytes) {
         if (bytes == null)
            throw new ArgumentNullException("bytes");

         return StlBinaryReader.IsBinaryFormat(bytes)
            ? new StlBinaryReader().Read(bytes)
            : new StlTextReader().Read(bytes);
      }
   }
}
