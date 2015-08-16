using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/ask")]
    public class Ask : IReturn<AskResponse>
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string [] Tags { get; set; }
    }

    public class AskResponse
    {
        public string Result { get; set; }
    }
}
