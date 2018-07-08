using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using s32.Sceh.Jobs;

namespace s32.Sceh.Tests
{
    [TestClass]
    public class PriorityQueueTests
    {
        private enum CommandType
        {
            Put,
            Remove,
        }

        [TestMethod]
        public void Create()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new JobManager(0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new JobManager(JobManager.MAX_JOBS + 1));
            var a = new JobManager(1);
            var b = new JobManager(JobManager.MAX_JOBS);
        }

        [DataTestMethod]
        [DynamicData(nameof(PutAndRemoveData), DynamicDataSourceType.Method)]
        public void PutAndRemove(int maxJobs, string input, string result)
        {
            var manager = new JobManager(maxJobs);
            ValidateIntegrity(manager);

            StatefulJob job;
            var dict = new Dictionary<String, StatefulJob>(maxJobs);
            foreach (var cmd in ParseInput(input))
            {
                switch (cmd.Type)
                {
                    case CommandType.Put:
                        job = manager.PutJob(cmd.Argument, TestJob.Builder);
                        if (job != null)
                            dict[job.Id] = job;
                        break;
                    case CommandType.Remove:
                        job = dict[cmd.Argument];
                        manager.RemoveJob(job);
                        break;
                }

                ValidateIntegrity(manager);
            }

            ValidateResult(manager, result);
        }

        private static IEnumerable<object[]> PutAndRemoveData()
        {
            // PutJob tests
            yield return new object[] { 8, "p1;p2", "2;1" };
            yield return new object[] { 8, "p1;p2;p3", "3;1;2" };
            yield return new object[] { 3, "p1;p2;p3;p2", "2;1;3" };
            yield return new object[] { 8, "p1;p2;p3;p2;p4;p5", "5;4;3;1;2" };
            yield return new object[] { 8, "p1;p2;p3;p2;p4;p5;p3;p6", "6;4;3;1;2;5" };
            yield return new object[] { 7, "p1;p2;p3;p2;p4;p5;p3;p6;p7;p2;p8", "2;7;6;1;4;5;3" };

            // RemoveJob tests
            yield return new object[] { 8, "p1;p2;p3;p4;p5;p6;p7", "7;4;6;1;3;2;5" };
            yield return new object[] { 8, "p1;p2;p3;p4;p5;p6;p7;r6", "7;4;5;1;3;2" };
            yield return new object[] { 8, "p1;p2;p3;p4;p5;p6;p7;r6;r3", "7;4;5;1;2" };
            yield return new object[] { 8, "p1;p2;p3;p4;p5;p6;p7;r1", "7;5;6;4;3;2" };
        }

        private IEnumerable<Command> ParseInput(string input)
        {
            var array = input.Split(';');
            foreach (var dt in array)
            {
                if (dt.StartsWith("p"))
                    yield return new Command(CommandType.Put, dt.Substring(1));
                else if (dt.StartsWith("r"))
                    yield return new Command(CommandType.Remove, dt.Substring(1));
                else
                    throw new ArgumentException("Invalid input string", "input");
            }
        }

        private void ValidateIntegrity(JobManager manager)
        {
            var priority = Int64.MaxValue;
            var jobs = manager.JobsStorage;

            Assert.IsTrue(manager.JobCount <= jobs.Count);

            for (int i = 0; i < jobs.Count; ++i)
            {
                var job = jobs[i];

                if (i < manager.JobCount)
                {
                    Assert.IsNotNull(job);
                    Assert.AreEqual(job.Position, i + 1);

                    if (job.Position > 1)
                    {
                        var parentIndex = (job.Position / 2) - 1;
                        var parent = jobs[parentIndex];
                        Assert.IsTrue(job.Priority < parent.Priority);
                    }

                    priority = job.Priority;
                }
                else
                {
                    Assert.IsNull(job);
                }
            }
        }

        private void ValidateResult(JobManager manager, string result)
        {
            var jobs = manager.JobsStorage;
            var array = result.Split(';');

            Assert.AreEqual(manager.JobCount, array.Length);

            for (int i = 0; i < manager.JobCount; ++i)
            {
                var job = jobs[i];
                Assert.IsNotNull(job);
                Assert.AreEqual(job.Id, array[i]);
            }
        }

        private struct Command
        {
            public string Argument;
            public CommandType Type;

            public Command(CommandType type, string argument)
                : this()
            {
                Type = type;
                Argument = argument;
            }
        }
    }
}
