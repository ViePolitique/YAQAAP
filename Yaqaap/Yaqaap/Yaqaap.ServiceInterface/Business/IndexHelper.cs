using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Yaqaap.ServiceInterface.TableRepositories;

namespace Yaqaap.ServiceInterface.Business
{
    class IndexHelper
    {
        public void CreateIndex(string id, string content, string table)
        {
            TableRepository tableRepository = new TableRepository();


            content = content.ToLowerInvariant();

            string[] terms = content.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            TableOperation[] toInsert = terms.Select(term => TableOperation.InsertOrMerge(new IndexEntry(id, term))).ToArray();

            tableRepository.ExecuteBatch(table, toInsert);

        }

        public static T[] Search<T>(string search, string tableName) where T : ITableEntity, new()
        {
            search = search.ToLowerInvariant();

            string[] terms = search.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);


            ConcurrentBag<string> bag = new ConcurrentBag<string>();

            Parallel.ForEach(terms, term =>
            {
                TableRepository tableRepository = new TableRepository();
                var query = tableRepository.GetTable(Tables.Indexes).CreateQuery<IndexEntry>();

                var result = from k in query
                             where k.PartitionKey == term
                             select k.RowKey;

                foreach (var id in result)
                {
                    bag.Add(id);
                }
            });

            ConcurrentBag<T> bagResult = new ConcurrentBag<T>();

            IEnumerable<string> idsToSearch = bag.Distinct();
            Parallel.ForEach(idsToSearch, id =>
            {
                TableRepository tableRepository = new TableRepository();
                var query = tableRepository.GetTable(tableName).CreateQuery<T>();

                var result = from k in query
                            where k.PartitionKey == id
                            select k;

                foreach (var data in result)
                {
                    bagResult.Add(data);
                }

            });

            return bagResult.ToArray();
        }
    }
}
