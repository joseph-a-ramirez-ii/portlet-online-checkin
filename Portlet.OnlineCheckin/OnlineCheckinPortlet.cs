using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jenzabar.Portal.Framework.Web.UI;
using Jenzabar.Portal.Framework.Security.Authorization;

namespace CUS.ICS.OnlineCheckin
{
    [PortletOperation(
     "CanAccess",
     "Can Access Portlet",
     "Whether a user can access this portlet or not",
     PortletOperationScope.Global)]

    [PortletOperation(
        "CanAdmin",
        "Can Admin Portlet",
        "Whether a user can admin this portlet or not",
        PortletOperationScope.Global)]

    public class OnlineCheckinPortlet : SecuredPortletBase
    {
        protected override PortletViewBase GetCurrentScreen()
        {
            PortletViewBase screen = null;
            
            try
            {
                screen = this.LoadPortletView("ICS/OnlineCheckinPortlet/" + this.CurrentPortletScreenName + ".ascx");
            }
            catch
            {
                screen = this.LoadPortletView("ICS/OnlineCheckInPortlet/Default_View.ascx");
            }

            return screen;
        }
    }
}
