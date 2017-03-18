using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyBatch
{
	public class HandlerForMails
	{
		public static void SendReportIfNecessary(bool allSuccess, CustomTraceLog log)
		{
			string iniFilePath = CraftSynth.BuildingBlocks.Common.Misc.ApplicationPhysicalExeFilePathWithoutExtension + ".ini";

			bool SendMailOnSuccess = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SendMailOnSuccess", iniFilePath, true, false, true, false, false, false);
			string MailSubjectOnSuccess = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailSubjectOnSuccess", iniFilePath, SendMailOnSuccess, null, SendMailOnSuccess, null, false, null);
			bool SendMailOnError = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SendMailOnError", iniFilePath, true, false, true, false, false, false);
			string MailSubjectOnError = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailSubjectOnError", iniFilePath, SendMailOnSuccess, null, SendMailOnSuccess, null, false, null);

			bool SmtpIgnoreCertErrors = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SmtpIgnoreCertErrors", iniFilePath, false, false, true, false, false, false);
			bool SmtpViaOffice365 = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("SmtpViaOffice365", iniFilePath, true, false, true, false, false, false);
			string SmtpAddress = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpAddress", iniFilePath, true, null, true, null, false, null);
			int SmtpPort = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<int>("SmtpPort", iniFilePath, false, 25, true, 25, false, -1);
			string SmtpFrom = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpFrom", iniFilePath, true, null, true, null, false, null);
			string SmtpToCsv = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpToCsv", iniFilePath, true, null, true, null, false, null);
			string SmtpUsername = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpUsername", iniFilePath, true, null, true, null, false, null);
			string SmtpPassword = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("SmtpPassword", iniFilePath, true, null, true, null, false, null);

			MailMessage m = new MailMessage();
			m.From = new MailAddress(SmtpFrom);
			List<string> smtpToList = SmtpToCsv.ParseCSV();
			foreach (string s in smtpToList)
			{
				m.To.Add(new MailAddress(s));
			}
			m.Body = log.ToString();
			m.IsBodyHtml = false;

			if (SmtpIgnoreCertErrors)
			{
				ServicePointManager.ServerCertificateValidationCallback =
					delegate(object s, X509Certificate certificate,
						X509Chain chain, SslPolicyErrors sslPolicyErrors)
					{ return true; };
			}

			if (allSuccess && SendMailOnSuccess)
			{
				log.AddLine("Sending mails...");
				m.Subject = MailSubjectOnSuccess;
				CraftSynth.BuildingBlocks.IO.EMail.SendMail(SmtpViaOffice365, SmtpAddress, SmtpPort, SmtpUsername, SmtpPassword, m);
				log.AddLine("Sending mails done.");
			}
			else if(!allSuccess && SendMailOnError)
			{
				log.AddLine("Sending mails...");
				m.Subject = MailSubjectOnError;
				CraftSynth.BuildingBlocks.IO.EMail.SendMail(SmtpViaOffice365, SmtpAddress, SmtpPort, SmtpUsername, SmtpPassword, m);
				log.AddLine("Sending mails done.");
			}
		}
	}
}
