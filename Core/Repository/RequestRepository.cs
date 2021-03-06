using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Streamstone;

namespace NosAyudamos
{
    /// <summary>
    /// Repository of requests for help.
    /// </summary>
    interface IRequestRepository
    {
        /// <summary>
        /// Inserts or updates the given help request.
        /// </summary>
        Task<Request> PutAsync(Request request);
        /// <summary>
        /// Retrieves an existing help request from its <paramref name="requestId"/>.
        /// </summary>
        /// <param name="requestId">The identifier for the request.</param>
        /// <param name="readOnly">If <see langword="true"/>, will only return the last-known state for the help request, 
        /// rather than loading its history too, and no mutation operations will be allowed on it.</param>
        /// <returns>The stored help request or <see langword="null"/> if none was found with the given <paramref name="requestId"/>.</returns>
        Task<Request?> GetAsync(string requestId, bool readOnly = true);
    }

    class RequestRepository : IRequestRepository
    {
        readonly ISerializer serializer;
        readonly CloudStorageAccount storageAccount;
        readonly CloudTable? cloudTable = null;
        readonly string tableName;

        public RequestRepository(ISerializer serializer, CloudStorageAccount storageAccount, string tableName = "Request")
            => (this.serializer, this.storageAccount, this.tableName)
            = (serializer, storageAccount, tableName ?? "Request");

        public async Task<Request?> GetAsync(string requestId, bool readOnly = true)
        {
            if (string.IsNullOrEmpty(requestId))
                return default;

            if (readOnly)
            {
                var header = await GetAsync<DataEntity>(requestId, typeof(Request).FullName!).ConfigureAwait(false);
                if (header == null)
                    return default;

                if (header.Data == null)
                    throw new ArgumentException(Strings.DomainRepository.EmptyData);

                return serializer.Deserialize<Request>(header.Data);
            }

            var table = await GetTableAsync();
            var partition = new Partition(table, requestId);
            var existent = await Stream.TryOpenAsync(partition);
            if (!existent.Found)
                return default;

            var events = (await Stream.ReadAsync<DomainEventEntity>(partition))
                .Events.Select(e => e.ToDomainEvent(serializer)).ToList();

            return new Request(events) { Version = existent.Stream.Version };
        }

        public async Task<Request> PutAsync(Request request)
        {
            var table = await GetTableAsync();
            var partition = new Partition(table, request.RequestId!);
            var result = await Stream.TryOpenAsync(partition);
            var stream = result.Found ? result.Stream : new Stream(partition);
            var header = DataEntity.Create(request.RequestId!, request, serializer);
            header.Version = stream.Version + request.Events.Count;

            await Stream.WriteAsync(partition, request.Version, request.Events.Select((e, i) =>
                e.ToEventData(stream.Version + i, header)).ToArray());

            request.AcceptEvents();

            return request;
        }

        async Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            var table = await GetTableAsync();

            var result = await table.ExecuteAsync(
                TableOperation.Retrieve<T>(partitionKey, rowKey)).ConfigureAwait(false);

            return (T)result.Result;
        }

        async Task<CloudTable> GetTableAsync()
            => cloudTable ?? await GetTableAsync(tableName);

        async Task<CloudTable> GetTableAsync(string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
