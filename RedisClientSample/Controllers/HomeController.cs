using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RedisClientSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string message = string.Empty;
            string host = "localhost";
            using (var redisClient = new RedisClient(host))
            {
                //Create a 'strongly-typed' API that makes all Redis Value operations to apply against Phones
                IRedisTypedClient<Phone> redis = redisClient.As<Phone>();

                //Redis lists implement IList<T> while Redis sets implement ICollection<T>
                IRedisList<Phone> mostSelling = redis.Lists["urn:phones:mostselling"];
                IRedisList<Phone> oldCollection = redis.Lists["urn:phones:oldcollection"];

                Person phonesOwner = new Person
                    {
                        Id = 7,
                        Age = 90,
                        Name = "OldOne",
                        Profession = "sportsmen",
                        Surname = "OldManSurname"
                    };
                
                // adding new items to the list
                mostSelling.Add(new Phone
                        {
                            Id = 5,
                            Manufacturer = "Sony",
                            Model = "768564564566",
                            Owner = phonesOwner
                        });

                mostSelling.Add(new Phone
                        {
                            Id = 8,
                            Manufacturer = "Motorolla",
                            Model = "324557546754",
                            Owner = phonesOwner
                        });

                var upgradedPhone  = new Phone
                {
                    Id = 3,
                    Manufacturer = "LG",
                    Model = "634563456",
                    Owner = phonesOwner
                };

                mostSelling.Add(upgradedPhone);

                // remove item from the list
                oldCollection.Remove(upgradedPhone);

                // find objects in the cache
                IEnumerable<Phone> LGPhones = mostSelling.Where(ph => ph.Manufacturer == "LG");

                // find specific
                Phone singleElement = mostSelling.FirstOrDefault(ph => ph.Id == 8);

                //reset sequence and delete all lists
                redis.SetSequence(0);
                redisClient.Remove("urn:phones:mostselling");
                redisClient.Remove("urn:phones:oldcollection");
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult RedisSessionState()
        {
            ViewBag.Message = Session["testRedisSession"];
            return View("Index");
        }

        public ActionResult RedisStringStored()
        {
            string message = string.Empty;
            string host = "localhost";
            string elementKey = "testKeyRedis";
            using (RedisClient redisClient = new RedisClient(host))
            {
                if (redisClient.Get(elementKey) == null)
                {
                    // adding delay to see the difference
                    Thread.Sleep(5000);
                    redisClient.Set(elementKey, "some cached value");
                }
                message = "String item value is: " + redisClient.Get<string>("some cached value");
            }
            ViewBag.Message = message;
            return View("Index");
        }

        public ActionResult AboutPhone()
        {
            string message = string.Empty;
            string host = "localhost";
            using (RedisClient redisClient = new RedisClient(host))
            {
                IRedisTypedClient<Phone> phones = redisClient.As<Phone>();
                Phone phoneFive = phones.GetValue(5.ToString());
                if (phoneFive == null)
                {
                    // make a small delay
                    Thread.Sleep(5000);
                    phoneFive = new Phone
                    {
                        Id = 5,
                        Manufacturer = "Motorolla",
                        Model = "xxxxx",
                        Owner = new Person
                        {
                            Id = 1,
                            Age = 90,
                            Name = "OldOne",
                            Profession = "sportsmen",
                            Surname = "OldManSurname"
                        }
                    };
                    phones.SetEntry(phoneFive.Id.ToString(), phoneFive);
                }
                message = "Phone model is " + phoneFive.Manufacturer;
                message += "Phone Owner Name is: " + phoneFive.Owner.Name;
                message += "Phone Id is: " + phoneFive.Id.ToString();
            }
            ViewBag.Message = message;
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class Phone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public Person Owner { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Profession { get; set; }
    }
}
