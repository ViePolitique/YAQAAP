using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Yaqaap.ServiceModel
{
    [Route("/user/{Username}")]
    public class User : IReturn<UserResponse>
    {
        public string Username { get; set; }
    }

    public class UserResponse
    {
        public ErrorCode Result { get; set; }
        public DateTime Created { get; set; }
        public BadgeResponse[] Badges { get; set; }
    }

    public class BadgeResponse
    {
        public string Name { get; set; }
        public DateTime Awarded { get; set; }
    }
}
