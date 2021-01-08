using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IObjectMappingRepository
    /// </summary>
    public interface IObjectMappingRepository : IRepository<ObjectMappingEntity>
    {
        /// <summary>
        /// Gets the object mappings by owner identifier.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns>IQueryable{ObjectMapping}.</returns>
        IQueryable<ObjectMappingEntity> GetObjectMappingsByOwnerId(int ownerId);

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
        /// Finds the object identifier by mapping.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromIds">From ids.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>System.Int32.</returns>
        int FindObjectIdByMapping(int ownerId, int fromTableTypeId, List<int> fromIds,
                                  int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0);

        /// <summary>
        /// Finds from identifier by mapping.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="toId">To identifier.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>System.Int32.</returns>
        int FindFromIdByMapping(int ownerId, int fromTableTypeId, int toId, int toTableTypeId = (int)TableTypes.NotSet,
                                int relationTypeId = 0);

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
        /// Gets the object mapping.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromId">From identifier.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="toId">To identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>ObjectMapping</returns>
        ObjectMappingEntity GetObjectMapping(int ownerId, int fromTableTypeId, int fromId, int toTableTypeId, int toId, int relationTypeId);

        /// <summary>
        /// Gets the object mappings by relation type identifier.
        /// </summary>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>List of ObjectMapping</returns>
        List<ObjectMappingEntity> GetObjectMappingsByRelationTypeId(int relationTypeId);
        List<ObjectMappingEntity> GetObjectMappings(int ownerId, TableTypes fromTableTypeId, List<int> fromIds, TableTypes toTableTypeId = TableTypes.NotSet, int relationTypeId = 0);
    }
}
