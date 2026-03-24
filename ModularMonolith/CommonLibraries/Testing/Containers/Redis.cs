using Testcontainers.Redis;

namespace Testing.Containers;

public static class Redis
{
   public static RedisContainer CreateContainer(bool reuse, string label)
   {
       return new RedisBuilder("redis:latest")
           .WithLabel("ticketbuddy.suite", label)
           .WithReuse(reuse)
           .Build();
   }
   
   public static async Task Clear(this RedisContainer container)
   {
       await container.ExecScriptAsync("return redis.call('FLUSHALL')");
   }
}