using System;
using System.Runtime.CompilerServices;

namespace SexyFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public Type DelegateType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get;

            [MethodImpl(MethodImplOptions.NoInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public MonoPInvokeCallbackAttribute(Type delegateType)
        {
            DelegateType = delegateType;
        }
    }
}
