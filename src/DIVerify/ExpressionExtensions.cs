using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DIVerify {
    internal static class ExpressionExtensions {
        
        public static Expression<Func<T, bool>> CombineWithAnd<T>(this Expression<Func<T, bool>> original, Expression<Func<T, bool>> expressionToAdd) 
            => original.Combine(expressionToAdd, Expression.AndAlso);

        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> original, Expression<Func<T, bool>> expressionToAdd, Func<Expression, Expression, BinaryExpression> combiner) 
            => Expression.Lambda<Func<T, bool>>(
                combiner(original.Body, new ExpressionParameterReplacer(expressionToAdd.Parameters, original.Parameters).Visit(expressionToAdd.Body)),
                original.Parameters);

        private class ExpressionParameterReplacer : ExpressionVisitor {
            private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters) {
                ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

                for (var i = 0; i != fromParameters.Count && i != toParameters.Count; i++) {
                    ParameterReplacements.Add(fromParameters[i], toParameters[i]);
                }
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                if (ParameterReplacements.TryGetValue(node, out var replacement)) {
                    node = replacement;
                }

                return base.VisitParameter(node);
            }
        }
    }
}
