using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;

namespace WinFormsLab3
{
    public partial class Form1 : Form
    {
        private SQLiteHandler dbHandler = new SQLiteHandler();
        private bool isConnected = false;

        private List<string> blockedWords = new List<string>();
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
            if (!isConnected)
            {
                MessageBox.Show("Először csatlakozz az adatbázishoz!");
                return;
            }

            string newWord = toolStripTextBox2.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(newWord))
                return;

            if (blockedWords.Contains(newWord))
            {
                MessageBox.Show("A kulcsszó már létezik!");
                return;
            }

            try
            {
                dbHandler.InsertKeyword(newWord);

                blockedWords.Add(newWord);
                toolStripComboBox1.Items.Add(newWord);
                toolStripTextBox2.Clear();

                MessageBox.Show($"Kulcsszó sikeresen hozzáadva: {newWord}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a kulcsszó mentésekor: " + ex.Message);
            }
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dbHandler.ConnectToDb();
                isConnected = true;

                blockedWords.Clear();
                blockedWords.AddRange(dbHandler.GetAllKeywords());

                toolStripComboBox1.Items.Clear();

                foreach (string word in blockedWords)
                {
                    toolStripComboBox1.Items.Add(word);
                }

                MessageBox.Show(
                    "Successfully connected to the database!",
                    "Database",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                isConnected = false;

                MessageBox.Show(
                    "Database connection failed: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Az adatbázis nincs csatlakoztatva!");
                return;
            }

            dbHandler.DisconnectFromDb();
            isConnected = false;

            blockedWords.Clear();
            toolStripComboBox1.Items.Clear();

            MessageBox.Show("Sikeresen lecsatlakoztál az adatbázisról!");
        }

        private void addKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Először csatlakozz az adatbázishoz!");
                return;
            }

            using (Form2 form = new Form2())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string newWord = form.GetInputText().ToLower();

                    if (string.IsNullOrWhiteSpace(newWord))
                    {
                        MessageBox.Show("A kulcsszó nem lehet üres!");
                        return;
                    }

                    if (blockedWords.Contains(newWord))
                    {
                        MessageBox.Show("Ez a kulcsszó már létezik!");
                        return;
                    }

                    try
                    {
                        dbHandler.InsertKeyword(newWord);

                        blockedWords.Add(newWord);
                        toolStripComboBox1.Items.Add(newWord);

                        MessageBox.Show($"Kulcsszó sikeresen hozzáadva: {newWord}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hiba a mentés során: " + ex.Message);
                    }
                }
            }
        }

        private void viewKeywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Először csatlakozz az adatbázishoz!");
                return;
            }

            try
            {
                List<string> keywords = dbHandler.GetAllKeywords();

                if (keywords.Count == 0)
                {
                    MessageBox.Show("Az adatbázis nem tartalmaz kulcsszavakat.");
                    return;
                }

                string message = string.Join(Environment.NewLine, keywords);

                MessageBox.Show(
                    message,
                    "Adatbázisban tárolt kulcsszavak",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a kulcsszavak lekérésekor: " + ex.Message);
            }
        }

        private void removeKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Először csatlakozz az adatbázishoz!");
                return;
            }

            string keyword = toolStripComboBox1.Text.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Válassz ki vagy adj meg egy kulcsszót!");
                return;
            }

            if (!blockedWords.Contains(keyword))
            {
                MessageBox.Show("Ez a kulcsszó nem található a tiltólistában!");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Biztosan törölni szeretnéd ezt a kulcsszót: {keyword}?",
                "Kulcsszó törlése",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                dbHandler.DeleteKeyword(keyword);

                blockedWords.Remove(keyword);
                toolStripComboBox1.Items.Remove(keyword);
                toolStripComboBox1.Text = "";

                MessageBox.Show($"A(z) '{keyword}' kulcsszó sikeresen törölve!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a kulcsszó törlésekor: " + ex.Message);
            }
        }
    }
}
