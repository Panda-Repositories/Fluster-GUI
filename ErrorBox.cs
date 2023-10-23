using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fluster_GUI
{
    public class ErrorBox
    {
        public string application_name = "Fluster";
        public void Error(string message)
        {
            MessageBox.Show(message, application_name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void Warning(string message)
        {
            MessageBox.Show(message, application_name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
