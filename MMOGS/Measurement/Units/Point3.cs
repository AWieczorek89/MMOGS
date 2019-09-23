using System;

namespace MMOGS.Measurement.Units
{
    public class Point3<T>
    {
        private T _x, _y, _z;

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

        public T Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public Point3(T x, T y, T z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Point3<T> Copy()
        {
            return new Point3<T>(_x, _y, _z);
        }

        public string GetCoordsString()
        {
            return $"[{_x}; {_y}; {_z}]";
        }
    }
}
