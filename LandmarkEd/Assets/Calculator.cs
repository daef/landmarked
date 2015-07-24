using UnityEngine;

namespace Assets {
   // saved my day http://answers.unity3d.com/questions/64410/generating-uvs-for-a-scripted-mesh.html
   public class Calculator {
      private enum Facing {
         Up,
         Forward,
         Right
      };

      public static Vector3[] CalculateNormals(int[] tris, Vector3[] vertices) {
         var normals = new Vector3[vertices.Length];

         for (int i = 0; i < tris.Length; i += 3) {
            int i0 = tris[i + 0];
            int i1 = tris[i + 1];
            int i2 = tris[i + 2];

            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];

            Vector3 side1 = v1 - v0;
            Vector3 side2 = v2 - v0;

            var direction = Vector3.Cross(side1, side2);

            normals[i0] += direction;
            normals[i1] += direction;
            normals[i2] += direction;
         }

         for (int i = 0; i < vertices.Length; i++)
            normals[i].Normalize();

         return normals;
      }

      public static Vector2[] CalculateUVs(Vector3[] vertices, Vector3[] normals, float scale) {
         var uvs = new Vector2[vertices.Length];

         for (int i = 0; i < uvs.Length; i++) {
            var v = vertices[i];
            var facing = FacingDirection(normals[i]);
            switch (facing) {
               case Facing.Forward:
                  uvs[i] = ScaledUv(v.x, v.y, scale);
                  break;
               case Facing.Up:
                  uvs[i] = ScaledUv(v.x, v.z, scale);
                  break;
               case Facing.Right:
                  uvs[i] = ScaledUv(v.y, v.z, scale);
                  break;
            }
         }

         return uvs;
      }

      private static bool FacesThisWay(Vector3 v, Vector3 dir, Facing p, ref float maxDot, ref Facing ret) {
         float t = Vector3.Dot(v, dir);
         if (t > maxDot) {
            ret = p;
            maxDot = t;
            return true;
         }
         return false;
      }

      private static Facing FacingDirection(Vector3 v) {
         var ret = Facing.Up;
         // initializing maxDot  w/ negativeinfinity
         // (like proposed on //answers.unity3d)
         // introduces a bug where "right" is falsely identified as "forward"
         float maxDot = 0f;

         if (!FacesThisWay(v, Vector3.right, Facing.Right, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.left, Facing.Right, ref maxDot, ref ret);

         if (!FacesThisWay(v, Vector3.forward, Facing.Forward, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.back, Facing.Forward, ref maxDot, ref ret);

         if (!FacesThisWay(v, Vector3.up, Facing.Up, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.down, Facing.Up, ref maxDot, ref ret);

         return ret;
      }

      private static Vector2 ScaledUv(float uv1, float uv2, float scale) {
         return new Vector2(uv1/scale, uv2/scale);
      }
   }
}
