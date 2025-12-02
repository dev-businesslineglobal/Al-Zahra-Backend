//using System.Net.Mail;
//using System.Net;
//using System.Reflection;
//using System.Text;

//namespace GardeningAPI.Common
//{
//    public class Email
//    {
//        public void SendReport()

//        {

//            try

//            {

//                // ---------------- Filter emails with status P or F ----------------

//                var filteredEmails = email.Where(e => e.Status == "P" || e.Status == "F").ToList();

//                if (!filteredEmails.Any())

//                {

//                    Console.WriteLine("ℹ️ No processed emails to report (Status P/F).");

//                    return;

//                }

       


//                // ---------------- Build Email Body ----------------

//                var body = new StringBuilder();

//                body.AppendLine("<h2 style='color:#2E86C1; font-family:Arial, sans-serif;'>TCS Document Processing Summary</h2>");

//                body.AppendLine("<p>Dear Team,</p>");

//                body.AppendLine("<p>Please find attached the CSV file containing all successfully processed and failed documents.</p>");

//                body.AppendLine("<p><b>Report Highlights:</b></p>");

//                body.AppendLine("<ul>");

//                body.AppendLine($"<li>Total Documents Processed: {filteredEmails.Count}</li>");

//                body.AppendLine($"<li>Successful (P): {filteredEmails.Count(e => e.Status == "P")}</li>");

//                body.AppendLine($"<li>Failed (F): {filteredEmails.Count(e => e.Status == "F")}</li>");

//                body.AppendLine("</ul>");

//                body.AppendLine("<p>This is an auto-generated report by the <b>TCSService System</b>. Please do not reply to this email.</p>");

//                body.AppendLine("<p style='color:#888; font-size:12px;'>Generated automatically — for internal use only.</p>");

//                // ---------------- Send Email ----------------

//                using (var client = new SmtpClient(_smtpHost, _smtpPort))

//                {

//                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

//                    client.EnableSsl = true;

//                    var mail = new MailMessage

//                    {

//                        From = new MailAddress(_fromEmail),

//                        Subject = $"TCS Processed Documents Report - {DateTime.Now:yyyy-MM-dd HH:mm}",

//                        Body = body.ToString(),

//                        IsBodyHtml = true

//                    };

//                    mail.To.Add(_toEmail);

//                    foreach (var cc in _ccEmails)

//                    {

//                        if (!string.IsNullOrWhiteSpace(cc))

//                            mail.CC.Add(cc.Trim());

//                    }

//                    mail.Attachments.Add(new Attachment(filePath));

//                    client.Send(mail);

//                }

//                Console.WriteLine("📧 CSV report email sent successfully.");

//            }

//            catch (Exception ex)

//            {

//                Console.WriteLine($"❌ Failed to send email: {ex.Message}");

//            }

//        }


//    }
//}
