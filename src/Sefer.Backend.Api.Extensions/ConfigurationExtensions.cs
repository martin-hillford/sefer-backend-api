// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// This method tries to bind the configuration to the object to type T
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T BindSection<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var section = configuration.GetSection(key: sectionName);
        return section.Bind<T>();
    }

    /// <summary>
    /// This method tries to bind the configuration to the object to type T
    /// </summary>
    /// <param name="section"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static T Bind<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }
}