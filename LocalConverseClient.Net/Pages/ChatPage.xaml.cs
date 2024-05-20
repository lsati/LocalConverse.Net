using LocalConverseLib.Net;
using Markdig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalConverseClient.Net
{
    /// <summary>
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private LLMClient llmClient;

        public ChatPage()
        {
            InitializeComponent();
            InitializeAsync();
            ChatEntryTextBox.Visibility = Visibility.Collapsed;
        }

        private async void InitializeAsync()
        {
            this.llmClient = new LLMClient();
            Task.Run(async () =>
            {
                await this.llmClient.StartChatListener(LLMConfig.Default(),
                    OnModelLoadedCallback,
                    OnTokenCallback,
                    OnRepsonseCompletedCallback);
            });
        }

        private void OnRepsonseCompletedCallback(string fullResponse)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Paragraph lastPara = ChatHistoryRtb.Document.Blocks.Last() as Paragraph;
                lastPara?.Inlines.Clear();

                var xaml = ConvertHtmlToXaml(Markdown.ToHtml(fullResponse));
                FlowDocument flowDoc = XamlReader.Parse(xaml) as FlowDocument;
                if (flowDoc != null)
                {

                    ChatHistoryRtb.Document.Blocks.Add(flowDoc.Blocks.FirstBlock);
                }
            }));
        }

        private string ConvertHtmlToXaml(string html)
        {
            // Convert HTML to XAML using your own implementation or a library like HtmlToXamlConverter
            // For simplicity, let's assume a placeholder implementation here
            // Placeholder implementation may involve replacing HTML tags with corresponding XAML elements
            // and attributes accordingly.
            return "<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Paragraph>" + html + "</Paragraph></FlowDocument>";
        }

        private void OnModelLoadedCallback(string obj)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ChatEntryTextBox.Visibility = Visibility.Visible;
            }));
        }

        private void OnTokenCallback(string text)
        {
            if (string.Equals(text, "\nUser:"))
                return;

            this.Dispatcher.Invoke((Action)(() =>
            {
                var paragraph = ConvertToAssistantOutput(text);


                //ChatHistoryRtb.AppendText(paragraph);

                Paragraph? para = ChatHistoryRtb.Document.Blocks.LastBlock as Paragraph;
                if (para == null)
                {
                    para = new Paragraph();
                    ChatHistoryRtb.Document.Blocks.Add(para);
                }
                para.Inlines.Add(new Run(paragraph));

            }));
        }

        private void ChatEntryTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                var paragraph = ConvertToUserInput(ChatEntryTextBox.Text);
                this.llmClient.SubmitUserRequest(ChatEntryTextBox.Text);

                ChatHistoryRtb.Document.Blocks.Add(paragraph);
                ChatHistoryRtb.Document.Blocks.Add(new Paragraph());
                ChatEntryTextBox.Clear();
            }
        }


        private string ConvertToAssistantOutput(string text)
        {
            return (text);
        }
        private Paragraph ConvertToUserInput(string text)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("<div style=\"border: 1px solid #ccc; background-color: #f9f9f9; box-shadow: 2px 2px 5px rgba(0,0,0,0.2); padding: 10px;\">\r\n");
            //sb.Append("<b>User</b>: ");
            //sb.Append(ChatEntryTextBox.Text);
            //sb.Append("</div>");


            Paragraph paragraph = new Paragraph();

            // Create a Run with the first word bold
            Bold boldFirstWord = new Bold(new Run("User: "));

            // Add the bold word and the rest of the text to the paragraph
            paragraph.Inlines.Add(boldFirstWord);
            paragraph.Inlines.Add(new Run(text));
            paragraph.Inlines.Add(new Run("\r\n"));

            // Set background color for the paragraph
            paragraph.Background = Brushes.LightGray;

            return paragraph;
        }
    }
}
