using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using LFramework.ScriptableObjects;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace LFramework.GoogleSheets
{
    public class GoogleSheetExample : ScriptableObject
    {
        [SerializeField] private TextAsset _key;
        [SerializeField] private string _spreadSheetId;
        [SerializeField] private int _sheetId;
        [SerializeField] private GoogleSheetsConfig _config;

        [SerializeField] private List<GoogleSheetExampleData> _datas = new List<GoogleSheetExampleData>();

        public SheetsService GetService()
        {
            GoogleCredential credential = GoogleCredential.FromJson(_key.text)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Application.productName
            });

            return service;
        }

        [Button]
        public void PushDataToSheet()
        {
            var googleSheets = new GoogleSheets(GetService());
            googleSheets.SpreadSheetId = _spreadSheetId;

            googleSheets.PushObjectCollection(_sheetId, _datas);
        }

        [Button]
        public void PullDataFromSheet()
        {
            var googleSheets = new GoogleSheets(GetService());
            googleSheets.SpreadSheetId = _spreadSheetId;

            googleSheets.PullIntoObjectCollection(_sheetId, _datas, (x) => { return ScriptableObjectHelper.CreateAsset<GoogleSheetExampleData>("Assets/_ROOT", $"{typeof(GoogleSheetExampleData)}_{x}"); });
        }

        [Button]
        public void SetupSheet()
        {
            var googleSheets = new GoogleSheets(GetService());
            googleSheets.SpreadSheetId = _spreadSheetId;

            googleSheets.SetupSheet(_sheetId, _config);
        }
    }
}
