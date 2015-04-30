using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
    public class Login
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            int userCount = NetState.Instances.Count;
            args.Mobile.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online.",
                args.Mobile.RawName, userCount == 1 ? "is" : "are", userCount, userCount == 1 ? "" : "s");

            if(args.Mobile.BodyValue == 402 || args.Mobile.BodyValue == 403)
            {
                if(args.Mobile is Player)
                    ((Player)args.Mobile).AutoRespawn();
            }
        }
    }
}