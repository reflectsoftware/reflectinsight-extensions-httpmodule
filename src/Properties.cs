/*********************************************************************************
*
* Copyright (c) ReflectSoftware, Inc. All rights reserved. 
*
* See License.rtf in the solution root for license information.
*
**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Web;

using RI.System.Configuration;

namespace ReflectSoftware.Insight.Extensions.HttpModule
{
    internal class Properties
    {
        private readonly ConfigNode FProperties;
        
        public Boolean UserEnterEnterMethod { get; set; }        
        public Boolean IgnoreUserName { get; set; }
        public Boolean IgnoreHeader { get; set; }
        public Boolean IgnoreQueryString { get; set; }
        public Boolean IgnoreFormElements { get; set; }
        public Boolean IgnoreCookies { get; set; }        
        public Hashtable UrlParts { get; set; }
        public Hashtable HeaderParameters { get; set; }
        public Hashtable QueryStrings { get; set; }
        public Hashtable FormElements { get; set; }
        public Hashtable Cookies { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="Properties"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public Properties(XmlNode properties)
        {
            FProperties = new ConfigNode(properties);
            Prepare();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Prepares this instance.
        /// </summary>
        private void Prepare()
        {
            UserEnterEnterMethod = FProperties.GetAttribute("./userEnterEnterMethod", "value", "false") == "true";
            IgnoreUserName = FProperties.GetAttribute("./ignoreUsername", "value", "false") == "true";
            IgnoreHeader = FProperties.GetAttribute("./ignoreHeader", "all", "false") == "true";
            IgnoreQueryString = FProperties.GetAttribute("./ignoreQueryString", "all", "false") == "true";
            IgnoreFormElements = FProperties.GetAttribute("./ignoreFormElements", "all", "false") == "true";
            IgnoreCookies = FProperties.GetAttribute("./ignoreCookies", "all", "false") == "true";            

            UrlParts = new Hashtable();
            HeaderParameters = new Hashtable();
            QueryStrings = new Hashtable();
            FormElements = new Hashtable();
            Cookies = new Hashtable();

            if (!FProperties.IsSectionSet)
                return;

            PopulateHashtable(UrlParts, "./ignoreUrlsParts/part", "name");
            PopulateHashtable(HeaderParameters, "./ignoreHeader/parameter", "name");
            PopulateHashtable(QueryStrings, "./ignoreQueryString/parameter", "name");
            PopulateHashtable(FormElements, "./ignoreFormElements/element", "name");
            PopulateHashtable(Cookies, "./ignoreCookies/cookie", "name");
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Populates the hashtable.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="xPath">The x path.</param>
        /// <param name="attrName">Name of the attr.</param>
        private void PopulateHashtable(Hashtable table, String xPath, String attrName)
        {
            XmlNodeList nodes = FProperties.Section.SelectNodes(xPath);
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes[attrName] != null)
                    table[node.Attributes[attrName].Value.ToLower()] = true;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Ignores the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public Boolean IgnoreUrl(String url)
        {
            url = url.ToLower();            
            foreach (String part in UrlParts.Keys)
            {
                if (url.Contains(part))
                    return true;
            }

            return false;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Ignores the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static Boolean Ignore(Hashtable table, String key)
        {
            return table[key.ToLower()] != null;
        }
    }
}
