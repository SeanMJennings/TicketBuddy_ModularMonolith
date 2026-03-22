using Testcontainers.Redis;

namespace Testing.Containers;

public static class Redis
{
   public static RedisContainer CreateContainer(bool reuse, int port = 6380)
   {
       return new RedisBuilder("redis:latest")
           .WithPortBinding(port)
           .WithReuse(reuse)
           .Build();
   }
   
   public static async Task Clear(this RedisContainer container)
   {
       await container.ExecScriptAsync("return redis.call('FLUSHALL')");
   }
}