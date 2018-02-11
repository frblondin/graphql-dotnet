using GraphQL.StarWars.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphQL.StarWars.Conversion
{
    public static class FilterConverter
    {
        public static IQueryable<TSource> ApplyFilter<TSource, TFilter>(this IQueryable<TSource> source, TFilter filter) where TFilter : IFilter
        {
            var predicate = Convert<TSource, TFilter>(filter);
            return predicate != null ? source.Where(predicate) : source;
        }

        static Expression<Func<TSource, bool>> Convert<TSource, TFilter>(TFilter filter) where TFilter : IFilter
        {
            if (filter == null) return null;

            var operand = Expression.Parameter(typeof(TSource));
            var result = Convert(filter, operand);
            return result == null ? null : Expression.Lambda<Func<TSource, bool>>(
                result,
                operand);
        }

        static Expression Convert<TFilter>(TFilter filter, ParameterExpression operand) where TFilter : IFilter
        {
            var result = default(Expression);
            foreach (var property in typeof(TFilter).GetProperties())
            {
                if (property.Name.Equals("And") || property.Name.Equals("Or")) continue;

                var value = property.GetValue(filter);
                if (value == null) continue;

                var condition = Expression.Equal(
                    Expression.Property(operand, property.Name),
                    Expression.Constant(value));
                result = result != null ? Expression.AndAlso(result, condition) : condition;
            }

            if (filter.And != null)
            {
                var and = Convert((TFilter)filter.And, operand);
                result = result != null ? Expression.AndAlso(result, and) : and;
            }
            if (filter.Or != null)
            {
                var or = Convert((TFilter)filter.Or, operand);
                result = result != null ? Expression.OrElse(result, or) : or;
            }

            return result;
        }
    }
}
