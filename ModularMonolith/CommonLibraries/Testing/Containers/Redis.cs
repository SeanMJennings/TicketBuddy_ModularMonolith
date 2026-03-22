using Testcontainers.Redis;

namespace Testing.Containers;

public static class Redis
{
   public static RedisContainer CreateContainer(bool reuse)
   {
       return new RedisBuilder("redis:latest")
           .WithReuse(reuse)
           .Build();
   }
   
   public static async Task Clear(this RedisContainer container)
   {
       await container.ExecScriptAsync("return redis.call('FLUSHALL')");
   }
}