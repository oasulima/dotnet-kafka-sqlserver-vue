namespace Admin.API;

public static class Constants
{
    public static class ENV
    {
        public const string KAFKA__BOOTSTRAP_SERVERS = "KAFKA__BOOTSTRAP_SERVERS";
        public const string KAFKA__LOCATOR_QUOTE_RESPONSE_TOPIC =
            "KAFKA__LOCATOR_QUOTE_RESPONSE_TOPIC";
        public const string KAFKA__LOCATOR_QUOTE_REQUEST_TOPIC =
            "KAFKA__LOCATOR_QUOTE_REQUEST_TOPIC";
        public const string KAFKA__NOTIFICATION_TOPIC = "KAFKA__NOTIFICATION_TOPIC";
        public const string LOCATOR_BASE_URL = "LOCATOR_BASE_URL";
        public const string VELOCITY_MOCK_BASE_URL = "VELOCITY_MOCK_BASE_URL";
        public const string BROADRIDGE_MOCK_BASE_URL = "BROADRIDGE_MOCK_BASE_URL";
        public const string FIS_MOCK_BASE_URL = "FIS_MOCK_BASE_URL";
        public const string LBX_MOCK_BASE_URL = "LBX_MOCK_BASE_URL";
        public const string VELOCITY_FIX_ADAPTOR_BASE_URL = "VELOCITY_FIX_ADAPTOR_BASE_URL";
        public const string INTERNAL_INVENTORY_BASE_URL = "INTERNAL_INVENTORY_BASE_URL";
        public const string REPORTING_BASE_URL = "REPORTING_BASE_URL";
        public const string ORDER_BOOK_BASE_URL = "ORDER_BOOK_BASE_URL";
        public const string CONNECTION_STRING = "CONNECTION_STRING";
        public const string DATA_CLEANER_RUN_TIME_UTC = "DATA_CLEANER_RUN_TIME_UTC";
        public const string SETTINGS_UPDATER__UPDATE_PERIOD_TIME =
            "SETTINGS_UPDATER__UPDATE_PERIOD_TIME";
        public const string KAFKA__INVALIDATE_CACHE_COMMAND_TOPIC =
            "KAFKA__INVALIDATE_CACHE_COMMAND_TOPIC";
    }

    public static class SSEMethods
    {
        public const string LocateRequest = "locate-request";
        public const string LocateRequestHistory = "locate-request-history";
        public const string Locate = "locate";
        public const string LocateHistory = "locate-history";
        public const string Notification = "notification";
        public const string InternalInventory = "internal-inventory";
    }

    public static class KnownRoles
    {
        public const string Admin = "Locator.Admin";
        public const string Viewer = "Locator.View";
    }
}
