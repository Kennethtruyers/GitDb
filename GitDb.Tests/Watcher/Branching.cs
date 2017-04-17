using System.Threading.Tasks;
using FluentAssertions;
using GitDb.Core.Model;
using GitDb.Tests.Utils;
using GitDb.Watcher;
using Xunit;

namespace GitDb.Tests.Watcher
{
    public class AddingABranch : WithWatcher
    {
        protected override Task Because() =>
            GitDb.CreateBranch(new Reference {Name = "test", Pointer = "master"});

        [Fact]
        public void RaisesABranchAddedEvent() =>
            Subject.ShouldRaise("BranchAdded")
                   .WithArgs<BranchAdded>(args => args.BaseBranch == "master" &&
                                                  args.Branch.Name == "test" && 
                                                  args.Branch.Commit == Repo.Branches["master"].Tip.Sha);
    }

    public class RemovingABranch : WithWatcher
    {
        protected override Task Because()
        {
            Repo.Branches.Remove("master");
            return base.Because();
        }

        [Fact]
        public void RaisesABranchAddedEvent() =>
            Subject.ShouldRaise("BranchRemoved")
                   .WithArgs<BranchRemoved>(args => args.Branch.Name == "master");
    }
}
