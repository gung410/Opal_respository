using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Conexus.Opal.AccessControl.Services
{
    public static class UserSaver
    {
        public static async Task SaveUser(SaveUserRequestDto request, IRepository<UserEntity> userRepo)
        {
            var user = await userRepo.FirstOrDefaultAsync(u => u.OriginalUserId == request.Identity.Id && u.Id == request.Identity.ExtId);
            if (user == null)
            {
                user = new UserEntity()
                {
                    Id = request.Identity.ExtId,
                    OriginalUserId = request.Identity.Id,
                    DepartmentId = request.DepartmentId,
                    Email = request.EmailAddress,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };
                await userRepo.InsertAsync(user);
            }
            else
            {
                user.DepartmentId = request.DepartmentId;
                user.Email = request.EmailAddress;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                await userRepo.UpdateAsync(user);
            }
        }
    }
}
