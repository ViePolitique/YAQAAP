using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Authenticate]
    [Route("/love")]
    public class Love : IReturn<LoveResponse>
    {
        public Guid QuestionId { get; set; }
    }

    public class LoveResponse
    {
        public ErrorCode Result { get; set; }
    }
}
