using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.BBCode
{
    public class BBCodeParser
    {
        public BBRootNode MakeNodes(IEnumerable<BBToken> tokens)
        {
            var result = new BBRootNode();
            BBNode current = result;

            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case BBTokenType.Text: PrepareNode(result, ref current, true, BBNodeType.Text); break;
                    case BBTokenType.BeginTag: PrepareNode(result, ref current, false, BBNodeType.TagStart); break;
                    case BBTokenType.EndTag: PrepareNode(result, ref current, false, BBNodeType.TagEnd); break;
                }
                current.AddToken(token);
            }

            return result;
        }

        public BBRootNode MakeTree(IReadOnlyList<BBNode> nodes)
        {
            var stack = new List<BBNode>(nodes.Count);
            foreach (var node in nodes)
            {
                var tagEnd = node as BBTagEndNode;
                if (tagEnd != null)
                {
                    int pos = stack.Count;
                    BBTagStartNode tagStart = null;
                    while (--pos >= 0)
                    {
                        tagStart = stack[pos] as BBTagStartNode;
                        if (tagStart != null && tagStart.CloseTag == null && tagStart.TagName == tagEnd.TagName)
                            break;
                    }

                    if (pos >= 0)
                    {
                        var cut = pos + 1;
                        if (cut < stack.Count)
                        {
                            for (int i = cut; i < stack.Count; ++i)
                                tagStart.Content.Add(stack[i]);
                            stack.RemoveRange(cut, stack.Count - cut);
                        }
                        tagStart.Content.Add(tagEnd);

                        tagStart.CloseTag = tagEnd;
                        tagEnd.OpenTag = tagStart;
                        SelfCloseTags(tagStart.Content);
                    }
                    else
                    {
                        tagEnd.OpenTag = tagEnd;
                        stack.Add(tagEnd);
                    }
                }
                else
                {
                    stack.Add(node);
                }
            }

            SelfCloseTags(stack);

            var result = new BBRootNode();
            result.Content.AddRange(stack);

            return result;
        }

        public BBRootNode Parse(IEnumerable<BBToken> tokens)
        {
            var root = MakeNodes(tokens);
            return MakeTree(root.Content);
        }

        private void PrepareNode(BBRootNode result, ref BBNode current, bool allowMerge, BBNodeType nodeType)
        {
            if (allowMerge && current.NodeType == nodeType)
                return;

            switch (nodeType)
            {
                case BBNodeType.Text: current = new BBTextNode(); break;
                case BBNodeType.TagStart: current = new BBTagStartNode(); break;
                case BBNodeType.TagEnd: current = new BBTagEndNode(); break;
            }

            result.Content.Add(current);
        }

        private void SelfCloseTags(IEnumerable<BBNode> nodes)
        {
            foreach (var node in nodes)
            {
                var tagStartNode = node as BBTagStartNode;
                if (tagStartNode != null && tagStartNode.CloseTag == null)
                    tagStartNode.CloseTag = tagStartNode;
            }
        }
    }
}