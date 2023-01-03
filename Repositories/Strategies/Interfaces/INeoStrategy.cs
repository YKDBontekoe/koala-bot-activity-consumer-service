namespace Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;

public interface INeoStrategy<in T> where T : class
{
    bool IsActivityValid(T activity); 
}