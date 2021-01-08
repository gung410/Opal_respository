using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    /// <summary>
    /// Interface ICommonService
    /// </summary>
    public interface ICommonService
    {
        List<ObjectMappingEntity> GetObjectMappingsByOwnerId(int ownerId);
        /// <summary>
        /// Gets the object mappings by relation type identifier.
        /// </summary>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>List of ObjectMapping</returns>
        List<ObjectMappingEntity> GetObjectMappingsByRelationTypeId(int relationTypeId);
        /// <summary>
        /// Finds the object by mapping.
        /// </summary>
        /// <param name="objectMappings">The object mappings.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromId">From identifier.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>List{ObjectMapping}.</returns>
        List<ObjectMappingEntity> FindObjectByMapping(List<ObjectMappingEntity> objectMappings, int fromTableTypeId, int fromId = 0,
                                                int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0);

        /// <summary>
        /// Find all object mapping from an object
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="fromTableTypeId"></param>
        /// <param name="fromIds"></param>
        /// <param name="toTableTypeId"></param>
        /// <param name="relationTypeId"></param>
        /// <returns></returns>
        List<ObjectMappingEntity> FindObjectMappings(int ownerId, int fromTableTypeId, List<int> fromIds, int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0);


        /// <summary>
        /// Adds the object mapping.
        /// </summary>
        /// <param name="objMapping">The object mapping.</param>
        /// <returns>
        /// ObjectMapping
        /// </returns>
        ObjectMappingEntity AddObjectMapping(ObjectMappingEntity objMapping);
    }
}
