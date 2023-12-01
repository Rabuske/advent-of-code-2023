using System.Numerics;

record Point3D<T> where T: INumber<T> {

    public T x {get; init;}
    public T y {get; init;}
    public T z {get; init;}

    public Point3D(T x, T y, T z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point3D(string x, string y, string z) {
        this.x = T.Parse(x, null);
        this.y = T.Parse(y, null);
        this.z = T.Parse(z, null);
    }


    public static Point3D<T> operator +(Point3D<T> p1, Point3D<T> p2) {
        return new Point3D<T>(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }

    public static Point3D<T> operator *(Point3D<T> p1, T scalar) {
        return new Point3D<T>(p1.x * scalar, p1.y * scalar, p1.z * scalar);
    }

    public static Point3D<T> operator -(Point3D<T> p1, Point3D<T> p2) {
        return new Point3D<T>(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }
    
    public T ManhattanDistance(Point3D<T> p) {
        return T.Abs(this.x - p.x) + T.Abs(this.y - p.y) +  T.Abs(this.z - p.z);
    }

    public override string ToString(){
        return $"({this.x},{this.y},{this.z})";
    }   
}

