using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using SpeedoModels.Models;
using Microsoft.Ajax.Utilities;
using System.Runtime.Remoting.Contexts;
using System;

namespace SpeedoModels.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    /// <summary>
    /// getters and setters for the application user
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityUser" />

    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// gets a user identity
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>
        /// userIdentity
        /// </returns>


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    /// <summary>
    /// Initialises the ApplicationDbContext
    /// </summary>
    /// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext{SpeedoModels.Models.ApplicationUser}" />

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("test", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>
        /// returns a new applicationDbContexr
        /// </returns>

        public static ApplicationDbContext Create()
        {
            

            return new ApplicationDbContext();

        }

        /// <summary>
        /// gets and sets the databases
        /// </summary>

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public System.Data.Entity.DbSet<SpeedoModels.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<SpeedoModels.Models.CompleteOrders> CompleteOrders { get; set; }

        public System.Data.Entity.DbSet<SpeedoModels.Models.Refunds> Refunds { get; set; }

        public System.Data.Entity.DbSet<SpeedoModels.Models.OrderBackup> OrderBackups { get; set; }

        public System.Data.Entity.DbSet<SpeedoModels.Models.ProductBackup> ProductBackups { get; set; }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {

        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected override void Seed(ApplicationDbContext context)
        {



            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var userStore = new UserStore<ApplicationUser>(context);


                if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
                }
                string userName = "admin@admin.com";
                string password = "123456";


                var user = userManager.FindByName(userName);
                if (user == null)
                {
                    var newadmin = new ApplicationUser()
                    {

                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true,
                    };

                    userManager.Create(newadmin, password);
                    userManager.AddToRole(newadmin.Id, RoleNames.ROLE_ADMINISTRATOR);


                }

             
                if (!roleManager.RoleExists(RoleNames.ROLE_AssistantManager))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_AssistantManager));
                }

                string suserName = "amanager@amanager.com";
                    string spassword = "123456";

                    var staff = userManager.FindByName(suserName);
                    if (user == null)
                    {
                        var newstaff = new ApplicationUser()
                        {

                            UserName = suserName,
                            Email = suserName,
                            EmailConfirmed = true,
                        };

                        userManager.Create(newstaff, spassword);
                        userManager.AddToRole(newstaff.Id, RoleNames.ROLE_AssistantManager);

                    }



                if (!roleManager.RoleExists(RoleNames.ROLE_OrdersClerk))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_OrdersClerk));
                }

                string ocuserName = "OrdersClerk@OrdersClerk.com";
                    string ocpassword = "123456";

                    var OrdersClerk = userManager.FindByName(ocuserName);
                    if (user == null)
                    {
                        var newOrdersClerk = new ApplicationUser()
                        {

                            UserName = ocuserName,
                            Email = ocuserName,
                            EmailConfirmed = true,
                        };

                        userManager.Create(newOrdersClerk, ocpassword);
                        userManager.AddToRole(newOrdersClerk.Id, RoleNames.ROLE_OrdersClerk);

                    }


                if (!roleManager.RoleExists(RoleNames.ROLE_StoreManager))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_StoreManager));
                }

                string smuserName = "SManager@SManager.com";
                    string smpassword = "123456";

                    var StoreManager = userManager.FindByName(smuserName);
                    if (user == null)
                    {
                        var newStoreManager = new ApplicationUser()
                        {

                            UserName = smuserName,
                            Email = smuserName,
                            EmailConfirmed = true,
                        };

                        userManager.Create(newStoreManager, smpassword);
                        userManager.AddToRole(newStoreManager.Id, RoleNames.ROLE_StoreManager);

                    }

                

                base.Seed(context);
                context.SaveChanges();
            }
        }

        }
   }













        
        
    
