﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitDb.Core.Interfaces;
using GitDb.Core.Model;

namespace GitDb.Remote
{
    class RemoteTransaction : ITransaction
    {
        readonly HttpClient _client;
        readonly string _baseUrl;
        readonly string _transactionId;
        bool _isOpen = false;
        RemoteTransaction(HttpClient client, string baseUrl, string transactionId)
        {
            _client = client;
            _baseUrl = baseUrl;
            _transactionId = transactionId;
            _isOpen = true;
        }

        public static async Task<RemoteTransaction> Create(HttpClient client, string baseUrl, string branch)
        {
            var transactionId = (await (await client.PostAsync($"{baseUrl}/{branch}/transaction", new StringContent("", Encoding.UTF8))
                                                    .WhenSuccessful())
                                                    .Content
                                                    .ReadAsStringAsync())
                                                    .Replace("\"", "");
            return new RemoteTransaction(client, baseUrl, transactionId);
        }

        T executeIfOpen<T>(Func<T> action)
        {
            if (_isOpen)
                return action();
            throw new Exception("Transaction is not open");
        }

        string url(string resource) => _baseUrl + resource;

        public Task Add(Document document) =>
            executeIfOpen(() => _client.PostAsync(url($"/{_transactionId}/add"), document).WhenSuccessful());

        public Task Add<T>(Document<T> document) => 
            Add(Document.From(document));

        public Task Delete(string key) =>
            executeIfOpen(() => _client.PostAsync(url($"/{_transactionId}/delete/{key}"), new StringContent("", Encoding.UTF8)));

        public Task DeleteMany(IEnumerable<string> keys) =>
            executeIfOpen(async () =>
             {
                 foreach (var batch in keys.Batch(50))
                     await _client.PostAsync(url($"/{_transactionId}/deleteMany"), batch);
             });
            

        public Task AddMany<T>(IEnumerable<Document<T>> documents) =>
            AddMany(documents.AsParallel().Select(Document.From));

        public Task AddMany(IEnumerable<Document> documents) =>
            executeIfOpen(async () =>
            {
                foreach (var batch in documents.Batch(50))
                    await _client.PostAsync(url($"/{_transactionId}/addMany"), batch);
            });

        public async Task<string> Commit(string message, Author author)
        {
            if (!_isOpen)
                throw new Exception("Transaction is not open");

            _isOpen = false;
            return await _client.PostAsync(url($"/{_transactionId}/commit"), new CommitTransaction
            {
                Message = message,
                Author = author
            }).WhenSuccessful()
              .AsStringResponse();
        }

        public async Task Abort()
        {
            if (_isOpen)
                await _client.PostAsync(url($"/{_transactionId}/abort"), new StringContent("", Encoding.UTF8));
            _isOpen = false;
        }

        public void Dispose() =>
            Abort().Wait();
    }
}
