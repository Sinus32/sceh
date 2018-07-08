using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.Jobs;

namespace s32.Sceh.Tests
{
    public class TestJob : StatefulJob
    {
        public TestJob(string id)
            : base(id)
        {
        }
        
        public static StatefulJob Builder(string id)
        {
            return new TestJob(id);
        }

        protected override JobResult DoWork()
        {
            throw new NotImplementedException();
        }
    }
}
