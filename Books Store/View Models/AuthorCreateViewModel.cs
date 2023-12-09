using Books_Store.Models.AuthorModels;
using System.ComponentModel.DataAnnotations;

namespace Books_Store.View_Models
{

    public class AuthorCreateViewModel
    {
        public Author _author { get; set; }

        //we want:

        //1-enable upload file(image) to our server(project) this done by 
        // create a property with type IFormFile
        //IFormFile contain complex information about the upladed file, and we save the author details in 
        //our Db, so we dont want save all that complex information,
        //so we create seprate viewModel that will contain the property with type IFormFile


        //2-save only the name(path) of uploaded file(image) the Db,
        //and path is a string, so that in our main author model, the type of PhotoPath is string,
        //and this string will be take its value from the name property from the IFormFile type from the
        //photo property from the separated view model
        public List<IFormFile> Photos { get; set; }
    }
}
