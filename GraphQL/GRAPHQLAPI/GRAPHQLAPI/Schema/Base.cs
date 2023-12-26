using System.Collections.Generic;

namespace GRAPHQLAPI.Schema
{
    public class Base
    {
        public List<Table> Tables { get; set; } = new List<Table>();
        public string Name { get; set; }

        public Base(string n)
        {
            Name = n;
        }
        public Table GetTableByName(string name)
        {
            foreach (Table t in Tables)
            {
                if (t.Name.Equals(name))
                {
                    return t;
                }
            }
            throw new GraphQLException(new Error("Table not found"));
            return new Table("NO TABLE WITH THIS NAME");
        }
    }
}
