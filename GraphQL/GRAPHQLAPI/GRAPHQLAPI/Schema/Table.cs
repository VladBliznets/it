namespace GRAPHQLAPI.Schema
{
    public class Table
    {
        public List<StringField> Fields { get; set; } = new List<StringField>();
        public string Name { get; set; }
        public Table(string NameVal)
        {
            Name = NameVal;
            Fields = new List<StringField>();


        }
        public StringField GetFieldByName(string name)
        {
            foreach (StringField f in Fields)
            {
                if (f.Name.Equals(name))
                {
                    return f;
                }
            }
            throw new GraphQLException(new Error("Field not found"));
            return new StringField("NO FIELD WITH THIS NAME");
        }
        public Table(string NameVal, List<Tuple<string, string>> NamesTypes)
        {
            Name = NameVal;
            Fields = new List<StringField>();
            foreach (Tuple<string, string> nametype in NamesTypes)
            {
                switch (nametype.Item2)
                {
                    case "Int":
                        Fields.Add(new IntField(nametype.Item1));
                        break;
                    case "Real":
                        Fields.Add(new RealField(nametype.Item1));
                        break;
                    case "Char":
                        Fields.Add(new CharField(nametype.Item1));
                        break;
                    case "String":
                        Fields.Add(new StringField(nametype.Item1));
                        break;
                    case "Color":
                        Fields.Add(new ColorField(nametype.Item1));
                        break;
                    case "ColorInterval":
                        Fields.Add(new ColorInvlField(nametype.Item1));
                        break;
                }
            }

        }
        [GraphQLIgnore]
        public void AddRow()
        {
            foreach (StringField f in Fields) f.addRow();
        }
        [GraphQLIgnore]
        public HashSet<Tuple<string, string>> FieldsSet()
        {
            HashSet<Tuple<string, string>> ans = new HashSet<Tuple<string, string>>();
            foreach (StringField f in Fields) ans.Add(new Tuple<string, string>(f.Name, f.Typ.ToString()));
            return ans;
        }
    }
}
