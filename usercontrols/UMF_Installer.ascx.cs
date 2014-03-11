using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.relation;

namespace UMF.usercontrols
{
    public partial class installer : Umbraco.Web.UmbracoUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Install the Relations we need
            try
            {
                var rs = Services.RelationService;
                this.BulletedList1.Items.Add("Successfully installed relations");
            }
            catch (Exception ex) {
                this.BulletedList1.Items.Add("Error while installing relations: " + ex.Message);
            }
        }
    }
}