using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Fellow.Epi.Metrics.Infrastructure.Interception.Hook
{
    public class SomeMethodsHook : AllMethodsHook
    {
        private readonly string[] _methods;

        public SomeMethodsHook(params string[] methodNames)
        {
            this._methods = methodNames;
        }

        public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            bool manage = base.ShouldInterceptMethod(type, methodInfo);

            if (!manage)
                return false;

            return this._methods.Contains(methodInfo.Name, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
