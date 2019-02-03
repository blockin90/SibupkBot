using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpkServices
{
    /// <summary>
    /// Глобальная точка доступа к зарегистрированным в системе сервисам
    /// </summary>
    public static class ServiceProvider
    {
        static Dictionary<Type, Tuple<Type, object[]>> registeredServices;
        /// <summary>
        /// Зарегистрированные типы-обработчики
        /// </summary>
        static Dictionary<Type, Tuple<Type, object[]>> RegisteredServices
        {
            get => registeredServices ?? (registeredServices = new Dictionary<Type, Tuple<Type, object[]>>());
        }
        static Dictionary<Type, object> registeredServiceInstances;
        /// <summary>
        /// Зарегистрированные объекты-обработчики
        /// </summary>
        static Dictionary<Type, object> RegisteredInstances
        {
            get => registeredServiceInstances ?? (registeredServiceInstances = new Dictionary<Type, object>());
        }

        public static void RegisterService( Type serviceType, Type handlerType, params object[] parameters)
        {
            if (serviceType == null || !serviceType.IsInterface) {
                throw new ArgumentException("service param must be an interface!");
            }
            if(handlerType == null || !handlerType.GetInterfaces().Contains(serviceType)) {
                throw new ArgumentException("handler must be derived class from service interface!");
            }
            RegisteredServices[serviceType] = Tuple.Create(handlerType, parameters);
        }

        public static void RegisterService( Type serviceType, object handler)
        {
            var handlerType = handler.GetType();
            if( handlerType.GetInterfaces().Contains(serviceType) == false) {
                throw new ArgumentException("handler must be derived from service interface!");
            }
            RegisteredInstances[serviceType] = handler;
        }

        public static T GetService<T>() where T :class
        {
            var serviceInterface = typeof(T);
            if( RegisteredServices.ContainsKey(serviceInterface)) {
                var handler = RegisteredServices[serviceInterface];
                return Activator.CreateInstance(handler.Item1, handler.Item2) as T;
            }else if( RegisteredInstances.ContainsKey(serviceInterface) ) {
                return RegisteredInstances[serviceInterface] as T;
            }
            throw new NotImplementedException();
        }
        
    }
}
