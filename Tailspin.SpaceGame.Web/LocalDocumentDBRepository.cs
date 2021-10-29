using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text.Json;
using TailSpin.SpaceGame.Web.Models;

namespace TailSpin.SpaceGame.Web
{
    public class LocalDocumentDBRepository<T> : IDocumentDBRepository<T> where T : Model
    {
        // An in-memory list of all items in the collection.
        private readonly List<T> _items;

        public LocalDocumentDBRepository(string fileName)
        {
            // Serialize the items from the provided JSON document.
            _items = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(fileName));
        }

        public LocalDocumentDBRepository(Stream stream)
        {
            // Serialize the items from the provided JSON document.
            _items = JsonSerializer.Deserialize<List<T>>(new StreamReader(stream).ReadToEnd());
        }

        /// <summary>
        /// Retrieves the item from the store with the given identifier.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the retrieved item.
        /// </returns>
        /// <param name="id">The identifier of the item to retrieve.</param>
        public Task<IEnumerable<T>> GetItemsAsync(
          Expression<Func<T, bool>> queryPredicate,
          Expression<Func<T, int>> orderDescendingPredicate,
          int page = 1, int pageSize = 10
        )
        {
          var result = _items.AsQueryable()
           .Where(queryPredicate) // filter
           .OrderByDescending(orderDescendingPredicate) // sort
           .Skip(page * pageSize) // find page
           .Take(pageSize) // take items
           .AsEnumerable(); // make enumeratable

          return Task<IEnumerable<T>>.FromResult(result);
        }

        

        /// <summary>
        /// Retrieves the number of items that match the given query predicate.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the number of items that match the query predicate.
        /// </returns>
        /// <param name="queryPredicate">Predicate that specifies which items to select.</param>
        public Task<int> CountItemsAsync(Expression<Func<T, bool>> queryPredicate)
        {
            var count = _items.AsQueryable()
                .Where(queryPredicate) // filter
                .Count(); // count

            return Task<int>.FromResult(count);
        }
    }
}
