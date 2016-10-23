using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitStd;

namespace RevitStd.Geometry
{

    /// <summary> 将任意曲面转换为平面网格集合 </summary>
    public class SurfaceTriangulator
    {
        private Face _face;

        private Transform _transform;

        /// <summary> 构造函数 </summary>
        /// <param name="pickedFace">要进行转换的任意形态的曲面</param>
        /// <param name="transform">此曲面从其定义的空间位置（比如族空间）到希望的目标位置（比如模型空间）的变换矩阵</param>
        public SurfaceTriangulator(Face pickedFace, Transform transform)
        {
            _face = pickedFace;
            _transform = transform;
        }

        public List<PlanarFace> GetMeshedFaces()
        {
            Mesh mesh = GetMesh();
            TessellatedShapeBuilder builder
              = new TessellatedShapeBuilder();

            builder.OpenConnectedFaceSet(false);

            List<XYZ> args = new List<XYZ>(3);

            XYZ[] triangleCorners = new XYZ[3];

            for (int i = 0; i < mesh.NumTriangles; ++i)
            {
                MeshTriangle triangle = mesh.get_Triangle(i);

                triangleCorners[0] = triangle.get_Vertex(0);
                triangleCorners[1] = triangle.get_Vertex(1);
                triangleCorners[2] = triangle.get_Vertex(2);

                TessellatedFace tesseFace
                  = new TessellatedFace(triangleCorners,
                    ElementId.InvalidElementId);

                if (builder.DoesFaceHaveEnoughLoopsAndVertices(
                  tesseFace))
                {
                    builder.AddFace(tesseFace);
                }
            }

            builder.CloseConnectedFaceSet();

            TessellatedShapeBuilderResult result
              = builder.Build(
                TessellatedShapeBuilderTarget.AnyGeometry,
                TessellatedShapeBuilderFallback.Mesh,
                ElementId.InvalidElementId);

            var geo = result.GetGeometricalObjects();

            var solids = GeoHelper.GetSolidsInModel(geo, GeoHelper.SolidVolumnConstraint.Any);
            var faces = GeoHelper.GetSurfaces(solids.Keys);

            return faces.OfType<PlanarFace>().ToList();
        }


        /// <summary>
        /// 返回 项目中的face所对应的Mesh（其定位也是位于项目中）
        /// </summary>
        /// <returns></returns>
        private Mesh GetMesh()
        {

            Mesh mesh = _face.Triangulate();
            mesh = mesh.get_Transformed(_transform);

            return mesh;
        }

    }
}