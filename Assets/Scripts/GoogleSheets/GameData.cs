using System.Collections.Generic;
using UnityEngine;

namespace GoogleSheets
{
    [System.Serializable]
    public class SerializableRow
    {
        public List<string> rowData = new();
    }

    [System.Serializable]
    public class GameData : ScriptableObject
    {
        public string[] columnNames;
        [SerializeField] private List<SerializableRow> dataRows = new();

        public void ApplyData(string[] newColumnNames, string[,] newData)
        {
            columnNames = newColumnNames;

            dataRows.Clear();
            for (var i = 0; i < newData.GetLength(0); i++)
            {
                var row = new SerializableRow();
            
                for (var j = 0; j < newData.GetLength(1); j++)
                {
                    row.rowData.Add(newData[i, j]);
                }
            
                dataRows.Add(row);
            }

            // 변경된 데이터를 Unity에 알림
            // EditorUtility.SetDirty(this);
        }

        public string[,] GetData()
        {
            if (dataRows.Count == 0 || columnNames == null) return null;

            var result = new string[dataRows.Count, columnNames.Length];
        
            for (var i = 0; i < dataRows.Count; i++)
            {
                for (var j = 0; j < columnNames.Length; j++)
                {
                    result[i, j] = dataRows[i].rowData[j];
                }
            }
        
            return result;
        }

        public List<SerializableRow> GetDataRows()
        {
            return dataRows;
        }
    }
}