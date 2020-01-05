using System.Collections.Generic;

namespace MMD
{
    public class Vertex
    {
        public Matrix3D<float> Position;
        public Matrix3D<float> Normal;
        public Matrix2D<float> UV;
        //Maybe its just a List<float>? i dont know yet, will test it at a later point
        public List<float> Additional_Vec4;
        /// <summary>
        /// BDEF1, BDEF2, BDEF4, SDEF, QDEF -> [0, 1, 2, 3, 4]
        /// </summary>
        public byte WeightDeformType;

        public object WeightDeform;
        public float EdgeScale;

        public Vertex(){}
        public Vertex(Matrix3D<float> position, Matrix3D<float> normal, Matrix2D<float> uv, List<float> additionalVec4, byte weightDeformType, object weightDeform, float edgeScale)
        {
            Position = position;
            Normal = normal;
            UV = uv;
            Additional_Vec4 = additionalVec4;
            WeightDeformType = weightDeformType;
            WeightDeform = weightDeform;
            EdgeScale = edgeScale;
        }
    }
}