using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace cxOrganization.WebServiceAPI.AppStart
{
   
    public class RemoveEmptyCollectionQueryStringValueProviderFactory : IValueProviderFactory
    {

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var actionParams = context?.ActionContext?.ActionDescriptor?.Parameters;
            if (actionParams != null)
            {
                var collectionQueryParams = context.ActionContext.ActionDescriptor.Parameters.Where(IsCollectionQueryParameter).ToList();
                if (collectionQueryParams.Count > 0)
                {
                    var queryCollection = context.ActionContext.HttpContext.Request.Query;
                    if (queryCollection != null && queryCollection.Count > 0)
                    {
                        var modifiedQuery = new Dictionary<string, StringValues>();
                        var hasModified = false;
                        foreach (var query in queryCollection)
                        {
                            var isCollectionQuery = collectionQueryParams.Any(p =>
                                string.Equals(query.Key, p.Name, StringComparison.CurrentCultureIgnoreCase));

                            //if query parameter is an collection type but has no value we skip handling to avoid error when parsing data to action argument
                            if (isCollectionQuery && IsNullOrEmpty(query.Value))
                            {
                                hasModified = true;
                                continue;
                            }

                            modifiedQuery.Add(query.Key, query.Value);
                        }

                        if (hasModified)
                        {
                            context.ActionContext.HttpContext.Request.QueryString = QueryString.Create(modifiedQuery);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool IsNullOrEmpty(StringValues values)
        {
            if (StringValues.IsNullOrEmpty(values))
                return true;

            if (values.All(string.IsNullOrEmpty))
                return true;
            return false;
        }

        private bool IsCollectionQueryParameter(ParameterDescriptor parameter)
        {
            return parameter.BindingInfo.BindingSource.IsFromRequest && parameter.BindingInfo.BindingSource.Id == "Query"
                                                                     && IsCollectionType(parameter.ParameterType);
        }

        private bool IsCollectionType(Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }
    }
  

}
