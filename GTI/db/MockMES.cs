using MDL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace UnitTestProject.db
{
    public class MockMES : MESContext
    {
        public MockMES(string conn) : base(conn) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);
            }
            catch (System.Exception)
            {
                string schema = "dbo";
                if (!string.IsNullOrEmpty(schema)) modelBuilder.HasDefaultSchema(schema);

                //產生Table名稱時不要自動變為複數
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            }
        }
    }
}
