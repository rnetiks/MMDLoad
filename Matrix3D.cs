namespace MMD
{
    public class Matrix3D<T>
    {
        public Matrix3D(){}
        public T X;
        public T Y;
        public T Z;

        public Matrix3D(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}