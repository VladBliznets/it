
using GRAPHQLAPI.Schema;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddGraphQLServer()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>();
var app = builder.Build();
Table t = new Table("t1");
t.Fields.Add(new IntField("int1"));
Base b = new Base("B1");
b.Tables.Add(t);
Syst.Bases.Add(b);

app.MapGraphQL();

app.Run();



