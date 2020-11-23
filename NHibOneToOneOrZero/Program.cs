using NHibernate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NHibOneToOneOrZero
{
    class Program
    {
        private const ConsoleColor foregroundColor = ConsoleColor.Yellow;

        static async Task Main(string[] _)
        {
            Console.WriteLine("One to one mapping demo based on this article:");
            Console.WriteLine("https://blog.novanet.no/one-to-one-or-zero-relationship-with-nhibernate/");

            try
            {
                WriteLineWithColor("Checking if database exists...");
                if (await IsEmptyDatabase())
                {
                    WriteLineWithColor("Seeding database...");
                    await SeedDatabase();
                }
                
                await QuerySheep();
            }
            finally
            {
                await ConnectionHelper.CloseSessionFactory();
            }            
        }

        private static async Task QuerySheep()
        {
            
            await ExecuteDb(session =>
            {
                WriteLineWithColor("Querying living sheep (no slaughter info)...");

                // No data returned because of null check (SlaughterInfo)
                WriteHorizontalRule();
                var query1 = session
                    .Query<Sheep>()
                    .Where(s => s.SheepId == 1 && s.SlaughterInfo == null)
                    .SingleOrDefault();

                WriteLineWithColor($"Living sheep by Id (with null check): Expected = Dolly, actual = {query1?.Name}", 
                    color: GetResultColor(null != query1));

                // No data returned because of null check (SlaughterInfo)
                WriteHorizontalRule();
                var query2 = session
                    .Query<Sheep>()
                    .Where(s => s.SlaughterInfo == null)
                    .SingleOrDefault();

                WriteLineWithColor($"Living sheep (with null check): Expected = Dolly, actual = {query2?.Name}",
                    color: GetResultColor(null != query2));

                // Data returned as expected (no null check of SlaughterInfo)
                WriteHorizontalRule();
                var query3 = session
                    .Query<Sheep>()
                    .Where(s => s.SheepId == 1)
                    .SingleOrDefault();

                WriteLineWithColor($"Living sheep by Id (without null check): Expected = Dolly, actual = {query3?.Name}", 
                    color: GetResultColor(null != query3));

                WriteLineWithColor("Querying dead sheep (with slaughter info)...");

                // Data returned as expected
                WriteHorizontalRule();
                var query4 = session
                    .Query<Sheep>()
                    .Where(s => s.SheepId == 2 && s.SlaughterInfo != null)
                    .SingleOrDefault();

                WriteLineWithColor($"Dead sheep by Id (with null check): Expected = Fresh meat, actual = {query4?.Name}",
                    color: GetResultColor(null != query4));

                // Data returned as expected
                WriteHorizontalRule();
                var query5 = session
                    .Query<Sheep>()
                    .Where(s => s.SlaughterInfo != null)
                    .SingleOrDefault();

                WriteLineWithColor($"Dead sheep (with null check): Expected = Fresh meat, actual = {query5?.Name}",
                    color: GetResultColor(null != query5));

                // Data returned as expected
                WriteHorizontalRule();
                var query6 = session
                    .Query<Sheep>()
                    .Where(s => s.SheepId == 2)
                    .SingleOrDefault();

                WriteLineWithColor($"Dead sheep by Id (without null check): Expected = Fresh meat, actual = {query6?.Name}",
                    color: GetResultColor(null != query6));
                WriteHorizontalRule();

                return Task.CompletedTask;
            });
        }

        private static void WriteHorizontalRule()
        {
            WriteLineWithColor(new string('-', 80), ConsoleColor.Blue);
        }

        private static async Task<bool> IsEmptyDatabase()
        {
            return await QueryDb(session => Task.FromResult(session.Query<Sheep>().Count() <= 0));
        }

        private static async Task SeedDatabase()
        {
            await ExecuteDb(async session =>
            {
                var dolly = new Sheep { Name = "Dolly" };
                var freshMeat = new SlaughteredSheep 
                { 
                    Sheep = new Sheep { Name = "Fresh meat" }, 
                    DateOfSlaughter = DateTime.Now.AddDays(-666) 
                };

                await session.SaveAsync(dolly);
                await session.SaveAsync(freshMeat);
            });
        }

        private static async Task<TResult> QueryDb<TResult>(Func<ISession, Task<TResult>> dbQuery)
        {
            var session = ConnectionHelper.GetCurrentSession();
            TResult result = default;
            using var tx = session.BeginTransaction();
            try
            {
                result = await dbQuery(session);
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                WriteLineWithColor($"ERROR: {ex.Message}", ConsoleColor.Red);
                await tx.RollbackAsync();
            }
            finally
            {
                await ConnectionHelper.CloseSession();
            }

            return result;
        }

        private static async Task ExecuteDb(Func<ISession, Task> dbAction)
        {
            var _ = await QueryDb(async (session) =>
            {
                await dbAction(session);
                return default(string);
            });
        }

        private static ConsoleColor GetResultColor(bool result)
        {
            return result == true 
                ? ConsoleColor.Green 
                : ConsoleColor.Red;
        }

        private static void WriteLineWithColor(string message, ConsoleColor color = foregroundColor)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
