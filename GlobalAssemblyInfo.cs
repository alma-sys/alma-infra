using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("-")]
[assembly: AssemblyProduct("-")]
[assembly: AssemblyCopyright("-")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(AssemblyValues.Version)]
[assembly: AssemblyFileVersion(AssemblyValues.Version)]
[assembly: AssemblyInformationalVersion(AssemblyValues.InfoVersion)]
[assembly: ComVisible(false)]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// ARQUIVO SUBSTITUIDO NO PROCESSO DE RELEASE PELO .RELEASE

static class AssemblyValues
{
    public const string Version = "1.11.1111.1111";
    public const string InfoVersion = Version + "-alpha";
}