namespace Sefer.Backend.Api.Data.Handlers.Test.Extensions;

public static class TypeExtensions
{
    public static T InvokeFirstConstructor<T>(object? parameter) where T : class
        => InvokeFirstConstructor<T>([parameter]);
    
    public static T InvokeFirstConstructor<T>(object?[] parameters) where T : class
    {
        var type = typeof(T);
        var ctor =  type.GetConstructors().First();
        var instance = ctor.Invoke(parameters) as T;
        Assert.IsNotNull(instance);
        return instance;
    }
    
    public static List<T> GetEnumList<T>()
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>().ToList();
        return enumList;
    }
}