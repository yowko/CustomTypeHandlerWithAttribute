using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yowko.Models;

namespace TypeHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistTypeHandler();

            var users = new List<User>();
            // db 連線
            using (var conn = new SqlConnection("Data Source=.;database=YowkoTest;Integrated Security=SSPI;app=LINQPad"))
            {
                // dapper 取得資料
                users = conn.Query<User>("SELECT * FROM dbo.[User]").ToList();
            }
            users.ForEach(a => Console.WriteLine(JsonConvert.SerializeObject(a)));
            Console.ReadKey();
        }

        private static void RegistTypeHandler()
        {
            //載入當下執行 assembly 所參考到的所有 assembly
            var assemblies = Assembly
                .GetExecutingAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load);


            var convertClasses = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.StartsWith("Yowko", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(x => x.DefinedTypes.Where(type => typeof(IConvertType).GetTypeInfo().IsAssignableFrom(type.AsType())))//取得有實作 IConvertType 的類別
                                                                                                                                  //.SelectMany(a => a.GetTypes().Where(p => p.GetInterfaces().Contains(typeof(IConvertType))))//效果與上句相同，據說上句較快，但個人覺得這句語意比較清楚且效能差異只有一次應可忽略
                .SelectMany(a => a.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(b => b.GetCustomAttribute<JsonTypeAttribute>() != null))//取出有 JsonTypeAttribute 標記的 property
                .Select(c => c.PropertyType)
                .ToList();

            // 將需要轉型的 property 型別註冊至 Dapper 使用 TypeHandler 做型別轉換
            convertClasses.ForEach(a => SqlMapper.AddTypeHandler(a, new UserTypeHandler()));
        }
    }
}
