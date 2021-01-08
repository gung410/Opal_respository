using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    public interface IFileInfoRepository : IRepository<FileInfoEntity>
    {
        public IQueryable<FileInfoEntity> GetFileInfosAsNoTracking();

        public IQueryable<FileInfoEntity> GetFileInfos();

        public Task<FileInfoEntity> GetFileInfoByIdAsync(int fileInfoId);

        public Task<FileInfoEntity> CreateFileInfoAsync(FileInfoEntity fileInfoEntity);

        public IQueryable<FileInfoEntity> GetFileInfosByUserIdAsync(Guid userGuid);
    }
}
