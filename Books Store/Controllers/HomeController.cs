using Books_Store.Models.AuthorModels;
using Books_Store.Models.EF_Core_DB_Models;
using Books_Store.Models.Pagination;
using Books_Store.Security.EncryptionAndDecryption;
using Books_Store.View_Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Books_Store.Controllers
{
    //public class HomeController
    //{
    //public JsonResult Index2()
    //{
    //    //return Json(null); //null 
    //    return Json("the name===", iAuthorRepository.GetAuthor(1).Name);//"Mary"
    //}

    //}


    //when an controler  hitted=> an instance from it will created 
    public class HomeController : Controller
    {
        private readonly IAuthorRepository iAuthorRepository;
        private readonly IWebHostEnvironment iWebHostEnvironment;
        private readonly IConfiguration iConfiguration;
        private readonly MyDbContext MyDbContext;
        // It is through IDataProtector interface Protect and Unprotect methods,
        // we encrypt and decrypt respectively
        private readonly IDataProtector IDataProtector;




        public HomeController(
            IAuthorRepository _iAuthorRepository,
            IWebHostEnvironment _iWebHostEnvironment,
            IConfiguration _iConfiguration,
            MyDbContext _MyDbContext, //also Di will automatically create the argument for specfic parameters
            IDataProtectionProvider IDataProtectionProvider,
            OurCustomDataProtectionPurposeStrings OurCustomDataProtectionPurposeStrings

            )

        {
            this.iAuthorRepository = _iAuthorRepository;//after we recived the argument from Di here we manually reassign it to our  private readonly variable 
            this.iWebHostEnvironment = _iWebHostEnvironment;
            this.iConfiguration = _iConfiguration;
            this.MyDbContext = _MyDbContext;

            // Pass the purpose string as a parameter
            this.IDataProtector = IDataProtectionProvider.CreateProtector(
                OurCustomDataProtectionPurposeStrings.AuthorIdRouteValue);

        }


        #region 1-Index
        public ViewResult Index(int TheCurrentPage = 1)
        {
            //1-
            ViewBag.test = iConfiguration["testJsonKeys"];
            //2-
            //IEnumerable<Author> Authors = iAuthorRepository.GetAllAuthors().Select(Author=>
            //{
            //    // Encrypt the value of ID prop and store it in EncryptedId property
            //    Author.EncryptedId = IDataProtector.Protect(Author.Id.ToString());
            //    return Author;
            //});

            //3-Encrypted author Id
            IEnumerable<Author> Authors = iAuthorRepository.GetAllAuthors();
            foreach (Author auth in Authors)
            {
                // Encrypt the value of ID prop and store it in EncryptedId property
                auth.EncryptedId = IDataProtector.Protect(auth.Id.ToString());
            }

            //4-implement pagination
            int authorsCount = Authors.Count();
            const int ThepageSize = 9;

            if (TheCurrentPage < 1) TheCurrentPage = 1;

            Pager pager = new Pager(authorsCount, TheCurrentPage, ThepageSize);



            //use the Skip and Take methods to specify the range of authors to retrieve. For example, to retrieve
            //authors 1 - 10, you can use Skip(0).Take(10).To retrieve authors 11 - 20, you can use Skip(10).Take(10), and so on.
            //Skip and Take are methods that are used to implement pagination in LINQ queries.

            //Skip is used to skip a specified number of elements in a sequence and returns the remaining elements.
            //For example, if you have a sequence of authors and you want to skip the first 5 authors, you can use
            //the Skip method like this:
            //var authors = context.Authors.Skip(5);
            //This will return all authors in the sequence after the first 5.

            //Take is used to take a specified number of elements from a sequence and returns those elements.
            //For example, if you have a sequence of authors and you want to take the first 10 authors, you can use
            //the Take method like this:
            //var authors = context.Authors.Take(10);
            //This will return the first 10 authors in the sequence.

            //When used together, Skip and Take can be used to implement pagination.For example, if you want to display
            //the second page of a list of authors where each page contains 10 authors, you can use the following code:
            //var authors = context.Authors.Skip(10).Take(10);
            //This will skip the first 10 authors and return the next 10 authors, which corresponds to the second page of the list.

            //if the page we request(current) has id 6 so we want escape (6-1)*9 ==45 item
            int numberOfSkippedAuthors = (TheCurrentPage - 1) * ThepageSize;

            IEnumerable<Author> data = Authors.Skip(numberOfSkippedAuthors).Take(pager.PageSize).ToList();    //after scip we take only page size items

            ViewBag.myPager = pager;



            return View(data);
        }
        #endregion


        #region 2-Search Author
        public IActionResult Search(string searchTerm)
        {
            IEnumerable<Author> result = iAuthorRepository.Search(searchTerm);

            return View("index", result);
        }
        #endregion

        #region 2-Create
        [HttpGet]
        public ViewResult create()
        {
            // Author Author = new Author();
            //return View(Author);

            //return the view model that contain the property from type of IFormFile
            AuthorCreateViewModel model = new AuthorCreateViewModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult create(AuthorCreateViewModel authorCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                //file name
                string uniqueFileName = ProcessUploadedFile(authorCreateViewModel);


                /*
                     manage identity column in ef core
                    In Entity Framework Core, you can manage an identity column (also known as an auto-incrementing primary key) by using the following steps:

                    1-In your model class, add a property to represent the identity column and decorate it with
                    the [Key] and [DatabaseGenerated(DatabaseGeneratedOption.Identity)] attributes.
                    For example:
                    public class MyEntity
                    {
                        [Key]
                        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                        public int Id { get; set; }
                        // Other properties
                    }


                     2-To reset the identity column in EF Core, you can use raw SQL commands to perform the manual reset
                        You can execute raw SQL commands in EF Core using the Database.ExecuteSqlRaw method from the Microsoft.EntityFrameworkCore namespace.

                    Here's an example of resetting the identity seed of a table named "MyTable" in EF Core:         
                    using (var context = new MyDbContext())
                    {
                        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('MyTable', RESEED, 0)");
                    }


                    The context.Database.ExecuteSqlRaw method is used to execute raw SQL commands in EF Core. In this example, the raw SQL command DBCC CHECKIDENT ('MyTable', RESEED, 0) is being executed.

                    The DBCC CHECKIDENT command is a T-SQL command that is used to check and reset the current identity value of a specified table in a database. It takes three arguments:

                    'MyTable': The name of the table whose identity value needs to be checked and reset.

                    RESEED: The action to be performed on the identity value. In this case, RESEED is used to reset the identity value.

                    0: The new seed value to be used for the identity column. In this example, the identity value is being reset to start from 0.

                    This raw SQL command will check the current identity value of the MyTable table and reset it to the specified value (0 in this case). This will cause the next insert operation to use the new seed value, effectively resetting the identity column to start counting from the specified value.

                    Note: Make sure to replace 'MyTable' with the actual name of your table, and adjust the new_seed_value to the desired starting value.
                    
                 
                 */
                //int maxId;

                //if (iAuthorRepository.GetAllAuthors().Count() > 0)
                //{
                //    maxId = iAuthorRepository.GetAllAuthors().Max<Author>(author => author.Id);
                //    MyDbContext.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('Authors', RESEED, {maxId})");

                //}
                //else
                //{
                //    maxId = 0;
                //    MyDbContext.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('Authors', RESEED, {maxId})");

                //}

                Author author = new Author
                {
                    Name = authorCreateViewModel._author.Name,
                    Email = authorCreateViewModel._author.Email,
                    Department = authorCreateViewModel._author.Department,
                    // Store the file name in PhotoPath property of the author object
                    // which gets saved to the Authors database table
                    //TADA because of that line we create  a sperate view modell, cause in our DB
                    //we want just the name of the photo, however in our server we want the file itself 
                    PhotoPath = uniqueFileName
                };
                iAuthorRepository.Add(author);
                return RedirectToAction("Details", new { id = author.Id });

            }
            return View();

        }
        #endregion

        #region 3-Read
        // Details Action receives the encrypted Author ID and Decrypt it
        //it will recive it as string like this:
        //https://localhost:44308/Home/Details/CfDJ8LGH0N8yv9JDkMCMCPCJchSa6oHiqEcWU6HiPnPAEBUD4foh_kb76oeqYGwVMbg6t_k2dneSng4WzKYmkRLdh1BZvjacwcckJil-9jExCa1cZ_RC2K7GIBXW7k7aUD9NnQ
        //then internally it will Decrypt it to the corrspending id
        public ViewResult Details(string id)
        {
            // Decrypt the employee id using Unprotect method
            string decryptedId = IDataProtector.Unprotect(id);
            int decryptedIntId = Convert.ToInt32(decryptedId);

            Author Author = iAuthorRepository.GetAuthor(decryptedIntId);

            //Handle first type of 404 Error (Id not found)
            if (Author == null)
            {
                Response.StatusCode = 404;
                return View("AuthorNotFound", id);
            }
            return View(Author);
        }
        #endregion

        #region 4-Update
        [HttpGet]
        public ViewResult Edit(int id)
        {
            AuthorEditViewModel authorEditViewModel = new AuthorEditViewModel();


            authorEditViewModel._author = iAuthorRepository.GetAuthor(id);
            authorEditViewModel.Id = authorEditViewModel._author.Id;
            authorEditViewModel.ExistingPhotoPath = authorEditViewModel._author.PhotoPath;

            return View(authorEditViewModel);
        }



        [HttpPost]
        // Through model binding, the action method parameter
        // AuthorEditViewModel receives the posted edit form data
        public IActionResult Edit(AuthorEditViewModel model)
        {
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // 1-Retrieve the author being edited from the database
                Author author = iAuthorRepository.GetAuthor(model.Id);

                //2- Update the author object with the data in the model object
                author.Name = model._author.Name;
                author.Email = model._author.Email;
                author.Department = model._author.Department;

                // 3-If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(iWebHostEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the employee object which will be
                    // eventually saved in the database
                    author.PhotoPath = ProcessUploadedFile(model);
                }

                //4- Call update method on the repository service passing it the
                // author object to update the data in the database table
                Author updatedAuthor = iAuthorRepository.Update(author);

                return RedirectToAction("index");
            }

            return View(model);
        }        //Delete

        private string ProcessUploadedFile(AuthorCreateViewModel model)
        {
            //1-folder path:
            // The image must be uploaded to the images folder in wwwroot
            // To get the path of the wwwroot folder we are using the inject
            // iWebHostEnvironment service provided by ASP.NET Core
            string uploadsFolder = Path.Combine(iWebHostEnvironment.WebRootPath, "Images");




            string uniqueFileName = null;


            // If the Photos property on the incoming model object is not null and if count > 0,
            // then the user has selected at least one file to upload

            if (model.Photos != null && model.Photos.Count > 0)
            {
                // Loop thru each selected file
                foreach (IFormFile photo in model.Photos)
                {
                    //2-uniqueFileName:
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    //TADA because of that line we create  a sperate view modell, cause in our DB
                    //we want just the name of the photo, however in our server we want the file itself 
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;


                    //3-full path[folder +file]:
                    string FullFilePath = Path.Combine(uploadsFolder, uniqueFileName);


                    //4-create the content of the image to by copy it to our specific full file path: 
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    photo.CopyTo(new FileStream(FullFilePath, FileMode.Create));
                }
            }

            return uniqueFileName;
        }
        #endregion


        #region 5-Delete
        //Deleting data using a GET request is not recommended.

        //Just imagine what can happen if there is an image tag in a malicious email as shown below.
        //The moment we open the email, the image tries to load and issues a GET request, which would delete
        //the data.//<img src = "http://localhost/Administration/DeleteUser/123" />
        //Also, when search engines index your page, they issue a GET request which would delete the data.
        //In general GET request should be free of any side-effects, meaning it should not change the state
        //on the server. Deletes should always be performed using a POST request.
        //Deleting data using a POST request is the best
        //Delete button type is set to submit
        //It is placed inside the form element and the method attribute is set to post
        //So when the Delete button is clicked a POST request is issued to DeleteUser() action passing it the ID of the user to delete
        //<form method="post" asp-action="DeleteUser" asp-route-id="@user.Id">
        //    <button type = "submit" class="btn btn-danger">Delete</button>
        //</form>

        [HttpPost]
        public IActionResult Delete(int id)
        {


            iAuthorRepository.Delete(id);
            return RedirectToAction("index");
        }
        #endregion


        #region 6-Test Actions

        public string Test1()
        {
            return "Hello from Test2";
        }
        public int Test2()
        {
            return 100;
        }

        //returns JSON data that does not respect content negotiation and ignores the Accept Header.
        public JsonResult Test3()
        {
            Author authorModel = iAuthorRepository.GetAuthor(1);

            //return Json(null); //null 
            return Json("the Model name");//"the Model name"
            //return Json(authorModel.Name);//"Mary"
        }
        public JsonResult Test4()
        {
            Author authorModel = iAuthorRepository.GetAuthor(1);
            return Json(authorModel);//{"id":1,"name":"Mary","email":"mary@pragimtech.com","department":"HR"}
        }

        //The following example respects content negotiation. It looks at the Request Accept Header and if it is set to application/xml, then XML data is returned. If the Accept header is set to application/json, then JSON data is returned.
        //you need to set the accept header
        //Please note : To be able to return data in XML format, we have to add Xml Serializer Formatter by calling AddXmlSerializerFormatters()
        //    services.AddMvc().AddXmlSerializerFormatters();

        public ObjectResult Test5()
        {
            Author authorModel = iAuthorRepository.GetAuthor(1);
            return new ObjectResult(authorModel);//"Mary"
        }
        public ViewResult Test6()
        {

            Author authorModel = iAuthorRepository.GetAuthor(1);


            //passing data through ViewData
            ViewData["myViewDataPro1"] = "passing value through View data withe value= myViewDataPro1";
            ViewData["myViewDataPro2"] = "passing value through View data withe value= myViewDataPro2";
            ViewData["myViewDataPro3"] = 'B';
            ViewData["myViewDataPro4"] = 100;

            //passing data through ViewBag
            ViewBag.myViewBagPro1 = "passing value through View bag withe value= myViewBagPro1";
            ViewBag.myViewBagPro2 = "passing value through View bag withe value= myViewBagPro2";
            ViewBag.myViewBagPro3 = 'A';
            ViewBag.myViewBagPro4 = 100;

            //passing data through Strongly type(Model)
            return View(authorModel);



            //Please note : With the absolute path, to get to the root project directory, we can use / or ~/.So the following 3 lines of code does the same thing
            //When specifying a view file path, we can also use a relative path. With relative path we do not specify the file extension .cshtml. In the following example, MVC looks for Update.cshtml file in "Views/Test" folder.    
            //return View( );
            //return View("MyViews/Test.cshtml");
            //return View("/MyViews/Test.cshtml");
            //return View("~/MyViews/Test.cshtml");
            //return View("../Test/Update");

        }





        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Test7")]
        /*
             The Route() attribute is specified 3 times on the Test7() action method. With each instance of 
        the Route() attribute we specified a different route template. With these 3 route templates in place, 
        the Test7() action method of the HomeController will be executed for any of the following 3 URL paths.
            /
            /Home
            /Home/Test7  


        With conventional routing we can specify route parameter as part of the route template.
        We can do the same with attribute routing as well=> [Route("Home/Details/{id?}")]

         */

        public ViewResult Test7()
        {
            return View();

            /*
                 With attribute routing the controller name and action method names play no role in which action is selected. Consider the example below in different controller=> 

                public class WelcomeController : Controller
                {
                    [Route("")]
                    [Route("Home")]
                    [Route("Home/Index")]
                    public ViewResult Welcome()
                    {
                        return View();
                    }
                }
             


            also we can use placeholder with attribute 
             
             */
        }



        //also we can use placeholder with attribute routing
        [Route("Product/{id}")]
        public ViewResult Test8(int id)
        {
            //[Route("Product/{MyProductId}")]
            //In this example, the[Route] attribute specifies the URL pattern with the productId placeholder,
            //while the action method takes a parameter named id.When a user navigates to the URL / Product / 123,
            //the value of productId will be set to 123, and the action method will receive this value as the id parameter.
            //So the parameter name in the action method and the placeholder name in the URL pattern=>
            //do not have to match,You can use any name you want for the parameter, and another name for the placeholder
            //but what if user insert string  / Product / "hello"   ??
            //Answer: If a user navigates to the URL / Product / hello, the framework will attempt to
            //bind the value "hello" to the id parameter of the action method.
            //If the action method is expecting an int type for the id parameter,
            //this may result in an error, depending on the framework's default behavior for
            //handling type mismatches in URL parameter binding.

            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //

            //but in asp core framework to get an binding
            //the place holder must be same name as parameter
            ViewBag.theId = id;
            return View();


        }

        public ViewResult Test9()
        {
            throw new Exception("Error/Exception otherthan 404 errors");
        }
        public ViewResult Test10()
        {
            //return View("Test11");//view from same controller
            //return View("create");//view from same controller
            return View("Register");//view from differ controller return viw not found
            //return View("Global Exception Handling");//view from differ controller return the view because it is in shared  folder

        }
        public ViewResult Test11()
        {
            return View();

        }

        #endregion

    }



}
