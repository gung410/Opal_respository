using System;
using Microservice.Badge.Domain.Constants;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class CourseInfo
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string PdActivityType { get; set; }

        public string LearningMode { get; set; } = MetadataTagConstants.ELearningTagId;

        [BsonIgnore]
        public bool IsMicroLearning => PdActivityType == MetadataTagConstants.MicroLearningTagId;

        [BsonIgnore]
        public bool IsELearning => LearningMode == MetadataTagConstants.ELearningTagId;

        [BsonIgnore]
        public bool IsBlended => LearningMode == MetadataTagConstants.BlendedLearningTagId;

        [BsonIgnore]
        public bool IsFaceToFace => LearningMode == MetadataTagConstants.FaceToFaceTagId;
    }
}
