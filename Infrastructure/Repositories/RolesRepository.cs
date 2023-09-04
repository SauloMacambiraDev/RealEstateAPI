using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class RolesRepository<T> : Repository<T>, IRolesRepository<T> where T: Role
    {
        public RolesRepository(RealEStateDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Role> GetRoleByIdWithUsers(int id) => await dbSet.Where(r => r.Id == id)
                                                                            .Include(r => r.Users)
                                                                            .FirstOrDefaultAsync();


    }
}
