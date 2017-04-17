using System.IO;
using System.Linq;
using FluentAssertions;
using GitDb.Tests.Utils;
using Xunit;

namespace GitDb.Tests
{
    public class CreatingANewLocalGitDbClientInAnEmptyPath : WithRepo
    {
        [Fact]
        public void InitializesTheRepository() =>
            Directory.Exists(LocalPath).Should().BeTrue();

        [Fact]
        public void AddsAMasterBranch() =>
            Repo.Branches.Select(b => b.FriendlyName).Should().Contain("master");

        [Fact]
        public void CreatesAnInitialCommit() =>
            Repo.Branches["master"].Commits.Count().Should().Be(1);
    }
}
