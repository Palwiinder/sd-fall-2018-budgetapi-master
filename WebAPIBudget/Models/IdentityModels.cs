using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebAPIBudget.Models.Domain;

namespace WebAPIBudget.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty(nameof(Household.Owner))]
        public virtual List<Household> OwnedHouseholds { get; set; }

        [InverseProperty(nameof(Household.Members))]
        public virtual List<Household> JoinedHouseholds { get; set; }

        [InverseProperty(nameof(Household.InvitedUsers))]
        public virtual List<Household> InvitedHouseholds { get; set; }

        public ApplicationUser()
        {
            OwnedHouseholds = new List<Household>();
            JoinedHouseholds = new List<Household>();
            InvitedHouseholds = new List<Household>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Household> Households { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ApplicationUser>()
                .HasMany(p => p.JoinedHouseholds)
                .WithMany(p => p.Members)
                .Map(p => p.ToTable("HouseholdMembers"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.InvitedHouseholds)
                .WithMany(p => p.InvitedUsers)
                .Map(p => p.ToTable("HouseholdInvitedUsers"));

            modelBuilder
                .Entity<Household>()
                .HasMany(p => p.Categories)
                .WithRequired(p => p.Household)
                .WillCascadeOnDelete(false);


        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}