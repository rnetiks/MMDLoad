namespace MMD
{
    public class Morph
    {
    }

    public class MorphGroup
    {
        /// <summary>
        /// See Index Types
        /// </summary>
        public int MorphIndex;
        /// <summary>
        /// Weight of an Indexed Morph
        /// </summary>
        public float Influence;
    }

    public class MorphVertex
    {
        public int VertexIndex;
        public Matrix3D<float> Translation;

        public MorphVertex(){}
        public MorphVertex(int vertexIndex, Matrix3D<float> translation)
        {
            VertexIndex = vertexIndex;
            Translation = translation;
        }
    }

    public class MorphBone
    {
        public int BoneIndex;
        public Matrix3D<float> Translation;
        public Matrix4D<float> Rotation;

        public MorphBone(){}
        public MorphBone(int boneIndex, Matrix3D<float> translation, Matrix4D<float> rotation)
        {
            BoneIndex = boneIndex;
            Translation = translation;
            Rotation = rotation;
        }
    }

    public class MorphUV
    {
        public int VertexIndex;
        public Matrix4D<float> Floats;

        public MorphUV(){}
        public MorphUV(int vertexIndex, Matrix4D<float> floats)
        {
            VertexIndex = vertexIndex;
            Floats = floats;
        }
    }

    public class MorphMaterial
    {
        public int MaterialIndex;
        public byte wtf_i_dont_even_know;
        public Matrix4D<float> Diffuse;
        public Matrix3D<float> Specular;
        public float Specularity;
        public Matrix3D<float> Ambient;
        public Matrix4D<float> EdgeColor;
        public float EdgeSize;
        public Matrix4D<float> TextureTint;
        public Matrix4D<float> EnviromentTint;
        public Matrix4D<float> ToonTint;
    }
}