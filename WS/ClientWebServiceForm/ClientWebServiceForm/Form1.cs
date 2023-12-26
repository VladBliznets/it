using ClientWebServiceForm.SUBDWebService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ClientWebServiceForm
{
    public partial class Form1 : Form
    {
        bool autoremove = false;
        
        DBMenuSoapClient menu = new DBMenuSoapClient();
        public void Draw()
        {
            autoremove = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            autoremove = false;
            if (menu.TableOpened() )
            {
                List<List<string>> Values = new List<List<string>>();
                int n = menu.RowsInTable();
                n++;
                int m = dataGridView1.ColumnCount = menu.ColumnsInTable();
                Values.Add(new List<string>());
                for(int i=0;i<m;i++)
                {
                    Values[0].Add(menu.GetNameInCurrentTable(i) + "\n" + menu.GetTypeInCurrentTable(i));
                }
                for (int i = 1; i < n; i++)
                {
                    Values.Add(new List<string>());
                    for (int j=0;j<m;j++)
                    {
                        Values[i].Add(menu.GetValueInTable(j,i-1));
                    }
                }
                for (int i = 0; i < m; i++) { dataGridView1.Columns[i].Name = Values[0][i]; dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; } 
                for (int i = 1; i < n; i++) dataGridView1.Rows.Add(Values[i].ToArray());
                dataGridView1.ReadOnly = false;
                //foreach(DataGridViewColumn c in dataGridView1.Columns) c.HeaderCell.ReadOnly = true;
                return;
            }
            else
            {
                int TC = menu.GetNumberOfTables();
                if (TC == 0) return;
                int n = 0;
                int m = TC * 3;
                for (int i=0;i<TC;i++)
                {
                    int FC = menu.GetNumberOfFieldsInTable(i);
                    if (FC > n) n = FC;
                }
                List<List<string>> Values = new List<List<string>>();
                n++;
                Values.Add(new List<string>());
                for (int i=0;i<TC;i++)
                {
                    string TN = menu.GetTableName(i);
                    Values[0].Add(TN);
                    Values[0].Add(TN);
                    Values[0].Add("");
                }
                for (int i = 1; i < n; i++)
                {
                    Values.Add(new List<string>());
                    for (int j=0;j<TC;j++)
                    {
                        if (menu.GetNumberOfFieldsInTable(j) > (i - 1))
                        {
                            Values[i].Add(menu.GetNameInTable(j,i-1) );
                            Values[i].Add(menu.GetTypeInTable(j, i - 1));
                            Values[i].Add("");
                        }
                        else
                        {
                            Values[i].Add("");
                            Values[i].Add("");
                            Values[i].Add("");
                        }
                    }
                }
                dataGridView1.ColumnCount = m;
                for (int i = 0; i < m; i++) { dataGridView1.Columns[i].Name = Values[0][i]; dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; }
                for (int i = 1; i < n; i++) dataGridView1.Rows.Add(Values[i].ToArray());
                dataGridView1.ReadOnly = true;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void New_Click(object sender, EventArgs e)
        {
            if (menu.BaseOpened())
            if (CheckSave() == 1) return;
            autoremove = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            autoremove = false;
            menu.OpenNew();
            richTextBox1.Text = "Base: nameless";
            saveToolStripMenuItem.Visible = true;
            saveAsToolStripMenuItem.Visible = true;
            addTableToolStripMenuItem.Visible = true;
            closeToolStripMenuItem.Visible = true;
            deleteTableToolStripMenuItem.Visible = true;
            unionToolStripMenuItem.Visible = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!menu.TableOpened())
            {
                menu.ChangeCurrentTable(e.ColumnIndex / 3);
                addRowToolStripMenuItem.Visible = true;
                dataGridView1.AllowUserToDeleteRows = true;
                Draw();
                richTextBox1.Text = "Table: " + menu.GetCurrentTableName();
            }
        }

       
        public static DialogResult InputBoxNumber(string title, string promptText, ref Tuple<string,string> value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox nameBox = new TextBox();
            TextBox numBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            label.SetBounds(30, 20, 350, 60);
            nameBox.SetBounds(30, 90, 700, 60);
            numBox.SetBounds(30, 115, 700, 60);
            buttonOk.SetBounds(200, 150, 150, 50);
            buttonCancel.SetBounds(400, 150, 150, 50);
            form.ClientSize = new Size(800, 300);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.AddRange(new Control[] { label, nameBox,numBox, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            DialogResult dialogResult = form.ShowDialog();
            value = new Tuple<string,string>(nameBox.Text,numBox.Text);
            return dialogResult;
        }
        public static DialogResult InputBoxName(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox nameBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            label.SetBounds(30, 20, 350, 60);
            nameBox.SetBounds(30, 90, 700, 60);
            buttonOk.SetBounds(200, 150, 150, 50);
            buttonCancel.SetBounds(400, 150, 150, 50);
            form.ClientSize = new Size(800, 300);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.AddRange(new Control[] { label, nameBox, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            DialogResult dialogResult = form.ShowDialog();
            value = (nameBox.Text);
            return dialogResult;
        }
        public static DialogResult InputBoxNum(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox numBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            label.SetBounds(30, 20, 350, 60);
            numBox.SetBounds(30, 90, 700, 60);
            buttonOk.SetBounds(200, 150, 150, 50);
            buttonCancel.SetBounds(400, 150, 150, 50);
            form.ClientSize = new Size(800, 300);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.AddRange(new Control[] { label,  numBox, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            DialogResult dialogResult = form.ShowDialog();
            value = ( numBox.Text);
            return dialogResult;
        }
        public static DialogResult InputBoxArgs(string title,int n, string promptText, ref List<Tuple<string,string>> value)
        {
            value = new List<Tuple<string, string>>();
            Form form = new Form();
            Label label = new Label();
            List<TextBox> names = new List<TextBox>();
            List<ComboBox> Types = new List<ComboBox>();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            for(int i = 0; i < n; i++)
            {
                TextBox t = new TextBox();
                t.SetBounds(30, 90+35*i, 340, 30);
                names.Add(t);
                ComboBox c = new ComboBox();
                c.SetBounds(380, 90 + 35 * i, 340, 30);
                List<string> TypeNames = new List<string>() { "Int", "Real", "Char", "String", "Color", "ColorInterval" };
                c.DataSource = TypeNames;
                c.DropDownStyle = ComboBoxStyle.DropDownList;
                Types.Add(c);
            }
            label.SetBounds(30, 20, 350, 60);
            buttonOk.SetBounds(200, 150+35*n, 150, 50);
            buttonCancel.SetBounds(400, 150+35*n, 150, 50);
            form.ClientSize = new Size(800, 300);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.AddRange(new Control[] { label, buttonOk, buttonCancel });
            foreach (TextBox t in names) form.Controls.Add(t);
            foreach (ComboBox c in Types) form.Controls.Add(c);
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.AutoScroll= true; 
            DialogResult dialogResult = form.ShowDialog();
            for(int i=0;i<n;i++)value.Add(new Tuple<string, string>(names[i].Text, Types[i].Text));
            return dialogResult;
        }
        public static DialogResult InputBoxArgNames(string title, int n, string promptText, ref List<string> value)
        {
            value = new List<string>();
            Form form = new Form();
            Label label = new Label();
            List<TextBox> names = new List<TextBox>();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            form.Text = title;
            label.Text = promptText;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            for (int i = 0; i < n; i++)
            {
                TextBox t = new TextBox();
                t.SetBounds(30, 90 + 35 * i, 600, 30);
                names.Add(t);
            }
            label.SetBounds(30, 20, 350, 60);
            buttonOk.SetBounds(200, 150 + 35 * n, 150, 50);
            buttonCancel.SetBounds(400, 150 + 35 * n, 150, 50);
            form.ClientSize = new Size(800, 300);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.AddRange(new Control[] { label, buttonOk, buttonCancel });
            foreach (TextBox t in names) form.Controls.Add(t);
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.AutoScroll = true;
            DialogResult dialogResult = form.ShowDialog();
            for (int i = 0; i < n; i++) value.Add(names[i].Text);
            return dialogResult;
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menu.BaseHasPath()) menu.Save();
            else
            {
                DialogResult dr=DialogResult.Yes;
                string FileName="";
                bool Val = false;
                while (dr != DialogResult.Cancel && !Val) {
                    dr = InputBoxName("Збереження", "Введіть назву файлу", ref FileName);
                    Val = !string.IsNullOrEmpty(FileName) && FileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
                }
                
                if (dr == DialogResult.OK)
                {
                    menu.SaveAs(FileName);
                }
            }
        }
    
    private void addTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tuple<string,string> input=new Tuple<string,string>("Name","NaN");
            int n=1;
            string prompt = "Enter the name and the number of fields";
            DialogResult result=DialogResult.OK;
            bool error = true;
            while (result != DialogResult.Cancel && (error) )
            {
                error = false;
                result = InputBoxNumber("Add Table", prompt, ref input);
                prompt = "Enter the name and the number of fields";
                if (input.Item1.Length == 0)
                {
                    prompt = "Error! The name is empty\n" + prompt;
                    error = true;
                }
                int TC = menu.GetNumberOfTables();
                for(int i=0;i<TC;i++)
                {
                    if (menu.GetTableName(i).Equals(input.Item1))
                    {
                        prompt = "Error! The table with this name already exists\n"+prompt;
                        error = true;
                        break;
                    }
                }

                if(!int.TryParse(input.Item2,out n))
                {
                    prompt = "Error! It is not a positive number\n" + prompt;
                    error = true;
                }
                else if (n<=0)
                {
                    prompt = "Error! It is not a positive number\n" + prompt;
                    error = true;
                }
            }
            if (result == DialogResult.Cancel) return;
            string name = input.Item1;
            prompt = "Enter the names and types of fields";
            List<Tuple<string, string>> inputargs=new List<Tuple<string,string>>();
            bool unique=true;
            while(result!= DialogResult.Cancel)
            {
                result = InputBoxArgs("Add Table",n, prompt, ref inputargs);
                bool empty = false;
                foreach(Tuple<string,string> t in inputargs)
                {
                    if (t.Item1.Length == 0)
                    {
                        empty = true;
                        break;
                    }
                }
                if (empty)
                {
                    prompt = "Error! The names must not be empty!\nEnter the names and types of fields";
                    continue;
                }
                unique = true;
                foreach(Tuple<string,string> first  in inputargs)
                {
                    foreach(Tuple<string,string> second in inputargs)
                    {
                        if (first!=second && first.Item1.Equals(second.Item1))
                        {
                            unique = false;
                            break;
                        }
                    }
                    if (!unique) break;
                }
                if (!unique)
                {
                    prompt = "Error! The names must be unique!\nEnter the names and types of fields";
                }
                else
                {
                    List<string> N = new List<string>();
                    List<string> T = new List<string>();
                    for(int i = 0; i < inputargs.Count; i++)
                    {
                        N.Add(inputargs[i].Item1);
                        T.Add(inputargs[i].Item2);
                    }
                    var Narrofstr = new ArrayOfString();
                    Narrofstr.AddRange(N);
                    var Tarrofstr = new ArrayOfString();
                    Tarrofstr.AddRange(T);
                    menu.CreateTable(name, Narrofstr,Tarrofstr);
                    Draw();
                    break;
                    
                }
            }
        }
        private int CheckSave()
        {
            if (!menu.Changed()) return 0;
            switch(MessageBox.Show("Do you want to save last changes?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)){
                case DialogResult.Yes:
                    if (menu.BaseHasPath()) menu.Save();
                    else
                    {
                        DialogResult dr = DialogResult.Yes;
                        string FileName = "";
                        bool Val = false;
                        while (dr != DialogResult.Cancel && !Val)
                        {
                            dr = InputBoxName("Збереження", "Введіть назву файлу", ref FileName);
                            Val = !string.IsNullOrEmpty(FileName) && FileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
                        }
                        if (dr == DialogResult.OK)
                            {

                                menu.SaveAs(FileName);
                            }
                            else return 1;
                        
                    }
                    break;
                case DialogResult.No:
                    return 0;
                    break;
                case DialogResult.Cancel:
                    return 1;
                    break;
            }
            return 0;
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menu.BaseOpened())
                if (CheckSave() == 1) return;
            string possibleNames = menu.getAllFiles();
            DialogResult dr = DialogResult.Yes;
            string FileName = "";
            bool Val = false;
            while (dr != DialogResult.Cancel && !Val)
            {
                dr = InputBoxName("Завантаження", "Введіть назву файлу, вибрану серед:\n"+possibleNames.Replace("Files\\",""), ref FileName);
                Val = false;
                if (dr == DialogResult.OK)
                {
                    foreach (string s in possibleNames.Split('\n'))
                    {
                        if (s.Equals(FileName))
                        {
                            Val = true;
                            break;
                        }
                    }
                }
            }
            if (dr == DialogResult.OK)
                {
                    
                    if (menu.OpenBase(FileName) == 0)
                    {
                        
                        Draw();
                    richTextBox1.Text = "Base: " + menu.GetPath();
                    saveToolStripMenuItem.Visible = true;
                    saveAsToolStripMenuItem.Visible = true;
                    addTableToolStripMenuItem.Visible = true;
                    deleteTableToolStripMenuItem.Visible = true;
                    unionToolStripMenuItem.Visible = true;
                    closeToolStripMenuItem.Visible = true;
                }
                    else
                    {
                        MessageBox.Show("The file is corrupted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.Yes;
            string FileName = "";
            bool Val = false;
            while (dr != DialogResult.Cancel && !Val)
            {
                dr = InputBoxName("Збереження", "Введіть назву файлу", ref FileName);
                Val = !string.IsNullOrEmpty(FileName) && FileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            }
            if (dr == DialogResult.OK)
                {
                    menu.SaveAs(FileName);
                richTextBox1.Text = "Base: " + menu.GetPath();
                }
            
            
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!menu.TableOpened())
            {
                if (CheckSave() == 1) return;
                autoremove = true;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                autoremove = false;
                saveToolStripMenuItem.Visible = false;
                saveAsToolStripMenuItem.Visible = false;
                addTableToolStripMenuItem.Visible = false;
                deleteTableToolStripMenuItem.Visible = false;
                unionToolStripMenuItem.Visible = false;
                closeToolStripMenuItem.Visible = false;
                richTextBox1.Text = "";
            }
            menu.Close();
            if (menu.BaseOpened())
            {
                Draw();
                addRowToolStripMenuItem.Visible = false;
                dataGridView1.AllowUserToDeleteRows = false;
                if (menu.GetPath() == "") richTextBox1.Text = "Base: nameless";
                else richTextBox1.Text = "Base: " + menu.GetPath();
            } 
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menu.AddRow();
            Draw();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!autoremove)
            {
                menu.DeleteRows(e.RowIndex, e.RowCount);
            }
            
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = "";
            string prompt = "Enter the name of the Table to delete";
            DialogResult result = DialogResult.OK;
            bool error = true;
            while (result != DialogResult.Cancel || error)
            {
                error = false;
                result = InputBoxName("Delete Table", prompt, ref input);
                if (result == DialogResult.OK) {
                    if (menu.DeleteTable(input) == 1) { error = true; prompt = "There is no table with this name\nEnter the name of the Table to delete"; continue; }
                    Draw();
                    break;
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!autoremove)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    menu.ChangeRowValue(e.RowIndex, e.ColumnIndex, "");
                    return;
                }
                if (menu.ChangeRowValue(e.RowIndex, e.ColumnIndex, dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()) != 0)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = menu.GetValueInTable(e.ColumnIndex, e.RowIndex);
                    MessageBox.Show("The value is wrong for this type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void unionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tuple<string, string> input = new Tuple<string, string>("Name", "NaN");
            int n = 1;
            string prompt = "Enter the name of the new table and the number of arguments";
            DialogResult result = DialogResult.OK;
            bool error = true;
            while (result != DialogResult.Cancel && (error))
            {
                error = false;
                result = InputBoxNumber("Union", prompt, ref input);
                prompt = "Enter the name of the new table and the number of arguments";
                if (input.Item1.Length == 0)
                {
                    prompt = "Error! The name is empty\n" + prompt;
                    error = true;
                }
                int TC = menu.GetNumberOfTables();
                for (int i=0;i<TC;i++)
                {
                    if (menu.GetTableName(i).Equals(input.Item1))
                    {
                        prompt = "Error! The table with this name already exists\n" + prompt;
                        error = true;
                        break;
                    }
                }

                if (!int.TryParse(input.Item2, out n))
                {
                    prompt = "Error! It is not a positive number\n" + prompt;
                    error = true;
                }
                else if (n <= 0)
                {
                    prompt = "Error! It is not a positive number\n" + prompt;
                    error = true;
                }
            }
            if (result == DialogResult.Cancel) return;
            string name = input.Item1;

            prompt = "Enter the names of arguments";
            List<string> inputargs = new List<string>();
            error = true;
            while (result != DialogResult.Cancel && error)
            {
                result = InputBoxArgNames("Union", n, prompt, ref inputargs);
                error = false;
                var arrofstr = new ArrayOfString();
                arrofstr.AddRange(inputargs);
                int res=menu.Union(name, arrofstr);
                switch (res)
                {
                    case 6:
                        error = true;
                        prompt = "Some of the arguments does not exist\nEnter the names of arguments";
                        break;
                    case 7:
                        error = true;
                        prompt = "Arguments have different sets of fields\nEnter the names of arguments";
                        break;
                    default:
                        Draw();
                        break;
                }
            }
        }

    }
}
