using System;
using System.Threading.Tasks;
using Arbor.App.Extensions.Time;
using Arbor.AspNetCore.Host.Scheduling;
using Cronos;
using FluentAssertions;
using Serilog.Core;
using Xunit;

namespace Arbor.AspNetCore.Host.Tests
{
    public class ScheduledServiceTests
    {
        [Fact]
        public async Task ScheduleOnce()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
            var dateTimeOffset = new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero);
            var schedule = new ScheduleOnce(dateTimeOffset);
            using var timer = new TestTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);
            var testService = new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 8; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                timer.Tick();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            testService.Invokations.Should().Be(1);
        }

        [Fact]
        public async Task ScheduleEvery4thSeconds()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
            var start = new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero);
            var schedule = new ScheduleEveryInterval(TimeSpan.FromSeconds(4), start);
            using var timer = new TestTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);
            var testService = new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                timer.Tick();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            testService.Invokations.Should().Be(2);
        }

        [Fact]
        public async Task ScheduleEverySecond()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMilliseconds(200));

            var start = new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero);
            var schedule = new ScheduleEveryInterval(TimeSpan.FromSeconds(1), start);
            using var timer = new TestTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);

            var testService = new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 50; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                timer.Tick();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            testService.Invokations.Should().Be(7);
        }

        [Fact]
        public async Task CronScheduleEvery5Minutes()
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(1));

            var schedule = new CronSchedule(CronExpression.Parse("*/5 * * * *"));
            using var timer = new TestTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);

            var testService = new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                timer.Tick();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            testService.Invokations.Should().Be(2);
        }

        [InlineData(10, 4)]
        [InlineData(100, 49)]
        [Theory]
        public async Task ScheduleEveryOtherSecondSecond(int ticks, int expected)
        {
            var clock = new TestClock(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
            var start = new DateTimeOffset(2000, 1, 1, 0, 0, 4, 0, TimeSpan.Zero);
            var schedule = new ScheduleEveryInterval(TimeSpan.FromSeconds(2), start);
            using var timer = new TestTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);
            var testService = new TestScheduledService(schedule, scheduler);

            for (int i = 0; i < ticks; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                timer.Tick();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            testService.Invokations.Should().Be(expected);
        }

        [InlineData(1000, 100, 10)]
        [Theory]
        public async Task ScheduleEverySecondWithRealTimer(int milliseconds, int interval, int expected)
        {
            var clock = new CustomSystemClock();
            var start = clock.UtcNow().AddMilliseconds(100);
            var schedule = new ScheduleEveryInterval(TimeSpan.FromMilliseconds(interval), start);
            using var timer = new SystemTimer();
            using var scheduler = new Scheduler(clock, timer, Logger.None);
            var testService = new TestScheduledService(schedule, scheduler);

            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds));

            testService.Invokations.Should().Be(expected);
        }
    }
}