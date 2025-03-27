using System;
using System.Collections.Generic;

namespace GostSpec2023
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<TInterface, TImplementation>()
            where TImplementation : TInterface, new()
        {
            _services[typeof(TInterface)] = new TImplementation();
        }

        public static TInterface Resolve<TInterface>()
        {
            return (TInterface)_services[typeof(TInterface)];
        }
        
        public static bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

    }
}