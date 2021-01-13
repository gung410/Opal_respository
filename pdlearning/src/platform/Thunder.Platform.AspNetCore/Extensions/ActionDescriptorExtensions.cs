using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Platform.AspNetCore.Extensions
{
    public static class ActionDescriptorExtensions
    {
        public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
        {
            if (!actionDescriptor.IsControllerAction())
            {
                throw new GeneralException($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
            }

            return actionDescriptor as ControllerActionDescriptor;
        }

        public static MethodInfo GetMethodInfo([NotNull] this ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptor));
            }

            return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
        }

        public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor is ControllerActionDescriptor;
        }
    }
}