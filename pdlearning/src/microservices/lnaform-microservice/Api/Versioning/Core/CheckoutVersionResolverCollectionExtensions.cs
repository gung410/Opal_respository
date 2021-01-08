using System.Collections.Generic;
using System.Linq;
using Microservice.LnaForm.Versioning.Entities;

namespace Microservice.LnaForm.Versioning.Core
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
