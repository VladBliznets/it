using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SUBDService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISUBDService
    {
        [OperationContract]
        void OpenNew();
        [OperationContract]
        int OpenBase(string Path);
        [OperationContract]

        bool BaseOpened();
        [OperationContract]
        bool TableOpened();
        [OperationContract]
        int RowsInTable();
        [OperationContract]
        int ColumnsInTable();
        [OperationContract]
        string GetNameInCurrentTable(int i);
        [OperationContract]
        string GetTypeInCurrentTable(int i);
        [OperationContract]
        string GetValueInTable(int i, int j);
        [OperationContract]
        int GetNumberOfTables();
        [OperationContract]
        int GetNumberOfFieldsInTable(int i);
        [OperationContract]
        string GetTableName(int i);
        [OperationContract]
        string GetCurrentTableName();
        [OperationContract]
        string GetNameInTable(int i, int j);
        [OperationContract]
        string GetTypeInTable(int i, int j);
        [OperationContract]
        int ChangeCurrentTable(int i);
        [OperationContract]
        bool BaseHasPath();
        [OperationContract]
        bool Changed();
        [OperationContract]
        int Save();
        [OperationContract]
        string getAllFiles();
        [OperationContract]
        int SaveAs(string Path);
        [OperationContract]
        int CreateTable(string Name, List<string>Names ,List<string> Types);
        [OperationContract]
        int OpenTable(string Name);
        [OperationContract]
        int DeleteTable(string Name);
        [OperationContract]
        void AddRow();
        [OperationContract]
        int DeleteRows(int PosRow, int num);
        [OperationContract]
        int ChangeRowValue(int RowPos, int ColumnPos, string value);
        [OperationContract]
        string GetPath();
        [OperationContract]
        int Union(string name, List<string> Inputargs);
        [OperationContract]
        void Close();
    }
}
