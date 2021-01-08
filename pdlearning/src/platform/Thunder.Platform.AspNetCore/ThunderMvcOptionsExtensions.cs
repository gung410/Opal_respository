using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.AspNetCore.ExceptionHandling;
using Thunder.Platform.AspNetCore.ModelBinding;
using Thunder.Platform.AspNetCore.UnitOfWork;
using Thunder.Platform.AspNetCore.Validation;
using Thunder.Platform.Core.Json;

namespace Thunder.Platform.AspNetCore
{
    public static class ThunderMvcOptionsExtensions
    {
        public static void AddThunderMvcOptions([NotNull] this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            AddActionFilters(options);
            AddModelBinders(options);
        }

        public static IMvcBuilder AddThunderJsonOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.IgnoreNullValues = true;

                options.JsonSerializerOptions.Converters.Add(new ThunderDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new ThunderNullableDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return builder;
        }

        /// <summary>
        /// Use filter or middleware.
        /// </summary>
        /// <param name="options">The MvcOptions.</param>
        public static void AddUowActionFilter([NotNull] this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Filters.AddService(typeof(ThunderUowActionFilter));
        }

        private static void AddActionFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(ThunderExceptionFilter));
            options.Filters.AddService(typeof(ThunderModelValidationFilter));
        }

        private static void AddModelBinders(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new ThunderDateTimeModelBinderProvider());
        }
    }
}
