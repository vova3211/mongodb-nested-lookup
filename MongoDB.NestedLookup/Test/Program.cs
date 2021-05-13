using MongoDB.Driver;
using MongoDB.NestedLookup;
using System;
using System.Threading.Tasks;
using Test.Models;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repository = new Repository();

            var authorCollection = repository.GetCollection<Author>();
            //authorCollection.InsertOne(new Author("Author2", DateTime.Now));

            var bookCollection = repository.GetCollection<Book>();
            //bookCollection.InsertOne(new Book("Book3", 200, "607d63dd6e1fc9dfbfdb7db8"));

            //var list = authorCollection.Aggregate()
            //    .Lookup(nameof(Book), nameof(Author.Id), nameof(Book.AuthorId), $"{nameof(Author)}_{nameof(Book)}")
            //    .Project(p => new Author())
            //    .ToList();

            //var book = await repository.FirstOrDefaultWithRelationsAsync<Book>(x => x.Name == "Book3");

            var authorWithBooks = await repository.FirstOrDefaultWithRelationsAsync<Author>(x => x.Name == "Author1");

            var s = 1;
        }
    }
}
