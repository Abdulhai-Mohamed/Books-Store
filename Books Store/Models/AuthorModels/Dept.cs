namespace Books_Store.Models.AuthorModels
{
    public enum Dept
    {
        //we reassign the first value as 1 not 0 by default, because we want 0 set to the please select option
        None =1,
        Beginner =2,
        Intermediate =3,
        Professional =4
    }
}
