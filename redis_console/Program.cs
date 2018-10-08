using System; 
using System.Threading; 
using StackExchange.Redis; 

namespace redis_console {
    class Program {
		private const string mykey = "mykey";
		private const string databaseConnectionUrl = "localhost";
		private static ConnectionMultiplexer _redis; 

        static void Main(string[] args) {
            _redis = ConnectionMultiplexer.Connect(databaseConnectionUrl); 
            entry(); 
        }

        private static void entry() {
            try {
                var db = _redis.GetDatabase(); 
                db.StringSet(mykey, "Hello World"); 
                var value = db.StringGet(mykey); 
                Console.WriteLine(value); 
                Thread.Sleep(10000); 
                entry(); 
            }catch(Exception ex) {
                Console.WriteLine($"error: { ex.Message }");
            }
        }
    }
}
