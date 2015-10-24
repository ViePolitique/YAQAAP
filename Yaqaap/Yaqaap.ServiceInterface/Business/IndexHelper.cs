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
        public static void CreateIndex(Guid id, string content, string table)
        {
            TableRepository tableRepository = new TableRepository();


            content = content.ToLowerInvariant();

            var terms = content.Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries).Distinct();

            TableOperation[] toInsert = terms.Select(term => TableOperation.InsertOrMerge(new IndexEntry(id, term, table))).ToArray();

            tableRepository.ExecuteBatch(Tables.Indexes, toInsert);

        }

        public static T[] Search<T>(string search, string tableName) where T : ITableEntity, new()
        {
            search = search.ToLowerInvariant();

            string[] terms = search.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);


            ConcurrentBag<string> bag = new ConcurrentBag<string>();


            Parallel.ForEach(terms, term =>
                                    {
                                        try
                                        {
                                            term = $"{tableName}-{term}";

                                            term = TableEntityHelper.RemoveDiacritics(term);
                                            term = TableEntityHelper.ToAzureKeyString(term);

                                            TableRepository tableRepository = new TableRepository();
                                            var query = tableRepository.GetTable(Tables.Indexes).CreateQuery<IndexEntry>();

                                            var result = from k in query
                                                         where k.RowKey == term
                                                         select k.PartitionKey;

                                            foreach (var id in result)
                                            {
                                                bag.Add(id);
                                            }
                                        }
                                        catch
                                        {
                                            // pas de log ?
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
