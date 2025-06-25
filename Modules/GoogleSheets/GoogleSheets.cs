using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Apis.Requests;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using LFramework.ScriptableObjects;
using UnityEngine;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;
using Data = Google.Apis.Sheets.v4.Data;

namespace LFramework.GoogleSheets
{
    /// <summary>
    /// Provides an interface for syncing localization data to a Google Sheet.
    /// </summary>
    public class GoogleSheets
    {
        /// <summary>
        /// The sheets provider is responsible for providing the SheetsService and configuring the type of access.
        /// <seealso cref="SheetsServiceProvider"/>.
        /// </summary>
        public SheetsService SheetsService { get; private set; }

        /// <summary>
        /// The Id of the Google Sheet. This can be found by examining the url:
        /// https://docs.google.com/spreadsheets/d/<b>SpreadsheetId</b>/edit#gid=<b>SheetId</b>
        /// Further information can be found <see href="https://developers.google.com/sheets/api/guides/concepts#spreadsheet_id">here.</see>
        /// </summary>
        public string SpreadSheetId { get; set; }

        /// <summary>
        /// Creates a new instance of a GoogleSheets connection.
        /// </summary>
        /// <param name="service">The Google Sheets service provider. See <see cref="SheetsServiceProvider"/> for a default implementation.</param>
        public GoogleSheets(SheetsService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            SheetsService = service;
        }

        /// <summary>
        /// Opens the spreadsheet in a browser.
        /// </summary>
        /// <param name="spreadSheetId"></param>
        public static void OpenSheetInBrowser(string spreadSheetId) => Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/");

        /// <summary>
        /// Opens the spreadsheet with the sheet selected in a browser.
        /// </summary>
        /// <param name="spreadSheetId"></param>
        /// <param name="sheetId"></param>
        public static void OpenSheetInBrowser(string spreadSheetId, int sheetId) => Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/#gid={sheetId}");

        /// <summary>
        /// Creates a new Google Spreadsheet.
        /// </summary>
        /// <param name="spreadSheetTitle">The title of the Spreadsheet.</param>
        /// <param name="sheetTitle">The title of the sheet(tab) that is part of the Spreadsheet.</param>
        /// <param name="config"></param>
        /// <returns>Returns the new Spreadsheet and sheet id.</returns>
        public (string spreadSheetId, int sheetId) CreateSpreadsheet(string spreadSheetTitle, string sheetTitle, GoogleSheetsConfig config)
        {
            try
            {
                var createRequest = SheetsService.Spreadsheets.Create(new Spreadsheet
                {
                    Properties = new SpreadsheetProperties
                    {
                        Title = spreadSheetTitle
                    },
                    Sheets = new Sheet[]
                    {
                        new Sheet
                        {
                            Properties = new SheetProperties
                            {
                                Title = sheetTitle,
                            }
                        }
                    }
                });

                var createResponse = ExecuteRequest<Spreadsheet, CreateRequest>(createRequest);
                SpreadSheetId = createResponse.SpreadsheetId;
                var sheetId = createResponse.Sheets[0].Properties.SheetId.Value;

                SetupSheet(sheetId, config);

                return (SpreadSheetId, sheetId);
            }
            catch (Exception e)
            {
                LDebug.LogError<GoogleSheets>(e);
                throw;
            }
        }

        /// <summary>
        /// Creates a new sheet within the Spreadsheet with the id <see cref="SpreadSheetId"/>.
        /// </summary>
        /// <param name="title">The title for the new sheet</param>
        /// <param name="config">The settings to apply to the new sheet.</param>
        /// <returns>The new sheet id.</returns>
        public int AddSheet(string title, GoogleSheetsConfig config)
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"{nameof(SpreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var createRequest = new Request()
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties { Title = title }
                }
            };

            var batchUpdateReqTask = SendBatchUpdateRequest(createRequest);
            var sheetId = batchUpdateReqTask.Replies[0].AddSheet.Properties.SheetId.Value;
            SetupSheet(sheetId, config);
            return sheetId;
        }

        /// <summary>
        /// Returns a list of all the sheets in the Spreadsheet with the id <see cref="SpreadSheetId"/>.
        /// </summary>
        /// <returns>The sheets names and id's.</returns>
        public List<(string name, int id)> GetSheets()
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"The {nameof(SpreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");

            var sheets = new List<(string name, int id)>();
            var spreadsheetInfoRequest = SheetsService.Spreadsheets.Get(SpreadSheetId);
            var sheetInfoReq = ExecuteRequest<Spreadsheet, GetRequest>(spreadsheetInfoRequest);

            foreach (var sheet in sheetInfoReq.Sheets)
            {
                sheets.Add((sheet.Properties.Title, sheet.Properties.SheetId.Value));
            }

            return sheets;
        }

        /// <summary>
        /// Returns all the column titles(values from the first row) for the selected sheet inside of the Spreadsheet with id <see cref="SpreadSheetId"/>.
        /// This method requires the <see cref="SheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
        /// </summary>
        /// <param name="sheetId">The sheet id.</param>
        /// <returns>All the </returns>
        public IList<string> GetColumnTitles(int sheetId)
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"{nameof(SpreadSheetId)} is required.");

            var batchGetValuesByDataFilterRequest = new BatchGetValuesByDataFilterRequest
            {
                DataFilters = new DataFilter[1]
                {
                    new DataFilter
                    {
                        GridRange = new GridRange
                        {
                            SheetId = sheetId,
                            StartRowIndex = 0,
                            EndRowIndex = 1
                        }
                    }
                }
            };

            var request = SheetsService.Spreadsheets.Values.BatchGetByDataFilter(batchGetValuesByDataFilterRequest, SpreadSheetId);
            var result = ExecuteRequest<BatchGetValuesByDataFilterResponse, ValuesResource.BatchGetByDataFilterRequest>(request);

            var titles = new List<string>();
            if (result?.ValueRanges?.Count > 0 && result.ValueRanges[0].ValueRange.Values != null)
            {
                foreach (var row in result.ValueRanges[0].ValueRange.Values)
                {
                    foreach (var col in row)
                    {
                        titles.Add(col.ToString());
                    }
                }
            }
            return titles;
        }

        /// <summary>
        /// Asynchronous version of <see cref="GetRowCount"/>
        /// <inheritdoc cref="GetRowCount"/>
        /// </summary>
        /// <param name="sheetId">The sheet to get the row count from</param>
        /// <returns>The row count for the sheet.</returns>
        public async Task<int> GetRowCountAsync(int sheetId)
        {
            var rowCountRequest = GenerateGetRowCountRequest(sheetId);
            var task = ExecuteRequestAsync<Spreadsheet, GetByDataFilterRequest>(rowCountRequest);
            await task.ConfigureAwait(true);

            if (task.Result.Sheets == null || task.Result.Sheets.Count == 0)
                throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");
            return task.Result.Sheets[0].Properties.GridProperties.RowCount.Value;
        }

        /// <summary>
        /// Returns the total number of rows in the sheet inside of the Spreadsheet with id <see cref="SpreadSheetId"/>.
        /// This method requires the <see cref="SheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
        /// </summary>
        /// <param name="sheetId">The sheet to get the row count from.</param>
        /// <returns>The row count for the sheet.</returns>
        public int GetRowCount(int sheetId)
        {
            var rowCountRequest = GenerateGetRowCountRequest(sheetId);
            var response = ExecuteRequest<Spreadsheet, GetByDataFilterRequest>(rowCountRequest);

            if (response.Sheets == null || response.Sheets.Count == 0)
                throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");
            return response.Sheets[0].Properties.GridProperties.RowCount.Value;
        }

        private GetByDataFilterRequest GenerateGetRowCountRequest(int sheetId)
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"{nameof(SpreadSheetId)} is required.");

            return SheetsService.Spreadsheets.GetByDataFilter(new GetSpreadsheetByDataFilterRequest
            {
                DataFilters = new DataFilter[]
                {
                    new DataFilter
                    {
                        GridRange = new GridRange
                        {
                            SheetId = sheetId,
                        },
                    },
                },
            }, SpreadSheetId);
        }

        public void SetupSheet(int sheetId, GoogleSheetsConfig config)
        {
            var requests = new List<Request>
            {
                SetTitleStyle(sheetId, config),
                FreezeTitleRowAndKeyColumn(sheetId),
                HighlightDuplicateKeys(sheetId, config)
            };

            if (requests.Count > 0)
                SendBatchUpdateRequest(requests);
        }

        private Request FreezeTitleRowAndKeyColumn(int sheetId)
        {
            return new Request()
            {
                UpdateSheetProperties = new UpdateSheetPropertiesRequest
                {
                    Fields = "GridProperties.FrozenRowCount,GridProperties.FrozenColumnCount,",
                    Properties = new SheetProperties
                    {
                        SheetId = sheetId,
                        GridProperties = new GridProperties
                        {
                            FrozenRowCount = 1,
                            FrozenColumnCount = 1
                        }
                    }
                }
            };
        }

        private Request HighlightDuplicateKeys(int sheetId, GoogleSheetsConfig config)
        {
            return new Request
            {
                // Highlight duplicates in the A(Key) field
                AddConditionalFormatRule = new AddConditionalFormatRuleRequest
                {
                    Rule = new ConditionalFormatRule
                    {
                        BooleanRule = new BooleanRule
                        {
                            Condition = new BooleanCondition
                            {
                                Type = "CUSTOM_FORMULA",
                                Values = new[] { new ConditionValue { UserEnteredValue = "=countif(A:A;A1)>1" } }
                            },
                            Format = new CellFormat { BackgroundColor = UnityColorToDataColor(config.KeyDuplicateColor) }
                        },
                        Ranges = new[]
                        {
                            new GridRange
                            {
                                SheetId = sheetId,
                                EndColumnIndex = 1
                            }
                        }
                    }
                },
            };
        }

        private Request SetTitleStyle(int sheetId, GoogleSheetsConfig config)
        {
            return new Request
            {
                // Header style
                RepeatCell = new RepeatCellRequest
                {
                    Fields = "*",
                    Range = new GridRange
                    {
                        SheetId = sheetId,
                        StartRowIndex = 0,
                        EndRowIndex = 1,
                    },
                    Cell = new CellData
                    {
                        UserEnteredFormat = new CellFormat
                        {
                            BackgroundColor = UnityColorToDataColor(config.HeaderBackgroundColor),
                            TextFormat = new TextFormat
                            {
                                Bold = true,
                                ForegroundColor = UnityColorToDataColor(config.HeaderForegroundColor)
                            }
                        }
                    }
                }
            };
        }

        private Request ResizeRow(int sheetId, int newSize)
        {
            return new Request
            {
                UpdateSheetProperties = new UpdateSheetPropertiesRequest
                {
                    Properties = new SheetProperties
                    {
                        SheetId = sheetId,
                        GridProperties = new GridProperties
                        {
                            RowCount = newSize
                        },
                    },
                    Fields = "gridProperties.rowCount"
                }
            };
        }

        /// <summary>
        /// Pushes data from a collection of objects with GoogleSheetAttribute to Google Sheets
        /// </summary>
        public void PushObjectCollection<T>(int sheetId, IList<T> collection) where T : class, new()
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"{nameof(SpreadSheetId)} is required.");

            if (collection == null || collection.Count == 0)
                return;

            try
            {
                var columnInfos = GenerateColumnMapping<T>();

                // Prepare the data as rows
                var rows = new List<RowData>();

                // Add header row
                var headerRow = new RowData
                {
                    Values = columnInfos.Select(c => new CellData
                    {
                        UserEnteredValue = new ExtendedValue
                        {
                            StringValue = c.ColumnName
                        }
                    }).ToList()
                };
                rows.Add(headerRow);

                // Add data rows
                foreach (var item in collection)
                {
                    var row = new RowData
                    {
                        Values = columnInfos.Select(c => CreateCellData(c.GetValue(item), c.MemberType)).ToList()
                    };
                    rows.Add(row);
                }

                // Use the existing batch update mechanism
                var requests = new List<Request>();

                // Check if we need to resize the sheet
                var rowCount = GetRowCount(sheetId);
                var requiredRows = rows.Count;

                if (requiredRows > rowCount)
                {
                    requests.Add(ResizeRow(sheetId, requiredRows));
                }

                // Create update request
                var updateRequest = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Start = new GridCoordinate
                        {
                            SheetId = sheetId,
                            RowIndex = 0,
                            ColumnIndex = 0
                        },
                        Rows = rows,
                        Fields = "userEnteredValue"
                    }
                };

                requests.Add(updateRequest);

                SendBatchUpdateRequest(requests);
            }
            catch (Exception e)
            {
                LDebug.LogError<GoogleSheets>(e);
                throw;
            }
        }

        /// <summary>
        /// Pulls data from Google Sheets and merges it with existing collection based on key matching
        /// </summary>
        /// <param name="sheetId">The sheet ID to pull data from</param>
        /// <param name="collection">The existing collection to merge data into</param>
        /// <param name="updateExisting">If true, updates existing items with matching keys. If false, only adds new items.</param>
        public void PullIntoObjectCollection<T>(int sheetId, IList<T> collection, Func<string, T> createNew) where T : class, new()
        {
            if (string.IsNullOrEmpty(SpreadSheetId))
                throw new Exception($"{nameof(SpreadSheetId)} is required.");

            try
            {
                var columnInfos = GenerateColumnMapping<T>();
                var keyColumns = columnInfos.Where(c => c.IsKey).ToList();

                if (!keyColumns.Any())
                    throw new InvalidOperationException($"Type {typeof(T).Name} must have at least one field/property marked as IsKey=true for merging");

                // Get the sheet data
                var request = SheetsService.Spreadsheets.Get(SpreadSheetId);
                request.IncludeGridData = true;
                request.Fields = "sheets.properties.sheetId,sheets.data.rowData.values.formattedValue";

                var response = ExecuteRequest<Spreadsheet, GetRequest>(request);
                var sheet = response.Sheets?.FirstOrDefault(s => s?.Properties?.SheetId == sheetId);

                if (sheet == null)
                    throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");

                var rowData = sheet.Data?[0]?.RowData;

                if (rowData == null || rowData.Count <= 1) // No data or only header
                {
                    LDebug.Log<GoogleSheets>("No data rows found in sheet.");
                    return;
                }

                // Create a dictionary of existing items based on their key values
                var existingItemsDict = new Dictionary<string, T>();
                foreach (var existingItem in collection)
                {
                    var keyValue = GenerateKeyValue(existingItem, keyColumns);
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        existingItemsDict[keyValue] = existingItem;
                    }
                }

                int mergedCount = 0;
                int addedCount = 0;

                // Skip header row (index 0) and process data rows
                for (int rowIndex = 1; rowIndex < rowData.Count; rowIndex++)
                {
                    var row = rowData[rowIndex];
                    if (row?.Values == null) continue;

                    T newObject = new T();
                    bool hasData = false;

                    // First, populate the new object with all data
                    for (int colIndex = 0; colIndex < columnInfos.Count && colIndex < row.Values.Count; colIndex++)
                    {
                        var cellValue = row.Values[colIndex]?.FormattedValue;
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            columnInfos[colIndex].SetValue(newObject, cellValue);
                            hasData = true;
                        }
                    }

                    if (!hasData) continue;

                    // Generate key for the new object
                    var newObjectKey = GenerateKeyValue(newObject, keyColumns);
                    if (string.IsNullOrEmpty(newObjectKey)) continue;

                    // Check if an item with this key already exists
                    if (existingItemsDict.ContainsKey(newObjectKey))
                    {
                        // Update existing item
                        var existingItem = existingItemsDict[newObjectKey];
                        UpdateObjectFromAnother(existingItem, newObject, columnInfos);
                        mergedCount++;
                    }
                    else
                    {
                        T addedItem = createNew(newObjectKey);
                        UpdateObjectFromAnother(addedItem, newObject, columnInfos);

                        // Add new item
                        collection.Add(addedItem);
                        addedCount++;
                    }
                }

                LDebug.Log<GoogleSheets>($"Successfully pull data: {addedCount} new items added, {mergedCount} existing items updated.");
            }
            catch (Exception e)
            {
                LDebug.LogError<GoogleSheets>(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Generates a composite key value from the key columns of an object
        /// </summary>
        private string GenerateKeyValue<T>(T obj, List<GoogleSheetsObjectColumnInfo> keyColumns) where T : class
        {
            var keyParts = new List<string>();

            foreach (var keyColumn in keyColumns)
            {
                var value = keyColumn.GetValue(obj);
                keyParts.Add(value?.ToString() ?? "");
            }

            return string.Join("|", keyParts);
        }

        /// <summary>
        /// Updates an existing object with values from another object
        /// </summary>
        private void UpdateObjectFromAnother<T>(T target, T source, List<GoogleSheetsObjectColumnInfo> columnInfos) where T : class
        {
            foreach (var columnInfo in columnInfos)
            {
                var sourceValue = columnInfo.GetValue(source);
                if (sourceValue != null)
                {
                    columnInfo.SetValue(target, sourceValue);
                }
            }
        }

        private CellData CreateCellData(object value, Type valueType)
        {
            var cellData = new CellData();
            var extendedValue = new ExtendedValue();

            if (value == null)
            {
                extendedValue.StringValue = "";
            }
            else if (valueType == typeof(int) || valueType == typeof(int?))
            {
                if (int.TryParse(value.ToString(), out int intValue))
                    extendedValue.NumberValue = intValue;
                else
                    extendedValue.StringValue = value.ToString();
            }
            else if (valueType == typeof(float) || valueType == typeof(float?) ||
                     valueType == typeof(double) || valueType == typeof(double?) ||
                     valueType == typeof(decimal) || valueType == typeof(decimal?))
            {
                if (double.TryParse(value.ToString(), out double doubleValue))
                    extendedValue.NumberValue = doubleValue;
                else
                    extendedValue.StringValue = value.ToString();
            }
            else if (valueType == typeof(bool) || valueType == typeof(bool?))
            {
                if (bool.TryParse(value.ToString(), out bool boolValue))
                    extendedValue.BoolValue = boolValue;
                else
                    extendedValue.StringValue = value.ToString();
            }
            else
            {
                // For strings and other types, use string value
                extendedValue.StringValue = value.ToString();
            }

            cellData.UserEnteredValue = extendedValue;
            return cellData;
        }

        private List<GoogleSheetsObjectColumnInfo> GenerateColumnMapping<T>() where T : class
        {
            var columns = new List<GoogleSheetsObjectColumnInfo>();
            var type = typeof(T);

            // Get fields with GoogleSheetAttribute
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<GoogleSheetsAttribute>();
                if (attr != null)
                {
                    var columnIndex = attr.ColumnIndex >= 0 ? attr.ColumnIndex : columns.Count;
                    columns.Add(new GoogleSheetsObjectColumnInfo(attr.ColumnName, columnIndex, field, attr.IsKey));
                }
            }

            // Get properties with GoogleSheetAttribute
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<GoogleSheetsAttribute>();
                if (attr != null)
                {
                    var columnIndex = attr.ColumnIndex >= 0 ? attr.ColumnIndex : columns.Count;
                    columns.Add(new GoogleSheetsObjectColumnInfo(attr.ColumnName, columnIndex, property, attr.IsKey));
                }
            }

            // Sort by column index
            columns.Sort((a, b) => a.ColumnIndex.CompareTo(b.ColumnIndex));

            // Ensure we have at least one key column
            if (!columns.Any(c => c.IsKey))
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must have at least one field/property marked as IsKey=true");
            }

            return columns;
        }

        private Data.Color UnityColorToDataColor(UnityEngine.Color color) => new Data.Color() { Red = color.r, Green = color.g, Blue = color.b, Alpha = color.a };

        protected Task<BatchUpdateSpreadsheetResponse> SendBatchUpdateRequestAsync(string spreadsheetId, IList<Request> requests)
        {
            var service = SheetsService;
            var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
            var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
            return batchUpdateReq.ExecuteAsync();
        }

        protected BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(IList<Request> requests)
        {
            var service = SheetsService;
            var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
            var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, SpreadSheetId);
            return batchUpdateReq.Execute();
        }

        protected BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(params Request[] requests)
        {
            var service = SheetsService;
            var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
            var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, SpreadSheetId);
            return batchUpdateReq.Execute();
        }

        protected Task<TResponse> ExecuteRequestAsync<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.ExecuteAsync();

        protected TResponse ExecuteRequest<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.Execute();
    }
}
