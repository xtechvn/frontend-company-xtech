using MongoDB.Bson;
using HuloToys_Service.Controllers.CarRegistration.Model;
using XTECH_FRONTEND.Utilities;
using MongoDB.Driver;
using XTECH_FRONTEND.Controllers.API.CarRegistration.IRepositories;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Core.Events;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace XTECH_FRONTEND.Controllers.API.CarRegistration.Repositories
{
    public class MongoService : IMongoService
    {
        private readonly IConfiguration _configuration;
        public MongoService(IConfiguration configuration)
        {
            _configuration=configuration;
         
        }
        public async Task<long> Insert(RegistrationRecord model)
        {
            try
            {
                string url = "mongodb://" + _configuration["MongoServer:user"] + ":" + _configuration["MongoServer:pwd"] + "@" + _configuration["MongoServer:Host"] + ":" + _configuration["MongoServer:Port"] + "/" + _configuration["MongoServer:catalog_log"];
                var client = new MongoClient(url);

                IMongoDatabase db = client.GetDatabase(_configuration["MongoServer:catalog_log"]);
                RegistrationRecord log = new RegistrationRecord()
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    PhoneNumber = model.PhoneNumber,
                    PlateNumber = model.PlateNumber.ToUpper(),
                    Name = model.Name,
                    Referee = model.Referee.ToUpper(),
                    GPLX = model.GPLX.ToUpper(),
                    QueueNumber = model.QueueNumber,
                    RegistrationTime = model.RegistrationTime,
                    ZaloStatus = model.ZaloStatus,
                    Camp = model.Camp

                };
                IMongoCollection<RegistrationRecord> affCollection = db.GetCollection<RegistrationRecord>(_configuration["MongoServer:Data_Car"]);


                await affCollection.InsertOneAsync(log);


                return model.QueueNumber;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PushLog - LogActionMongoService: " + ex.Message);
            }
            return 0;
        }
        public static IMongoDatabase GetDatabase()
        {
            var host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["Host"];
            var port = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["Port"]);
            var catalog_log = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["catalog_log"];
            var user = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["user"];
            var pwd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["pwd"];
            var cred = MongoCredential.CreateCredential(catalog_log, user, pwd);
            var client = new MongoClient(
                    new MongoClientSettings()
                    {
                        Server = new MongoServerAddress(host, port),
                        ClusterConfigurator = cb =>
                        {
                            //var textWriter = TextWriter.Synchronized(new StreamWriter("mylogfile.txt"));
                            cb.Subscribe<CommandStartedEvent>(e =>
                            {
                                //log.Debug(e.Command.ToString());
                                //LogHelper.InsertLogTelegram(e.Command.ToString());
                            });
                        },
                        Credential = cred
                    });

            //them tinh nang bo qua cac truong co trong db nhung khong co trong mo ta class
            var pack = new ConventionPack();
            pack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);
            var db = client.GetDatabase(catalog_log);
            return db;
        }
        public List<RegistrationRecordMongo> GetList()
        {
            var list = new List<RegistrationRecordMongo>();
            try
            {
                var now = DateTime.Now;
                var expireAt =  new DateTime(now.Year, now.Month, now.Day, 17, 55, 0);
                if (now.Hour >= 20)
                {
                    expireAt = new DateTime(now.Year, now.Month, now.Day, 19, 55, 0);

                }
                var db = GetDatabase();
                var collection = db.GetCollection<RegistrationRecordMongo>(_configuration["MongoServer:Data_Car"]);
                var filter = Builders<RegistrationRecordMongo>.Filter.Empty;
                if (now>= expireAt)
                {
                    filter &= Builders<RegistrationRecordMongo>.Filter.Gte("RegistrationTime", expireAt);
                }
                else
                {
                    filter &= Builders<RegistrationRecordMongo>.Filter.Gte("RegistrationTime", expireAt.AddDays(-1));
                }
               
                var S = Builders<RegistrationRecordMongo>.Sort.Ascending("QueueNumber");
                list = collection.Find(filter).Sort(S).ToList();
                return list;

            }
            catch (Exception ex)
            {
                
                LogHelper.InsertLogTelegram("SearchTransactionSMs - TransferSmsService. " + JsonConvert.SerializeObject(ex));
            }
            return list;
        }
    }
}
