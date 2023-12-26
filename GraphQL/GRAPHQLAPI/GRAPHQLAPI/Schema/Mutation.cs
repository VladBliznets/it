namespace GRAPHQLAPI.Schema
{
    public class Mutation
    {
        public string CreateBase(string name)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(name))
                {
                    throw new GraphQLException(new Error("Base with this name exists"));
                }

            }
            Base ba = new Base(name);
            Syst.Bases.Add(ba);
            return "Ok";
        }
        public string DeleteBase(string BaseName)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    Syst.Bases.Remove(b);
                    return "Ok";

                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
            return null;
        }
        public string DeleteTable(string BaseName, string TableName)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            b.Tables.Remove(t);
                            return "Ok";
                        }
                    }
                    return "Ok. There was no table with this name";

                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }
        public string CreateTable(string BaseName, string TableName)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            throw new GraphQLException(new Error("Table with this name exists"));
                        }
                    }
                    b.Tables.Add(new Table(TableName));
                    return "Ok";
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }

        public string CreateField(string BaseName, string TableName, string FieldName, string typ)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            foreach (StringField f in t.Fields)
                            {
                                if (f.Name.Equals(FieldName)) throw new GraphQLException(new Error("Field with this name exists"));
                            }
                            switch (typ)
                            {
                                case "Int":
                                    t.Fields.Add(new IntField(FieldName));
                                    break;
                                case "Real":
                                    t.Fields.Add(new RealField(FieldName));
                                    break;
                                case "Char":
                                    t.Fields.Add(new CharField(FieldName));
                                    break;
                                case "String":
                                    t.Fields.Add(new StringField(FieldName));
                                    break;
                                case "Color":
                                    t.Fields.Add(new ColorField(FieldName));
                                    break;
                                case "ColorInterval":
                                    t.Fields.Add(new ColorInvlField(FieldName));
                                    break;
                                default:
                                    throw new GraphQLException(new Error("Unknown field type"));
                            }
                            if (t.Fields.Count > 1)
                            {
                                while (t.Fields[0].Values.Count > t.Fields.Last().Values.Count)
                                {
                                    t.Fields.Last().addRow();
                                }
                            }
                            return "Ok";
                        }
                    }
                    throw new GraphQLException(new Error("There is no table with this name"));

                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }

        public string FieldDelete(string BaseName, string TableName, string FieldName)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            foreach (StringField f in t.Fields)
                            {
                                if (f.Name.Equals(FieldName))
                                {
                                    t.Fields.Remove(f);
                                    return "Ok";
                                }
                            }
                            throw new GraphQLException(new Error("There is no field with this name"));
                        }
                    }
                    throw new GraphQLException(new Error("There is no table with this name"));
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }

        public string TableAddRow(string BaseName, string TableName)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            if (t.Fields.Count == 0) throw new GraphQLException(new Error("There are no fields in the table"));
                            t.AddRow();
                            return "Ok";
                        }
                    }
                    throw new GraphQLException(new Error("There is no table with this name"));
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }

        public string TableDeleteRow(string BaseName, string TableName, int pos, int k)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            if (t.Fields.Count == 0) throw new GraphQLException(new Error("There are no fields in the table"));
                            if (t.Fields[0].Values.Count < pos + k) throw new GraphQLException(new Error("Impossible delete"));
                            foreach (StringField f in t.Fields)
                            {
                                f.Values.RemoveRange(pos, k);
                            }
                            return "Ok";
                        }
                    }
                    throw new GraphQLException(new Error("There is no table with this name"));
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }
        public string FieldPut(string BaseName, string TableName, string FieldName, int pos, string value)
        {
            foreach (Base b in Syst.Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            foreach (StringField f in t.Fields)
                            {
                                if (f.Name.Equals(FieldName))
                                {
                                    if (f.Values.Count <= pos) throw new GraphQLException(new Error("There is no row with this index"));
                                    if (f.ChangeValue(pos, value) == 0)
                                    {
                                        return "Ok";
                                    }
                                    else throw new GraphQLException(new Error("Wrong type"));
                                }
                            }
                            throw new GraphQLException(new Error("There is no field with this name"));
                        }
                    }
                    throw new GraphQLException(new Error("There is no table with this name"));
                }
            }
            throw new GraphQLException(new Error("There is no base with this name"));
        }
    
    public string UnionOperation(string BaseName, string TableName, string first, string second)
    {
        foreach (Base b in Syst.Bases)
        {
            if (b.Name.Equals(BaseName))
            {
                Table sec = null;
                Table fir = null;
                foreach (Table t in b.Tables)
                {
                    if (t.Name.Equals(first)) fir = t;
                    if (t.Name.Equals(second)) sec = t;
                }
                if (fir == null || sec == null) throw new GraphQLException(new Error("Cannot find some table from arguments"));
                bool good = true;
                foreach (StringField f in fir.Fields)
                {
                    bool q = false;
                    foreach (StringField f2 in sec.Fields)
                    {
                        if (f.Name.Equals(f2.Name) && f.Typ.Equals(f2.Typ)) { q = true; break; }
                    }
                    if (!q) { good = false; break; }
                }
                if (!good || fir.Fields.Count != sec.Fields.Count) throw new GraphQLException(new Error("Arguments have different sets of names and types"));
                Dictionary<string, int> pos = new Dictionary<string, int>();
                List<Tuple<string, string>> NT = new List<Tuple<string, string>>();
                for (int i = 0; i < fir.Fields.Count; i++) { pos.Add(fir.Fields[i].Name, i); NT.Add(new Tuple<string, string>(fir.Fields[i].Name, fir.Fields[i].Typ.ToString())); }
                foreach (Table t in b.Tables)
                {
                    if (t.Name.Equals(TableName))
                    {
                        b.Tables.Remove(t);
                        break;
                    }
                }
                Table newt = new Table(TableName, NT);
                List<List<string>> Rows = new List<List<string>>();
                if (fir.Fields.Count > 0)
                {
                    for (int i = 0; i < fir.Fields[0].Values.Count; i++)
                    {
                        List<string> Row = new List<string>(new string[NT.Count]);
                        foreach (StringField f in fir.Fields)
                        {
                            Row[pos[f.Name]] = f.Values[i];
                        }
                        bool exists = false;
                        foreach (List<string> r in Rows) if (r.SequenceEqual(Row)) { exists = true; break; }
                        if (exists) continue;
                        for (int j = 0; j < fir.Fields.Count; j++)
                        {
                            newt.Fields[j].Values.Add(Row[j]);
                        }
                        Rows.Add(Row);
                    }
                    for (int i = 0; i < sec.Fields[0].Values.Count; i++)
                    {
                        List<string> Row = new List<string>(new string[NT.Count]);
                        foreach (StringField f in sec.Fields)
                        {
                            Row[pos[f.Name]] = f.Values[i];
                        }
                        bool exists = false;
                        foreach (List<string> r in Rows) if (r.SequenceEqual(Row)) { exists = true; break; }
                        if (exists) continue;
                        for (int j = 0; j < sec.Fields.Count; j++)
                        {
                            newt.Fields[j].Values.Add(Row[j]);
                        }
                        Rows.Add(Row);
                    }
                }
                b.Tables.Add(newt);
                return "Ok";
            }
        }
        throw new GraphQLException(new Error("There is no base with this name"));
    }
}
}
