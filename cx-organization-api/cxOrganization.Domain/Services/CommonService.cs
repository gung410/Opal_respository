using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    /// <summary>
    /// Class CommonService
    /// </summary>
    public class CommonService : ICommonService
    {
        private readonly IObjectMappingRepository _objectMappingRepository;

        public CommonService(IObjectMappingRepository objectMappingRepository)
        {
            _objectMappingRepository = objectMappingRepository;
        }

        public ObjectMappingEntity AddObjectMapping(ObjectMappingEntity objMapping)
        {
            return _objectMappingRepository.Insert(objMapping);
        }

        /// <summary>
        /// Find object mapping
        /// </summary>
        /// <param name="objectMappings">The object mappings.</param>
        /// <param name="fromTableTypeId">From table type id.</param>
        /// <param name="fromId">From id.</param>
        /// <param name="toTableTypeId">To table type id.</param>
        /// <param name="relationTypeId">The relation type id.</param>
        /// <returns>List of ObjectMapping.</returns>
        public List<ObjectMappingEntity> FindObjectByMapping(List<ObjectMappingEntity> objectMappings,
            int fromTableTypeId, 
            int fromId = 0,
            int toTableTypeId = (int)TableTypes.NotSet, 
            int relationTypeId = 0)
        {
            return _objectMappingRepository.FindObjectByMapping(objectMappings,
                                                                fromTableTypeId, 
                                                                fromId, 
                                                                toTableTypeId,
                                                                relationTypeId);
        }

        public List<ObjectMappingEntity> FindObjectMappings(int ownerId, int fromTableTypeId, List<int> fromIds, int toTableTypeId = 0, int relationTypeId = 0)
        {
            var objectMappings = _objectMappingRepository.FindObjectMappings(ownerId, fromTableTypeId, fromIds, toTableTypeId, relationTypeId);

            return objectMappings;
        }

        /// <summary>
        /// Gets the mapped item.
        /// </summary>
        /// <param name="ownerId">The owner id.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <param name="id">The id.</param>
        /// <returns>ObjectMapping.</returns>
        public ObjectMappingEntity GetMappedItem(int ownerId, int tableType, int Id)
        {
            var objectmappings = GetObjectMappingsByOwnerId(ownerId);
            return objectmappings.FirstOrDefault(t => t.FromTableTypeId == tableType && t.FromId == Id);

        }
        /// <summary>
        /// Get Object mapping by owner id
        /// </summary>
        /// <returns>
        /// IQueryable of ObjectMapping.
        /// </returns>
        public List<ObjectMappingEntity> GetObjectMappingsByOwnerId(int ownerId)
        {
            return _objectMappingRepository.GetObjectMappingsByOwnerId(ownerId).ToList();
        }

        /// <summary>
        /// Gets the object mappings by relation type identifier.
        /// </summary>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>
        /// List of ObjectMapping
        /// </returns>
        public List<ObjectMappingEntity> GetObjectMappingsByRelationTypeId(int relationTypeId)
        {
            var objectMappings = _objectMappingRepository.GetObjectMappingsByRelationTypeId(relationTypeId);

            return objectMappings;
        }
    }
}
