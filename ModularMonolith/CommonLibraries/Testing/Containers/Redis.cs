using Testcontainers.Redis;

namespace Testing.Containers;

public static class Redis
{
   public static RedisContainer CreateContainer() 
   {
       return new RedisBuilder()
           .WithPortBinding(6380)
           .Build();
   }
}