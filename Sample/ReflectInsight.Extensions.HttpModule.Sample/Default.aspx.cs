using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using ReflectSoftware.Insight;

namespace HttpModule_Sample
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RILogManager.Default.SendMessage("Page loading");

            using (RILogManager.Default.TraceMethod(MethodBase.GetCurrentMethod(), true))
            {
                RILogManager.Default.SendInformation("Information1");
            }
        }

        protected void Button_NoException(object sender, EventArgs e)
        {
            using (RILogManager.Default.TraceMethod(MethodBase.GetCurrentMethod(), true))
            {
                RILogManager.Default.SendInformation("Button_NoException");
            }

            RILogManager.Default.AddSeparator();
        }

        protected void Button_Exception1(object sender, EventArgs e)
        {
            using (RILogManager.Default.TraceMethod(MethodBase.GetCurrentMethod(), true))
            {
                try
                {
                    RILogManager.Default.SendInformation("Button_Exception1");

                    throw new Exception("Throw exception1");
                }
                catch (Exception ex)
                {
                    RILogManager.Default.SendException(ex);
                }
            }
        }

        protected void Button_Exception2(object sender, EventArgs e)
        {
            using (RILogManager.Default.TraceMethod(MethodBase.GetCurrentMethod(), true))
            {
                try
                {
                    RILogManager.Default.EnterMethod(MethodBase.GetCurrentMethod(), true);
                    RILogManager.Default.SendInformation("Button_Exception2");
                    RILogManager.Default.ExitMethod(MethodBase.GetCurrentMethod(), true);

                    throw new Exception("Throw exception2");
                }
                catch (Exception ex)
                {
                    RILogManager.Default.SendException(ex);
                }
            }
        }
    }
}
