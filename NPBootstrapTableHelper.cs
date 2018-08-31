using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPoco.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace StatusUpdater.Helpers
{
    public class NPBootstrapTableHelper
    {
        public string GenerateTable<T>(IQueryProvider<T> objectList, int pageSize, int pageNumber, string sortBy, string sortOrder = "Asc", string searchString = "", bool searchStartOnly = false, bool caseSensitive = false)
        {
            var results = FilterData(objectList, searchString, sortBy, sortOrder, searchStartOnly, caseSensitive);
            var jsonTableData = BootstrapTableFormatter(results, pageSize, pageNumber);
            return jsonTableData;
        }

        public DataTable ToDataTable<T>(IList<T> list, string tableName = "table")
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            table.TableName = tableName;
            return table;
        }

        private string BootstrapTableFormatter<T>(IQueryProvider<T> objectList, int pageSize, int pageNumber)
        {
            var skip = (pageNumber - 1) * pageSize;
            var listCount = objectList.Count();
            dynamic expandoObject = new ExpandoObject();

            expandoObject.total = listCount;
            expandoObject.rows = objectList.Limit(skip, pageSize).ToList();

            string json = JsonConvert.SerializeObject(expandoObject);
            return json;
        }

        private IQueryProvider<T> FilterData<T>(IQueryProvider<T> objectList, string search, string sortBy, string sortOrder, bool searchStartOnly, bool caseSensitive)
        {
            string query = "";
            if (!objectList.Any())
            {
                return objectList;
            }
            //Dynamic Search
            if (!search.IsNullOrWhiteSpace())
            {
                search = search.ToLower();
                query = SearchQueryGenerator(objectList, search, searchStartOnly, caseSensitive);
                objectList = objectList.Where(query);
            }
            //Dynamic Order By
            if (!sortBy.IsNullOrWhiteSpace() && !sortOrder.IsNullOrWhiteSpace())
            {
                var isAscending = sortOrder.ToLower() == "asc" ? true : false;
                objectList = objectList.OrderByField(sortBy, isAscending );
            }

            return objectList;
        }

        private List<Searchable> GetSearchableColumns<T>(IQueryProvider<T> objectList)
        {
            var first = objectList.FirstOrDefault();
            if (first == null)
            {
                return null;
            }

            List<Searchable> searchableList = new List<Searchable>();
            var properties = objectList.First().GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                var type = property.PropertyType;

                if (type == typeof(string) || type == typeof(float) ||
                    type == typeof(double) || type == typeof(int) ||
                    type == typeof(decimal) || type == typeof(long))
                {
                    searchableList.Add(new Searchable
                    {
                        Name = name,
                        Type = type
                    });
                }
            }

            return searchableList;
        }

        private string SearchQueryGenerator<T>(IQueryProvider<T> objectList, string searchString, bool searchStartOnly, bool isCaseSensitive) 
        {

            var searchFloat = -9001.0f;
            var isNumber = float.TryParse(searchString, out searchFloat);
            var searchable = GetSearchableColumns(objectList);
            var searchableStringColumns = searchable.Where(c => c.Type == typeof(string));
            var searchableNumColumns = searchable.Except(searchableStringColumns);

            var searchStringArray = searchString.Split(' ');
            var combinedQueryList = new List<string>();
            foreach(var row in searchStringArray)
            {
                var queryList = new List<string>();

                var searchWord = SetSearchType(row, searchStartOnly);

                foreach (var str in searchableStringColumns)
                {
                    var queryString = $"{str.Name}{searchWord}";
                    queryList.Add(queryString);
                }

                if (isNumber)
                {
                    foreach (var num in searchableNumColumns)
                    {
                        var queryString = $"CONVERT(varchar(MAX), {num.Name}) '{searchWord}'"; 
                        queryList.Add(queryString);
                    }
                }

                var combinedQuery = string.Join(" OR ", queryList); //CONCAT function would be more performant, however not supported in SQLServer 2008. Manual concatenation is less performant than ORs. 
                combinedQuery = $"({combinedQuery})";
                combinedQueryList.Add(combinedQuery);
            }

            var sensitivityString = SetCaseSensitivity(isCaseSensitive);
            var finalQuery = $"{string.Join(" AND ", combinedQueryList)}{sensitivityString}";

            return finalQuery;
        }

        private string SetSearchType(string value, bool searchStartOnly)
        {
            if (searchStartOnly)
            {
                return $" LIKE '{value}%'";
            }

            return $" LIKE '%{value}%'";
        }

        private string SetCaseSensitivity(bool isCaseSensitive) //Must be placed once at end of query 
        {
            if (isCaseSensitive)
            {
                return " COLLATE Latin1_General_CS_AS";
            }

            return "";
        }

        private class Searchable
        {
            public string Name;
            public Type Type;
        }
    }
}
