using System.Collections;

namespace MMD
{
    public class Material
    {
        public string MaterialNameLocal;
        public string MaterialNameUniversal;
        public Matrix4D<float> DiffuseColor;
        public Matrix3D<float> SpecularColor;
        public float SpecularStrength;
        public Matrix3D<float> AmbientColor;
        /// <summary>
        /// 8 Bits from 1 Byte, use BitArray
        /// </summary>
        public BitArray DrawingFlags;

        public Matrix4D<float> EdgeColor;
        public float EdgeScale;
        public int TextureIndex;
        public int EnviromentIndex;
        public byte EnviromentBlendMode;
        public byte ToonReference;
        public int ToonValue;
        public string MetaData;
        /// <summary>
        /// Times 3, Divide by 3 To get Result
        /// </summary>
        public int SurfaceCount;
    }
}