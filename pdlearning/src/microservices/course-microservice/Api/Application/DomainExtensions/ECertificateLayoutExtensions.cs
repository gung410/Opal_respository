using System.IO;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.DomainExtensions
{
    public static class ECertificateLayoutExtensions
    {
        public static string GetLayoutFilePath(this ECertificateLayout ecertificateLayout)
        {
            return Path.Combine(ECertificateConstant.ECertificateLayoutsPath, ecertificateLayout.LayoutFileName);
        }
    }
}