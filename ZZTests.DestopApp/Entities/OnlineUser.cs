using System;

namespace ZZTests.DestopApp.Entities
{
    [Serializable]
    class OnlineUser
    {
        public string LoginName { get; set; }
        public string DisplayName { get; set; }
        public DateTime LoginOn { get; set; }
    }
}
