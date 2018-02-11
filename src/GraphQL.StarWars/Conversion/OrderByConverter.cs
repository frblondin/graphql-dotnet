using GraphQL.StarWars.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphQL.StarWars.Conversion
{
    public static class OrderByConverter
    {
        static readonly MethodInfo _orderByMethodDefinition = (from m in typeof(Queryable).GetMethods()
                                                               where m.Name.Equals("OrderBy")
                                                               where m.GetParameters().Length == 2
                                                               select m).SingleOrDefault();
        static readonly MethodInfo _orderByDescendingMethodDefinition = (from m in typeof(Queryable).GetMethods()
                                                               where m.Name.Equals("OrderByDescending")
                                                               where m.GetParameters().Length == 2
                                                               select m).SingleOrDefault();

        public static IQueryable<TSource> ApplyOrderBy<TSource, TOrderBy>(this IQueryable<TSource> source, TOrderBy orderBy) where TOrderBy : IOrderBy
        {
            var operand = Expression.Parameter(typeof(TSource));
            var property = Expression.Property(operand, orderBy.FieldName);
            var keySelector = Expression.Lambda(
                property,
                operand);

            var methodDefinition = orderBy.Descending ? _orderByDescendingMethodDefinition : _orderByMethodDefinition;
            var result = (IQueryable<TSource>)source.Provider.CreateQuery(Expression.Call(
                null,
                methodDefinition.MakeGenericMethod(typeof(TSource), property.Type),
                source.Expression,
                Expression.Quote(keySelector)));

            if (orderBy.And != null)
            {
                result = ApplyOrderBy(result, (TOrderBy)orderBy.And);
            }

            return result;
        }
    }
}
