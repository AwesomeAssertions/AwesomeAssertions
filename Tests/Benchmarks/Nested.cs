namespace Benchmarks;

public sealed class Nested
{
    public int A { get; set; }

    public Nested B { get; set; }

    public Nested C { get; set; }

#pragma warning disable AV1562 // Don't use `ref` or `out` parameters: Keep benchmark as it was
    public static Nested Create(int i, ref int objectCount)
#pragma warning restore AV1562
    {
        if (i < 0)
        {
            return null;
        }

        if (i == 0)
        {
            return new Nested();
        }

        return new Nested
        {
            A = ++objectCount,
            B = Create(i - 1, ref objectCount),
            C = Create(i - 2, ref objectCount),
        };
    }
}
