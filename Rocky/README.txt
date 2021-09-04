##  create Category model
    0, Add connection string to "appsettings.json"
    1, create Category class   (Pay attention to Attribute, such as [Key])
    2, create ApplicationDbContext class (Install EF package and inherit DbContext base class)
    3, DI:  Setup Startup.cs, inject DBcontext into Service container
    4, Using PM to do the migration.   a, add-migration, b, update-database  (Maybe need install EF TOOL package)
       Not very sure how to tune those fields....
