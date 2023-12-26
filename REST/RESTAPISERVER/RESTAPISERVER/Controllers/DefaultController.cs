using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace RESTAPISERVER.Controllers
{
    public class DefaultController : ApiController
    {
        static List<Base> Bases = new List<Base>();
        [HttpPut]
        [Route("{BaseName}/{TableName}/Union/{first}/{second}")]
        public IHttpActionResult Union(string BaseName,string TableName,string first,string second)
        {
            foreach(Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    Table sec=null;
                    Table fir=null;
                    foreach(Table t in b.Tables)
                    {
                        if (t.Name.Equals(first))fir = t;
                        if (t.Name.Equals(second)) sec = t;
                    }
                    if (fir == null || sec == null) return BadRequest("Cannot find some table from arguments");
                    bool good = true;
                    foreach(StringField f in fir.Fields)
                    {
                        bool q = false;
                        foreach(StringField f2 in sec.Fields)
                        {
                            if(f.Name.Equals(f2.Name) && f.Type.Equals(f2.Type)) { q = true; break; }
                        }
                        if (!q) { good = false; break; }
                    }
                    if (!good || fir.Fields.Count!=sec.Fields.Count ) return BadRequest("Arguments have different sets of names and types");
                    Dictionary<string, int> pos = new Dictionary<string, int>();
                    List<Tuple<string, string>> NT=new List<Tuple<string, string>>();
                    for (int i = 0; i < fir.Fields.Count; i++) { pos.Add(fir.Fields[i].Name, i); NT.Add(new Tuple<string, string>(fir.Fields[i].Name, fir.Fields[i].Type.ToString())); }
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
                    if (fir.Fields.Count > 0) {
                        for (int i = 0; i < fir.Fields[0].Values.Count; i++)
                        {
                            List<string> Row = new List<string>(new string[NT.Count]);
                            foreach(StringField f in fir.Fields)
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

                    Dictionary<string, string> Links = new Dictionary<string, string>();
                    Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                    Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                    Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                    Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                    Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                    Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                    return Ok(Links);
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPut]
        [Route("{BaseName}/{TableName}/{FieldName}/{pos:int}/{value}")]
        public IHttpActionResult FieldPut(string BaseName, string TableName, string FieldName, int pos,string value)
        {
            foreach (Base b in Bases)
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
                                    if (f.Values.Count <= pos) return BadRequest("There is no row with this index");
                                    if (f.ChangeValue(pos, value) == 0)
                                    {

                                        Dictionary<string, string> Links = new Dictionary<string, string>();
                                        Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                                        Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                                        Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName);
                                        Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                                        Links.Add("GET|GetValue", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName + "/{pos}");
                                        Links.Add("PUT|PutValue", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName + "/" + pos.ToString() + "/{value}");
                                        return Ok(Links);
                                    }
                                    else return BadRequest("Wrong type");
                                }
                            }
                            return BadRequest("There is no field with this name");
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpGet]
        [Route("{BaseName}/{TableName}/{FieldName}/{pos:int}")]
        public IHttpActionResult FieldGet(string BaseName, string TableName, string FieldName,int pos)
        {
            foreach (Base b in Bases)
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
                                    if(f.Values.Count<pos)return BadRequest("There is no row with this index");

                                    Dictionary<string, string> Links = new Dictionary<string, string>();
                                    Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                                    Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName);
                                    Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                                    Links.Add("PUT|PutValue", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName + "/"+pos.ToString()+"/{value}");
                                    return Ok(new { val=f.Values[pos], Links });
                                }
                            }
                            return BadRequest("There is no field with this name");
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpGet]
        [Route("{BaseName}/{TableName}/{FieldName}")]
        public IHttpActionResult FieldGet(string BaseName, string TableName, string FieldName)
        {
            foreach (Base b in Bases)
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

                                    Dictionary<string, string> Links = new Dictionary<string, string>();
                                    Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                                    Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                                    Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                                    Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName);
                                    Links.Add("GET|GetValue", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName+"/{pos}");
                                    Links.Add("PUT|PutValue", "https://localhost:44349/" + BaseName + "/" + TableName + "/" + FieldName + "/{pos}/{value}");
                                    return Ok(new { f.Values, Links });
                                }
                            }
                            return BadRequest("There is no field with this name");
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPut]
        [Route("{BaseName}/{TableName}/Delete/{pos:int}/{k:int}")]
        public IHttpActionResult TableDelete(string BaseName, string TableName,int pos,int k)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            if (t.Fields.Count == 0) return BadRequest("There are no fields in the table"); 
                            if (t.Fields[0].Values.Count < pos + k) return BadRequest("Impossible delete");
                            foreach (StringField f in t.Fields)
                            {
                                f.Values.RemoveRange(pos, k);
                            }

                            Dictionary<string, string> Links = new Dictionary<string, string>();
                            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                            Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                            Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                            Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                            Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                            Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            return Ok(Links);
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPut]
        [Route("{BaseName}/{TableName}/Add")]
        public IHttpActionResult TableAdd(string BaseName,string TableName)
        {
            foreach(Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach(Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName)){
                            if (t.Fields.Count == 0) return BadRequest("There are no fields in the table");
                            t.AddRow();

                            Dictionary<string, string> Links = new Dictionary<string, string>();
                            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                            Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                            Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                            Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                            Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                            Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            return Ok(Links);
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpDelete]
        [Route("{BaseName}/{TableName}/{FieldName}")]
        public IHttpActionResult FieldDelete(string BaseName,string TableName,string FieldName)
        {
            foreach(Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach(Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            foreach(StringField f in t.Fields)
                            {
                                if (f.Name.Equals(FieldName))
                                {
                                    t.Fields.Remove(f);

                                    Dictionary<string, string> Links = new Dictionary<string, string>();
                                    Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                                    Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                                    Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                                    Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                                    Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                                    Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                                    return Ok(Links);
                                }
                            }
                            return BadRequest("There is no field with this name");
                        }
                    }
                    return BadRequest("There is no table with this name");
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPost]
        [Route("{BaseName}/{TableName}/{FieldName}")]
        public IHttpActionResult FieldPost(string BaseName,string TableName,string FieldName, [FromUri] string type)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            foreach(StringField f in t.Fields)
                            {
                                if (f.Name.Equals(FieldName)) return BadRequest("Field with this name exists");
                            }
                            switch (type)
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
                                    return BadRequest("Unknown field type");
                            }
                            if (t.Fields.Count > 1)
                            {
                                while (t.Fields[0].Values.Count > t.Fields.Last().Values.Count)
                                {
                                    t.Fields.Last().addRow();
                                }
                            }

                            Dictionary<string, string> Links = new Dictionary<string, string>();
                            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                            Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                            Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                            Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                            Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/"+FieldName);
                            Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/"+FieldName);
                            return Ok(Links);
                        }
                    }
                    return BadRequest("There is no table with this name");

                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpGet]
        [Route("{BaseName}/{TableName}")]
        public IHttpActionResult TableGet(string BaseName, string TableName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            List < Tuple<Tuple<string, string>, string>> ans= new List<Tuple<Tuple<string, string>, string>>();
                            foreach(StringField f in t.Fields)
                            {
                                Tuple<string, string> q = new Tuple<string, string>(f.Name,f.Type.ToString());
                                string s2="";
                                foreach(string v in f.Values)
                                {
                                    s2+=v+"/";
                                }
                                ans.Add(new Tuple<Tuple<string,string>,string>(q,s2));
                            }

                            Dictionary<string, string> Links = new Dictionary<string, string>();
                            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                            Links.Add("DELETE|DeleteBase", "https://localhost:44349/" + BaseName);
                            Links.Add("DELETE|DeleteTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                            Links.Add("PUT|AddRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Add");
                            Links.Add("PUT|DeleteRow", "https://localhost:44349/" + BaseName + "/" + TableName + "/Delete/{posistion}/{number}");
                            Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                            Links.Add("DELETE|DeleteField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            Links.Add("GET|GetField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}");
                            return Ok( new { ans, Links });
                        }
                    }
                    return BadRequest("There is no table with this name");

                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPost]
        [Route("{BaseName}/{TableName}")]
        public IHttpActionResult TablePost(string BaseName,string TableName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach(Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            return BadRequest("Table with this name exists");
                        }
                    }
                    b.Tables.Add(new Table(TableName));
                    Dictionary<string, string> Links = new Dictionary<string, string>();
                    Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                    Links.Add("DELETE|DeleteBase", "https://localhost:44349/" + BaseName);
                    Links.Add("DELETE|DeleteTable", "https://localhost:44349/" + BaseName + "/"+TableName);
                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/" + TableName);
                    Links.Add("POST|AddField", "https://localhost:44349/" + BaseName + "/" + TableName + "/{FieldName}?type={Type}");
                    return Ok(Links);
                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpDelete]
        [Route("{BaseName}/{TableName}")]
        public IHttpActionResult TableDelete(string BaseName, string TableName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    foreach (Table t in b.Tables)
                    {
                        if (t.Name.Equals(TableName))
                        {
                            b.Tables.Remove(t);
                            Dictionary<string, string> Links = new Dictionary<string, string>();
                            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
                            Links.Add("DELETE|DeleteBase", "https://localhost:44349/" + BaseName);
                            Links.Add("POST|AddTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                            Links.Add("DELETE|DeleteTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                            Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                            Links.Add("PUT|Union", "https://localhost:44349/" + BaseName + "/{NewTableName}/Union/{FirstTableName}/{SecondTableName}");
                            return Ok(Links);
                        }
                    }
                    return BadRequest("There is no table with this name");

                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpPost]
        [Route("{BaseName}")]
        public IHttpActionResult PostBase(string BaseName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    return BadRequest("Base with this name exists");
                }

            }
            Bases.Add(new Base(BaseName));
            Dictionary<string, string> Links = new Dictionary<string, string>();
            Links.Add("DELETE|DeleteBase", "https://localhost:44349/" + BaseName);
            Links.Add("GET|GetBase", "https://localhost:44349/" + BaseName);
            Links.Add("POST|AddTable", "https://localhost:44349/" + BaseName + "/{TableName}");

            return Ok(Links);
        }
        [HttpDelete]
        [Route("{BaseName}")]
        public IHttpActionResult DeleteBase(string BaseName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    Bases.Remove(b);
                    Dictionary<string, string> Links = new Dictionary<string, string>();
                    Links.Add("DELETE|DeleteBase", "https://localhost:44349/BaseName");
                    Links.Add("POST|AddBase", "https://localhost:44349/BaseName");
                    Links.Add("GET|GetBase", "https://localhost:44349/BaseName");
                    return Ok(Links);

                }
            }
            return BadRequest("There is no base with this name");
        }
        [HttpGet]
        [Route("{BaseName}")]
        public IHttpActionResult GetTables(string BaseName)
        {
            foreach (Base b in Bases)
            {
                if (b.Name.Equals(BaseName))
                {
                    List<string> ans = new List<string>();
                    foreach(Table t in b.Tables)
                    {
                        ans.Add(t.Name);
                    }
                    Dictionary<string, string> Links = new Dictionary<string, string>();
                    Links.Add("DELETE|DeleteBase", "https://localhost:44349/" + BaseName);
                    Links.Add("POST|AddTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                    Links.Add("DELETE|DeleteTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                    Links.Add("GET|GetTable", "https://localhost:44349/" + BaseName + "/{TableName}");
                    Links.Add("PUT|Union", "https://localhost:44349/" + BaseName + "/{NewTableName}/Union/{FirstTableName}/{SecondTableName}");

                    return Ok(new Tuple<List<string>,Dictionary<string,string>>( ans,Links));

                }
            }
            return BadRequest("There is no base with this name");
        }


        public class Base
        {
            public List<Table> Tables;
            public string Path;
            public string Name;
            public Base(string n)
            {
                Tables = new List<Table>();
                Name = n;
            }
        }
        public class Table
        {
            public List<StringField> Fields;
            public string Name;
            public Table(string NameVal)
            {
                Name = NameVal;
                Fields = new List<StringField>();
                

            }
            public Table(string NameVal,List<Tuple<string,string>>NamesTypes )
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
                if (Value.Length != 6) return 1;
                string pattern = "[0-9a-fA-F]{6}";
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
                if (Value.Length != 13) return 1;
                string pattern = "[0-9a-fA-F]{6}-[0-9a-fA-F]{6}";
                Match m = Regex.Match(Value, pattern);
                if (m.Success)
                {
                    string s1 = Value.Substring(0, 6).ToUpper();
                    string s2 = Value.Substring(7, 6).ToUpper();
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
