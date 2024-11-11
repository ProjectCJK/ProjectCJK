using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleSheets
{
    [Serializable]
    public class SerializableRow
    {
        public List<string> rowData = new();
    }

    [Serializable]
    public class GameData : ScriptableObject
    {
        public string[] columnNames;
        [SerializeField] private List<SerializableRow> dataRows = new();

        // 캐시된 데이터를 저장하는 필드
        private string[,] cachedData;

        public void ApplyData(string[] newColumnNames, string[,] newData)
        {
            columnNames = newColumnNames;
            dataRows.Clear();

            for (var i = 0; i < newData.GetLength(0); i++)
            {
                var row = new SerializableRow();
                for (var j = 0; j < newData.GetLength(1); j++) row.rowData.Add(newData[i, j]);
                dataRows.Add(row);
            }

            // 데이터가 변경되었으므로 캐시 초기화
            cachedData = null;
            // 변경된 데이터를 Unity에 알림
            // EditorUtility.SetDirty(this);
        }

        public string[,] GetData()
        {
            // 캐시가 존재하면 캐시된 데이터를 반환
            if (cachedData != null) return cachedData;

            // 데이터가 없으면 null 반환
            if (dataRows.Count == 0 || columnNames == null) return null;

            // 새 배열을 생성하고 캐시에 저장
            cachedData = new string[dataRows.Count, columnNames.Length];

            for (var i = 0; i < dataRows.Count; i++)
            for (var j = 0; j < columnNames.Length; j++)
                cachedData[i, j] = dataRows[i].rowData[j];

            return cachedData;
        }

        public List<SerializableRow> GetDataRows()
        {
            return dataRows;
        }
    }
}