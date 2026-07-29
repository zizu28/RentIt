using System.Linq.Expressions;
using Hangfire;

namespace RentIt.Shared.Abstractions.BackgroundJobs;

public class HangfireBackgroundJob(
    IBackgroundJobClient jobClient,
    IRecurringJobManager recurringJob) : IBackgroundJob
{
    private readonly IBackgroundJobClient _jobClient = jobClient;
    private readonly IRecurringJobManager _recurringJob = recurringJob;

    public string ContinueWith<T>(string parentJobId, string continuationJobName, Expression<Func<T, Task>> methodCall)
    {
        return _jobClient.ContinueJobWith(parentJobId, continuationJobName, methodCall);
    }

    public bool Delete(string jobId)
    {
        return _jobClient.Delete(jobId);
    }

    public string Enqueue<T>(string jobName, Expression<Func<T, Task>> methodCall)
    {
        return _jobClient.Enqueue(jobName, methodCall);
    }

    public void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        _recurringJob.AddOrUpdate(jobName, methodCall, cronExpression);
    }

    public string Schedule<T>(string jobName, Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return _jobClient.Schedule(jobName, methodCall, delay);
    }
}
