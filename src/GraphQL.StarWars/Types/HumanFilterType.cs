using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.StarWars.Types
{
    public class HumanFilterType : InputObjectGraphType<HumanFilter>
    {
        public HumanFilterType()
        {
            Name = "HumanFilter";
            Field(h => h.Name, nullable: true).Description("The name of the human.");
            Field(h => h.HomePlanet, nullable: true).Description("The home planet of the human.");
            Field(h => h.And, nullable: true, type: typeof(HumanFilterType));
            Field(h => h.Or, nullable: true, type: typeof(HumanFilterType));
        }
    }

    public class HumanOrderByType : InputObjectGraphType<HumanOrderBy>
    {
        public HumanOrderByType()
        {
            Name = nameof(HumanOrderBy);
            Field(h => h.Field, nullable: false, type: typeof(HumanOrderByFieldEnum));
            Field(h => h.Descending, nullable: true);
            Field(h => h.And, nullable: true, type: typeof(HumanOrderByType));
        }
    }

    public class HumanOrderByFieldEnum : EnumerationGraphType
    {
        public HumanOrderByFieldEnum()
        {
            Name = "Field";
            foreach(var value in Enum.GetValues(typeof(HumanOrderByField)))
            {
                var name = Enum.GetName(typeof(HumanOrderByField), value);
                AddValue(name, name, value);
            }
        }
    }
}
