using System.Collections.Generic;
using System.Linq;
using Microservice.Content.Versioning.Entities;

namespace Microservice.Content.Versioning.Core
{
    public static class CheckoutVersionResolverCollectionExtensions
    {
        public static ICheckoutVersionResolver GetCheckoutVersionResolver(
            this IEnumerable<ICheckoutVersionResolver> checkoutVersionResolvers,
            VersionSchemaType type)
        {
            return checkoutVersionResolvers.First(x => x.CanResolveSchemaType(type));
        }
    }
}