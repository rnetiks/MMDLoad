using System.Collections;

namespace MMD
{
    /// <summary>
    /// The bone section starts with an int defining how many bones there are
    ///  (Note that it is a signed int)
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// Handy name for Bone (usually Japanese)
        /// </summary>
        public string BoneNameLocal;
        /// <summary>
        /// Handy name for Bone (usually English)
        /// </summary>
        public string BoneNameUniversal;
        /// <summary>
        /// The Local translation of the Bone
        /// </summary>
        public Matrix3D<float> Position;
        /// <summary>
        /// See index types
        /// </summary>
        public int ParentBoneIndex;
        /// <summary>
        /// Deformation hierarchy
        /// </summary>
        public int Layer;
        /// <summary>
        /// Flags[2]
        /// </summary>
        public BitArray Flags;
        /// <summary>
        /// Matrix3D/Index
        /// </summary>
        public object TailPosition;
        /// <summary>
        /// Used if either of the Inherit Flags are set. See Inherit Bone.
        /// </summary>
        public InheritBone InheritBone;
        /// <summary>
        /// Used if fixed axis flag is set. See BoneFixedAxis
        /// </summary>
        public BoneFixedAxis FixedAxis;
        /// <summary>
        /// Used if LocalCoordinate flag is set. See BoneLocalCoordinate.
        /// </summary>
        public BoneLocalCoordinate LocalCoordinate;

        /// <summary>
        /// Used if eternal parent deform flag is set. See BoneExternalParent.
        /// </summary>
        public BoneExternalParent ExternalParent;
        /// <summary>
        /// Used if IK flag is set. See BoneIK
        /// </summary>
        public BoneIK IK;
        /// <summary>
        /// Initializes a Bone with no Values
        /// </summary>
        public Bone(){}
        /// <summary>
        /// Initializes a Bone
        /// </summary>
        public Bone(string boneNameLocal, string boneNameUniversal, Matrix3D<float> position, int parentBoneIndex,
            int layer, BitArray flags, object tailPosition, InheritBone inheritBone, BoneFixedAxis fixedAxis,
            BoneLocalCoordinate localCoordinate, BoneExternalParent externalParent, BoneIK ik)
        {
            BoneNameLocal = boneNameLocal;
            BoneNameUniversal = boneNameUniversal;
            Position = position;
            ParentBoneIndex = parentBoneIndex;
            Layer = layer;
            Flags = flags;
            TailPosition = tailPosition;
            InheritBone = inheritBone;
            FixedAxis = fixedAxis;
            LocalCoordinate = localCoordinate;
            ExternalParent = externalParent;
            IK = ik;
        }
    }

    public class InheritBone
    {
        public int ParentIndex;
        public float ParentInfluence;
    }

    public class BoneFixedAxis
    {
        public Matrix3D<float> AxisDirection;
    }

    public class BoneLocalCoordinate
    {
        public Matrix3D<float> XVector;
        public Matrix3D<float> ZVector;
    }

    public class BoneExternalParent
    {
        public int ParentIndex;
    }

    public class IKAngleLimit
    {
        public Matrix3D<float> LimitMin;
        public Matrix3D<float> LimitMax;
    }

    public class IKLinks
    {
        public int BoneIndex;
        public byte HasLimits;
        /// <summary>
        /// Used if HasLimits is set to 1.
        /// </summary>
        public IKAngleLimit IKAngleLimits;
    }

    public class BoneIK
    {
        /// <summary>
        /// See Index Types
        /// </summary>
        public int TargetIndex;
        public int LoopCount;
        public float LimitRadian;
        /// <summary>
        /// How many Bones this IK links with
        /// </summary>
        public int LinkCount;
        /// <summary>
        /// N is LinkCount, can be Zero. See IKLinks
        /// </summary>
        public IKLinks IkLinks;
    }
    
}