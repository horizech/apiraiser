using System;
using System.Collections.Generic;
using Apiraiser.Models;

namespace Apiraiser.Constants
{
    public class Config
    {
        // Database
        public const string DatabaseType = "APIRAISER_DATABASE_TYPE";
        public const string DatabaseHost = "APIRAISER_DATABASE_HOST";
        public const string DatabasePort = "APIRAISER_DATABASE_PORT";
        public const string DatabaseName = "APIRAISER_DATABASE_NAME";
        public const string DatabaseUsername = "APIRAISER_DATABASE_USERNAME";
        public const string DatabasePassword = "APIRAISER_DATABASE_PASSWORD";

        // Plugins
        public const string AWSSESRegion = "APIRAISER_AWS_SES_REGION";
        public const string AWSSESSMPTPUsername = "APIRAISER_AWS_SES_SMTP_USERNAME";
        public const string AWSSESSMPTPPassword = "APIRAISER_AWS_SES_SMTP_PASSWORD";
        public const string AWSSESS3Bucket = "APIRAISER_AWS_SES_S3_BUCKET";
        public const string AWSSESS3Region = "APIRAISER_AWS_SES_S3_REGION";
        public const string AWSSESS3AccessKeyId = "APIRAISER_AWS_SES_S3_ACCESS_KEY_ID";
        public const string AWSSESS3SecretAccessKey = "APIRAISER_AWS_SES_S3_SECRET_ACCESS_KEY";

        // JWT Token
        public const string JwtTokenIssuer = "APIRAISER_JWT_TOKEN_ISSUER";
        public const string JwtTokenSecretKey = "APIRAISER_JWT_TOKEN_SECRET_KEY";
        public const string DefaultJwtTokenSecretKey = "KEY__SECRET__KEY";
        public const string DefaultJwtTokenIssuer = "Apiraiser";

        // Logging
        public const string LoggingLevel = "APIRAISER_LOGGING_LEVEL";
        public const string DefaultLoggingLevel = "Errors";

        // Media
        public const string MediaLocalPath = "APIRAISER_MEDIA_LOCAL_PATH";
        public const string MediaS3Url = "APIRAISER_MEDIA_S3_URL";
        public const string MediaS3Path = "APIRAISER_MEDIA_S3_PATH";
        public const string MediaS3AccessKeyId = "APIRAISER_MEDIA_S3_ACCESS_KEY_ID";
        public const string MediaS3SecretAccessKey = "APIRAISER_MEDIA_S3_SECRET_ACCESS_KEY";

        public const string ConfigurationDirectory = @"Configurations";
        public const string TablesJson = "Tables.json";

    }
}