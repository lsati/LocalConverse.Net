using LocalConverseClient.Net.Settings;
using LocalConverseLib.Net;
using Markdig;
using Markdown.Xaml;
using MSHTML;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;


namespace LocalConverseClient.Net
{
    /// <summary>s
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private LLMClient llmClient;
        private Markdown.Xaml.Markdown markdown;

        private LocalConverseSettings settings;

        public ChatPage()
        {
            InitializeComponent();
            InitializeAsync();
            ChatEntryTextBox.Visibility = Visibility.Collapsed;
            GoCancelButton.Visibility = Visibility.Collapsed;

            markdown = new Markdown.Xaml.Markdown();
        }

        private async void InitializeAsync()
        {
            var settings = LocalConverseSettingsManager.GetInstance();
            var llmConfig = new LLMConfig() { ModelPath = settings.LastSelectedModelName };

            this.llmClient = new LLMClient();
            _ = Task.Run(async () =>
            {
                await this.llmClient.StartChatListener(llmConfig,
                    OnModelLoadedCallback,
                    OnTokenCallback,
                    OnRepsonseCompletedCallback);
            });
        }


        private void OnModelLoadedCallback(string obj)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ChatEntryTextBox.Visibility = Visibility.Visible;
                GoCancelButton.Visibility = Visibility.Visible;
                ChatEntryTextBox.Focus();
            }));
        }



        private void OnRepsonseCompletedCallback(string fullResponse)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ChatEntryTextBox.Clear();
                ChatEntryTextBox.IsEnabled = true;
                GoCancelButton.Content = "Go";
            }));
        }

        private void OnTokenCallback(string text)
        {
            Trace.TraceInformation(text);
            if (string.Equals(text, "\nUser:") || string.Equals(text, "User")
                || text.Contains("User:"))
                return;

            this.Dispatcher.Invoke((Action)(() =>
            {
                AppendMarkdown(text);
            }));
        }

        private void ChatEntryTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //Keyboard.Modifiers == ModifierKeys.Control && 
            if (e.Key == Key.Enter)
            {
                SubmitUserQuery(ChatEntryTextBox.Text);
            }
        }

        private void SubmitUserQuery(string text)
        {
            this.llmClient.SubmitUserRequest(ChatEntryTextBox.Text);
            var html = $"\r\n\r\n> User: {ChatEntryTextBox.Text}\r\n\r\n";
            AppendMarkdown(html);


            ChatEntryTextBox.IsEnabled = false;
            GoCancelButton.Content = "X";
        }

        private void AppendMarkdown(string markdownText)
        {
            MarkdownViewer.Markdown += (markdownText);
            Scroller.ScrollToBottom();

        }

        private void GoCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(GoCancelButton.Content, "X"))
            {
                llmClient.CancelGeneration();
            }
            else
            {
                SubmitUserQuery(ChatEntryTextBox.Text);
            }
        }
    }
}
