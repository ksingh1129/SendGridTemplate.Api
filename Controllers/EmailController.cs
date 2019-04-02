using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;

namespace SendGridTemplate.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Email")]
    public class EmailController : Controller
    {

        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("email/send")]
        public async Task<ActionResult> SendEmail([FromBody] RecipientEmail recipient ) {
            try
            {
                var sendGridApiKey = _configuration.GetSection("SendGridAccount:ApiKey").Value;
                var senderEmail = _configuration.GetSection("SendGridAccount:SenderEmail").Value;
                var senderName = _configuration.GetSection("SendGridAccount:SenderName").Value;

                var client = new SendGridClient(sendGridApiKey);
                var msg = new SendGridMessage();

                var from = new EmailAddress(senderEmail, senderName);
                var toEmail = new EmailAddress(recipient.Email, recipient.Name);

                var reciepData = getReceiptData(recipient.Name);

                msg.SetFrom(from);
                msg.AddTo(toEmail);

                //SendGrid Email Template Id: "d-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
                msg.SetTemplateId(recipient.TemplateId); 
                msg.SetTemplateData(reciepData);

                var sendEmailResponse = await client.SendEmailAsync(msg);

                return Ok(sendEmailResponse);
            }
            catch(Exception ex) {
                return BadRequest(ex);
            }
        }

        private Receipt getReceiptData(string name)
        {
            var receipt = new Receipt {
                Name = name,
                Date = "1/4/2019",
                Total = "249",
                Cash = "500",
                Change = "251",
                ReceiptItems = new List<ReceiptItem> {
                    new ReceiptItem { Name = "ข้าวหอมมะลิ", Amount = "1", Price = "189.00"  },
                    new ReceiptItem { Name = "เรดบูลเอ็กซ์ต้ราแคน", Amount = "2", Price = "26.00" },
                    new ReceiptItem { Name = "โค้ก", Amount = "1", Price = "17.00"},
                    new ReceiptItem { Name = "เป๊ปซี่", Amount = "1", Price = "17.00"}
                }
            };

            return receipt;
        }
    }

    public class RecipientEmail {
        public string Name { get; set; }
        public string Email { get; set; }
        public string TemplateId { get; set; }
    }

    public class Receipt
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Date")]
        public string Date { get; set; }
        [JsonProperty("Total")]
        public string Total { get; set; }
        [JsonProperty("Cash")]
        public string Cash { get; set; }
        [JsonProperty("Change")]
        public string Change { get; set; }
        [JsonProperty("ReceiptItems")]
        public List<ReceiptItem> ReceiptItems { get; set; }
    }

    public class ReceiptItem
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Amount")]
        public string Amount { get; set; }
        [JsonProperty("Price")]
        public string Price { get; set; }
    }
}


