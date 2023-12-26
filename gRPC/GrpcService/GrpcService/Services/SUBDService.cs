using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Protos;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GrpcService.Services
{
    public class SUBDService : GrpcService.Protos.SUBDServiceGRPC.SUBDServiceGRPCBase
    {
        public static Base CurrentBase;
        public static bool IsChanged = false;
        public static Table CurrentTable;
        public override Task<Empty> OpenNew(Empty request, ServerCallContext context)
        {
            CurrentBase = new Base();
            CurrentTable = null;
            IsChanged = true;
            return Task.FromResult(new Empty());
        }
        public override Task<oneindex> OpenBase(onlystring request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                string np = request.Ar;
                string Path = request.Ar;
                Path = "Files/" + Path;
                using (StreamReader sr = new StreamReader(Path))
                {
                    Base NewBase = new Base();
                    NewBase.Path = np;
                    string line;
                    line = sr.ReadLine();
                    int n;
                    if (!int.TryParse(line, out n)) { output.Ar = 2;  return Task.FromResult(output); }
                    for (int i = 0; i < n; i++)
                    {
                        string TableName = sr.ReadLine();
                        List<Tuple<string, string>> args = new List<Tuple<string, string>>();
                        line = sr.ReadLine();
                        int m;
                        if (!int.TryParse(line, out m)) { output.Ar = 2; return Task.FromResult(output); }
                        for (int j = 0; j < m; j++)
                        {
                            line = sr.ReadLine();
                            string Type = sr.ReadLine();
                            args.Add(new Tuple<string, string>(line, Type));
                            StringField f;
                            switch (Type)
                            {
                                case "Int":
                                    f = new IntField(line);
                                    break;
                                case "Real":
                                    f = new IntField(line);
                                    break;
                                case "Char":
                                    f = new IntField(line);
                                    break;
                                case "String":
                                    f = new IntField(line);
                                    break;
                                case "Color":
                                    f = new IntField(line);
                                    break;
                                case "ColorInterval":
                                    f = new IntField(line);
                                    break;
                                default:
                                    output.Ar = 2; 
                                    return Task.FromResult(output);
                            }
                        }
                        Table t = new Table(TableName, args);
                        int l;
                        line = sr.ReadLine();
                        if (!int.TryParse(line, out l)) { output.Ar = 2; return Task.FromResult(output); }
                        for (int j = 0; j < m; j++)
                        {
                            for (int k = 0; k < l; k++)
                            {
                                line = sr.ReadLine();
                                t.Fields[j].addRow();
                                if (t.Fields[j].ChangeValue(k, line) != 0) { output.Ar = 2; return Task.FromResult(output); }
                            }
                        }
                        NewBase.Tables.Add(t);
                    }
                    CurrentBase = NewBase;
                    CurrentTable = null;
                    IsChanged = false;
                }
                output.Ar = 0; return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                output.Ar = 1; return Task.FromResult(output);
            }
        }
        public override Task<logical> BaseOpened(Empty request, ServerCallContext context)
        {
            logical output = new logical();
            output.Ans = (CurrentBase != null);
            return Task.FromResult(output);
        }
        public override Task<logical> TableOpened(Empty request, ServerCallContext context)
        {
            logical output = new logical();
            output.Ans = (CurrentTable != null);
            return Task.FromResult(output);
        }
        public override Task<oneindex> RowsInTable(Empty request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                output.Ar = CurrentTable.Fields[0].Values.Count;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = -1;
                return Task.FromResult(output);
            }
        }
        public override Task<oneindex> ColumnsInTable(Empty request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                output.Ar = CurrentTable.Fields.Count;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = -1;
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetNameInCurrentTable(oneindex request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                output.Ar = CurrentTable.Fields[request.Ar].Name;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetTypeInCurrentTable(oneindex request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                output.Ar = CurrentTable.Fields[request.Ar].Type.ToString();
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetValueInTable(twoindex request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                output.Ar= CurrentTable.Fields[ request.Ar1 ].Values[ request.Ar2 ];
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<oneindex> GetNumberOfTables(Empty request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                output.Ar = CurrentBase.Tables.Count;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = -1;
                return Task.FromResult(output);
            }
        }
        public override Task<oneindex> GetNumberOfFieldsInTable(oneindex request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                output.Ar = CurrentBase.Tables[request.Ar].Fields.Count;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = -1;
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetTableName(oneindex request, ServerCallContext context)
        {
            onlystring output = new onlystring(); 
            try
            {
                output.Ar = CurrentBase.Tables[request.Ar].Name;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetCurrentTableName(Empty request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                output.Ar = CurrentTable.Name;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetNameInTable(twoindex request, ServerCallContext context)
        {

            onlystring output = new onlystring();
            try
            {
                output.Ar= CurrentBase.Tables[request.Ar1].Fields[request.Ar2].Name;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<onlystring> GetTypeInTable(twoindex request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                output.Ar= CurrentBase.Tables[request.Ar1].Fields[request.Ar2].Type.ToString();
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<oneindex> ChangeCurrentTable(oneindex request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            try
            {
                CurrentTable = CurrentBase.Tables[request.Ar];
                output.Ar = 1;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ar = -1;
                return Task.FromResult(output);
            }
        }
        public override Task<logical> BaseHasPath(Empty request, ServerCallContext context)
        {
            logical output = new logical();
            try
            {
                output.Ans = (CurrentBase.Path != "");
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.Ans = false;
                return Task.FromResult(output);
            }
        }
        public override Task<logical> Changed(Empty request, ServerCallContext context)
        {
            logical output = new logical();
            output.Ans = IsChanged;
            return Task.FromResult(output);

        }
        public override Task<oneindex> Save(Empty request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            if (CurrentBase == null)
            {
                output.Ar = 1;
                return Task.FromResult(output);
            }
            SaveAs(CurrentBase.Path);
            IsChanged = false;
            output.Ar = 0;
            return Task.FromResult(output);
        }
        public override Task<onlystring> getAllFiles(Empty request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            try
            {
                string ans = "";
                string[] fileEntries = Directory.GetFiles("Files");
                ans = String.Join("\n", fileEntries);
                output.Ar = ans;
                return Task.FromResult(output);
            }
            catch (System.Exception ex)
            {
                output.Ar = "ERR";
                return Task.FromResult(output);
            }
        }
        public override Task<oneindex> SaveAs(onlystring request, ServerCallContext context)
        {
            oneindex output = new oneindex(); try
            {
                string Path = request.Ar;
                Path = "Files/" + Path;
                using (FileStream fop = File.Create(Path)) { }
                CurrentBase.Path = Path;
                FileStream fs = new FileStream(Path, FileMode.Open);
                StreamWriter stream = new StreamWriter(fs);
                stream.WriteLine((CurrentBase.Tables.Count).ToString());
                foreach (Table t in CurrentBase.Tables)
                {
                    stream.WriteLine(t.Name);
                    stream.WriteLine(t.Fields.Count.ToString());
                    foreach (StringField f in t.Fields)
                    {
                        stream.WriteLine(f.Name);
                        stream.WriteLine(f.Type.ToString());
                    }
                    stream.WriteLine(t.Fields[0].Values.Count);
                    foreach (StringField f in t.Fields)
                    {
                        foreach (string s in f.Values)
                        {
                            stream.WriteLine(s);
                        }
                    }
                }
                stream.Close();
                fs.Close();
                IsChanged = false;
                output.Ar = 0;
                return Task.FromResult(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("!" + ex.Message);
                output.Ar = 1;
                return Task.FromResult(output);
            }
        }
        public int SaveAs(string Path)
        {
            try
            {
                Path = "Files/" + Path;
                using (FileStream fop = File.Create(Path)) { }
                CurrentBase.Path = Path;
                FileStream fs = new FileStream(Path, FileMode.Open);
                StreamWriter stream = new StreamWriter(fs);
                stream.WriteLine((CurrentBase.Tables.Count).ToString());
                foreach (Table t in CurrentBase.Tables)
                {
                    stream.WriteLine(t.Name);
                    stream.WriteLine(t.Fields.Count.ToString());
                    foreach (StringField f in t.Fields)
                    {
                        stream.WriteLine(f.Name);
                        stream.WriteLine(f.Type.ToString());
                    }
                    stream.WriteLine(t.Fields[0].Values.Count);
                    foreach (StringField f in t.Fields)
                    {
                        foreach (string s in f.Values)
                        {
                            stream.WriteLine(s);
                        }
                    }
                }
                stream.Close();
                fs.Close();
                IsChanged = false;
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("!" + ex.Message);
                return 1;
            }
        }
        public override Task<oneindex> CreateTable(table request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            foreach (Table t in CurrentBase.Tables)
            {
                if (t.Name.Equals(request.Name)) { output.Ar = 1; return Task.FromResult(output); }
            }
            var NamesTypes = new List<Tuple<string, string>>();
            for (int i = 0; i < request.Names.Count; i++)
            {
                NamesTypes.Add(new Tuple<string, string>(request.Names[i], request.Types_[i]));
            }
            CurrentBase.Tables.Add(new Table(request.Name, NamesTypes));
            IsChanged = true;
            output.Ar = 0;
            return Task.FromResult(output);
        }
        public override Task<oneindex> OpenTable(onlystring request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            foreach (Table t in CurrentBase.Tables)
            {
                if (t.Name.Equals(request.Ar))
                {
                    CurrentTable = t;
                    output.Ar = 0;
                    return Task.FromResult(output);
                }
            }
            output.Ar = 1;
            return Task.FromResult(output);
        }
        public override Task<oneindex> DeleteTable(onlystring request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            foreach (Table t in CurrentBase.Tables) if (t.Name.Equals(request.Ar)) { CurrentBase.Tables.Remove(t); if (CurrentTable == t) CurrentTable = null; IsChanged = true; output.Ar = 0;
                    return Task.FromResult(output);
                }
            output.Ar = 1;
            return Task.FromResult(output);
        }
        public override Task<Empty> AddRow(Empty request, ServerCallContext context)
        {
            CurrentTable.AddRow();
            IsChanged = true;
            return Task.FromResult(new Empty());
        }
        public override Task<oneindex> DeleteRows(twoindex request, ServerCallContext context)
        {
            oneindex output=new oneindex();
            if (CurrentTable.Fields[0].Values.Count < request.Ar1 + request.Ar2) { output.Ar = 1; return Task.FromResult(output); }
            foreach (StringField f in CurrentTable.Fields)
            {
                f.Values.RemoveRange(request.Ar1, request.Ar2);
            }
            output.Ar = 0;
            return Task.FromResult(output);
        }
        public override Task<oneindex> ChangeRowValue(changevalue request, ServerCallContext context)
        {
            oneindex output=new oneindex(); 
            IsChanged = true;
            output.Ar= CurrentTable.Fields[request.J].ChangeValue(request.I, request.Val);
            return Task.FromResult(output);
        }
        public override Task<onlystring> GetPath(Empty request, ServerCallContext context)
        {
            onlystring output = new onlystring();
            output.Ar= CurrentBase.Path;
            return Task.FromResult(output);
        }
        public override Task<oneindex> Union(union request, ServerCallContext context)
        {
            oneindex output = new oneindex();
            if (CurrentBase == null) { output.Ar = 1; return Task.FromResult(output); }
            if (request.Name.Length == 0) { output.Ar = 2; return Task.FromResult(output); }
            foreach (Table t in CurrentBase.Tables) if (t.Name.Equals(request.Name)) { output.Ar = 3; return Task.FromResult(output); }
            if (request.Args.Count == 0) { output.Ar = 4; return Task.FromResult(output); }
                /*foreach(string s1 in Inputargs)foreach(string s2 in Inputargs)
                    {
                        if (s1 != s2 && s1.Equals(s2)) return 5;
                    }*/
            List<Table> Tablesargs = new List<Table>();

            foreach (string s in request.Args)
            {
                bool exists = false;
                foreach (Table t in CurrentBase.Tables) if (t.Name.Equals(s)) { exists = true; Tablesargs.Add(t); break; }
                if (!exists) { output.Ar = 6; return Task.FromResult(output); }
                }
            for (int i = 0; i < Tablesargs.Count - 1; i++)
            {
                HashSet<Tuple<string, string>> h1 = Tablesargs[i].FieldsSet();
                bool ex = true;
                while (ex)
                {
                    ex = false;
                    foreach (Tuple<string, string> t1 in h1) foreach (Tuple<string, string> t2 in h1) if (t1 != t2 && t1.Equals(t2)) { ex = true; h1.Remove(t2); }
                }
                HashSet<Tuple<string, string>> h2 = Tablesargs[i + 1].FieldsSet();
                ex = true;
                while (ex)
                {
                    ex = false;
                    foreach (Tuple<string, string> t1 in h2) foreach (Tuple<string, string> t2 in h2) if (t1 != t2 && t1.Equals(t2)) { ex = true; h2.Remove(t2); }
                }
                if (h1.Count != h2.Count) { output.Ar = 7; return Task.FromResult(output); }
                foreach (Tuple<string, string> t1 in h1)
                {
                    bool exists = false;
                    foreach (Tuple<string, string> t2 in h2) { if (t1.Equals(t2)) { exists = true; break; } }
                    if (!exists) { output.Ar = 7; return Task.FromResult(output); }
                    }
            }
            List<Tuple<string, string>> NamesTypes = Tablesargs[0].FieldsSet().ToList();
            Dictionary<string, int> pos = new Dictionary<string, int>();
            for (int i = 0; i < NamesTypes.Count; i++) pos.Add(NamesTypes[i].Item1, i);
            Table newTable = new Table(request.Name, NamesTypes);
            List<List<string>> Rows = new List<List<string>>();
            foreach (Table t in Tablesargs)
            {

                for (int i = 0; i < t.Fields[0].Values.Count; i++)
                {
                    List<string> Row = new List<string>(new string[NamesTypes.Count]);
                    foreach (StringField f in t.Fields)
                    {
                        Row[pos[f.Name]] = (f.Values[i]);
                    }
                    bool exists = false;
                    foreach (List<string> r in Rows) if (r.SequenceEqual(Row)) { exists = true; break; }
                    if (exists) continue;
                    for (int j = 0; j < t.Fields.Count; j++)
                    {
                        newTable.Fields[j].Values.Add(Row[j]);
                    }
                    Rows.Add(Row);
                }
            }
            CurrentBase.Tables.Add(newTable);
            IsChanged = true;
            output.Ar = 0;
            return Task.FromResult(output);
        }
        public override Task<Empty> Close(Empty request, ServerCallContext context)
        {
            if (CurrentTable == null)
            {
                CurrentBase = null;
                IsChanged = false;
            }
            else
                CurrentTable = null;
            return Task.FromResult(new Empty());
        }

        public class Base
        {
            public List<Table> Tables;
            public string Path;
            public Base()
            {
                Tables = new List<Table>();
                Path = "";
            }
        }
        public class Table
        {
            public List<StringField> Fields;
            public string Name;
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
            public void AddRow()
            {
                foreach (StringField f in Fields) f.addRow();
            }
            public HashSet<Tuple<string, string>> FieldsSet()
            {
                HashSet<Tuple<string, string>> ans = new HashSet<Tuple<string, string>>();
                foreach (StringField f in Fields) ans.Add(new Tuple<string, string>(f.Name, f.Type.ToString()));
                return ans;
            }
        }
        public enum FieldTypes
        {
            Int,
            Real,
            Char,
            String,
            Color,
            ColorInterval
        }
        public class StringField
        {
            public string Name;
            public FieldTypes Type;
            public List<string> Values = new List<string>();
            public StringField(string NameVal)
            {
                Name = NameVal;
                Type = FieldTypes.String;
            }
            public void addRow()
            {
                Values.Add("");
            }
            public virtual int ChangeValue(int Pos, string Value)
            {
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                Values[Pos] = Value;
                return 0;
            }
        }
        public class IntField : StringField
        {
            public IntField(string NameVal) : base(NameVal)
            {
                Type = FieldTypes.Int;
            }
            public override int ChangeValue(int Pos, string Value)
            {
                if (Value.Length == 0) return 0;
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                int n;
                if (int.TryParse(Value, out n))
                {
                    Values[Pos] = Value;
                    return 0;
                }
                return 1;
            }
        }
        public class RealField : StringField
        {
            public RealField(string NameVal) : base(NameVal)
            {
                Type = FieldTypes.Real;
            }
            public override int ChangeValue(int Pos, string Value)
            {
                if (Value.Length == 0) return 0;
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                double n;
                if (double.TryParse(Value, out n))
                {
                    Values[Pos] = Value;
                    return 0;
                }
                return 1;
            }
        }
        public class CharField : StringField
        {
            public CharField(string NameVal) : base(NameVal)
            {
                Type = FieldTypes.Char;
            }
            public override int ChangeValue(int Pos, string Value)
            {
                if (Value.Length == 0) return 0;
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                char n;
                if (char.TryParse(Value, out n))
                {
                    Values[Pos] = Value;
                    return 0;
                }
                return 1;
            }
        }
        public class ColorField : StringField
        {
            public ColorField(string NameVal) : base(NameVal)
            {
                Type = FieldTypes.Color;
            }
            public override int ChangeValue(int Pos, string Value)
            {
                if (Value.Length == 0) return 0;
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                if (Value.Length != 7) return 1;
                string pattern = "#[0-9a-fA-F]{6}";
                Match m = Regex.Match(Value, pattern);
                if (m.Success)
                {
                    Values[Pos] = Value;
                    return 0;
                }
                return 1;
            }
        }
        public class ColorInvlField : StringField
        {
            public ColorInvlField(string NameVal) : base(NameVal)
            {
                Type = FieldTypes.ColorInterval;
            }
            public override int ChangeValue(int Pos, string Value)
            {
                if (Value.Length == 0) return 0;
                if (Pos >= Values.Count || Pos < 0)
                {
                    return 2;
                }
                if (Value.Length != 15) return 1;
                string pattern = "#[0-9a-fA-F]{6}-#[0-9a-fA-F]{6}";
                Match m = Regex.Match(Value, pattern);
                if (m.Success)
                {
                    string s1 = Value.Substring(1, 6).ToUpper();
                    string s2 = Value.Substring(9, 6).ToUpper();
                    for (int i = 0; i < 6; i++)
                    {
                        if (s1[i] < s2[i]) break;
                        if (s1[i] > s2[i]) return 2;
                    }
                    Values[Pos] = Value;
                    return 0;
                }
                return 1;
            }
        }
    }
}
