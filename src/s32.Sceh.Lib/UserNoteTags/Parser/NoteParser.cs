using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.UserNoteTags.Lexer;

namespace s32.Sceh.UserNoteTags.Parser
{
    public class NoteParser
    {
        public RootNode MakeNodes(IEnumerable<Token> tokens)
        {
            var result = new RootNode();
            Node current = result;

            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Text: PrepareNode(result, ref current, true, NodeType.Text); break;
                    case TokenType.BeginTag: PrepareNode(result, ref current, false, NodeType.TagStart); break;
                    case TokenType.EndTag: PrepareNode(result, ref current, false, NodeType.TagEnd); break;
                }
                current.AddToken(token);
            }

            return result;
        }

        public RootNode MakeTree(IReadOnlyList<Node> nodes)
        {
            var stack = new List<Node>(nodes.Count);
            foreach (var node in nodes)
            {
                var tagEnd = node as TagEndNode;
                if (tagEnd != null)
                {
                    int pos = stack.Count;
                    TagStartNode tagStart = null;
                    while (--pos >= 0)
                    {
                        tagStart = stack[pos] as TagStartNode;
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

            var result = new RootNode();
            result.Content.AddRange(stack);

            return result;
        }

        public RootNode Parse(IEnumerable<Token> tokens)
        {
            var root = MakeNodes(tokens);
            return MakeTree(root.Content);
        }

        private void PrepareNode(RootNode result, ref Node current, bool allowMerge, NodeType nodeType)
        {
            if (allowMerge && current.NodeType == nodeType)
                return;

            switch (nodeType)
            {
                case NodeType.Text: current = new TextNode(); break;
                case NodeType.TagStart: current = new TagStartNode(); break;
                case NodeType.TagEnd: current = new TagEndNode(); break;
            }

            result.Content.Add(current);
        }

        private void SelfCloseTags(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var tagStartNode = node as TagStartNode;
                if (tagStartNode != null && tagStartNode.CloseTag == null)
                     tagStartNode.CloseTag = tagStartNode;
            }
        }
    }
}