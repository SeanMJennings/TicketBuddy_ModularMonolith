using Testcontainers.Redis;

namespace Testing.Containers;

public static class Redis
{
   public static RedisContainer CreateContainer(int port = 6380) 
   {
       return new RedisBuilder()
           .WithPortBinding(port)
           .Build();
   }
}