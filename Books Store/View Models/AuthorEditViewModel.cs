namespace Books_Store.View_Models
{
    public class AuthorEditViewModel : AuthorCreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
