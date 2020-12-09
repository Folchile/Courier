using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Courier.Controllers
{
    public static class DataSetExtension
    {

        public static DataSet ToDataSet<T>(this List<T> list)
        {

            Type type = typeof(T);

            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            ds.Tables.Add(dt);



            var propertyInfos = type.GetProperties().ToList();



            //For each property of generic List (T), add a column to table

            propertyInfos.ForEach(propertyInfo =>
            {

                Type columnType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                dt.Columns.Add(propertyInfo.Name, columnType);

            });



            //Visit every property of generic List (T) and add each value to the data table 

            list.ForEach(item =>
            {

                DataRow row = dt.NewRow();

                propertyInfos.ForEach(

                                       propertyInfo =>

                                       row[propertyInfo.Name] = propertyInfo.GetValue(item, null) ?? DBNull.Value

                                     );

                dt.Rows.Add(row);

            });



            //Return the dataset

            return ds;

        }

    }
}