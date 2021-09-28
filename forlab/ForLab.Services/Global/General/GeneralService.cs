using ForLab.Data.Enums;
using ForLab.Repositories.Security.UserRole;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ForLab.Services.Global.General
{
    public class GeneralService : IGeneralService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        public GeneralService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public List<int> SuperAdminIds()
        {
            var userIds = _userRoleRepository.GetAll(x => x.RoleId == (int)ApplicationRolesEnum.SuperAdmin).Select(x => x.UserId).ToList();
            return userIds;
        }
    }
}
