using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public static class RepositoryExtensions
{
    public static List<T> GetRecordsByActionLinkSid<T>(this IQueryable<T> queryable, string key) where T : class
    {
        // 獲取要查詢的類型
        var entityType = typeof(T);

        // 獲取 ACTION_LINK_SID 屬性
        var property = entityType.GetProperty("ACTION_LINK_SID");
        if (property == null)
        {
            throw new InvalidOperationException($"Type {entityType.Name} does not contain a property named ACTION_LINK_SID");
        }

        // 生成表達式樹
        var parameter = Expression.Parameter(entityType, "c");
        var propertyAccess = Expression.Property(parameter, property);
        var constant = Expression.Constant(key);
        var equal = Expression.Equal(propertyAccess, constant);

        var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

        // 執行查詢
        return queryable.Where(lambda).AsNoTracking().ToList();
    }
}

// 使用範例
public class WP_EQP_TRACE
{
    public string ACTION_LINK_SID { get; set; }
    // 其他屬性
}

public class WP_LOT_HIST
{
    public string ACTION_LINK_SID { get; set; }
    // 其他屬性
}

public class EFQuery_MES : DbContext
{
    public DbSet<WP_EQP_TRACE> WP_EQP_TRACE { get; set; }
    public DbSet<WP_LOT_HIST> WP_LOT_HIST { get; set; }
    // 其他 DbSet
}

public class Program
{
    public static void Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<EFQuery_MES>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new EFQuery_MES(options))
        {
            // 插入測試數據
            context.WP_EQP_TRACE.Add(new WP_EQP_TRACE { ACTION_LINK_SID = "someKey" });
            context.WP_LOT_HIST.Add(new WP_LOT_HIST { ACTION_LINK_SID = "someKey" });
            context.SaveChanges();

            var key = "someKey";

            // 使用擴充方法查詢 WP_EQP_TRACE
            var wpEqpTraceResults = context.WP_EQP_TRACE.GetRecordsByActionLinkSid(key);
            foreach (var item in wpEqpTraceResults)
            {
                Console.WriteLine($"WP_EQP_TRACE: {item.ACTION_LINK_SID}");
            }

            // 使用擴充方法查詢 WP_LOT_HIST
            var wpLotHistResults = context.WP_LOT_HIST.GetRecordsByActionLinkSid(key);
            foreach (var item in wpLotHistResults)
            {
                Console.WriteLine($"WP_LOT_HIST: {item.ACTION_LINK_SID}");
            }
        }
    }
}
