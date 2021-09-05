##  create Category model
    0, Add connection string to "appsettings.json"
    1, create Category class   (Pay attention to Attribute, such as [Key])
    2, create ApplicationDbContext class (Install EF package and inherit DbContext base class)
    3, DI:  Setup Startup.cs, inject DBcontext into Service container
    4, Using PM to do the migration.   a, add-migration, b, update-database  (Maybe need install EF TOOL package)
       Not very sure how to tune those fields....

    ## Add new record to Database
      # Use @model to bind model with razor view
      # Use asp-helper to bind model-properties to each INPUT tag(provide Path, loose coupling)
      # Use Attributes[HttpPost, DisplayName] on Model or Action to specify different intent
      # EF operations -- saveChanges
      # Once insertion is over, redirect to Index action to review the result of insertion
    
    ## Validation-Server-side
      # This is Server-side validation !!!  The error will be found by the Server, then inform the client
      # Add validation attribute to Model class
      # Add validation error information to View
      # Use validate in Action and control the data flow !!!
      # If no Error, insertion will happen without any tip. then return a Index view
      # If server find Error, it will return data to create VIEW (at this moment, Error information display) 
    
    ## Validation-Client-side
      # Add "_ValidationScriptsPartial.cshtml" to the view.
      # using the following format:(imbedded js file to view)

        @section Scripts{ 
            @{<partial name="_ValidationScriptsPartial" />}
        }