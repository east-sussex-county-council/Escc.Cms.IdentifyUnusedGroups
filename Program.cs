using System;
using System.Net.Mail;
using EsccWebTeam.Cms.Permissions;
using log4net;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace Escc.Cms.IdentifyUnusedGroups
{
    class Program
    {
        // REMEMBER: if copying logging code, set assembly attribute for Log4Net in AssemblyInfo.cs
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Help();
                    return;
                }

                var groups = CmsPermissions.ReadUnusedCmsEditorGroups();
                if (groups.Count > 0)
                {
                    // Send it as an email
                    using (var message = new MailMessage())
                    {
                        message.To.Add(args[0]);
                        message.From = new MailAddress(Environment.MachineName + "@eastsussex.gov.uk");
                        message.Subject = "CMS unused groups";
                        foreach (var group in groups)
                        {
                            message.Body += group + Environment.NewLine;
                        }

                        var smtp = new SmtpClient();
                        smtp.Send(message);
                    }
                }

                Console.WriteLine(groups.Count + " unused groups found. Email sent to " + args[0]);
                log.Info(groups.Count + " unused groups found. Email sent to " + args[0]);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                log.Error(ex.Message);
            }

        }

        private static void Help()
        {
            Console.WriteLine("Usage: Escc.Cms.IdentifyUnusedGroups.exe emailTo");
        }
    }
}
