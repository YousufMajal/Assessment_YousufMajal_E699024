using System.Reflection;

namespace Application.Config;

// Used to easily reference Application Project Assembly
public class AppAssemblyReference
{
    public static readonly Assembly assembly = typeof(AppAssemblyReference).Assembly;
}