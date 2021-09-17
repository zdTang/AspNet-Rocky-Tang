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

  ##  Add summernote library -> multi-ui-editor
      1. import library ( css and js )
      2. copy sample code from official and insert the the cshtml we will insert the summernote
      3. the original <input> will change to <textarea> 

  ## var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
     AsNoTracking()  ---> EF don't allow track the same entity two times
     see Controller(production--upsert)

  ## Add "MultipleActiveResultSets=True" into the connection string
     see Controller(production--index)
     reference: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/enabling-multiple-active-result-sets

  ## Understand View Model
  ## Create navigator: Drop-down list

  ## use Category items to create a bunch of buttons to work as Filter.
  ## use WC static class to contain global parameters such as Directory path
  ## Create two session extension method GET, SET
  ## Add session
        // Dependency Injection
            services.AddHttpContextAccessor();
            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

        // Add middleware

        app.UseSession();

  ## User registeration
  #  for DbContext, switch DbContext to IdentityDbContext

  ## Add userName field to user registeration page
  add ApplicationUser tabel

  # Admin key: Temp123*


  # add shopping Cart
  # use Session to Add or Remove item from shopping cart
  # even user has not registered, he can still use shopping cart
  # [authorize] will force user to login when using certain Controller


  # mailjet   -- send email
  ## By now, the lastest version mailjetapi doesn't work  use old version as in tutorials ---1.2.2 works
  # need DI 

  # Proton Mail -- works not good
  whatever send from Google or Proton Email address.   the file we send can be found in the JUNK folder !!!
  if using Proton Email address,   the ClickME will be deleted so that we cannot confirm the account

  #  we can report these email not a spam.

  ## Move MailJet API keys to appsettings.json   and read it using configuration class


  ## Move dataAccess, data, Utility out of the main project
  #  Need to re-build the relationship and dependency



  # Add new version Bootstrap and Toastr