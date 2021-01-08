using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class ObjectMappingRepository.
    /// </summary>
    public class ObjectMappingRepository : RepositoryBase<ObjectMappingEntity>, IObjectMappingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMappingRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public ObjectMappingRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        #region IObjectMappingRepository Members

        /// <summary>
        /// Get Object mapping by owner id
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns>IQueryable{ObjectMapping}.</returns>
        public IQueryable<ObjectMappingEntity> GetObjectMappingsByOwnerId(int ownerId)
        {
            return GetAllAsNoTracking().Where(o => o.OwnerId == ownerId);
        }

        /// <summary>
        /// Find object mapping
        /// </summary>
        /// <param name="objectMappings">The object mappings.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromId">From identifier.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>List{ObjectMapping}.</returns>
        public List<ObjectMappingEntity> FindObjectByMapping(List<ObjectMappingEntity> objectMappings, int fromTableTypeId, int fromId = 0, int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0)
        {
            var listObjectMapping = (from obj in objectMappings
                                     where obj.FromTableTypeId == fromTableTypeId &&
                                          (obj.FromId == fromId || fromId == 0) &&
                                          (obj.ToTableTypeId == toTableTypeId || toTableTypeId == 0) &&
                                          (obj.RelationTypeId == relationTypeId || relationTypeId == 0)
                                     select obj).ToList();

            return listObjectMapping;
        }

        /// <summary>
        /// Find object id by mapping
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromIds">From ids.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>System.Int32.</returns>
        public int FindObjectIdByMapping(int ownerId, int fromTableTypeId, List<int> fromIds, int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0)
        {
            var objectMapping = (from obj in GetAll()
                                 where obj.FromTableTypeId == fromTableTypeId &&
                                       fromIds.Contains(obj.FromId) &&
                                       (obj.ToTableTypeId == toTableTypeId || toTableTypeId == 0) &&
                                       (obj.RelationTypeId == relationTypeId || relationTypeId == 0) &&
                                       obj.OwnerId == ownerId
                                 select obj).FirstOrDefault();

            if (objectMapping != null)
                return objectMapping.ToId;
            return 0;
        }

        /// <summary>
        /// Find all object mapping from an object
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="fromIds">From ids.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>System.Int32.</returns>
        public List<ObjectMappingEntity> FindObjectMappings(int ownerId, int fromTableTypeId, List<int> fromIds, int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0)
        {
            var objectMapping = (from obj in GetAll()
                                 where obj.FromTableTypeId == fromTableTypeId &&
                                       fromIds.Contains(obj.FromId) &&
                                       (obj.ToTableTypeId == toTableTypeId || toTableTypeId == 0) &&
                                       (obj.RelationTypeId == relationTypeId || relationTypeId == 0) &&
                                       obj.OwnerId == ownerId
                                 select obj);
            return objectMapping.ToList();
        }

        /// <summary>
        /// Find object id by mapping
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="fromTableTypeId">From table type identifier.</param>
        /// <param name="toId">To identifier.</param>
        /// <param name="toTableTypeId">To table type identifier.</param>
        /// <param name="relationTypeId">The relation type identifier.</param>
        /// <returns>System.Int32.</returns>
        public int FindFromIdByMapping(int ownerId, int fromTableTypeId, int toId, int toTableTypeId = (int)TableTypes.NotSet, int relationTypeId = 0)
        {
            var fromId = (from obj in GetAll()
                          where obj.FromTableTypeId == fromTableTypeId &&
                                (obj.ToId == toId || toId == 0) &&
                                (obj.ToTableTypeId == toTableTypeId || toTableTypeId == 0) &&
                                (obj.RelationTypeId == relationTypeId || relationTypeId == 0) &&
                                obj.OwnerId == ownerId
                          select obj.FromId).FirstOrDefault();
            return fromId;
        }

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
        public ObjectMappingEntity GetObjectMapping(int ownerId, int fromTableTypeId, int fromId, int toTableTypeId, int toId, int relationTypeId)
        {
            return GetAll().FirstOrDefault(t =>
                t.OwnerId == ownerId && t.FromTableTypeId == fromTableTypeId && t.FromId == fromId &&
                t.ToTableTypeId == toTableTypeId && t.ToId == toId && t.RelationTypeId == relationTypeId);
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
            return GetAll().Where(t => t.RelationTypeId == relationTypeId).ToList();
        }

        public List<ObjectMappingEntity> GetObjectMappings(int ownerId, TableTypes fromTableTypeId, List<int> fromIds, TableTypes toTableTypeId = TableTypes.NotSet, int relationTypeId = 0)
        {
            return GetAllAsNoTracking().Where(x => fromIds.Contains(x.FromId)
                    && x.FromTableTypeId == (short)fromTableTypeId
                    && x.ToTableTypeId == (short)toTableTypeId
                    && x.RelationTypeId == relationTypeId).ToList();
        }

        #endregion
    }
}