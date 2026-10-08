using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;

namespace WinFormsLab3
{
    public partial class Form1 : Form
    {
        private List<string> blockedWords = new List<string>
        {
            "facebook",
            "tiktok",
            "instagram"
        };
        private const string HomePage = "https://www.google.com";
        public Form1()
        {
            InitializeComponent();

            webView21.NavigationCompleted += WebView21_NavigationCompleted;

        }

        private void WebView21_NavigationCompleted(
            object? sender,
            CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess &&
                e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
            {
                MessageBox.Show(
                    $"Navigációs hiba: {e.WebErrorStatus}");
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();

            webView21.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

            Trace.Listeners.Clear();
            Trace.Listeners.Add(
                new TextWriterTraceListener("log.txt"));
            Trace.AutoFlush = true;

            webView21.Source = new Uri(HomePage);

            foreach (string word in blockedWords)
            {
                toolStripComboBox1.Items.Add(word);
            }
        }

        private void CoreWebView2_NavigationStarting(
            object? sender,
            CoreWebView2NavigationStartingEventArgs e)
        {
            string url = e.Uri.ToLower();

            string? blockedWord =
                blockedWords.FirstOrDefault(word => url.Contains(word));

            if (blockedWord != null)
            {
                e.Cancel = true;

                MessageBox.Show(
                    $"Tiltott oldal! ({blockedWord})");
            }
        }

        private async Task<string?> FindBlockedWordAsync(string url)
        {
            return await Task.Run(() =>
            {
                return blockedWords
                    .FirstOrDefault(word => url.Contains(word));
            });
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri(HomePage);
            await WriteLogAsync("Home button clicked");
        }

        private async void toolStripButton4_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri(toolStripTextBox1.Text);
            await WriteLogAsync("Go button clicked");
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (webView21.CanGoBack)
            {
                webView21.GoBack();
                await WriteLogAsync("Back button clicked");
            }
        }

        private async void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (webView21.CanGoForward)
            {
                webView21.GoForward();
                await WriteLogAsync("Forward button clicked");
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                webView21.Source = new Uri(toolStripTextBox1.Text);
            }
        }

        private async Task WriteLogAsync(string message)
        {
            await Task.Run(() =>
            {
                string logMessage =
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";

                Trace.WriteLine(logMessage);
            });
        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string newWord =
        toolStripTextBox2.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(newWord))
            {
                return;
            }

            bool exists =
                blockedWords.Any(word => word == newWord);

            if (exists)
            {
                MessageBox.Show(
                    "A kulcsszó már létezik!");

                return;
            }

            blockedWords.Add(newWord);

            toolStripComboBox1.Items.Add(newWord);

            toolStripTextBox2.Clear();

            MessageBox.Show(
                $"Kulcsszó hozzáadva: {newWord}");
        }
    }
}
