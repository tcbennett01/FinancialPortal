namespace FinancialPortal.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinancialPortal.Models.ApplicationDbContext context)
        {
            // Create role(s)
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            // Create user(s) if s/he do not exist
            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "tcbenett01@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "tcbennett01@gmail.com",
                    Email = "tcbennett01@gmail.com",
                    FirstName = "Tim",
                    LastName = "Bennett",
                    DisplayName = "Tim Bennett",
                    AdminUser = true
                }, "Abc*123!");
            }

            if (!context.Users.Any(u => u.Email == "jdoe@testemail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jdoe@testemail.com",
                    Email = "jdoe@testemail.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    DisplayName = "Jane Doe",
                    AdminUser = false
                }, "Abc*123!");
            }

            if (!context.Users.Any(u => u.Email == "bbob@testemail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "bbob@testemail.com",
                    Email = "bbob@testemail.com",
                    FirstName = "Billy",
                    LastName = "Bob",
                    DisplayName = "Billy Bob",
                    AdminUser = false
                }, "Abc*123!");
            }

            // Add user(s) to role
            var userAdmin = userManager.FindByEmail("tcbennett01@gmail.com").Id;
            userManager.AddToRole(userAdmin, "Admin");

            // Seed transaction category types - income/credits
            if (!context.Categories.Any(t => t.Name == "Salary"))
            {
                context.Categories.Add(new Categories { Name = "Salary" });
            }
            if (!context.Categories.Any(t => t.Name == "Bonus"))
            {
                context.Categories.Add(new Categories { Name = "Bonus" });
            }
            if (!context.Categories.Any(t => t.Name == "Deposit"))
            {
                context.Categories.Add(new Categories { Name = "Deposit" });
            }
            if (!context.Categories.Any(t => t.Name == "Interest/Dividend"))
            {
                context.Categories.Add(new Categories { Name = "Interest/Dividend" });
            }
            if (!context.Categories.Any(t => t.Name == "Investments"))
            {
                context.Categories.Add(new Categories { Name = "Investments" });
            }
            if (!context.Categories.Any(t => t.Name == "Other-Income"))
            {
                context.Categories.Add(new Categories { Name = "Other-Income" });
            }

            // Category types - expenses/debits
            if (!context.Categories.Any(t => t.Name == "Automobile"))
            {
                context.Categories.Add(new Categories { Name = "Automobile" });
            }
            if (!context.Categories.Any(t => t.Name == "Childcare"))
            {
                context.Categories.Add(new Categories { Name = "Childcare" });
            }
            if (!context.Categories.Any(t => t.Name == "Clothing"))
            {
                context.Categories.Add(new Categories { Name = "Clothing" });
            }
            if (!context.Categories.Any(t => t.Name == "Dining"))
            {
                context.Categories.Add(new Categories { Name = "Dining" });
            }
            if (!context.Categories.Any(t => t.Name == "Education"))
            {
                context.Categories.Add(new Categories { Name = "Education" });
            }
            if (!context.Categories.Any(t => t.Name == "Entertainment"))
            {
                context.Categories.Add(new Categories { Name = "Entertainment" });
            }
            if (!context.Categories.Any(t => t.Name == "Grocery"))
            {
                context.Categories.Add(new Categories { Name = "Grocery" });
            }
            if (!context.Categories.Any(t => t.Name == "Insurance"))
            {
                context.Categories.Add(new Categories { Name = "Insurance" });
            }
            if (!context.Categories.Any(t => t.Name == "Mortgage"))
            {
                context.Categories.Add(new Categories { Name = "Mortgage" });
            }
            if (!context.Categories.Any(t => t.Name == "Rent"))
            {
                context.Categories.Add(new Categories { Name = "Rent" });
            }
            if (!context.Categories.Any(t => t.Name == "Taxes"))
            {
                context.Categories.Add(new Categories { Name = "Taxes" });
            }
            if (!context.Categories.Any(t => t.Name == "Travel"))
            {
                context.Categories.Add(new Categories { Name = "Travel" });
            }
            if (!context.Categories.Any(t => t.Name == "Utilities"))
            {
                context.Categories.Add(new Categories { Name = "Utilities" });
            }
            if (!context.Categories.Any(t => t.Name == "Withdrawal"))
            {
                context.Categories.Add(new Categories { Name = "Withdrawal" });
            }
            if (!context.Categories.Any(t => t.Name == "Other-Expense"))
            {
                context.Categories.Add(new Categories { Name = "Other-Expense" });
            }

            // Seed transaction type
            if (!context.TransactionTypes.Any(t => t.Name == "Credit"))
            {
                context.TransactionTypes.Add(new TransactionTypes { Name = "Credit" });
            }
            if (!context.TransactionTypes.Any(t => t.Name == "Debit"))
            {
                context.TransactionTypes.Add(new TransactionTypes { Name = "Debit" });
            }
        }
    }
}
