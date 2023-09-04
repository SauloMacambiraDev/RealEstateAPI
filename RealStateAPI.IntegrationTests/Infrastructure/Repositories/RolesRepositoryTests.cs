using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateAPI.IntegrationTests.Infrastructure.Repositories
{
    public class RolesRepositoryTests
    {
        private RolesRepository<Role> rolesRepositry;

        [SetUp]
        public void SetUp()
        {
            rolesRepositry = new RolesRepository<Role>(new RealEStateDbContext());
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task GetRoleByIdWithUsers_WithExistingIdAndUsers_ReturnsRoleWithUsers(int roleId)
        {
            // Act
            var result = await rolesRepositry.GetRoleByIdWithUsers(roleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Users.Count, Is.AtLeast(1));
        }

        [Test]
        public async Task GetRoleByIdWithUsers_WithExistingIdAndUsers_ReturnsRoleWithNoUsers()
        {
            // Arrange
            int roleId = 3;

            // Act
            var result = await rolesRepositry.GetRoleByIdWithUsers(roleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Users.Count, Is.EqualTo(0));
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(25)]
        [TestCase(2343232)]
        public async Task GetRoleByIdWithUsers_WithoutExistingId_ReturnsNull(int roleId)
        {
            var result = await rolesRepositry.GetRoleByIdWithUsers(roleId);

            Assert.IsNull(result);
        }

    }
}
