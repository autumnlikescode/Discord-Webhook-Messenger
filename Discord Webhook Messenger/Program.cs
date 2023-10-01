using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Webhook_Messenger
{
    internal class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        private static string webhookUrl;

        static async Task Main(string[] args)
        {
            Console.Title = "Discord Webhook Messenger";

            Console.WriteLine("Type 'exit' to quit.");
            Console.Write("Webhook > ");
            webhookUrl = Console.ReadLine();
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    break;
                }
                else if (input.StartsWith("!file "))
                {
                    string filePath = input.Substring(6); // Extract the file path from the command
                    await SendFileToDiscordWebhook(filePath);
                }
                else if (input.StartsWith("!embed"))
                {
                    await SendEmbedToDiscordWebhook();
                }
                else
                {
                    await SendMessageToDiscordWebhook(input);
                }
            }
        }

        static async Task SendMessageToDiscordWebhook(string message)
        {
            try
            {
                var payload = new
                {
                    content = message
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(webhookUrl, content);
                response.EnsureSuccessStatusCode();

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{message} (success)");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{message} (error: {ex.Message})");
                Console.ResetColor();
            }
        }

        static async Task SendFileToDiscordWebhook(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileContent = File.ReadAllBytes(filePath);

                    var formData = new MultipartFormDataContent();
                    formData.Add(new ByteArrayContent(fileContent), "file", Path.GetFileName(filePath));

                    var response = await _client.PostAsync(webhookUrl, formData);
                    response.EnsureSuccessStatusCode();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"File '{filePath}' sent successfully");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File '{filePath}' not found");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error sending file: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task SendEmbedToDiscordWebhook()
        {
            Console.Write("Enter embed title > ");
            string title = Console.ReadLine();

            Console.Write("Enter embed description/body > ");
            string description = Console.ReadLine();

            Console.Write("Enter embed author name > ");
            string authorName = Console.ReadLine();

            Console.Write("Enter embed footer text > ");
            string footerText = Console.ReadLine();

            Console.Write("Enter embed image URL (optional) > ");
            string imageUrl = Console.ReadLine();

            var embed = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = title,
                        description = description,
                        author = new { name = authorName },
                        footer = new { text = footerText },
                        image = new { url = imageUrl }
                    }
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(embed);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(webhookUrl, content);
                response.EnsureSuccessStatusCode();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Embed sent successfully");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error sending embed: {ex.Message}");
                Console.ResetColor();
            }
        }

    }
}
