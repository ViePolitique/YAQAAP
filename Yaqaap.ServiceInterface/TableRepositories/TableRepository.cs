using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Yaqaap.Framework;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public class Tables
    {
        public const string Questions = "questions";
        public const string Indexes = "indexes";
        public const string Answers = "answers";
        public const string Users = "users";
    }

    class TableRepository
    {
        private readonly CloudStorageAccount _storageAccount;

        private CloudTableClient _tableClient;

        /// <summary>
        /// Liste des réféences aux différentres tables
        /// </summary>
        private readonly Dictionary<string, CloudTable> _cloudTables = new Dictionary<string, CloudTable>();

        private readonly object _lock = new object();

        public TableRepository()
        {
            if (StorageConfig.StorageConnexionString != null)
                _storageAccount = CloudStorageAccount.Parse(StorageConfig.StorageConnexionString);
            else
                _storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        }


        private CloudTableClient TableClient
        {
            get
            {
                if (_tableClient == null)
                {
                    // Create the table client.
                    _tableClient = _storageAccount.CreateCloudTableClient();
                }

                return _tableClient;
            }
        }


        public CloudTable GetTable(string name)
        {
            lock (_lock)
            {
                if (name == null)
                    return null;

                if (_cloudTables.ContainsKey(name) == false)
                {
                    _cloudTables.Add(name, TableClient.GetTableReference(name));
                    _cloudTables[name].CreateIfNotExists();
                }

                return _cloudTables[name];
            }
        }


        public bool InsertOrReplace<T>(T entry, string table)
            where T : TableEntity
        {
            return InsertOrReplace(entry, GetTable(table));
        }

        private bool InsertOrReplace<T>(T entry, CloudTable table)
            where T : TableEntity
        {
            try
            {
                // Create the InsertOrReplace TableOperation
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entry);

                // Execute the operation.
                table.Execute(insertOrReplaceOperation);

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);

                return false;
            }
        }


        public bool InsertOrMerge<T>(T entry, string table)
            where T : TableEntity
        {
            return InsertOrMerge(entry, GetTable(table));
        }

        private bool InsertOrMerge<T>(T entry, CloudTable table)
            where T : TableEntity
        {
            try
            {
                // Create the InsertOrMerge TableOperation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entry);

                // Execute the operation.
                table.Execute(insertOrMergeOperation);

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);

                return false;
            }
        }

        public void Merge<T>(T[] entries, CloudTable table)
            where T : ITableEntity
        {
            try
            {
                TableBatchOperation batch = new TableBatchOperation();

                for (int i = 0; i < entries.Length; i++)
                {
                    // Create the Delete TableOperation
                    batch.InsertOrMerge(entries[i]);

                    if (batch.Count >= 100)
                    {
                        table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                    }
                }

                if (batch.Count > 0)
                {
                    table.ExecuteBatch(batch);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);
            }
        }


        public bool Delete<T>(T entry, string table)
            where T : TableEntity
        {
            return Delete(entry, GetTable(table));
        }

        private bool Delete<T>(T entry, CloudTable table)
            where T : TableEntity
        {

            try
            {
                entry.ETag = "*";

                // Create the Delete TableOperation
                TableOperation deleteOperation = TableOperation.Delete(entry);

                // Execute the operation.
                table.Execute(deleteOperation);

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);

                return false;
            }

        }

        public bool Delete<T>(T[] entries, string table, out List<T> deleted)
            where T : ITableEntity
        {
            return Delete(entries, GetTable(table), out deleted);
        }

        private bool Delete<T>(T[] entries, CloudTable table, out List<T> deleted)
            where T : ITableEntity
        {
            deleted = new List<T>();
            List<T> toDelete = new List<T>();

            try
            {
                TableBatchOperation batch = new TableBatchOperation();

                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i].ETag = "*";

                    // Create the Delete TableOperation
                    batch.Delete(entries[i]);
                    toDelete.Add(entries[i]);

                    if (batch.Count >= 100)
                    {
                        table.ExecuteBatch(batch);

                        deleted.AddRange(toDelete);
                        toDelete = new List<T>();

                        batch = new TableBatchOperation();
                    }
                }

                if (batch.Count > 0)
                {
                    table.ExecuteBatch(batch);

                    deleted.AddRange(toDelete);
                }

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);
                return false;
            }
        }

        public bool BatchDelete<T>(string from, string to = null, params T[] elements) where T : ITableEntity
        {
            CloudTable fromTable = from != null ? GetTable(from) : null;
            CloudTable toTable = to != null ? GetTable(to) : null;

            return BatchDelete(fromTable, toTable, elements);
        }

        private bool BatchDelete<T>(CloudTable from, CloudTable to = null, params T[] elements) where T : ITableEntity
        {
            bool result = true;
            List<T> deleted = null;

            try
            {
                result = Delete(elements, from, out deleted);
            }
            catch (Exception)
            {
                result = false;
            }

            if (deleted != null && to != null)
            {
                try
                {
                    // Execute the operation.
                    result = InsertOrReplace(deleted.ToArray(), to);
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }


        public bool InsertOrReplace<T>(T[] entries, string table)
            where T : ITableEntity
        {
            return InsertOrReplace(entries, GetTable(table));
        }

        private bool InsertOrReplace<T>(T[] entries, CloudTable table)
    where T : ITableEntity
        {
            try
            {
                TableBatchOperation batch = new TableBatchOperation();

                for (int i = 0; i < entries.Length; i++)
                {
                    // Create the Delete TableOperation
                    batch.InsertOrMerge(entries[i]);

                    if (batch.Count >= 100)
                    {
                        table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                    }
                }

                if (batch.Count > 0)
                {
                    table.ExecuteBatch(batch);
                }

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);
                return false;
            }
        }


        private string NextId(string currentId)
        {
            string nextId = currentId;

            nextId = nextId.Substring(0, nextId.Length - 1) + (char)(nextId[nextId.Length - 1] + 1);

            return nextId;
        }

        public bool ExecuteBatch(string table, TableOperation[] operations)
        {
            return ExecuteBatch(GetTable(table), operations);
        }

        private bool ExecuteBatch(CloudTable table, TableOperation[] operations)
        {
            try
            {
                TableBatchOperation batch = new TableBatchOperation();

                for (int i = 0; i < operations.Length; i++)
                {
                    // Create the Delete TableOperation
                    batch.Add(operations[i]);

                    if (batch.Count >= 100)
                    {
                        table.ExecuteBatch(batch);
                        batch = new TableBatchOperation();
                    }
                }

                if (batch.Count > 0)
                {
                    table.ExecuteBatch(batch);
                }

                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.GetLogger().Error(ex.ToString(), ex);
                return false;
            }
        }

    }
}
