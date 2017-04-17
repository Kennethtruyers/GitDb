using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GitDb.Core;
using GitDb.Core.Interfaces;
using GitDb.Core.Model;
using GitDb.Local;
using GitDb.Watcher;
using LibGit2Sharp;
using Moq.AutoMock;
using Xunit;

namespace GitDb.Tests.Utils
{
    public abstract class WithWatcher : IAsyncLifetime
    {
        protected GitDb.Watcher.Watcher Subject;
        protected readonly string LocalPath = Path.GetTempPath() + Guid.NewGuid();
        protected IGitDb GitDb;
        protected Repository Repo;
        protected readonly Author Author = new Author("author", "author@mail.com");

        protected virtual Task Setup() => Task.CompletedTask;

        protected virtual Task Because() => Task.CompletedTask;

        static void deleteReadOnlyDirectory(string directory)
        {
            Directory.EnumerateDirectories(directory)
                .ForEach(deleteReadOnlyDirectory);
            Directory.EnumerateFiles(directory).Select(file => new FileInfo(file) { Attributes = FileAttributes.Normal })
                .ForEach(fi => fi.Delete());

            Directory.Delete(directory);
        }

        public async Task InitializeAsync()
        {
            GitDb = new LocalGitDb(LocalPath, new AutoMocker().Get<ILogger>());
            Repo = new Repository(LocalPath);
            await Setup();
            Subject = new GitDb.Watcher.Watcher(LocalPath, new AutoMocker().Get<ILogger>(), 1);
            Subject.MonitorEvents();
            Subject.Start(new List<BranchInfo>());
            await Because();
            Thread.Sleep(150);
        }

        public Task DisposeAsync()
        {
            Repo.Dispose();
            Subject.Dispose();
            GitDb.Dispose();
            deleteReadOnlyDirectory(LocalPath);
            return Task.CompletedTask;
        }
    }
}