namespace MMD
{
    public class Matrix4D<T>
    {
        public Matrix4D(){}
        public T X;
        public T Y;
        public T Z;
        public T W;

        public Matrix4D(T x, T y, T z, T w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}