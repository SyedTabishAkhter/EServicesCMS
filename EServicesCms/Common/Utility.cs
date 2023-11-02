using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace EServicesCms.Common
{
    public static class Utility
    {
        public static string HtmlStrip(this string input)
        {
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
            input = Regex.Replace(input, "<script>(.|\n)*?</script>", string.Empty);
            input = Regex.Replace(input, "<.*?>|&.*?;", string.Empty);
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", string.Empty); 
            return Regex.Replace(input, @"<(.|\n)*?>", string.Empty);
        }
        public static List<T> TrimList<T>(this IEnumerable<T> sourceList)
        {
            List<T> rList = new List<T>();

            if (sourceList == null || sourceList.Count() == 0)
                return rList;

            PropertyInfo[] props = null;
            foreach (T obj in sourceList)
            {
                Type itemType = typeof(T);
                props = itemType.GetProperties();

                for (int i = 0; i < props.Length; i++)
                {
                    if (!props[i].CanRead) continue;
                    if (!props[i].CanWrite) continue;
                    if (props[i].PropertyType != typeof(string)) continue;
                    string o = (string)props[i].GetValue(obj, null);
                    if (string.IsNullOrEmpty(o) == false)
                        (props[i]).SetValue(obj, Helper.RefineString(o).TrimEnd(
                                                   ' ',
                                                   '\t',
                                                   '\v',
                                                   '\f',
                                                   '\r').HtmlStrip(), null);
                }
                rList.Add(obj);
            }
            return rList;
        }
        public static T TrimObject<T>(this T item)
        {
            if (item == null)
                return item;

            Type itemType = typeof(T);
            var props = itemType.GetProperties();
            foreach (var obj in props)
            {
                if (!obj.CanRead) continue;
                if (!obj.CanWrite) continue;
                if (obj.PropertyType != typeof(string)) continue;
                string currentValue = (string)obj.GetValue(item, null);
                if (string.IsNullOrEmpty(currentValue) == false)
                    obj.SetValue(item, Helper.RefineString(currentValue).TrimEnd(
                                                   ' ',
                                                   '\t',
                                                   '\v',
                                                   '\f',
                                                   '\r').HtmlStrip(), null);
            }

            return item;
        }
        public static string ToTitle(string textToConvert)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            textToConvert = textInfo.ToTitleCase(textToConvert);

            return textToConvert;
        }
        public static List<T> ToList<T>(this Array arrayList)
        {
            List<T> list = new List<T>(arrayList.Length);
            foreach (T instance in arrayList)
            {
                list.Add(instance);
            }
            return list;
        }
        public static DataTable ToDataTable<T>(this List<T> iList, string dtName = "")
        {
            DataTable dataTable = new DataTable(dtName);
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static DataTable ConvertObjectToDataTable(object objects, string dtName = "")
        {
            DataTable dt = null;
            try
            {
                if (objects != null)
                {
                    Type t = objects.GetType();
                    //
                    dt = new DataTable(dtName);
                    //
                    foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
                    {
                        dt.Columns.Add(new DataColumn(pi.Name));
                    }
                    //
                    DataRow dr = dt.NewRow();
                    //
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = t.GetProperty(dc.ColumnName).GetValue(objects, null);
                    }
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception Exp)
            {
                throw new ApplicationException("Error in ConvertObjectToDataTable = " + Exp.ToString());
            }
            return dt;
        }
        /// <summary>
        /// ToHtmlTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOfClassObjects"></param>
        /// <returns></returns>
        public static string ToHtmlTable<T>(this List<T> listOfClassObjects)
        {
            var ret = string.Empty;

            return listOfClassObjects == null || !listOfClassObjects.Any()
                ? ret
                : "<table style='border:1px solid gray;'>" +
                  listOfClassObjects.First().GetType().GetProperties().Select(p => p.Name).ToList().ToColumnHeaders() +
                  listOfClassObjects.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
                  "</table>";
        }
        /// <summary>
        /// ToColumnHeaders
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOfProperties"></param>
        /// <returns></returns>
        public static string ToColumnHeaders<T>(this List<T> listOfProperties)
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current +
                          ("<th style='font-size: 11pt; font-weight: bold; border: 1pt solid black'>" +
                           (Convert.ToString(propValue).Length <= 100
                               ? Convert.ToString(propValue)
                               : Convert.ToString(propValue).Substring(0, 100)) + "" + "</th>")) +
                  "</tr>";
        }
        /// <summary>
        /// ToHtmlTableRow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classObject"></param>
        /// <returns></returns>
        public static string ToHtmlTableRow<T>(this T classObject)
        {
            var ret = string.Empty;

            return classObject == null
                ? ret
                : "<tr>" +
                  classObject.GetType()
                      .GetProperties()
                      .Aggregate(ret,
                          (current, prop) =>
                              current + ("<td style='font-size: 11pt; font-weight: normal; border: 1pt solid black'>" +
                                         (Convert.ToString(prop.GetValue(classObject, null)).Length <= 100
                                             ? Convert.ToString(prop.GetValue(classObject, null))
                                             : Convert.ToString(prop.GetValue(classObject, null)).Substring(0, 100) +
                                               "") +
                                         "</td>")) + "</tr>";
        }
        public static System.Data.DataTable ConvertListToDataTable<T>(this List<T> data, string dataTableName = "")
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = null;
            try
            {
                table = new System.Data.DataTable(dataTableName);

                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];

                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (props[i] != null)
                            values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    }
                    table.Rows.Add(values);
                }
                if (table != null && table.Rows.Count > 0)
                {
                    for (int i = 0; i <= table.Rows.Count - 1; i++)
                    {
                        table.Rows[i][1] = table.Rows[i][1].ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                    }
                }
            }
            catch (Exception Exp)
            {
                throw new ApplicationException("Error in ConvertListToDataTable = " + Exp.ToString());
            }
            return table;
        }
        public static string ConvertListToXml<T>(this List<T> strData)
        {
            string xmlString = string.Empty;
            try
            {
                //
                System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                //
                System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
                //
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
                //
                List<T> lst = strData;
                //
                serializer.Serialize(xmlWriter, lst);
                //
                xmlString = stringWriter.ToString();
            }
            catch (Exception Exp)
            {
                throw new ApplicationException("Error in ConvertListToXml = " + Exp.ToString());
            }
            return xmlString;
        }
        public static object[,] ConvertDataTableToObjectArray(DataTable dt)
        {
            var rows = dt.Rows;
            int rowCount = rows.Count;
            int colCount = dt.Columns.Count;
            var result = new object[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                for (int j = 0; j < colCount; j++)
                {
                    result[i, j] = row[j];
                }
            }

            return result;
        }
        public static object ConvertDataTableToObject(DataTable dt)
        {
            var rows = dt.Rows;
            int rowCount = rows.Count;
            int colCount = dt.Columns.Count;
            var result = new object[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                for (int j = 0; j < colCount; j++)
                {
                    result[i] = row[j];
                }
            }
            return result;
        }
        public static string ConvertDataTableToJson(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            System.Text.StringBuilder JsonString = new System.Text.StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
        public static string ConvertDataTableToXml(System.Data.DataTable table)
        {
            string xmlString = string.Empty;
            try
            {
                using (System.IO.TextWriter writer = new System.IO.StringWriter())
                {
                    table.WriteXml(writer, XmlWriteMode.IgnoreSchema);
                    xmlString = writer.ToString();
                }
            }
            catch (Exception Exp)
            {
                throw new ApplicationException("Error in ConvertDataTableToXml = " + Exp.ToString());
            }
            return xmlString;
        }
        public static DataTable ConvertObjectToDataTable(object objects)
        {
            DataTable dt = null;
            try
            {
                if (objects != null)
                {
                    Type t = objects.GetType();
                    //
                    dt = new DataTable(t.Name);
                    //
                    foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
                    {
                        dt.Columns.Add(new DataColumn(pi.Name));
                    }
                    //
                    DataRow dr = dt.NewRow();
                    //
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = t.GetProperty(dc.ColumnName).GetValue(objects, null);
                    }
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception Exp)
            {
                throw new ApplicationException("Error in ConvertObjectToDataTable = " + Exp.ToString());
            }
            return dt;
        }
        public static DataTable ConvertObjectsToDataTable(object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                Type t = objects[0].GetType();
                //
                DataTable dt = new DataTable(t.Name);
                //
                try
                {
                    foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
                    {
                        dt.Columns.Add(new DataColumn(pi.Name));
                    }
                    //
                    foreach (var o in objects)
                    {
                        if (o != null)
                        {
                            DataRow dr = dt.NewRow();
                            //
                            foreach (DataColumn dc in dt.Columns)
                            {
                                dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                            }
                            //
                            dt.Rows.Add(dr);
                        }
                    }
                }
                catch (Exception Exp)
                {
                    throw new ApplicationException("Error in ConvertObjectsToDataTable = " + Exp.ToString());
                }
                return dt;
            }
            return null;
        }

        public static void MapProp(object sourceObj, object targetObj)
        {
            Type T1 = sourceObj.GetType();
            Type T2 = targetObj.GetType();

            PropertyInfo[] sourceProprties = T1.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] targetProprties = T2.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var sourceProp in sourceProprties)
            {
                object osourceVal = sourceProp.GetValue(sourceObj, null);
                int entIndex = Array.IndexOf(targetProprties, sourceProp);
                if (entIndex >= 0)
                {
                    var targetProp = targetProprties[entIndex];
                    targetProp.SetValue(targetObj, osourceVal);
                }
            }
        }
    }
}