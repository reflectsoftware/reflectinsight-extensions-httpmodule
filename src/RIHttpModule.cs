/******************************************************************************
*
* Copyright (c) ReflectSoftware, Inc. All rights reserved. 
*
* See License.rtf in the solution root for license information.
*
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;

using ReflectSoftware.Insight;
using ReflectSoftware.Insight.Common;
using ReflectSoftware.Insight.Common.Data;

namespace ReflectSoftware.Insight.Extensions.HttpModule
{
    public class RIHttpModule : IHttpModule
    {
        private readonly static MethodInfo FSendMethodInfo;

        private ReflectInsight FReflectInsight;
        private Properties FProperties;
        private MessageType FMessageType;
        private Int32 FLastIndentLevel;
        private Boolean FEnabled;
        private String FUrl;

        //---------------------------------------------------------------------
        static RIHttpModule()
        {
            FSendMethodInfo = typeof(ReflectInsight).GetMethod("_SendCustomData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="RIHttpModule"/> class.
        /// </summary>
        public RIHttpModule()
        {
            OnConfigChange();
            RIEventManager.OnServiceConfigChange += DoOnConfigChange;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                RIEventManager.OnServiceConfigChange -= DoOnConfigChange;
                
                if (FReflectInsight != null)
                {
                    FReflectInsight.Dispose();
                    FReflectInsight = null;
                }
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Does the on config change.
        /// </summary>
        private void DoOnConfigChange()
        {
            OnConfigChange();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Called when [config change].
        /// </summary>
        private void OnConfigChange()
        {
            try
            {
                lock (this)
                {
                    String instanceName = ReflectInsightConfig.Settings.GetExtensionAttribute("riHttpModule", "instance", "http");
                    FReflectInsight = RILogManager.Get(instanceName) ?? RILogManager.Default;

                    FEnabled = ReflectInsightConfig.Settings.GetExtensionAttribute("riHttpModule", "enabled", "true") == "true";
                    if (!FEnabled)
                        return;

                    String properties = ReflectInsightConfig.Settings.GetExtensionAttribute("riHttpModule", "properties", String.Empty);

                    String xPath = String.Format("./httpModule/properties[@name='{0}']", properties);
                    FProperties = new Properties(ReflectInsightConfig.Settings.GetNode(xPath));

                    if (FProperties.UserEnterEnterMethod)
                        FMessageType = MessageType.EnterMethod;
                    else
                        FMessageType = MessageType.SendHttpModuleInformation;
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.Publish(ex, "Failed during: RIHttpModule.OnConfigChange()");
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Begins the request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BeginRequest(object sender, EventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (!FEnabled)
                        return;

                    FUrl = HttpContext.Current.Request.RawUrl;
                    if (FProperties.IgnoreUrl(FUrl))
                    {
                        FUrl = null;
                        return;
                    }

                    RICustomData cData = RequestBuilder.BuildData(FProperties);
                    if (cData != null)
                    {
                        Send(FMessageType, FUrl, cData);
                        if (FMessageType == MessageType.EnterMethod)
                            FLastIndentLevel = ReflectInsight.IndentLevel;
                    }
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.PublishIfEvented(ex, "Failed during: RIHttpModule.BeginRequest()");
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Ends the request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EndRequest(object sender, EventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (!FEnabled || FUrl == null || FMessageType != MessageType.EnterMethod)
                        return;

                    Int32 currentIndentLevel = ReflectInsight.IndentLevel;
                    while (currentIndentLevel > FLastIndentLevel)
                    {
                        FReflectInsight.ExitMethod("Matching ExitMethod was missing...");
                        currentIndentLevel--;
                    }

                    FReflectInsight.ExitMethod(FUrl);
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.PublishIfEvented(ex, "Failed during: RIHttpModule.EndRequest()");
            }
        }

        //--------------------------------------------------------------------- 
        /// <summary>
        /// Sends the specified m type.
        /// </summary>
        /// <param name="mType">The message type.</param>
        /// <param name="str">The STR.</param>
        /// <param name="data">The data.</param>
        private void Send(MessageType mType, String str, RICustomData data)
        {
            FSendMethodInfo.Invoke(FReflectInsight, new object[] { mType, str, data, new object[] {} });
        }
    }
}
