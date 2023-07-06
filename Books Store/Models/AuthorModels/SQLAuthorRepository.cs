using Books_Store.Models.EF_Core_DB_Models;
using Microsoft.EntityFrameworkCore;

namespace Books_Store.Models.AuthorModels
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly MyDbContext myDbContext;
        public SQLAuthorRepository(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        #region 1-Create

        public Author Add(Author AuthorToBeCreate)
        {
            myDbContext.Authors.Add(AuthorToBeCreate);
            myDbContext.SaveChanges();
            return AuthorToBeCreate;
        }

        #endregion

        #region 2-Read

        public Author GetAuthor(int IdOfAuthorToBeRead)
        {
            return myDbContext.Authors.Find(IdOfAuthorToBeRead);
        }
        public IEnumerable<Author> GetAllAuthors()
        {
            return myDbContext.Authors;
        }
        #endregion

        #region 3-Update

        public Author Update(Author AuthorToBeUpdate)
        {
            //Author TheAuthorToBeUpdate = myDbContext.Authors.Find( AuthorToBeUpdate.Id);
            //if (TheAuthorToBeUpdate != null)
            //{
            //    TheAuthorToBeUpdate.Name = AuthorToBeUpdate.Name;
            //    TheAuthorToBeUpdate.Email = AuthorToBeUpdate.Email;
            //    TheAuthorToBeUpdate.Department = AuthorToBeUpdate.Department;

            //}
            //return TheAuthorToBeUpdate;



            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Author> TrakedAuthor =
              myDbContext.Authors.Attach(AuthorToBeUpdate);
            /*
             EntityEntry is a class in the Microsoft.EntityFrameworkCore.ChangeTracking namespace. 
            It represents an entity instance that is being tracked by the Entity Framework's change tracker. 
            EntityEntry provides a way to access and modify the state of an entity, such as its property values, 
            as well as to perform various operations on the entity, such as marking it as added, modified, or deleted. 
            Additionally, EntityEntry provides information about the entity, such as its entity type and its original and current values.
             */
            TrakedAuthor.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            myDbContext.SaveChanges();

            return AuthorToBeUpdate;

        }
        #endregion

        #region 4-Delete

        public void Delete(int IdOfAuthorToBeDelete)
        {
            Author AuthorToBeDelete = myDbContext.Authors.Find(IdOfAuthorToBeDelete);
            if (AuthorToBeDelete != null)
            {
                myDbContext.Authors.Remove(AuthorToBeDelete);
                myDbContext.SaveChanges();
            }
        }

        #endregion


        #region 5-Search
        public IEnumerable<Author> Search(string searchTerm)
        {
            IEnumerable<Author> searchResult = myDbContext.Authors
                //.Include(author => author.Name)//Name property is string not list, so we cant use include 
                .Where(author=>author.Name.Contains(searchTerm));   
            
            return searchResult;
        }
        #endregion
    }
}
