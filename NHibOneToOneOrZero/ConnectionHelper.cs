using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Threading.Tasks;

namespace NHibOneToOneOrZero
{
    class ConnectionHelper
    {
        private static readonly ISessionFactory _sessionFactory;
        static ConnectionHelper()
        {
            _sessionFactory = FluentConfigure();
        }

        public static ISession GetCurrentSession()
        {
            return _sessionFactory.OpenSession();
        }

        public static async Task CloseSession()
        {
            await _sessionFactory.CloseAsync();
        }

        public static async Task CloseSessionFactory()
        {
            if (_sessionFactory != null)
            {
                await _sessionFactory.CloseAsync();
            }
        }

        private static ISessionFactory FluentConfigure()
        {
            return Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2012
                    .ConnectionString("Data Source=(LocalDb)\\MSSQLLocalDB;database=OneToOneDemo;trusted_connection=yes;Integrated Security=True;")
                    .ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SheepMap>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(useStdOut: true, doUpdate: true))
                .BuildSessionFactory();
        }
    }
}
