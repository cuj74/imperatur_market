using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.shared
{

    [AttributeUsage(AttributeTargets.All)]
    public class DesignAttribute : System.Attribute
    {
        public readonly bool VisibleAtPresentation;
        public readonly bool Expand;

        /*  public string Topic               // Topic is a named parameter
          {
              get
              {
                  return topic;
              }
              set
              {

                  topic = value;
              }
          }
          */
        public DesignAttribute(bool visibleAtPresentation, bool expand)  // url is a positional parameter
        {
            this.VisibleAtPresentation = visibleAtPresentation;
            this.Expand = expand;
        }
        public DesignAttribute(bool visibleAtPresentation)  // url is a positional parameter
        {
            this.VisibleAtPresentation = visibleAtPresentation;
            this.Expand = false;
        }

        //private string topic;
    }
}
