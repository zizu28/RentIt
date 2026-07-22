using System.Linq.Expressions;

namespace RentIt.Shared.Abstractions.BackgroundJobs;

public interface IBackgroundJob
{
    string Enqueue<T>(string jobName, Expression<Func<T, Task>> methodCall);
    string Schedule<T>(string jobName, Expression<Func<T, Task>> methodCall, TimeSpan delay);
    void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression);
    string ContinueWith<T>(string parentJobId, string continuationJobName, Expression<Func<T, Task>> methodCall);
    bool Delete(string jobId);
}
