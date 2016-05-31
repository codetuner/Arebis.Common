using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Tasks
{
    public class ProgressEventArgs : System.EventArgs
    {
        public double ProgressFraction { get; set; }
        
        public string ProgressMessage { get; set; }
    }

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}
