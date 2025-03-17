using System.Reflection;

namespace WebConsole.Helper;

public static class InjectProgramHelper
{
    // 可選：如果需要更細緻的控制，可以分別註冊
    public static IServiceCollection AddAllServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var serviceTypes = assembly.GetTypes()
            .Where(t => t.Namespace != null
                        && t.Namespace.EndsWith(".Services")
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var type in serviceTypes) services.AddScoped(type);

        return services;
    }

    public static IServiceCollection AddAllRepositories(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.Namespace != null
                        && t.Namespace.EndsWith(".Repository")
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var type in repositoryTypes) services.AddScoped(type);

        return services;
    }
}