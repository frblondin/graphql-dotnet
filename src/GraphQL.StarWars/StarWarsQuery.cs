using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.StarWars.Conversion;
using GraphQL.StarWars.Types;
using GraphQL.Types;

namespace GraphQL.StarWars
{
    public class StarWarsQuery : ObjectGraphType<object>
    {
        public StarWarsQuery(StarWarsData data)
        {
            Name = "Query";

            Field<CharacterInterface>("hero", resolve: context => data.GetDroidByIdAsync("3"));
            Field<HumanType>(
                "human",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the human" }
                ),
                resolve: context => data.GetHumanByIdAsync(context.GetArgument<string>("id"))
            );
            Field<ListGraphType<HumanType>>(
                "filterHumans",
                arguments: new QueryArguments(
                    new QueryArgument<HumanFilterType> { Name = "filter", Description = "filter applied on humans" },
                    new QueryArgument<HumanOrderByType> { Name = "orderBy" },
                    new QueryArgument<DecimalGraphType> { Name = "skip" },
                    new QueryArgument<DecimalGraphType> { Name = "top" }
                ),
                resolve: context =>
                {
                    var filter = context.GetArgument<HumanFilter>("filter");
                    var orderBy = context.GetArgument<HumanOrderBy>("orderBy");
                    var skip = context.GetArgument<int?>("skip");
                    var top = context.GetArgument<int?>("top");

                    var query = data.GetHumans()
                        .ApplyFilter(filter)
                        .ApplyOrderBy(orderBy);

                    if (skip.HasValue) query = query.Skip(skip.Value);
                    if (top.HasValue) query = query.Take(top.Value);

                    // TODO: project in anonymous type / tuple required fields to filter
                    // out selected fields

                    return Task.FromResult(query.ToList());
                }
            );

            Func<ResolveFieldContext, string, object> func = (context, id) => data.GetDroidByIdAsync(id);

            FieldDelegate<DroidType>(
                "droid",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
                ),
                resolve: func
            );
        }
    }
}
