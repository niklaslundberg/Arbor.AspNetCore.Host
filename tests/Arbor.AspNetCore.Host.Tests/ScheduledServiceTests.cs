using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduledServiceTests
    {
        [Fact]
        public async Task Schedule()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var schedule = new ScheduleOnce(new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero));
            using var testTimer = new TestTimer();
            IScheduler scheduler = new Scheduler(clock, testTimer);
            var testService =  new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 60; i++)
            {
                testTimer.Tick();
            }

            await Task.Delay(TimeSpan.FromSeconds(3));

            testService.Invokations.Should().Be(1);

        }

        [Fact]
        public async Task Schedule3()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var schedule = new ScheduleEvery3Second(new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero));
            using var testTimer = new TestTimer();
            IScheduler scheduler = new Scheduler(clock, testTimer);
            var testService =  new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 60; i++)
            {
                testTimer.Tick();
            }

            await Task.Delay(TimeSpan.FromSeconds(6));

            testService.Invokations.Should().NotBe(0);

            Console.WriteLine(testService.Invokations);

        }
    }
}