using System.Windows;

public static class PointExtensions
{
    public static Point Add(this Point point, Point vector)
    {
        return new Point(point.X + vector.X, point.Y + vector.Y);
    }

    public static Point Subtract(this Point point, Point vector)
    {
        return new Point(point.X - vector.X, point.Y - vector.Y);
    }

    public static Point Multiply(this Point point, Point otherPoint)
    {
        return new Point(point.X * otherPoint.X, point.Y * otherPoint.Y);
    }

    public static Point Multiply(this double constant, Point point)
    {
        return new Point(point.X * constant, point.Y * constant);
    }

    public static Point Multiply(this Point point, double constant)
    {
        return new Point(point.X * constant, point.Y * constant);
    }
}
