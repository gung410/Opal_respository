using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Constants
{
    public static class ECertificateConstant
    {
        public static readonly string ECertificateLayoutsPath =
            Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "ECertificateLayouts");

        public static readonly string ECertificateLayoutPreviewImagesPath =
            Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "ECertificateLayouts", "PreviewImages");

        public static readonly string FullNameDescription = "Full Name of Learner";
        public static readonly string CourseNameDescription = "Course name";
        public static readonly string PrincipalDescription = "Issuer's name";
        public static readonly string CompletedDateDescription = "Completed date";
        public static readonly string CompletedDateTextDescription = "Completed date text";
        public static readonly string LayoutDescription = "Layout description";
        public static readonly string LayoutDescription2 = "Layout description 2";
        public static readonly string PrincipalSignatureDescription = "Issuer's signature";
        public static readonly string LogoDescription = "Logo";
        public static readonly string Logo2Description = "Logo2";
        public static readonly string Logo3Description = "Logo3";
        public static readonly string BackgroundDescription = "Background";
    }

    public static class ECertificateLayoutConstant
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Duc Dang confirmed this")]
        public static readonly List<ECertificateLayout> AllECertificateLayoutsInSystem = new()
        {
            #region System
            new()
#pragma warning restore SA1124 // Do not use regions
            {
                Id = new Guid("0fe84ac9-cf78-4173-8aab-99dc9cd206d9"),
                Name = "Layout 1 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_1.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    }
                },
                IsSystem = true
            },
            new()
            {
                Id = new Guid("a169f864-0190-49ce-b427-501600d246f9"),
                Name = "Layout 2 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_2.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    }
                },
                IsSystem = true
            },
            new()
            {
                Id = new Guid("960d8dbc-5fb8-48b5-b528-3df57ca21a23"),
                Name = "Layout 3 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_3.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    }
                },
                IsSystem = true
            },
            new()
            {
                Id = new Guid("4829e987-d26e-4580-ad5e-5c1b0865a9e8"),
                Name = "Layout 4 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_4.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    }
                },
                IsSystem = true
            },
            #endregion

            #region Landscape
            new()
            {
                Id = new Guid("6687c3d4-0d9f-4457-9069-a84ba7c20e86"),
                Name = "Layout 5 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_5_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_5.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("922b76f4-85e2-4798-a7ac-828b0d2482a8"),
                Name = "Layout 6 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_6_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_6.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("19065889-bb08-49f4-939f-593469197a5f"),
                Name = "Layout 7 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_7_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_7.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("cb5f03a2-65ea-466a-908f-a3b68a9dc17f"),
                Name = "Layout 8 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_8_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_8.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("435bb6da-3fd4-412c-b36b-50989b680588"),
                Name = "Layout 9 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_9_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_9.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("de2b5bfd-9c36-45cf-bd98-85be7e2a20af"),
                Name = "Layout 10 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_10_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_10.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("70eb9068-eec7-418a-bb24-291c90b398e7"),
                Name = "Layout 11 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_11_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_11.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("55855fc0-8ef9-4871-ace8-3a4e9803bd33"),
                Name = "Layout 12 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_12_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_12.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("041e47e9-5b00-4f76-9eed-3877f57f8b56"),
                Name = "Layout 13 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_13_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_13.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("0740f3da-e01c-4cf0-90a6-76f910a7517a"),
                Name = "Layout 14 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_14_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_14.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("9246178f-2d9a-4483-9876-bdb34e2825aa"),
                Name = "Layout 15 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_15_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_15.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo3,
                        Title = "Logo 3",
                        Description = ECertificateConstant.Logo3Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("71f8df1f-78af-4ee8-9465-b1b752f36ff1"),
                Name = "Layout 16 (landscape - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_16_landscape.trdp",
                Description = "This layout is suitable for dimensions as 1056px x 816px",
                Width = 1056,
                Height = 816,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_16.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            #endregion

            #region Portrait
            new()
            {
                Id = new Guid("eba296cb-7de3-4af3-b65c-aecb8d83b0a1"),
                Name = "Layout 5 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_5_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_5.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("a2b715a1-480e-4d9a-b74c-b9faf6fdc134"),
                Name = "Layout 6 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_6_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_6.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("20a7154d-3213-4158-b579-7c1430095c37"),
                Name = "Layout 7 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_7_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_7.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("b906e0f1-93ce-49e6-ad00-36a37b84b50a"),
                Name = "Layout 8 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_8_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_8.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("fcd172d3-3b9a-490a-a020-e0e1f4266677"),
                Name = "Layout 9 (portrait - 1056 x 816)",
                LayoutFileName = "ecertificate_layout_9_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_9.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("4487dd3b-9f92-4b3a-8ca1-e772ce68a5b5"),
                Name = "Layout 10 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_10_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_10.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("46326950-b405-4674-b3db-9e7356148035"),
                Name = "Layout 11 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_11_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_11.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("2a47f7c2-1d4c-4a35-bb9b-168cde721f62"),
                Name = "Layout 12 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_12_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_12.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("2da8c27b-419f-470e-bf49-9e86a09265f7"),
                Name = "Layout 13 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_13_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_13.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("da144364-094e-4b14-a47d-d1237460d6e6"),
                Name = "Layout 14 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_14_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_14.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description2,
                        Title = "Description 2",
                        Description = ECertificateConstant.LayoutDescription2,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("a82c01a9-5d82-4478-8b9f-ad0afa62b292"),
                Name = "Layout 15 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_15_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_15.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo3,
                        Title = "Logo 3",
                        Description = ECertificateConstant.Logo3Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            },
            new()
            {
                Id = new Guid("0709880f-ef75-4b95-a520-d8da21b9cdd0"),
                Name = "Layout 16 (portrait - 816 x 1056)",
                LayoutFileName = "ecertificate_layout_16_portrait.trdp",
                Description = "This layout is suitable for dimensions as 816px x 1056px",
                Width = 816,
                Height = 1056,
                PreviewImagePath = Path.Combine(ECertificateConstant.ECertificateLayoutPreviewImagesPath, "ecertificate_layout_16.jpg"),
                Params = new List<ECertificateLayoutParam>
                {
                    new()
                    {
                        Key = ECertificateSupportedField.Background,
                        Title = "Background",
                        Description = ECertificateConstant.BackgroundDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo,
                        Title = "Logo",
                        Description = ECertificateConstant.LogoDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Logo2,
                        Title = "Logo 2",
                        Description = ECertificateConstant.Logo2Description,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Description,
                        Title = "Description",
                        Description = ECertificateConstant.LayoutDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CourseName,
                        Title = "Course Name",
                        Description = ECertificateConstant.CourseNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.FullName,
                        Title = "Full Name of Learner",
                        Description = ECertificateConstant.FullNameDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.PrincipalSignature,
                        Title = "Issuer's Signature",
                        Description = ECertificateConstant.PrincipalSignatureDescription,
                        Type = ECertificateParamType.Image,
                        IsAutoPopulated = false
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.Principal,
                        Title = "Issuer's Name",
                        Description = ECertificateConstant.PrincipalDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDate,
                        Title = "Certification Date",
                        Description = ECertificateConstant.CompletedDateDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = true
                    },
                    new()
                    {
                        Key = ECertificateSupportedField.CompletedDateText,
                        Title = "Text of Certification Date",
                        Description = ECertificateConstant.CompletedDateTextDescription,
                        Type = ECertificateParamType.Text,
                        IsAutoPopulated = false
                    }
                },
                IsSystem = false
            }
            #endregion
        };
    }
}
