namespace MMD
{
    public class Matrix2D<T>
    {
        public Matrix2D(){}
        public T X;
        public T Y;

        public Matrix2D(T x, T y)
        {
            Y = y;
            X = x;
        }
    }
}