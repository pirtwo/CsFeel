// // FeelParser.cs
// using System;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.Linq;
// using Sprache;

// namespace Feel
// {
//     // ─── AST NODES ───────────────────────────────────────────────

//     public abstract class Expression { }

//     public class Literal : Expression
//     {
//         public object Value { get; }
//         public Literal(object value) => Value = value;
//     }

//     public class Variable : Expression
//     {
//         public string Name { get; }
//         public Variable(string name) => Name = name;
//     }

//     public class ContextExpression : Expression
//     {
//         public IReadOnlyDictionary<string, Expression> Entries { get; }
//         public ContextExpression(IDictionary<string, Expression> entries) =>
//             Entries = new ReadOnlyDictionary<string, Expression>(entries);
//     }

//     public class PropertyAccess : Expression
//     {
//         public Expression Target { get; }
//         public string Property { get; }
//         public PropertyAccess(Expression target, string property) =>
//             (Target, Property) = (target, property);
//     }

//     public class FunctionCall : Expression
//     {
//         public string Name { get; }
//         public IReadOnlyList<Expression> Arguments { get; }
//         public FunctionCall(string name, IEnumerable<Expression> args) =>
//             (Name, Arguments) = (name, args.ToList().AsReadOnly());
//     }

//     public class UnaryExpression : Expression
//     {
//         public string Operator { get; }
//         public Expression Operand { get; }
//         public UnaryExpression(string op, Expression operand) =>
//             (Operator, Operand) = (op, operand);
//     }

//     public class BinaryExpression : Expression
//     {
//         public Expression Left { get; }
//         public string Operator { get; }
//         public Expression Right { get; }
//         public BinaryExpression(Expression left, string op, Expression right) =>
//             (Left, Operator, Right) = (left, op, right);
//     }

//     // ─── PARSER ──────────────────────────────────────────────────

//     public static class FeelParser
//     {
//         // Entry point (using Ref to allow recursion)
//         public static readonly Parser<Expression> Expr = Parse.Ref(() => Logical);

//         // 1) Context literal: { key: Expr, … } or {}
//         static readonly Parser<Expression> Context =
//             from open in Parse.Char('{').Token()
//             from entries in ContextEntry.DelimitedBy(Parse.Char(',').Token())
//             from close in Parse.Char('}').Token()
//             select new ContextExpression(
//                 entries.ToDictionary(kv => kv.Key, kv => kv.Value)
//             );

//         // 1a) One key:value pair
//         static readonly Parser<KeyValuePair<string, Expression>> ContextEntry =
//             from key in Parse.Letter.AtLeastOnce().Text().Token()
//             from colon in Parse.Char(':').Token()
//             from val in Expr
//             select new KeyValuePair<string, Expression>(key, val);

//         // 2) “Atom” parser: functions, parentheses, context, literals, variables
//         static readonly Parser<Expression> Atom =
//                // Function call: name '(' [arg, ...] ')'
//                (from name in Parse.Letter.AtLeastOnce().Text().Token()
//                 from lp in Parse.Char('(').Token()
//                 from args in Expr.DelimitedBy(Parse.Char(',').Token()).Optional()
//                 from rp in Parse.Char(')').Token()
//                 select (Expression)new FunctionCall(name,
//                     args.GetOrElse(Enumerable.Empty<Expression>())))
//             .Or(
//                // Parenthesized
//                from lp in Parse.Char('(').Token()
//                from expr in Expr
//                from rp in Parse.Char(')').Token()
//                select expr
//             )
//             .Or(Context)
//             .Or(
//                // Number literal
//                Parse.Decimal
//                     .Select(x => (Expression)new Literal(decimal.Parse(x)))
//                     .Token()
//             )
//             .Or(
//                // String literal
//                from open in Parse.Char('"')
//                from txt in Parse.CharExcept('"').Many().Text()
//                from close in Parse.Char('"')
//                select (Expression)new Literal(txt)
//             )
//             .Or(
//                // Variable
//                Parse.Letter.AtLeastOnce()
//                     .Text()
//                     .Select(name => (Expression)new Variable(name))
//                     .Token()
//             );

//         // 3) Chained property access: Atom(.prop)*
//         static readonly Parser<Expression> AccessChain =
//             Atom.ChainOperator(
//                 // operator parser: "." then identifier
//                 Parse.Char('.')
//                      .Then(_ => Parse.Letter.AtLeastOnce().Text().Token()),
//                 // aggregator: (propName, target, _) -> new PropertyAccess
//                 (propName, target, _) => new PropertyAccess(target, propName)
//             );

//         // 4) Unary operators on top of access chain
//         static readonly Parser<Expression> Unary =
//             (from ops in Parse.String("not")
//                              .Or(Parse.Char('-').Select(c => c.ToString()))
//                              .Token()
//                              .Many()
//              from term in AccessChain
//              select ops.Reverse()
//                        .Aggregate(term, (expr, op) => new UnaryExpression(op, expr)))
//             .Token();

//         // 5) Binary operators with proper precedence
//         static readonly Parser<Expression> Multiplicative =
//             Unary.ChainOperator(
//                 Parse.Chars('*', '/').Select(c => c.ToString()).Token(),
//                 (op, left, right) => new BinaryExpression(left, op, right)
//             );

//         static readonly Parser<Expression> Additive =
//             Multiplicative.ChainOperator(
//                 Parse.Chars('+', '-').Select(c => c.ToString()).Token(),
//                 (op, left, right) => new BinaryExpression(left, op, right)
//             );

//         static readonly Parser<Expression> Comparison =
//             Additive.ChainOperator(
//                 Parse.String(">=").Or("<=").Or(">").Or("<").Or("==").Or("!=").Token(),
//                 (op, left, right) => new BinaryExpression(left, op, right)
//             );

//         static readonly Parser<Expression> Logical =
//             Comparison.ChainOperator(
//                 Parse.String("and").Or("or").Token(),
//                 (op, left, right) => new BinaryExpression(left, op, right)
//             );
//     }
// }