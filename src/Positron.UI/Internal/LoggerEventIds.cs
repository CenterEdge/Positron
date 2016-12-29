namespace Positron.UI.Internal
{
    internal static class LoggerEventIds
    {
        public const int UnhandledError = 0;

        // PositronResourceHandler
        public const int RequestStarting = 1;
        public const int RequestFinished = 2;
        public const int RequestError = 2;
        public const int BadRedirectUrlFormat = 3;

        // WindowHandler
        public const int Startup = 100;
        public const int Shutdown = 101;
        public const int CreateBrowser = 102;
        public const int CreateWindow = 103;
        public const int RegisterGlobalScriptObjects = 104;

        // DisplayHandler
        public const int FaviconError = 200;

        // LifeSpanHandler
        public const int PopupWindowOpen = 300;
        public const int PopupWindowClose = 301;

        // KeyboardHandler
        public const int Refresh = 400;
        public const int OpenDebugTools = 401;

        // ResourceHandler
        public const int ExternalResource = 500;
        public const int ResourceRequestFilterError = 501;

        // DialogHandler
        public const int DialogHandlerError = 600;
        public const int UnknownDialogType = 601;
    }
}
