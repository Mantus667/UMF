using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;

namespace UMF.usercontrols
{
    public partial class UMF_MemberGroupPicker : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        public List<string> activeGroups;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                activeGroups = new List<string>();
                foreach (ListItem item in CheckBoxList1.Items)
                {
                    if (item.Selected)
                    {
                        activeGroups.Add(item.Value);
                    }
                }
            }

            CheckBoxList1.Items.Clear();

            var groups = MemberGroup.GetAll;
            foreach (var group in groups)
            {
                var item = new ListItem(group.Text, group.Text);
                if (activeGroups.Contains(group.Text))
                {
                    item.Selected = true;
                }
                CheckBoxList1.Items.Add(item);
            }
        }

        public object value
        {
            get
            {
                string values = String.Empty;
                values = string.Join(",", activeGroups.ToArray());
                return values;
            }
            set
            {
                string[] stringSeparators = new string[] { ",", "." };
                activeGroups = new List<string>();
                foreach (var val in value.ToString().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    activeGroups.Add(val);
                }
            }
        }
    }
}