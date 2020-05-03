﻿using System;
using Microsoft.Azure.Cosmos.Table;

namespace NosAyudamos
{
    class EntityData : TableEntity
    {
        public static EntityData Create<T>(string partitionKey, T data, ISerializer serializer) =>
            new EntityData(partitionKey, typeof(T).FullName!)
            {
                Data = serializer.Serialize(data),
                DataVersion = (typeof(T).Assembly.GetName().Version ?? new Version(1, 0)).ToString(2)
            };

        public EntityData() { }

        private EntityData(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string? Data { get; set; }
        public string? DataVersion { get; set; }
        public int Version { get; set; }
    }
}