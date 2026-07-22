using System.Linq.Expressions;
using Hangfire;

namespace RentIt.Shared.Abstractions.BackgroundJobs;

public class HangfireBackgroundJob(
    IBackgroundJobClient jobClient,
    IRecurringJobManager recurringJob) : IBackgroundJob
{
    public string ContinueWith<T>(string parentJobId, string continuationJobName, Expression<Func<T, Task>> methodCall)
    {
        return jobClient.ContinueJobWith(parentJobId, continuationJobName, methodCall);
    }

    public bool Delete(string jobId)
    {
        return jobClient.Delete(jobId);
    }

    public string Enqueue<T>(string jobName, Expression<Func<T, Task>> methodCall)
    {
        return jobClient.Enqueue(jobName, methodCall);
    }

    public void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        recurringJob.AddOrUpdate(jobName, methodCall, cronExpression);
    }

    public string Schedule<T>(string jobName, Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return jobClient.Schedule(jobName, methodCall, delay);
    }
}
