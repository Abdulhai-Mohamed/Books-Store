using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books_Store.Models.AuthorModels
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        //As the name implies, EncryptedId property holds the encrypted Author id.NotMapped attribute specifies
        //that this property must be excluded from mapping it to a database table column.
        [NotMapped]
        public string EncryptedId { get; set; }
        //IDataProtector is required in the HomeController.In this Index() action,
        //we encrypt the employee id values and in the Details() they are decrypted.



        [Display(Name = "Author Name"),
        Required(ErrorMessage = "Please provide a value for Name field من فضلك ضع بيانات  للإسم  !"),
        MaxLength(10, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }



        [Display(Name = "Official Email"),
        Required(ErrorMessage = "Please provide a value for Email field من فضلك ضع بيانات  للإيميل  !"),
        RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]

        public string Email { get; set; }


        [Display(Name = "Author Department"),
        Required(ErrorMessage = "Please provide a value for Department field من فضلك إختر بيانات  القسم  !")]
        public Dept? Department { get; set; }
        //we made Dept property nullable to enable valdtion of requreied attribute not the default enum(intgers) validation (erro message for  ntegers is: in valid)





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

        public string PhotoPath { get; set; }
    }
}
