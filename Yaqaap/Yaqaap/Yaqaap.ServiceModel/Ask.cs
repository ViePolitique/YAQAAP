using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/ask/{Question}")]
    public class Ask : IReturn<AskResponse>
    {
        public string Question { get; set; }
    }

    public class AskResponse
    {
        public string Result { get; set; }
    }
}
