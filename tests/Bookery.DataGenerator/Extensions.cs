namespace Bookery.DataGenerator;

public static class Extensions
{
    public static T Choose<T>(this List<T> c)
    {
        return c[Random.Shared.Next(0, c.Count)];
    }

    public static IEnumerable<T> ChooseMultiple<T>(this List<T> c, int count)
    {
        return Enumerable.Range(0, count).Select(_ => c.Choose());
    }
}