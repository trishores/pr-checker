/*
 * Copyright (C) 2021 Tris Shores
 * Open source software. Licensed under the MIT license: https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrChecker
{
    public partial class Ui : Form
    {
        #region Initialization

        public Ui()
        {
            InitializeComponent();

            // Defaults.
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Text = $"PR checker v{version}";
            cmbRepoType.Items.Add("GitHub");
            cmbRepoType.Items.Add("Azure DevOps");
            cmbRepoType.SelectedIndex = 0;
            //tabControl.SelectedTab = tabPullRequests;
            dtpFrom.Value = DateTime.Now.AddDays(DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - DateTime.Now.DayOfWeek);
            cmbPrState.Items.AddRange(new[] { "All", "Open or Merged", "Open", "Merged", "Closed" });
            cmbPrState.SelectedIndex = 1;
            cmbRepoType.SelectedIndexChanged += CmbRepoType_SelectedIndexChanged;

            // Events.
            Shown += Ui_Shown;
            btnRun.Click += BtnRun_Click;
            tabPullRequests.Enter += TabPullRequests_Enter;
        }

        private void Ui_Shown(object sender, EventArgs e)
        {
            btnRun.Focus();
        }

        #endregion

        #region Pull requests tab events

        private void TabPullRequests_Enter(object sender, EventArgs e)
        {
            CmbRepoType_SelectedIndexChanged(null, null);
        }

        private void CmbRepoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string product, url, authCmd;

            if (cmbRepoType.Text == "GitHub")
            {
                product = "GitHub CLI";
                url = "https://cli.github.com";
                authCmd = "gh auth login --web";
            }
            else
            {
                product = "Azure CLI";
                url = "https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows";
                authCmd = "az login";
            }

            txtPrereqs.Text = "Prerequisites:\r\n\r\n";
            txtPrereqs.Text += $"1) Download & install {product} from: {url}\r\n\r\n";
            txtPrereqs.Text += $"2) Run in your terminal to authenticate: {authCmd}\r\n\r\n";
            txtPrereqs.Text += $"3) Restart this app after authentication.";
        }

        #endregion

        #region Run button events

        private async void BtnRun_Click(object sender, EventArgs e)
        {
            try
            {
                // Set UI busy indicators.
                ShowUiWaitAnimation(true);

                // Select action.
                if (Equals(tabControl.SelectedTab.Text, "Pull Requests") && cmbRepoType.Text == "GitHub")
                {
                    await GitHubPrsAsync();
                }
                else if (Equals(tabControl.SelectedTab.Text, "Pull Requests") && cmbRepoType.Text == "Azure DevOps")
                {
                    await DevOpsPrsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PR checker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowUiWaitAnimation(false);
            }
        }

        #region Get GitHub pull request stub

        private async Task GitHubPrsAsync()
        {
            // Run I/O task asynchronously.
            var resPr = await PullRequests.GitHubPrsAsync(dtpFrom.Value, cmbPrState.Text);

            await Task.Run(() =>
            {
                try
                {
                    // Show result stats.
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "github-pr.xlsx");
                    ExcelTools.WriteExcelFile(ref filePath, resPr);
                    ProcessTools.RunProcess("cmd", $"start /r \"{filePath}\"", waitForExit: false);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "PR checker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            ShowUiWaitAnimation(false);
        }

        #endregion

        #region Get DevOps pull request stub

        private async Task DevOpsPrsAsync()
        {
            // Run I/O task asynchronously.
            var resPr = await PullRequests.DevOpsPrsAsync(dtpFrom.Value, cmbPrState.Text);

            await Task.Run(() =>
            {
                try
                {
                    // Show result stats.
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"ado-pr.xlsx");
                    ExcelTools.WriteExcelFile(ref filePath, resPr);
                    if (File.Exists(filePath))
                    {
                        ProcessTools.RunProcess("cmd", $"start /r \"{filePath}\"", waitForExit: false);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "PR checker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            ShowUiWaitAnimation(false);
        }

        #endregion

        #endregion

        #region Animation methods

        private void ShowUiWaitAnimation(bool animate)
        {
            // Set temporary focus during animation.
            if (animate) btnFocus.Focus();
            else btnRun.Focus();

            // Handle animation.
            pbxSpinner.Visible = animate;
            btnRun.Visible = !animate;
        }

        #endregion
    }
}