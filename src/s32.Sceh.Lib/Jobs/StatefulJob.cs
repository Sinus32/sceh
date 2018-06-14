using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Jobs
{
    public abstract class StatefulJob
    {
        private readonly string _id;
        private long _priority = 0;
        private int _position = 0;

        protected StatefulJob(string id)
        {
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public long Priority
        {
            get { return _priority; }
            internal set { _priority = value; }
        }

        /// <summary>
        /// Position in 1-indexed array
        /// </summary>
        public int Position
        {
            get { return _position; }
            internal set { _position = value; }
        }
    }
}
