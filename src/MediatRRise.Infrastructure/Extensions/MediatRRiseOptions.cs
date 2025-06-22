using System.Reflection;

namespace MediatRRise.Infrastructure.Extensions;

public class MediatRRiseOptions
{
    public List<Assembly> Assemblies { get; } = new();

    public void RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
    }
}