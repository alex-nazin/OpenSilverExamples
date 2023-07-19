using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MigrationExamples.OpenSilver.Pages.ObjectInstantiation
{
    /// <summary>
    /// Original approach was found here: https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/
    /// Constructor parameters were removed, so now this class supports only parameterless (default) constructors.
    /// </summary>
    internal static class ClassBuilder
    {
        private delegate T ObjectActivator<T>();

        private static readonly Dictionary<Type, object> activatorsCache = new Dictionary<Type, object>();

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);

            if (activatorsCache.TryGetValue(type, out object activator))
            {
                return ((ObjectActivator<T>)activator)();
            }

            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);

            if (ctor != null)
            {
                ObjectActivator<T> createdActivator = GetActivator<T>(ctor); 
                activatorsCache[type] = createdActivator;
                return createdActivator();
            }
            else
            {
                throw new ArgumentException("Unable to find constructor with specified parameters.");
            }
        }


        private static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            // Creates a NewExpression that represents calling the specified constructor with the specified arguments.
            NewExpression newExp = Expression.New(ctor);

            // Creates a LambdaExpression by first constructing a delegate type.
            LambdaExpression lambdaExpression = Expression.Lambda(typeof(ObjectActivator<T>), newExp);

            // Compile function will create a delegate function that represents the lamba expression
            ObjectActivator<T> compiledExpression = (ObjectActivator<T>)lambdaExpression.Compile();

            return compiledExpression;
        }
    }
}
