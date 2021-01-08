using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Microservice.Course.Common.Helpers
{
    public static class F
    {
        public static List<T> List<T>(params T[] items) => items.ToList();

        public static T ParseObject<T>(object value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }

        public static TR Pipe<T, TR>(this T @this, Func<T, TR> func) => func(@this);

        public static TR Pipe<T, T1, TR>(this T @this, Func<T, T1> func, Func<T1, TR> func1) => @this.Pipe(func).Pipe(func1);

        public static TR Pipe<T, T1, T2, TR>(this T @this, Func<T, T1> func, Func<T1, T2> func1, Func<T2, TR> func2) => @this.Pipe(func).Pipe(func1).Pipe(func2);

        /// <summary>
        /// Copies all public, readable properties from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TTarget">Type of the target.</typeparam>
        /// <param name="source">Source to copy properties from.</param>
        /// <param name="target">Target to copy properties to.</param>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }

        public static void ExecuteByPassException(Action fn)
        {
            try
            {
                fn();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    /// <summary>
    /// Static class to efficiently store the compiled delegate which can
    /// do the copying. We need a bit of work to ensure that exceptions are
    /// appropriately propagated, as the exception is generated at type initialization
    /// time, but we wish it to be thrown as an ArgumentException.
    /// Note that this type we do not have a constructor constraint on TTarget, because
    /// we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    /// <typeparam name="TSource">The copy from type.</typeparam>
    /// <typeparam name="TTarget">The copy to type.</typeparam>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        /// Delegate to create a new instance of the target type given an instance of the
        /// source type. This is a single delegate from an expression tree.
        /// </summary>
        private static readonly Func<TSource, TTarget> _creator;

        /// <summary>
        /// List of properties to grab values from. The corresponding targetProperties
        /// list contains the same properties in the target type. Unfortunately we can't
        /// use expression trees to do this, because we basically need a sequence of statements.
        /// We could build a DynamicMethod, but that's significantly more work :) Please mail
        /// me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> _sourceProperties = new List<PropertyInfo>();
        private static readonly List<PropertyInfo> _targetProperties = new List<PropertyInfo>();
        private static readonly Exception _initializationException;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static PropertyCopier()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            try
            {
                _creator = BuildCreator();
                _initializationException = null;
            }
            catch (Exception e)
            {
                _creator = null;
                _initializationException = e;
            }
        }

        internal static TTarget Copy(TSource source)
        {
            if (_initializationException != null)
            {
                throw _initializationException;
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return _creator(source);
        }

        internal static void Copy(TSource source, TTarget target)
        {
            if (_initializationException != null)
            {
                throw _initializationException;
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            for (int i = 0; i < _sourceProperties.Count; i++)
            {
                _targetProperties[i].SetValue(target, _sourceProperties[i].GetValue(source, null), null);
            }
        }

        private static Func<TSource, TTarget> BuildCreator()
        {
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();
            foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }

                PropertyInfo targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                if (targetProperty == null)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not present and accessible in " + typeof(TTarget).FullName);
                }

                if (!targetProperty.CanWrite)
                {
                    continue;
                }

                if (!targetProperty.CanWrite)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " + typeof(TTarget).FullName);
                }

                if ((targetProperty.GetSetMethod()?.Attributes & MethodAttributes.Static) != 0)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is static in " + typeof(TTarget).FullName);
                }

                if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " + typeof(TTarget).FullName);
                }

                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                _sourceProperties.Add(sourceProperty);
                _targetProperties.Add(targetProperty);
            }

            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }
    }
}
