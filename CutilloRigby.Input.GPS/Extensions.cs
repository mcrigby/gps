namespace CutilloRigby.Input.GPS;

internal static class Extensions
{
    public static string FactoryName(this Type type) =>
        type.FullName.Replace('+', '.');
}
