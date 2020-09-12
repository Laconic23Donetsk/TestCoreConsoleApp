using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler, start the schedular before triggers or anything else
            IScheduler sched = await schedFact.GetScheduler();
            await sched.Start();

            // create job
            IJobDetail job = JobBuilder.Create<SimpleJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

            // create trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();


            // Schedule the job using the job and trigger 
            await sched.ScheduleJob(job, trigger);

            Console.ReadKey();
        }
    }

    /// <summary>
    /// SimpleJOb is just a class that implements IJOB interface. It implements just one method, Execute method
    /// </summary>
    public class SimpleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Hello, JOb executed");
        }
    }
}
