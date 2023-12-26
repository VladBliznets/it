using System.Text.RegularExpressions;

namespace GRAPHQLAPI.Schema
{
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
        public string Name { get; set; }
        public FieldTypes Typ { get; set; }
        public List<string> Values { get; set; } = new List<string>();
        public StringField(string NameVal)
        {
            Name = NameVal;
            Typ = FieldTypes.String;
        }
        [GraphQLIgnore]
        public void addRow()
        {
            Values.Add("");
        }
        [GraphQLIgnore]
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
            Typ = FieldTypes.Int;
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
            Typ = FieldTypes.Real;
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
            Typ = FieldTypes.Char;
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
            Typ = FieldTypes.Color;
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
            Typ = FieldTypes.ColorInterval;
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
