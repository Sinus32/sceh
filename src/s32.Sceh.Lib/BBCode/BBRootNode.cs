using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBRootNode : BBNode
    {
        public override BBNodeType NodeType
        {
            get { return BBNodeType.Root; }
        }

        public override void AddToken(BBToken token)
        {
            throw new InvalidOperationException("Root node cannot have any tokens");
        }
    }
}
