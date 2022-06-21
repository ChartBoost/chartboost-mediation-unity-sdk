using Helium.Platforms;

namespace Helium.Platforms
{
    public class HeliumUnsupported : HeliumExternal
    {
        private static string _userIdentifier;
        
        public HeliumUnsupported()
        {
            LOGTag = "Helium(Unsupported)";
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            _userIdentifier = userIdentifier;
        }
        
        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _userIdentifier;
        }
    }
}
