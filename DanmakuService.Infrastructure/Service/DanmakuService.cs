using Bli.Common;
using Bli.JWT;
using DanmakuService.Domain;
using DanmakuService.Domain.Entity;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

using StackExchange.Redis;
using System.Collections;


namespace DanmakuService.Infrastructure.Service
{
    public class ReDanmakuService : IDanService
    {
        private readonly IMongoDatabase _database;
      
        public ReDanmakuService(IOptionsSnapshot<MongoDbSettings> settings, IConnectionMultiplexer connectionMultiplexer)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);        }
        public async Task AddDanmakuAsync(Danmaku danmaku)
        {
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = new BsonDocument("name", "dan" + danmaku.VideoID) });
            var exists = collections.Any();
            if (!exists) throw new Exception("集合不存在");
            var collection = _database.GetCollection<Danmaku>("dan"+danmaku.VideoID);
            await collection.InsertOneAsync(danmaku);
        } 

        public async Task<List<Danmaku>> GetDanmakuAsync(string videoId, int max)
        {
            var collection = _database.GetCollection<Danmaku>("dan" + videoId);
            var filter = Builders<Danmaku>.Filter.Empty;
            return await collection.Find(filter).SortByDescending(x => x.CreateTime).Limit(max)
    .ToListAsync();
        }
    }
}
