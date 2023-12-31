namespace Books_Store.Models.AuthorModels
{

    public class MockAuthorRepository : IAuthorRepository
    {
        private readonly List<Author> _AuthorList = new List<Author>()
            {
                new Author() { Id = 1, Name = "Mary", Department = Dept.Beginner, Email = "mary@pragimtech.com" },
                new Author() { Id = 2, Name = "John", Department = Dept.Intermediate, Email = "john@pragimtech.com" },
                new Author() { Id = 3, Name = "Sam", Department = Dept.Professional, Email = "sam@pragimtech.com" },
                new Author() { Id = 4, Name = "osama", Department = Dept.None, Email = "sam@pragimtech.com" },
            };

        #region 1-Create

        public Author Add(Author AuthorToBeCreate)
        {
            //Author.Id=_AuthorList.Max(author => author.Id);
            AuthorToBeCreate.Id = AuthorToBeCreate.Id = _AuthorList.Max<Author>(author => author.Id) + 1;
            _AuthorList.Add(AuthorToBeCreate);
            return AuthorToBeCreate;
        }
        #endregion

        #region 2-Read

        public Author GetAuthor(int IdOfAuthorToBeRead)
        {
            return _AuthorList.Find(author => author.Id == IdOfAuthorToBeRead);
        }
        public IEnumerable<Author> GetAllAuthors()
        {

            return _AuthorList;
        }
        #endregion

        #region 3-Update

        public Author Update(Author AuthorToBeUpdate)
        {

            Author TheAuthorToBeUpdate = _AuthorList.Find(author => author.Id == AuthorToBeUpdate.Id);
            if (TheAuthorToBeUpdate != null)
            {
                TheAuthorToBeUpdate.Name = AuthorToBeUpdate.Name;
                TheAuthorToBeUpdate.Email = AuthorToBeUpdate.Email;
                TheAuthorToBeUpdate.Department = AuthorToBeUpdate.Department;

            }
            return TheAuthorToBeUpdate;
        }
        #endregion

        #region 4-Delete

        public void Delete(int IdOfAuthorToBeDelete)
        {
            Author AuthorToBeDelete = _AuthorList.Find(author => author.Id == IdOfAuthorToBeDelete);
            if (AuthorToBeDelete != null)
            {
                _AuthorList.Remove(AuthorToBeDelete);
            }
        }


        #endregion


        #region 5-Search
        public IEnumerable<Author> Search(string searchTerm)
        {
            IEnumerable<Author> searchResult = _AuthorList.Where(author => author.Name.Contains(searchTerm));

            return searchResult;
        }
        #endregion
    }
}
