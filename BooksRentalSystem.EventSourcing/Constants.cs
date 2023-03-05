namespace BooksRentalSystem.EventSourcing;

public static class Constants
{
    public static class Subscriptions
    {
        public static class Users
        {
            public const string PublishersUsersGroupName = "UpdatePublishersUsers";
        }

        public static class BookAds
        {
            public const string PublishersBookAdsGroupName = "UpdatePublishersBookAds";
            public const string NotificationsBookAdsGroupName = "UpdateNotificationsBookAds";
            public const string StatisticsBookAdsGroupName = "UpdateStatisticsBookAds";
        }
    }
}