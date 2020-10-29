namespace PasteBin
{
    class LogEvents
    {
        // index 
        public const int TextUploaded = 1000;

        // index errors
        public const int TextUploadError = 1100;

        //  list
        public const int ViewFileRequest = 2000;
        public const int DeleteFileRequest = 2001;
        public const int DeleteAllRequest = 2002;

        // view text errors
        public const int ViewRequestError = 2100;
        public const int DeleteRequestError = 2101;

        // authentication 
        public const int RegisterSuccess = 4000;
        public const int RegisterFailed = 4001;
        public const int LoginSuccess = 4002;
        public const int LoginFailed = 4003;
        public const int LogoutSuccess = 4004;

    }
}