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

    ## Edit or Delete specified Row
       # use asp-route-Key="@obj.Id" to get the Id of specified 
       # when Edit certain row of list, we need use web form to collect field value
       
       
       # for field which is not needed to be modified, but we still need pass it
       # from the web from to the server, 
       # for example, in this case, the Id of each row is not to modify,
       we can use <input asp-for="Id" hidden />  to hide the field
           take a look at Edit.cshtml

           without doing this, the Id value cannot be passed back to Server

      # basic EF add,delete, update operation

    ## add frontawesome
      # download and import Fontawesome library
      # Add    <link rel="stylesheet" href="~/fontawesome/css/all.css" /> to _layout.cshtml
      # go to Fontawesome website and copy the tag you desire to the position
         <i class="fas fa-plus"></i>  &nbsp;   Create New Category
    
  ##  add attribute to model, such as [Required] will cause Database change
      should update-database to apply this change
  ##  For a new model class.  only after adding it into the dbContext, the migration will aware its existance


  ##  ViewBag vs ViewData  vs TempData

  ##  Strong type view vs Loosely type view
      # view model  vs model
      # Try to use strongly typed view other than loosely typed view
  ##  Use third library https://sweetalert2.github.io/#examples to display alert information
