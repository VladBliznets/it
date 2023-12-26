using HotChocolate.Execution;
using Microsoft.AspNetCore.Mvc;

namespace GRAPHQLAPI.Schema
{
    public class Query
    {

        public IEnumerable<Base> GetBases()
        {
            return Syst.Bases;
        }
        public Base GetBaseByName(string name) { 
            foreach(Base b in Syst.Bases)
            {
                if(b.Name.Equals(name))
                {
                    return b;
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }
    }
}
