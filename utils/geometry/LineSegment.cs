using System.Numerics;

class LineSegment<T> where T: INumber<T> {
    public Point2D<T> p1 {get; init;}
    public Point2D<T> p2 {get; init;}

    public LineSegment(Point2D<T> p1, Point2D<T> p2) {
        this.p1 = p1;
        this.p2 = p2;
    }

    public (T a, T b, T c) Coefficients() {
        var a = p2.y - p1.y;
        var b = p1.x - p2.x;
        var c = a * p1.x + b * p1.y;
        return (a, b, c);
    }

    private bool IsPointOnSegmentRange(Point2D<T> p) {
        bool isXOnLine = (p1.x <= p.x && p.x <= p2.x) || (p2.x <= p.x && p.x <= p1.x);
        bool isYOnLine = (p1.y <= p.y && p.y <= p2.y) || (p2.y <= p.y && p.y <= p1.y);
        return isXOnLine && isYOnLine;
    }

    public bool IsPointOnLine(Point2D<T> p) {
        var dxc = p.x - p1.x;
        var dyc = p.y - p1.y;
        var dxl = p2.x - p1.x;
        var dyl = p2.y - p1.y;

        var cross = dxc * dyl - dyc * dxl;
        if(!T.IsZero(cross)) return false;

        return IsPointOnSegmentRange(p);
    }    

    public Point2D<T>? GetIntersectionPoint(LineSegment<T> line) {
        var coefficientsLine1 = Coefficients();
        var coefficientsLine2 = line.Coefficients();
        var det = (coefficientsLine1.a * coefficientsLine2.b) - (coefficientsLine2.a * coefficientsLine1.b);
        if(T.IsZero(det)) {
            return null;
        }

        var iX = ((coefficientsLine2.b * coefficientsLine1.c) - (coefficientsLine1.b * coefficientsLine2.c)) / det;
        var iY = ((coefficientsLine1.a * coefficientsLine2.c) - (coefficientsLine2.a * coefficientsLine1.c)) / det;
        var resultingPoint = new Point2D<T>(iX, iY);

        if(IsPointOnSegmentRange(resultingPoint) && line.IsPointOnSegmentRange(resultingPoint)) {
            return resultingPoint;
        }

        return null;
    }

    public bool IsParallel(LineSegment<T> line) {
        var coefficientsLine1 = Coefficients();
        var coefficientsLine2 = line.Coefficients();
        var det = (coefficientsLine1.a * coefficientsLine2.b) - (coefficientsLine2.a * coefficientsLine1.b);
        return T.IsZero(det);        
    }

    public T? GetSlope() {
        var coefficients = Coefficients();
        if(T.IsZero(coefficients.b)) return default(T); // Vertical Line
        return coefficients.a / coefficients.b;
    }

    public T? GetYForX(T x) {
        var slope = GetSlope();
        if(slope is null) return default(T); // Vertical Line
        
        return (slope * (x - p1.x)) - p1.y;
    }

    // Return the intersection point if not parallel, otherwise, return a list of intersection points for segments that intersect each other in multiple places
    public List<Point2D<T>> GetMultipleIntersectionPoints(LineSegment<T> line) {
        List<Point2D<T>> resultingPoints = new List<Point2D<T>>();
        if(IsParallel(line)) {
            T? yAtX0ForLine1 = GetYForX(T.Zero);
            T? yAtX0ForLine2 = line.GetYForX(T.Zero);
            if(
                ((yAtX0ForLine1 == null || yAtX0ForLine2 == null) && this.p1.x == line.p1.x) // Vertical lines that are under the same X
                || (yAtX0ForLine1 == yAtX0ForLine2 && yAtX0ForLine1 != null)) // Parallel lines that have the same slope and cross the origin at the same Y
            { 
                var (smallestX, largestX) = p1.x < p2.x? (p1.x, p2.x) : (p2.x, p1.x);
                var (smallestY, largestY) = p1.y < p2.y? (p1.y, p2.y) : (p2.y, p1.y);
                for (var i = smallestX; i <= largestX; i++)
                {
                    for (var j = smallestY; j <= largestY; j++)
                    {

                        var point = new Point2D<T>(i,j);
                        if(this.IsPointOnLine(point) && line.IsPointOnLine(point)) {
                            resultingPoints.Add(point);
                        }
                    }
                }

            } 
        } else {
            var intersectionPoint = GetIntersectionPoint(line);
            if(!(intersectionPoint is null)) {
                resultingPoints.Add(intersectionPoint);
            }
        }

        return resultingPoints;
    }
}