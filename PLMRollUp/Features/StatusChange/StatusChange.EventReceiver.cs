using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections.Generic;
using Microsoft.SharePoint;
using System.IO;


namespace PLMRollUp.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c8ed3347-e9d3-4034-87da-2359ee074bcd")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Create an event receiver on the content type "Program Status"
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;
            SPWeb web = site.RootWeb;
            SPContentType ctype = web.ContentTypes["Program Status"];
            
            if (ctype != null) // If we found the "Program Status" event type
            {
                SPEventReceiverDefinition er = ctype.EventReceivers.Add();
                er.Class = "PLMRollUp.EventReceiver1.EventReceiver1";
                er.Assembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                er.SequenceNumber = 1000;
                er.Type = SPEventReceiverType.ItemUpdated;
                er.Name = "ItemUpdated";
               
                er.Update();
                ctype.Update(true);
                web.Dispose();
            }

        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            List <SPEventReceiverDefinition> toRemove = new List<SPEventReceiverDefinition>();

            SPSite site = (SPSite)properties.Feature.Parent;
            SPWeb web = site.OpenWeb();
            SPContentType contentType = web.ContentTypes["Program Status"];
            if (contentType != null)
            {
                int i;
                //Use the above integer to loop through the event recievers on the first list and delete the above assembly
                for (i = 0; i < contentType.EventReceivers.Count; i++)
                {       
                    if (contentType.EventReceivers[i].Assembly.Contains("PLM"))
                    {
                        toRemove.Add(contentType.EventReceivers[i]);    // build up a list of receivers to delete
                    }
                }
                foreach(SPEventReceiverDefinition delme in toRemove){
                    delme.Delete();
                }
                contentType.Update(true);   // If it turns out this doesnt remove the EventReceiver investigate contentType.Update(true, false) and other combos
            }
        }



        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}


        static void LogMessage(string msg)
        {
            StreamWriter wrtr = null;
            try
            {
                wrtr = new StreamWriter("C:\\Logs\\FeatureActivatedDeactivated.txt", true);
                wrtr.WriteLine(msg + "--[" + System.DateTime.Now.ToString() + "]" + Environment.NewLine);
                wrtr.WriteLine(Environment.NewLine + "==================================");
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (wrtr != null)
                {
                    wrtr.Close();
                    wrtr.Dispose();
                }
            }
        }
    }




}


