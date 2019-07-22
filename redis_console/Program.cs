using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Services;
using StackExchange.Redis;

namespace redis_console
{
    class Program
    {
        private const string databaseConnectionUrl = "localhost";
        private static RedisCacheService _redisCacheService;
        //private static ConnectionMultiplexer _redis;
        //private static IDatabase _cache;
        private static Random rnd = new Random(Guid.NewGuid().GetHashCode());

        private static Dictionary<string, string> _xs = new Dictionary<string, string> {
            {"a", "Only don't tell me you're innocent. Because it insults my intelligence and makes me very angry."},
            {"b", "I know it was you, Fredo. You broke my heart. You broke my heart!"},
            {"c", "Michael, you never told me you knew Johnny Fontane! Never hate your enemies."},
            {"d", "It affects your judgment. This one time, this one time I'll let you ask me about my affairs."},
            {"e", "Is that why you slapped my brother around in public?"},
            {"f", "You can act like a man! That's my family Kay, that's not me. "},
            {"g", "My father is no different than any powerful man, any man with power, like a president or senator."},
            {"h", "We're both part of the same hypocrisy, senator, but never think it applies to my family."},
            {"i", "I'm your older brother, Mike, and I was stepped over!"},
        };

        static void Main(string[] args)
        {
            setup();
            writeToCache();
            while (true)
            {
                readFromCache();
                Thread.Sleep(3000);
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\r\n");
                    Console.WriteLine("Sleep with the fishes!");
                    Thread.Sleep(3000);
                    break;
                }
            }
        }

        private static string getLetter(int value)
        {
            return ((char)('a' + value)).ToString();
        }

        private static void setup()
        {
            Console.WriteLine("\r\n");
            Console.WriteLine("Setup...");
            _redisCacheService = new RedisCacheService();
            //_redis = ConnectionMultiplexer.Connect(databaseConnectionUrl);
            //_cache = _redis.GetDatabase();
        }
        
        private static void readFromCache()
        {
            Console.WriteLine("\r\n");
            Console.WriteLine("----Press escape to exit----");
            Console.WriteLine("\r\n");
            Console.WriteLine("Reading from cache...");
            try
            {
                var count = _xs.Count;
                var some = count / 3;

                var listNumbers = new List<int>();
                int number = 0;
                for (int i = 0; i < some; i++)
                {
                    do
                    {
                        number = rnd.Next(0, count);
                    } while (listNumbers.Contains(number));
                    listNumbers.Add(number);
                    var letter = getLetter(number);
                    var x = get(letter);
                    Console.WriteLine($"{ letter }: { x }");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: { ex.Message }");
            }
        }

        private static void writeToCache()
        {
            Console.WriteLine("\r\n");
            Console.WriteLine("Writing to cache...");
            try
            {
                foreach (var x in _xs)
                {
                    Console.WriteLine($"{ x.Key }: { x.Value }");
                    set(x.Key, x.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: { ex.Message }");
            }
        }

        private static void set(string key, string value)
        {
            _redisCacheService.Set(key, value);
        }

        private static string get(string key)
        {
            return _redisCacheService.Get(key);
        }
    }
}