using DoorPrize.Generic;

namespace DoorPrize.Client.Helpers
{
    public static class Global
    {
        static Global()
        {
            AppInfo = new AppInfo(typeof(Global).Assembly);

            AccountPhone = Properties.Settings.Default.AccountPhone;

            DisplayPhone = string.Format("+1 ({0}) {1}-{2}",
                AccountPhone.Substring(0, 3),
                AccountPhone.Substring(3, 3),
                AccountPhone.Substring(6));
        }

        public static AppInfo AppInfo { get; }
        public static string DisplayPhone { get; }
        public static string AccountPhone { get; }
    }
}
