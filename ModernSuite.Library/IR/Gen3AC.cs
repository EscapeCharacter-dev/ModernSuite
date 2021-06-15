using ModernSuite.Library.CodeAnalysis.Parsing.AST;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Operations;
using ModernSuite.Library.CodeAnalysis.Parsing.AST.Statements;
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
        private Operation SingleOperation(Semantic node, int counter, out List<Operation> children)
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
            else if (node is RemainderOperation ro)
            {
                children = new List<Operation>();
                var left = SingleOperation(ro.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(ro.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "5",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is NegativeOperation no)
            {
                children = new List<Operation>();
                var left = SingleOperation(no.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "6",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is LNotOperation lno)
            {
                children = new List<Operation>();
                var left = SingleOperation(lno.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "7",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is BNotOperation bno)
            {
                children = new List<Operation>();
                var left = SingleOperation(bno.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "8",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is ParenthesizedOperation po)
            {
                children = new List<Operation>();
                var left = SingleOperation(po.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "9",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is BinaryLeftShiftOperation blso)
            {
                children = new List<Operation>();
                var left = SingleOperation(blso.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(blso.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "10",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is BinaryRightShiftOperation brso)
            {
                children = new List<Operation>();
                var left = SingleOperation(brso.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(brso.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "11",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is LowerOperation lo)
            {
                children = new List<Operation>();
                var left = SingleOperation(lo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(lo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "12",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is GreaterOperation go)
            {
                children = new List<Operation>();
                var left = SingleOperation(go.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(go.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "13",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is LowerEqualOperation leo)
            {
                children = new List<Operation>();
                var left = SingleOperation(leo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(leo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "14",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is GreaterEqualOperation geo)
            {
                children = new List<Operation>();
                var left = SingleOperation(geo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(geo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "15",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is EqualityOperation eo)
            {
                children = new List<Operation>();
                var left = SingleOperation(eo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(eo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "16",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is NotEqualOperation neo)
            {
                children = new List<Operation>();
                var left = SingleOperation(neo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(neo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "17",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is BAndOperation bao)
            {
                children = new List<Operation>();
                var left = SingleOperation(bao.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(bao.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "18",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is BXorOperation bxo)
            {
                children = new List<Operation>();
                var left = SingleOperation(bxo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(bxo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "19",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is BOrOperation boo)
            {
                children = new List<Operation>();
                var left = SingleOperation(boo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(boo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "20",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is LAndOperation lao)
            {
                children = new List<Operation>();
                var left = SingleOperation(lao.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(lao.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "21",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is LOrOperation loo)
            {
                children = new List<Operation>();
                var left = SingleOperation(loo.Left, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var right = SingleOperation(loo.Right, counter + more.Count + 1, out var rmore);
                children.AddRange(rmore);
                children.Add(right);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + rmore.Count + 2}",
                    Operator = "22",
                    FirstOperand = left.ResultIdentifier,
                    SecondOperand = right.ResultIdentifier
                };
            }
            else if (node is UnaryPlusOperation upo)
            {
                children = new List<Operation>();
                var left = SingleOperation(upo.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "23",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is AddressOfOperation aoo)
            {
                children = new List<Operation>();
                var left = SingleOperation(aoo.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "24",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is ValueOfOperation voo)
            {
                children = new List<Operation>();
                var left = SingleOperation(voo.Child, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "25",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is FunctionCallOperation fco)
            {
                children = new List<Operation>();
                var allocations = 0;
                var childrenHandler = "[";
                foreach (var child in fco.Children)
                {
                    var operation = SingleOperation(child, counter + allocations + 1, out var subchildren);
                    children.AddRange(subchildren);
                    allocations += subchildren.Count + 1;
                    children.Add(operation);
                    childrenHandler += allocations.ToString() + ",";
                }
                childrenHandler = childrenHandler.Trim(',');
                childrenHandler += "]";
                return new Operation
                {
                    ResultIdentifier = $"{counter + allocations + 1}",
                    Operator = "26",
                    FirstOperand = childrenHandler,
                    SecondOperand = fco.FuncName
                };
            }
            else if (node is IdentifierOperation io)
            {
                children = Enumerable.Empty<Operation>().ToList();
                return new Operation
                {
                    ResultIdentifier = $"{counter}",
                    Operator = "27",
                    FirstOperand = io.IdentName
                };
            }
            else if (node is GotoStatement gs)
            {
                children = new List<Operation>();
                var left = SingleOperation(gs.Objective, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "28",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else if (node is IfElseStatement ies)
            {
                children = new List<Operation>();
                var left = SingleOperation(ies.Expression, counter, out var more);
                children.AddRange(more);
                children.Add(left);
                var code = new List<Operation>();
                var if_true = SingleOperation(ies.TrueCode, counter + more.Count, out var if_true_more);
                return new Operation
                {
                    ResultIdentifier = $"{counter + more.Count + 1}",
                    Operator = "29",
                    FirstOperand = left.ResultIdentifier
                };
            }
            else
            {
                children = null;
                return null;
            }
        }
        public Operation[] Parse(Semantic[] nodes)
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
