using Microsoft.Data.Sqlite;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace OrleansStatisticsKeeper.Grains.Database
{
    public static class ConnectionsFactory
    {
        private static MongoClient _mongoClient;
        private static IDictionary<string, IDbConnection> _dbConnections = new Dictionary<string, IDbConnection>(20);

        public static MongoClient OpenMongo(string connString)
        {
            if (_mongoClient == default)
                _mongoClient = new MongoClient(connString);

            return _mongoClient;
        }

        public static IDbConnection OpenRdms(string type, string connString)
        { 
               switch (type.ToLower())
               {
                case "sqlite":
                    if (!_dbConnections.ContainsKey(type))
                        _dbConnections[type] = new SqliteConnection(connString);
                    else if (_dbConnections[type].State == ConnectionState.Closed || _dbConnections[type].State == ConnectionState.Broken)
                        _dbConnections[type] = new SqliteConnection(connString);
                    break;
                default: 
                    return null;
               };

            return _dbConnections[type];
        }
    }
}
