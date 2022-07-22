/*
 * Copyright (c) 2021 Tris Shores
 * Open source software. Licensed under the MIT License.
*/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrChecker
{
    internal class PullRequests
    {
        #region GitHub polling

        internal static async Task<string> GitHubPrsAsync(DateTime fromDate, string prState)
        {
            // gh auth login --web
            // gh auth logout --hostname github.com

            List<GitHubRepo> repoList = new();
            StringBuilder sb = new();
            string resPr = null;

            //return "Event date ||| Event description ||| Current state ||| Is draft ||| Web URL ||| Title\r\n9/24/2021 ||| PR created ||| OPEN ||| No ||| https://github.com/dotnet/docs-desktop/pull/1160 ||| Content update - Dependency properties overview (user story 1878471)\r\n9/24/2021 ||| PR merged ||| MERGED ||| No ||| https://github.com/dotnet/docs-desktop/pull/1153 ||| Content update - print-overview (user story 1861941)";

            await Task.Run(() =>
            {
                // Test whether GitHub CLI is installed.
                try
                {
                    ProcessTools.RunProcess("gh", $"--version");
                }
                catch
                {
                    throw new Exception("First install GitHub CLI.");
                }

                (int exitCode, string outData, string errData) = ProcessTools.RunProcess("gh", $"auth status --hostname \"github.com\"");
                if (exitCode != 0)
                {
                    // Initiate web login:
                    ProcessStartInfo si = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = "/C gh auth login --web"
                    };
                    Process res0 = Process.Start(si);
                    res0.WaitForExit();
                    if (res0.ExitCode != 0)
                    {
                        MessageBox.Show("Authentication failed.");
                        return;
                    }
                }

                (exitCode, outData, errData) = ProcessTools.RunProcess("gh", $"repo list --fork --json parent");
                if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception("repo list error");

                JArray jArray1 = JArray.Parse(outData);
                foreach (JToken obj in jArray1)
                {
                    GitHubRepo repo = new();
                    repo.Owner = (string)obj["parent"]["owner"]["login"];
                    repo.OwnerId = (string)obj["parent"]["owner"]["id"];
                    repo.Name = (string)obj["parent"]["name"];
                    repoList.Add(repo);
                }

                foreach (GitHubRepo repo in repoList)
                {
                    (int exitCode, string outData, string errData) test = ProcessTools.RunProcess("gh", $"pr list --json");

                    (exitCode, outData, errData) = ProcessTools.RunProcess("gh", $"pr list --author @me --state {(prState == "Open or Merged" ? "all" : prState.ToLower())} --repo {repo.FullName} --search created:>={fromDate:yyy-MM-dd} --limit 1000 --json number,url,title,state,isDraft,createdAt,mergedAt,closedAt,mergedBy,labels,comments,files,changedFiles");
                    if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception("pr list error");
                    JArray jArray2 = JArray.Parse(outData);

                    (exitCode, outData, errData) = ProcessTools.RunProcess("gh", $"pr list --author @me --state {(prState == "Open or Merged" ? "all" : prState.ToLower())} --repo {repo.FullName} --search merged:>={fromDate:yyy-MM-dd} --limit 1000 --json number,url,title,state,isDraft,createdAt,mergedAt,closedAt,mergedBy,labels,comments,files,changedFiles");
                    if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception("pr list error");
                    JArray jArray3 = JArray.Parse(outData);

                    foreach (JToken obj in new JArray(jArray2.Union(jArray3)))
                    {
                        GitHubPr pr = new();
                        pr.Number = (int)obj["number"];
                        pr.Url = (string)obj["url"];
                        pr.Title = (string)obj["title"];
                        pr.State = (string)obj["state"];
                        pr.IsDraft = (bool)obj["isDraft"];
                        pr.MergedBy = (string)obj.SelectToken("mergedBy.login");
                        pr.CreatedAt = DateTime.ParseExact((string)obj["createdAt"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        if (pr.State == "MERGED")
                            pr.MergedAt = DateTime.ParseExact((string)obj["mergedAt"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        if (pr.State == "CLOSED")
                            pr.ClosedAt = DateTime.ParseExact((string)obj["closedAt"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        if (!repo.prList.Any(x => x.Number == pr.Number)) repo.prList.Add(pr);

                        // Get changed file stats (`changedFiles` returns a count value, `files` returns stats for each changed file but only for first 100 changed files).
                        JToken fileItems = obj["files"];
                        pr.ChangedFileCount = obj["changedFiles"].Value<int>();
                        foreach (JToken fileItem in fileItems)
                        {
                            PrFile prFile = new();
                            prFile.Path = fileItem["path"].Value<string>();
                            prFile.Additions = fileItem["additions"].Value<int>();
                            prFile.Deletions = fileItem["deletions"].Value<int>();
                            pr.PrFileList.Add(prFile);
                        }

                        // Get comments.
                        JToken commentItems = obj["comments"];
                        foreach (JToken commentItem in commentItems)
                        {
                            PrComment prComment = new();
                            prComment.Author = (string)commentItem.SelectToken("author.login");
                            prComment.CreatedAt = DateTime.ParseExact((string)commentItem["createdAt"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            prComment.Body = (string)commentItem["body"];
                            pr.PrCommentList.Add(prComment);
                        }

                        // Get labels.
                        JToken labelItems = obj["labels"];
                        foreach (JToken labelItem in labelItems)
                        {
                            PrLabel prLabel = new();
                            prLabel.Name = (string)labelItem["name"];
                            pr.PrLabelList.Add(prLabel);
                        }
                    }
                }

                // Format stats.
                List<PrEvent> prEventList = new();
                foreach (GitHubRepo repo in repoList)
                    foreach (GitHubPr pr in repo.prList)
                    {
                        string stats = $"{pr.Url} ||| {pr.Title} ||| {pr.FileTypeCount.Text} $$ {pr.FileType.Text} ||| {pr.FileTypeCount.Image} $$ {pr.FileType.Image} ||| {pr.FileTypeCount.Code} $$ {pr.FileType.Code} ||| {pr.FileTypeCount.UnknownType} $$ {pr.FileType.UnknownType} ||| {pr.FileModsCount}";

                        if (pr.State == "OPEN" && (prState == "All" || prState == "Open or Merged" || prState == "Open"))
                        {
                            PrEvent prEvent1 = new(pr.CreatedAt, $"{pr.CreatedAt.ToShortDateString()} ||| PR created ||| OPEN ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                            prEventList.Add(prEvent1);
                        }

                        if (pr.State == "MERGED" && (prState == "All" || prState == "Open or Merged" || prState == "Merged"))
                        {
                            PrEvent prEvent1 = new(pr.CreatedAt, $"{pr.CreatedAt.ToShortDateString()} ||| PR created ||| MERGED ||| No ||| {stats}");
                            prEventList.Add(prEvent1);

                            PrEvent prEvent2 = new(pr.MergedAt, $"{pr.MergedAt.ToShortDateString()} ||| PR merged ||| MERGED ||| No ||| {stats}");
                            prEventList.Add(prEvent2);
                        }

                        if (pr.State == "CLOSED" && (prState == "All" || prState == "Closed"))
                        {
                            PrEvent prEvent1 = new(pr.CreatedAt, $"{pr.CreatedAt.ToShortDateString()} ||| PR created ||| CLOSED ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                            prEventList.Add(prEvent1);

                            PrEvent prEvent2 = new(pr.ClosedAt, $"{pr.ClosedAt.ToShortDateString()} ||| PR closed ||| CLOSED ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                            prEventList.Add(prEvent2);
                        }
                    }

                sb.AppendLine("Event date ||| Event description ||| Current state ||| Is draft ||| Web URL ||| Title ||| Text files ||| Image files ||| Code files ||| Unknown files ||| Text lines modified");
                prEventList.Where(x => x.Date >= fromDate).OrderByDescending(x => x.Date).ToList().ForEach(x => sb.AppendLine(x.Message));
            });

            resPr = sb.ToString();
            return resPr;
        }

        #endregion

        #region Azure DevOps polling

        internal static async Task<string> DevOpsPrsAsync(DateTime fromDate, string prState)
        {
            // az login

            List<DevOpsProject> projList = new();
            StringBuilder sb = new();
            string resPr = null;

            await Task.Run(async () =>
            {
                // Test whether Azure CLI is installed.
                (int exitCode, string outData, string errData) = ProcessTools.RunProcess("where", $"az");
                if (exitCode != 0) throw new Exception("First install Azure CLI.");

                // Get username.
                (exitCode, outData, errData) = ProcessTools.RunProcess("cmd", "/C az account list --query \"[0].{User:user.name}\"", timeoutMs: 5000);
                if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception($"Azure CLI error message: \"{errData}\"");
                if (outData.StartsWith("[]")) return;
                JObject jObjUsername = JObject.Parse(outData);
                string username = (string)jObjUsername["User"];

                // Get project list.
                (exitCode, outData, errData) = ProcessTools.RunProcess("cmd", $"/C az devops project list");
                if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception($"Azure CLI error message: \"{errData}\"");

                // Parse project list.
                JObject jObj = JObject.Parse(outData);
                JArray jArray1 = (JArray)jObj["value"];
                foreach (JToken obj in jArray1)
                {
                    DevOpsProject proj = new();
                    proj.Id = (string)obj["id"];
                    proj.Name = (string)obj["name"];
                    projList.Add(proj);
                }

                // Iterate projects to get repo info.
                foreach (DevOpsProject proj in projList)
                {
                    if (proj.Name != "Embedded") continue;  // debug

                    (exitCode, outData, errData) = ProcessTools.RunProcess("cmd", $"/C az repos list --project {proj.Id}");
                    if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) throw new Exception($"Azure CLI error message: \"{errData}\"");

                    JArray jArray2 = JArray.Parse(outData);
                    foreach (JToken obj2 in jArray2)
                    {
                        DevOpsRepo repo = new();
                        repo.Name = (string)obj2["name"];
                        repo.WebUrl = (string)obj2["webUrl"];
                        proj.repoList.Add(repo);

                        if (repo.Name != "PowerBI-CSharp") continue;  // debug

                        // Get PR info for each repo.
                        (exitCode, outData, errData) = ProcessTools.RunProcess("cmd", $"/C az repos pr list --project {proj.Id} --repository {repo.Name} --creator {username} --status all");
                        if (exitCode != 0 || errData.Trim(new[] { '\r', '\n' }).Length > 0) continue;
                        if (outData.StartsWith("[]")) continue;
                        JArray jArray3 = JArray.Parse(outData);

                        // Parse PRs.
                        foreach (JToken obj3 in jArray3)
                        {
                            DevOpsPr pr = new();
                            pr.PullRequestId = (int)obj3["pullRequestId"];
                            pr.CreationDate = DateTime.ParseExact((string)obj3["creationDate"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            pr.Url = repo.WebUrl + "/pullrequest/" + pr.PullRequestId;
                            pr.Title = (string)obj3["title"];
                            pr.Status = (string)obj3["status"];  // abandoned, active, all, completed
                            pr.mergeStatus = (string)obj3["mergeStatus"];
                            pr.IsDraft = (bool)obj3["isDraft"];
                            repo.PrList.Add(pr);

                            if (pr.Status != "active")
                            {
                                pr.ClosedDate = DateTime.ParseExact((string)obj3["closedDate"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            }

                            // Get commits in PR.
                            Uri prUrl = new Uri(pr.Url);
                            string collection = prUrl.Segments[1].Trim('/');
                            // e.g. $"https://powerbi.visualstudio.com/DefaultCollection/Embedded/_apis/git/repositories/PowerBI-CSharp/pullRequests/201738/commits?api-version=5.";
                            string url = $"https://{prUrl.Host}/{collection}/{proj.Name}/_apis/git/repositories/{repo.Name}/pullRequests/{pr.PullRequestId}/commits?api-version=5.";
                            if (pr.PullRequestId == 201738)
                            { }
                            outData = await AzureDevOps_GetStringAsync(url);
                            if (outData == null) continue;

                            // Parse commits.
                            jObj = JObject.Parse(outData);
                            JArray jArray4 = (JArray)jObj["value"];
                            foreach (JToken obj4 in jArray4)
                            {
                                DevOpsCommit commit = new();
                                commit.Id = (string)obj4["commitId"];
                                commit.WebUrl = (string)obj4["url"];
                                pr.PrCommitList.Add(commit);

                                // Get files in commit.
                                url = $"https://{prUrl.Host}/{collection}/{proj.Name}/_apis/git/repositories/{repo.Name}/commits/{commit.Id}/changes?api-version=5.0";
                                outData = await AzureDevOps_GetStringAsync(url);
                                if (outData == null) continue;

                                // Get files.
                                jObj = JObject.Parse(outData);
                                JArray jArray5 = (JArray)jObj["changes"];
                                foreach (JToken obj5 in jArray5)
                                {
                                    string isFolder = (string)obj5.SelectToken("item.isFolder");
                                    if (isFolder == "True")
                                        continue;
                                    DevOpsFile adoFile = new();
                                    string fileUrl = ((string)obj5.SelectToken("item.url")).Replace("%2F", "/");
                                    Uri fileUri = new Uri(fileUrl);
                                    adoFile.Name = fileUri.Segments.Last().Trim('/');
                                    commit.FileList.Add(adoFile);
                                }
                            }
                        }
                    }
                }

                // Format stats.
                List<PrEvent> prEventList = new();
                foreach (DevOpsProject proj in projList)
                    foreach (DevOpsRepo repo in proj.repoList)
                        foreach (DevOpsPr pr in repo.PrList)
                        {
                            string stats = $"{pr.Url} ||| {pr.Title} ||| {pr.FileTypeCount.Text} $$ {pr.FileType.Text} ||| {pr.FileTypeCount.Image} $$ {pr.FileType.Image} ||| {pr.FileTypeCount.Code} $$ {pr.FileType.Code} ||| {pr.FileTypeCount.UnknownType} $$ {pr.FileType.UnknownType} ||| {pr.FileModsCount}";

                            if (pr.Status == "active" && (prState == "All" || prState == "Open or Merged" || prState == "Open"))
                            {
                                PrEvent prEvent1 = new(pr.CreationDate, $"{pr.CreationDate.ToShortDateString()} ||| Created ||| OPEN ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                                prEventList.Add(prEvent1);
                            }

                            if (pr.Status == "completed" && (prState == "All" || prState == "Open or Merged" || prState == "Merged"))
                            {
                                PrEvent prEvent1 = new(pr.CreationDate, $"{pr.CreationDate.ToShortDateString()} ||| Created ||| MERGED ||| No ||| {stats}");
                                prEventList.Add(prEvent1);

                                PrEvent prEvent2 = new(pr.ClosedDate, $"{pr.ClosedDate.ToShortDateString()} ||| Merged ||| MERGED ||| No ||| {stats}");
                                prEventList.Add(prEvent2);
                            }

                            if (pr.Status == "abandoned" && (prState == "All" || prState == "Closed"))
                            {
                                PrEvent prEvent1 = new(pr.CreationDate, $"{pr.CreationDate.ToShortDateString()} ||| Created ||| CLOSED ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                                prEventList.Add(prEvent1);

                                PrEvent prEvent2 = new(pr.ClosedDate, $"{pr.ClosedDate.ToShortDateString()} ||| Closed ||| CLOSED ||| {(pr.IsDraft ? "Yes" : "No")} ||| {stats}");
                                prEventList.Add(prEvent2);
                            }
                        }

                sb.AppendLine("Event date ||| Event description ||| Current state ||| Is draft ||| Web URL ||| Title ||| Text files ||| Image files ||| Code files ||| Unknown files ||| Text lines modified");
                prEventList.Where(x => x.Date >= fromDate).OrderByDescending(x => x.Date).ToList().ForEach(x => sb.AppendLine(x.Message));
            });

            resPr = sb.ToString();
            return resPr;
        }

        #endregion

        #region Helper methods

        // Instantiate HttpClient once only.
        private static readonly HttpClient client = new();

        private static async Task<string> AzureDevOps_GetStringAsync(string uri)
        {
            try
            {
                string personalaccesstoken = Environment.GetEnvironmentVariable("AzureDevOps_Token", EnvironmentVariableTarget.User);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalaccesstoken))));

                string response = await client.GetStringAsync(uri);
                return response;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        #endregion
    }

    #region DevOps classes

    internal class DevOpsProject
    {
        internal string Id;
        internal string Name;
        internal List<DevOpsRepo> repoList = new();
    }

    internal class DevOpsRepo
    {
        internal string Name;
        internal string WebUrl;
        internal List<DevOpsPr> PrList = new();
    }

    internal class DevOpsCommit
    {
        internal string Id;
        internal string WebUrl;
        internal List<DevOpsFile> FileList = new();
    }

    internal class DevOpsPr
    {
        internal int PullRequestId;
        internal string Title;
        internal string Url;
        internal string Status;
        internal string mergeStatus;
        internal bool IsDraft;
        internal DateTime CreationDate;
        internal DateTime ClosedDate;
        internal List<DevOpsCommit> PrCommitList = new();
        internal int ChangedFileCount { get => PrFileList.Count(); }
        internal List<DevOpsFile> PrFileList
        {
            get
            {
                List<DevOpsFile> fileNames = new();
                foreach (DevOpsCommit prCommit in PrCommitList)
                {
                    foreach (DevOpsFile file in prCommit.FileList)
                    {
                        if (fileNames.All(x => !Equals(x.Name, file.Name)))
                            fileNames.Add(file);
                    }
                }
                return fileNames.OrderBy(x => x.Name).ToList();
            }
        }

        internal string FileList
        {
            get
            {
                return string.Join("\n", PrFileList.Select(x => Path.GetFileName(x.Name)));
            }
        }


        internal FileType FileType
        {
            get
            {
                FileType fileType = new();

                fileType.Text = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsTextFile(x.Name)).Select(x => Path.GetFileName(x.Name)));
                fileType.Image = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsImageFile(x.Name)).Select(x => Path.GetFileName(x.Name)));
                fileType.Code = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsCodeFile(x.Name)).Select(x => Path.GetFileName(x.Name)));
                fileType.UnknownType = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsUnknownFile(x.Name)).Select(x => Path.GetFileName(x.Name)));

                return fileType;
            }
        }

        internal FileTypeCount FileTypeCount
        {
            get
            {
                FileTypeCount fileTypeCount = new();
                fileTypeCount.Text = PrFileList.Where(x => FileTypeUtility.IsTextFile(x.Name)).Count();
                fileTypeCount.Image = PrFileList.Where(x => FileTypeUtility.IsImageFile(x.Name)).Count();
                fileTypeCount.Code = PrFileList.Where(x => FileTypeUtility.IsCodeFile(x.Name)).Count();
                fileTypeCount.UnknownType = PrFileList.Count - fileTypeCount.Text - fileTypeCount.Image - fileTypeCount.Code;
                return fileTypeCount;
            }
        }

        internal string FileModsCount
        {
            get
            {
                return "unknown";
            }
        }
    }

    internal class DevOpsFile
    {
        internal string Name;
    }

    #endregion

    #region GitHub classes

    internal class GitHubRepo
    {
        internal string Owner;
        internal string OwnerId;
        internal string Name;
        internal string FullName { get => $"{Owner}/{Name}"; }
        internal List<GitHubPr> prList = new();
    }

    internal class GitHubPr
    {
        internal int Number;
        internal string Title;
        internal string Url;
        internal string State;
        internal bool IsDraft;
        internal DateTime CreatedAt;
        internal DateTime MergedAt;
        internal DateTime ClosedAt;
        internal int ChangedFileCount;
        internal List<PrFile> PrFileList = new();   // gh cap at 100 changed files.
        internal string MergedBy;
        internal List<PrLabel> PrLabelList = new();
        internal List<PrComment> PrCommentList = new();

        internal FileType FileType
        {
            get
            {
                FileType fileType = new();

                if (ChangedFileCount <= 100)
                {
                    fileType.Text = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsTextFile(x.Path)).Select(x => Path.GetFileName(x.Path)));
                    fileType.Image = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsImageFile(x.Path)).Select(x => Path.GetFileName(x.Path)));
                    fileType.Code = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsCodeFile(x.Path)).Select(x => Path.GetFileName(x.Path)));
                    fileType.UnknownType = string.Join("\n", PrFileList.Where(x => FileTypeUtility.IsUnknownFile(x.Path)).Select(x => Path.GetFileName(x.Path)));
                }
                else
                {
                    fileType.Text = "N/A (> 100 files)";
                    fileType.Image = "N/A (> 100 files)";
                    fileType.Code = "N/A (> 100 files)";
                    fileType.UnknownType = "N/A (> 100 files)";
                }

                return fileType;
            }
        }

        internal FileTypeCount FileTypeCount
        {
            get
            {
                FileTypeCount fileTypeCount = new();

                if (ChangedFileCount <= 100)
                {
                    fileTypeCount.Text = PrFileList.Where(x => FileTypeUtility.IsTextFile(x.Path)).Count();
                    fileTypeCount.Image = PrFileList.Where(x => FileTypeUtility.IsImageFile(x.Path)).Count();
                    fileTypeCount.Code = PrFileList.Where(x => FileTypeUtility.IsCodeFile(x.Path)).Count();
                    fileTypeCount.UnknownType = PrFileList.Count - fileTypeCount.Text - fileTypeCount.Image - fileTypeCount.Code;
                    return fileTypeCount;
                }
                else
                {
                    fileTypeCount.UnknownType = ChangedFileCount;
                    return fileTypeCount;
                }
            }
        }

        internal string FileModsCount
        {
            get
            {
                if (ChangedFileCount <= 100)
                {
                    int mods = 0;
                    PrFileList.ForEach(x => mods += x.Additions + x.Deletions);
                    return mods.ToString();
                }
                else
                {
                    return "unknown";
                }
            }
        }
    }

    internal class PrComment
    {
        internal string Author;
        internal DateTime CreatedAt;
        internal string Body;
    }

    internal class PrLabel
    {
        internal string Name;
        internal DateTime CreatedAt;
    }

    internal class PrEvent
    {
        internal DateTime Date;
        internal string Message;

        public PrEvent(DateTime date, string message)
        {
            Date = date;
            Message = message;
        }
    }

    internal class PrFile
    {
        internal string Path;
        internal int Additions;
        internal int Deletions;
    }

    #endregion

    #region Common classes

    internal static class FileTypeUtility
    {
        internal static bool IsImageFile(string filePath)
        {
            return
                Path.GetExtension(filePath).ToLower() == ".png" ||
                Path.GetExtension(filePath).ToLower() == ".jpg" ||
                Path.GetExtension(filePath).ToLower() == ".jpeg" ||
                Path.GetExtension(filePath).ToLower() == ".gif";
        }

        internal static bool IsTextFile(string filePath)
        {
            return
                Path.GetExtension(filePath).ToLower() == ".md" ||
                Path.GetExtension(filePath).ToLower() == ".txt";
        }

        internal static bool IsCodeFile(string filePath)
        {
            if (string.Equals(Path.GetFileName(filePath), "app.xaml")) return false;
            if (string.Equals(Path.GetFileName(filePath), "App.xaml.cs")) return false;
            if (string.Equals(Path.GetFileName(filePath), "AssemblyInfo.cs")) return false;
            if (string.Equals(Path.GetFileName(filePath), "AssemblyInfo.vb")) return false;
            if (string.Equals(Path.GetFileName(filePath), "Application.xaml")) return false;
            if (string.Equals(Path.GetFileName(filePath), "Application.xaml.vb")) return false;
            if (string.Equals(Path.GetFileName(filePath), "Resources.Designer.cs")) return false;

            return
                Path.GetExtension(filePath).ToLower() == ".vb" ||
                Path.GetExtension(filePath).ToLower() == ".cs" ||
                Path.GetExtension(filePath).ToLower() == ".xaml" ||
                Path.GetExtension(filePath).ToLower() == ".json" ||
                Path.GetExtension(filePath).ToLower() == ".html" ||
                Path.GetExtension(filePath).ToLower() == ".xml" ||
                Path.GetExtension(filePath).ToLower() == ".yml";
                //Path.GetExtension(filePath).ToLower() == ".csproj" ||
                //Path.GetExtension(filePath).ToLower() == ".vbproj" ||
                //Path.GetExtension(filePath).ToLower() == ".resx";
        }

        internal static bool IsUnknownFile(string filePath)
        {
            return !IsTextFile(filePath) && !IsImageFile(filePath) && !IsCodeFile(filePath);
        }
    }

    internal class FileTypeCount
    {
        internal int Text;
        internal int Image;
        internal int Code;
        internal int UnknownType;
    }

    internal class FileType
    {
        internal string Text;
        internal string Image;
        internal string Code;
        internal string UnknownType;
    }

    #endregion
}