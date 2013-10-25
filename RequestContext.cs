using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace shazam
{
    public struct RequestContext
    {
        public IRequestBuilder RequestBuilder
        {
            get;
            set;
        }

        public object State
        {
            get;
            set;
        }

        public WebRequest WebRequest
        {
            get;
            set;
        }
    }
}
