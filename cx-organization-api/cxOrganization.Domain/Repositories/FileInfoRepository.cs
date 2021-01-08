using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    public class FileInfoRepository : RepositoryBase<FileInfoEntity>, IFileInfoRepository
    {
        private readonly OrganizationDbContext _organizationDbContext;

        public FileInfoRepository(OrganizationDbContext dbContext) : base(dbContext)
        {
            _organizationDbContext = dbContext;
        }

        public IQueryable<FileInfoEntity> GetFileInfos()
        {
            return GetAll().Where(x => x.Deleted == null);
        }

        public IQueryable<FileInfoEntity> GetFileInfosAsNoTracking()
        {
            return GetAllAsNoTracking().Where(x => x.Deleted == null);
        }

        public async Task<List<FileInfoEntity>> GetAllListFileInfos()
        {
            return await GetFileInfos().ToListAsync();
        }

        public async Task<FileInfoEntity> CreateFileInfoAsync(FileInfoEntity fileInfoEntity)
        {
            fileInfoEntity.CreatedDate = DateTime.UtcNow;

            var createdFileInfo = Insert(fileInfoEntity);

            await _organizationDbContext.SaveChangesAsync();

            return createdFileInfo;
        }

        public async Task<FileInfoEntity> GetFileInfoByIdAsync(int fileInfoId)
        {
            return await GetFileInfos().FirstOrDefaultAsync(x => x.FileInfoId == fileInfoId);
        }

        public IQueryable<FileInfoEntity> GetFileInfosByUserIdAsync(Guid userGuid)
        {
            return GetFileInfos().Where(x => x.UserGuid == userGuid);
        }
    }
}
