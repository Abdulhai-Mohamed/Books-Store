namespace Books_Store.Models.AuthorModels
{
    public interface IAuthorRepository //CRUD
    {
        //CREATE
        Author Add(Author AuthorToBeCreate);
        //READ
        Author GetAuthor(int IdOfAuthorToBeRead); 
        IEnumerable<Author> GetAllAuthors();
        //UPDATE
        Author Update(Author AuthorToBeUpdate);
        //DELETE
         void Delete(int IdOfAuthorToBeDelete);
        //Search
        IEnumerable<Author> Search(string searchTerm);
    }
}
