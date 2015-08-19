using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using DingusGaming.DingusGaming.helper;
using DingusGaming.Store;

namespace DingusGaming.DingusGaming.data
{
    class Dynamo : DataAccess
    {
        private static AmazonDynamoDBClient client = null;

        public Dynamo()
        {
            CreateClient();
        }

        private void CreateClient()
        {
            if (client != null) return;
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.ServiceURL = Settings.getSettings()["AWS.dynamo.serviceUrl"];
            client = new AmazonDynamoDBClient(config);
        }

        public Dictionary<string, int> getBalances()
        {
            throw new NotImplementedException();
        }

        public List<Store.Store> getStores()
        {
            throw new NotImplementedException();
        }

        public void setBalances(Dictionary<string, int> balances)
        {
            throw new NotImplementedException();
        }

        public void setStores(List<Store.Store> stores)
        {
            throw new NotImplementedException();
        }

    }
}
