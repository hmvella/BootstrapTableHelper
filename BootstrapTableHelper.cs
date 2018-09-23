using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;

namespace HMVella
{
    public class BootstrapTableHelper
    {
        public string GenerateTable<T>(IQueryable<T> objectList, int pageSize, int pageNumber, string sortBy, string sortOrder = "Asc", string searchString = "", bool searchStartOnly = false, bool caseSensitive = false)
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

        private string BootstrapTableFormatter<T>(IQueryable<T> objectList, int pageSize, int pageNumber)
        {
            var skip = (pageNumber - 1) * pageSize;
            var listCount = objectList.Count();
            dynamic expandoObject = new ExpandoObject();

            expandoObject.total = listCount;
            if(objectList.Count() > skip)
            {
                expandoObject.rows = objectList.Skip(skip).Take(pageSize).ToList();
            }
            else
            {
                expandoObject.rows = objectList.Take(pageSize).ToList();
            }

            string json = JsonConvert.SerializeObject(expandoObject, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return json;
        }

        private IQueryable<T> FilterData<T>(IQueryable<T> objectList, string search, string sortBy, string sortOrder, bool searchStartOnly, bool caseSensitive)
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
                objectList = objectList.OrderBy($"{sortBy} {sortOrder}");
            }

            return objectList;
        }

        private List<Searchable> GetSearchableColumns<T>(IQueryable<T> objectList)
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

        private string SearchQueryGenerator<T>(IQueryable<T> objectList, string searchString, bool searchStartOnly, bool caseSensitive)
        {
            var searchFloat = -9001.0f;
            var isNumber = float.TryParse(searchString, out searchFloat);
            var searchable = GetSearchableColumns(objectList);
            var searchableStrings = searchable.Where(c => c.Type == typeof(string));
            var searchableNums = searchable.Except(searchableStrings);
            var queryList = new List<string>();

            var searchType = ".Contains";
            if (searchStartOnly)
            {
                searchType = ".StartsWith";
            }

            var sensitivityString = "";
            if (!caseSensitive)
            {
                sensitivityString = ".ToUpper()";
                searchString = searchString.ToUpper();
            }

            foreach (var str in searchableStrings)
            {
                var queryString = $"{str.Name}{sensitivityString}{searchType}(\"{searchString}\")";
                queryList.Add(queryString);
            }
            if (isNumber)
            {
                foreach (var num in searchableNums)
                {
                    var queryString = $"{num.Name}.ToString(){sensitivityString}{searchType}(\"{searchString}\")";
                    queryList.Add(queryString);
                }
            }

            var combinedQuery = string.Join(" || ", queryList);

            return combinedQuery;
        }

        private class Searchable
        {
            public string Name;
            public Type Type;
        }
    }
}
