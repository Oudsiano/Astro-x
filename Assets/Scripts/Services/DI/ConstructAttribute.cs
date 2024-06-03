using System;
using JetBrains.Annotations;

namespace Services.DI
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    [MeansImplicitUse]
    public class ConstructAttribute : Attribute
    {
    }
}