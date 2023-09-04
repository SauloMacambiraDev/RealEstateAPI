using Microsoft.Identity.Client;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Interfaces
{
    public interface IRolesRepository<T> : IRepository<T> where T: Role
    {
        Task<Role> GetRoleByIdWithUsers(int id);
    }
}
