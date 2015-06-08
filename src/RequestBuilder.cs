/*********************************************************************************
*
* Copyright (c) ReflectSoftware, Inc. All rights reserved. 
*
* See License.rtf in the solution root for license information.
*
**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web;

using RI.Utils.Miscellaneous;
using RI.Utils.ASPNET;
using RI.Utils.Strings;

using ReflectSoftware.Insight.Common.Data;

namespace ReflectSoftware.Insight.Extensions.HttpModule
{
    static internal class RequestBuilder
    {
        private static Properties FProperties;
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        static public RICustomData BuildData(Properties properties)
        {
            FProperties = properties;
            HttpContextBase contextBase = new HttpContextWrapper(HttpContext.Current);

            List<RICustomDataColumn> columns = new List<RICustomDataColumn>();
            columns.Add(new RICustomDataColumn("Property"));
            columns.Add(new RICustomDataColumn("Value"));

            RICustomData requestData = new RICustomData("RIHttpModule", columns, false, true);

            RICustomDataCategory cat = requestData.AddCategory("Common");
            cat.AddRow("HttpMethod", contextBase.Request.HttpMethod);
            cat.AddRow("IPAddress", contextBase.Request.UserHostAddress);

            if(!FProperties.IgnoreUserName)
                cat.AddRow("Username", contextBase.User != null ? contextBase.User.Identity.Name : String.Empty);

            if(!FProperties.IgnoreHeader) 
                PopulateCategory(contextBase.Request.Headers, requestData, "Headers", FProperties.HeaderParameters);

            if (!FProperties.IgnoreQueryString)
                PopulateCategory(contextBase.Request.QueryString, requestData, "QueryString", FProperties.QueryStrings);

            if (!FProperties.IgnoreFormElements)            
                PopulateCategory(contextBase.Request.Form, requestData, "Form", FProperties.FormElements);

            if (!FProperties.IgnoreCookies)
                PopulateCategory(contextBase.Request.Cookies, requestData, "Cookies", FProperties.Cookies);

            return requestData;
        }

        //---------------------------------------------------------------------        
        /// <summary>
        /// Populates the category.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="parameters">The parameters.</param>
        static private void PopulateCategory(NameValueCollection collection, RICustomData requestData, String categoryName, Hashtable parameters)
        {
            RICustomDataCategory cat = requestData.AddCategory(categoryName);

            foreach (String key in collection.AllKeys)
            {
                if (StringHelper.IsNullOrEmpty(key)) continue;
                if (key.ToLower() == "cookie") continue;
                if (Properties.Ignore(parameters, key)) continue;

                cat.AddRow(key, collection[key]);
            }

            // remove if category is empty
            if (cat.Children.Count == 0)
                requestData.RemoveElement(cat);
        }

        //---------------------------------------------------------------------        
        /// <summary>
        /// Populates the category.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="parameters">The parameters.</param>
        static private void PopulateCategory(HttpCookieCollection collection, RICustomData requestData, String categoryName, Hashtable parameters)
        {
            RICustomDataCategory cat = requestData.AddCategory(categoryName);

            foreach (String key in collection.AllKeys)
            {
                if (StringHelper.IsNullOrEmpty(key)) continue;
                if (Properties.Ignore(parameters, key)) continue;

                cat.AddRow(key, collection[key].Value);
            }

            // remove if category is empty
            if (cat.Children.Count == 0)
                requestData.RemoveElement(cat);
        }
    }
}
