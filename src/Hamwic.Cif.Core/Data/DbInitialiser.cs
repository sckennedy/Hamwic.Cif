using Hamwic.Cif.Core.Constants;
using Hamwic.Cif.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hamwic.Cif.Core.Data
{
    public static class DbInitialiser
    {
        public static async Task Initialise(ApplicationDbContext context, IServiceProvider serviceProvider, string password)
        {
            context.Database.EnsureCreated();

            AddSchools(context);

            var adminId = await EnsureUser(serviceProvider, password, "Administrator", "admin@hamwic.org");
            await EnsureRole(serviceProvider, adminId, RoleNames.SystemAdministrator);

            var hamwicId = await EnsureUser(serviceProvider, password, "Hamwic User", "hamwic@hamwic.org");
            await EnsureRole(serviceProvider, hamwicId, RoleNames.HamwicUser);

            var execHeadId = await EnsureUser(serviceProvider, password, "Exec Head", "exec@hamwic.org");
            await EnsureRole(serviceProvider, execHeadId, RoleNames.ExecutiveHeadTeacher);

            var headId = await EnsureUser(serviceProvider, password, "Head Teacher", "head@hamwic.org");
            await EnsureRole(serviceProvider, headId, RoleNames.HeadTeacher);

            var userId = await EnsureUser(serviceProvider, password, "School User", "school@hamwic.org");
            await EnsureRole(serviceProvider, userId, RoleNames.SchoolUser);

            context.SaveChanges();
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string userName, string emailAddress)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userName, Email = emailAddress};
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            IdentityResult ir = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }


        /// <summary>
        /// Method to populate some initial school information
        /// </summary>
        /// <param name="context"></param>
        private static void AddSchools(ApplicationDbContext context)
        {
            var school1 = new School
            {
                Name = "Weston"
            };

            var school2 = new School
            {
                Name = "Bitterne Park"
            };

            var school3 = new School
            {
                Name = "Shirley High"
            };

            var schools = context.Schools;

            var school = schools.FirstOrDefault(x => x.Name == school1.Name);
            if (school == null)
                context.Add(school1);

            school = schools.FirstOrDefault(x => x.Name == school2.Name);
            if (school == null)
                context.Add(school2);

            school = schools.FirstOrDefault(x => x.Name == school3.Name);
            if (school == null)
                context.Add(school3);

            context.SaveChanges();
        }
    }
}