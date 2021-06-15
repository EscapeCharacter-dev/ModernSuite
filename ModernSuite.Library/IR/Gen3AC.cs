using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModernSuite.Library.IR
{
    public sealed class Gen3AC
    {
        private Operation SingleOperation(ASTNode node, int counter, out List<Operation> children)
        {
            if (node is LiteralASTNode lan)
            {
                children = Enumerable.Empty<Operation>().ToList();
                return new Operation
                {
                    ResultIdentifier = $"{counter}",
                    Operator = "0",
                    FirstOperand = $"{lan.Lexable.Representation}"
                };
            }

            else if (node is AdditionOperation ao)
            {
                children = new List<Operation>();
                var left = SingleOperation(ao.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(ao.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "1",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is SubtractionOperation so)
            {
                children = new List<Operation>();
                var left = SingleOperation(so.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(so.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "2",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is MultiplicationOperation mo)
            {
                children = new List<Operation>();
                var left = SingleOperation(mo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(mo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "3",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is DivisionOperation dvo)
            {
                children = new List<Operation>();
                var left = SingleOperation(dvo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(dvo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "4",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else
            {
                children = null;
                return null;
            }
        }
        public Operation[] Parse(ASTNode[] nodes)
        {
            var opCounter = 0;

            var operations = new List<Operation>();

            foreach (var node in nodes)
            {
                var operation = SingleOperation(node, opCounter, out var children);
                operations.AddRange(children);
                operations.Add(operation);
                opCounter += 1 + children.Count;
            }

            return operations.ToArray();
        }
    }
}
