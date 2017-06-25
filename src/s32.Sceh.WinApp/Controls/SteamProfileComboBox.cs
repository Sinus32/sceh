using s32.Sceh.Classes;
using s32.Sceh.WinApp.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s32.Sceh.WinApp.Controls
{
    public class SteamProfileComboBox : ComboBox
    {
        public ProfileSelectItem SelectedProfile
        {
            get { return base.SelectedItem as ProfileSelectItem; }
            set { base.SelectedItem = value; }
        }

        public void AddItem(ProfileSelectItem item)
        {
            base.Items.Add(item);
        }

        public void AddItem(SteamProfile item)
        {
            base.Items.Add(new ProfileSelectItem(item));
        }
    }
}
