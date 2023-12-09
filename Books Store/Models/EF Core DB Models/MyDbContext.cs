using Books_Store.Models.AuthorModels;
using Books_Store.Models.UserModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Security.Principal;
using System.Xml;
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4

namespace Books_Store.Models.EF_Core_DB_Models
{

    //Your application DbContext class must inherit from IdentityDbContext class instead of DbContext class.
    //This is required because IdentityDbContext provides all the DbSet properties needed to manage the
    //identity tables in SQL Server.If you go through the hierarchy chain of IdentityDbContext class,
    //you will see it inherits from DbContext class. So this is the reason you do not have to explicitly
    //inherit from DbContext class if your class is inheriting from IdentityDbContext class.


    //public class MyDbContext : DbContext
    public class MyDbContext : IdentityDbContext<ApplicationUser> /*IdentityDbContext<IdentityUser>*/
    //Specify ApplicationUser class as the generic argument for the IdentityDbContext class
    //This is how the IdentityDbContext class knows it has to work with our custom user class
    //(in this case ApplicationUser class) instead of the default built-in IdentityUser class.
    //The data in the ApplicationUser instance is then saved to the AspNetUsers table by
    //the IdentityDbContext class.
    {






        //DbContext class needs an instance of the DbContextOptions class.
        //The DbContextOptions instance carries configuration information such as the connection string, database provider to use etc.
        //To pass the DbContextOptions instance we pass it to the DbContext constructor by using base().
<<<<<<< HEAD
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
=======
        public MyDbContext(DbContextOptions<MyDbContext> options): base(options)
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4
        {

        }







        //Configure our models to  the DbSet class to map them as domain classes
        public DbSet<Author> Authors { get; set; }


<<<<<<< HEAD

=======
    
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //seed data to specific entity by override OnModelCreating() method
            //seed by use HasData method directly
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    Id = 1,
                    Name = "Mark",
                    Department = Dept.Intermediate,
                    Email = "mark@pragimtech.com"
                });

            //seed by use HasData method from extention Seed method from sperate class
            modelBuilder.Seed();



            //Error: The entity type 'IdentityUserLogin<string>' requires a primary key to be defined
            //If you get this error, the most likely cause is that you are overriding OnModelCreating() method
            //in your application DbContext class but not calling the OnModelCreating() method of
            //base IdentityDbContext class
            //OnModelCreating() method, the erro=> Keys of Identity tables are mapped in OnModelCreating
            //method of IdentityDbContext class. So, to fix this error, all you need to do is,
            //call the base class OnModelCreating() method using the base keyword as shown below.
            base.OnModelCreating(modelBuilder);








            //Users are stored in AspNetUsers table
            //Roles are stored in AspNetRoles table
            //User and Role mapping data is stored in AspNetUserRoles table
            //This table has just 2 columns: UserId & RoleId
            //Both are foreign keys

            //Cascading referential integrity constraint:-
            //Cascading referential integrity constraint allows to define the actions Microsoft SQL Server should take
            //when a user attempts to delete or update a key to which an existing foreign keys points.

            //We discussed foreign keys and cascading referential integrity constraint in detail in Part 5 of SQL Server tutorial.

            //Foreign key with Cascade DELETE:-
            //In Entity Framework Core, by default the foreign keys in AspNetUserRoles table have Cascade DELETE behaviour.
            //This means, if a record in the parent table(AspNetRoles) is deleted, then the corresponding records in
            //the child table(AspNetUserRoles) are automatically be deleted.

            //Foreign key with NO ACTION ON DELETE:-
            //What if you want to customise this default behaviour.We do not want to allow a role to be deleted, if
            //there are rows in the child table(AspNetUserRoles) which point to a role in the parent table(AspNetRoles).

            //To achieve this, modify foreign keys DeleteBehavior to Restrict. We do this in OnModelCreating() method
            //of AppDbContext class:

<<<<<<< HEAD
            IEnumerable<IMutableForeignKey> ForeignKeys =
=======
            IEnumerable<IMutableForeignKey> ForeignKeys = 
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4
                modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());

            foreach (IMutableForeignKey FK in ForeignKeys)
            {
                FK.DeleteBehavior = DeleteBehavior.Restrict;
            }

<<<<<<< HEAD
            //ALTER TABLE [AspNetUserRoles] ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION;
            //ALTER TABLE [AspNetUserRoles] ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

            //Now if ypu try delete role has users or user that inside a role => 
            //SqlException: The DELETE statement conflicted with the REFERENCE constraint
            //"FK_AspNetUserRoles_AspNetUsers_UserId".The conflict occurred in database "BookStoreDB",
            //table "dbo.AspNetUserRoles", column 'UserId'.
=======
        //ALTER TABLE [AspNetUserRoles] ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION;
        //ALTER TABLE [AspNetUserRoles] ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

        //Now if ypu try delete role has users or user that inside a role => 
        //SqlException: The DELETE statement conflicted with the REFERENCE constraint
        //"FK_AspNetUserRoles_AspNetUsers_UserId".The conflict occurred in database "BookStoreDB",
        //table "dbo.AspNetUserRoles", column 'UserId'.
>>>>>>> 38aec3c05e6ff1c9759294c787b8a0d08a70b7d4






        }






    }
}
