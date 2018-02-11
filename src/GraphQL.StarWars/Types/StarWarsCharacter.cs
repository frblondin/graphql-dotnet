using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.StarWars.Types
{
    public abstract class StarWarsCharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
        public int[] AppearsIn { get; set; }
    }

    public class Human : StarWarsCharacter
    {
        public string HomePlanet { get; set; }
    }

    public interface IFilter
    {
        object And { get; }
        object Or { get; }
    }

    public abstract class Filter<TFilter> : IFilter
    {
        object IFilter.And => And;
        public TFilter And { get; set; }
        object IFilter.Or => Or;
        public TFilter Or { get; set; }
    }

    public class HumanFilter : Filter<HumanFilter>
    {
        public string HomePlanet { get; set; }
        public string Name { get; set; }
    }

    public enum HumanOrderByField
    {
        Name,
        HomePlanet
    }

    public interface IOrderBy
    {
        string FieldName { get; }
        object And { get; }
        bool Descending { get; set; }
    }

    public abstract class OrderBy<TOrderBy> : IOrderBy
    {
        public abstract string FieldName { get; }
        public bool Descending { get; set; }

        object IOrderBy.And => And;
        public TOrderBy And { get; set; }
    }

    public class HumanOrderBy : OrderBy<HumanOrderBy>
    {
        public override string FieldName => Field.ToString();
        public HumanOrderByField Field { get; set; }
    }

    public class Droid : StarWarsCharacter
    {
        public string PrimaryFunction { get; set; }
    }
}
