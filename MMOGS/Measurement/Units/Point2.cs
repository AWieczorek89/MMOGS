namespace MMOGS.Measurement.Units
{
    public class Point2<T>
    {
        private T _x, _y;

        public T X
        {
            get { return _x; }
            set { _x = value; }
        }

        public T Y
        {
            get { return _y; }
            set { _y = value; }
        }
        
        public Point2(T x, T y)
        {
            _x = x;
            _y = y;
        }

        public Point2<T> Copy()
        {
            return new Point2<T>(_x, _y);
        }
    }
}
