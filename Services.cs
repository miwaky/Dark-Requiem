using System;
using System.Collections.Generic;

namespace DarkRequiem.services
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        // Enregistre un service dans le locator
        public static void Register<T>(T service)
        {
            // services[typeof(T)] = service;
        }

        //  Récupère un service du locator
        public static T Get<T>()
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service {typeof(T).Name} non enregistré !");
        }

        //  Vérifie si un service est enregistré
        public static bool IsRegistered<T>()
        {
            return services.ContainsKey(typeof(T));
        }
    }
}
