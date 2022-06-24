using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Apiraiser.Services;
using Apiraiser.Models;
using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Helpers;
using Apiraiser.Filters;
using Amazon.S3.Model;
using System.IO;
using System.Net;
using Amazon.S3;
using Amazon;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Transfer;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Apiraiser.Plugins
{
    public class SESPlugin : BasePlugin
    {

        public SESPlugin()
        {

        }

        public async static Task<bool> SendEmailAsync(MimeMessage emailMimeMessage)
        {
            string s3AccessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string s3SecretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            string s3Bucket = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Bucket, null);
            string s3RegionStr = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Region, null);
            RegionEndpoint s3Region = RegionEndpoint.GetBySystemName(s3RegionStr);

            string sesSMTPUsername = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESSMPTPUsername, null);
            string sesSMTPPassword = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESSMPTPPassword, null);
            string sesRegion = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESRegion, null);

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(string.Format("email-smtp.{0}.amazonaws.com", sesRegion), 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(sesSMTPUsername, sesSMTPPassword);
                await smtp.SendAsync(emailMimeMessage);
                smtp.Disconnect(true);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await emailMimeMessage.WriteToAsync(memoryStream);
                    await S3Plugin.UploadFileAsync(s3AccessKeyId, s3SecretAccessKey, s3Region, s3Bucket, string.Format("sent/{0}/{1}", emailMimeMessage.From.ToString(), emailMimeMessage.MessageId.ToString()), memoryStream, "text/plain");
                }
                return true;

            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }

        public async static Task<MimeMessage> ProcessEmailAsync(string key)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            string bucket = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Bucket, null);
            string regionStr = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Region, null);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(regionStr);

            MimeMessage emailMimeMessage = null;
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                MemoryStream ms = null;

                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };

                using (var response = await client.GetObjectAsync(getObjectRequest))
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            await response.ResponseStream.CopyToAsync(ms);
                            //ms.Position = 0;
                            ms.Seek(0, 0);
                            ParserOptions options = ParserOptions.Default;
                            emailMimeMessage = await MimeMessage.LoadAsync(options, ms);
                            await S3Plugin.UploadFileAsync(accessKeyId, secretAccessKey, region, bucket, string.Format("inbox/{0}/{1}", emailMimeMessage.To.ToString(), emailMimeMessage.MessageId), ms, "text/plain");
                            await S3Plugin.DeleteFileAsync(accessKeyId, secretAccessKey, region, bucket, key);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return emailMimeMessage;
        }

        public async static Task<MimeMessage> ReadEmailAsync(string key)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            string bucket = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Bucket, null);
            string regionStr = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3Region, null);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(regionStr);

            MimeMessage emailMimeMessage = null;
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                MemoryStream ms = null;

                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };

                using (var response = await client.GetObjectAsync(getObjectRequest))
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            await response.ResponseStream.CopyToAsync(ms);
                            //ms.Position = 0;
                            ms.Seek(0, 0);
                            ParserOptions options = ParserOptions.Default;
                            emailMimeMessage = await MimeMessage.LoadAsync(options, ms);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return emailMimeMessage;
        }

    }
}
