﻿using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    internal static class ReflectionHelpers
    {
        public static void GetEventMethods(this Type @this, string eventName, out MethodInfo addMethod, out MethodInfo removeMethod)
        {
            EventInfo eventInfo = @this.GetEvent(eventName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            addMethod = eventInfo.AddMethod;
            removeMethod = eventInfo.RemoveMethod;
        }

        public static Delegate CreateEventDelegate<TEventArgs, TReturn>(this object @this, string onMethodName, Type realEventArgsType, Type realHandlerType) where TEventArgs : EventArgs
        {
            ParameterExpression eParameter = Expression.Parameter(typeof(TEventArgs), "e");
            ParameterExpression argsParameter = Expression.Parameter(realEventArgsType, "args");
            return Expression.Lambda(
                realHandlerType,
                Expression.Invoke(
                    Expression.Lambda<Func<TEventArgs, TReturn>>(
                        Expression.Call(
                            Expression.Constant(
                                @this, typeof(AppDomain)),
                            typeof(AppDomain).GetMethod(
                                onMethodName,
                                BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic),
                            Enumerable.Repeat(eParameter, 1)),
                        Enumerable.Repeat(
                            eParameter,
                            1)),
                    Enumerable.Repeat(
                        Expression.New(
                            eParameter.Type.GetConstructor(new[] { typeof(object) }),
                            Enumerable.Repeat(
                                argsParameter,
                                1)),
                        1)),
                false,
                (new ParameterExpression[] {
                    Expression.Parameter(
                        typeof(object),
                        "sender"
                    ),
                    argsParameter
                }).AsEnumerable()
                ).Compile();
        }

        public static Delegate CreateEventDelegate<TEventArgs>(this object @this, string onMethodName, Type realEventArgsType, Type realHandlerType) where TEventArgs : EventArgs
        {
            ParameterExpression eParameter = Expression.Parameter(typeof(TEventArgs), "e");
            ParameterExpression argsParameter = Expression.Parameter(realEventArgsType, "args");
            return Expression.Lambda(
                realHandlerType,
                Expression.Invoke(
                    Expression.Lambda<Action<TEventArgs>>(
                        Expression.Call(
                            Expression.Constant(
                                @this, typeof(AppDomain)),
                            typeof(AppDomain).GetMethod(
                                onMethodName,
                                BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic),
                            Enumerable.Repeat(eParameter, 1)),
                        Enumerable.Repeat(
                            eParameter,
                            1)),
                    Enumerable.Repeat(
                        Expression.New(
                            eParameter.Type.GetConstructor(new[] { typeof(object) }),
                            Enumerable.Repeat(
                                argsParameter,
                                1)),
                        1)),
                false,
                (new ParameterExpression[] {
                    Expression.Parameter(
                        typeof(object),
                        "sender"
                    ),
                    argsParameter
                }).AsEnumerable()
                ).Compile();
        }

        public static void AttachOrDetachEvent(this object @this, MulticastDelegate @delegate, Delegate realDelegate, MethodInfo manipulationInfo)
        {
            if (@delegate == null || @delegate.GetInvocationList().Length == 0)
            {
                manipulationInfo.Invoke(@this, new[] { realDelegate });
            }
        }

        public static Func<object> GetStaticPropertyFunction(this Type @this, string propertyName)
        {
            return Expression.Lambda<Func<object>>(Expression.Property(null, @this.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetMethod), Enumerable.Repeat<ParameterExpression>(null, 0)).Compile();
        }

        public static Func<object, T> GetInstancePropertyFunction<T>(this Type @this, string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object), $"_{nameof(T)}");
            return Expression.Lambda<Func<object, T>>(Expression.Property(Expression.Convert(parameter, @this), @this.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetMethod), true, Enumerable.Repeat(parameter, 1)).Compile();
        }

        public static Type RealType(this Type @this)
        {
            return typeof(string).GetTypeInfo().Assembly.GetType(@this.FullName);
        }
    }
}