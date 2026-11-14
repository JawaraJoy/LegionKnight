#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rush.Tests
{
    public sealed class PlayGamesAccountTests
    {
        private sealed class Account : IPlayGamesAccount
        {
            public string PlayerId { get; set; } = "guest-A";
            public bool HasSession { get; set; } = true;
            public string GoogleOwner;
            public string Linked;
            public bool FailRestore;
            public readonly List<string> Calls = new();
            public Task RestoreAsync()
            {
                Calls.Add("restore");
                if (FailRestore) throw new InvalidOperationException();
                return Task.CompletedTask;
            }
            public Task<bool> HasLinkAsync() => Task.FromResult(Linked != null);
            public Task LinkAsync(string code)
            {
                Calls.Add("link");
                if (GoogleOwner != null) throw new InvalidOperationException();
                GoogleOwner = PlayerId;
                Linked = PlayerId;
                return Task.CompletedTask;
            }
            public Task SignInAsync(string code, bool create)
            {
                Calls.Add(create ? "sign-in/create" : "sign-in");
                if (GoogleOwner == null && !create) throw new InvalidOperationException();
                PlayerId = GoogleOwner ?? "new-player";
                return Task.CompletedTask;
            }
            public Task VerifyOwnerAsync(string code, string owner)
            {
                Calls.Add("verify");
                if (GoogleOwner != owner) throw new InvalidOperationException();
                return Task.CompletedTask;
            }
        }

        private readonly PlayGamesAccountFlow m_Flow = new();
        private string m_Owner;
        private int m_Codes;
        private Task<string> Run(Account a, bool local = true) =>
            m_Flow.AuthenticateAsync(a, m_Owner, local, () => Task.FromResult((++m_Codes).ToString()), id => m_Owner = id);
        [SetUp] public void Reset() { m_Owner = null; m_Codes = 0; }

        [Test] public void GuestLinksInPlace()
        {
            var a = new Account();
            Assert.That(Run(a).GetAwaiter().GetResult(), Is.EqualTo("guest-A"));
            Assert.That(a.Calls, Is.EqualTo(new[]{"restore","link"}));
            Assert.That(a.GoogleOwner, Is.EqualTo(m_Owner));
        }
        [Test] public void ConflictingGoogleAccountNeverSwitchesGuest()
        {
            var a = new Account { GoogleOwner = "another-player" };
            Assert.Throws<InvalidOperationException>(() => Run(a).GetAwaiter().GetResult());
            Assert.That(a.PlayerId, Is.EqualTo("guest-A"));
            Assert.That(a.Calls, Does.Not.Contain("sign-in"));
            Assert.That(m_Owner, Is.EqualTo(a.PlayerId));
        }
        [Test] public void RetryAfterLinkAlreadyCommittedDoesNotLinkTwice()
        {
            var a = new Account();
            Run(a).GetAwaiter().GetResult();
            Run(a).GetAwaiter().GetResult();
            Assert.That(a.Calls, Is.EqualTo(new[]{"restore","link","restore","verify"}));
        }
        [Test] public void ChangedGoogleAccountCannotOpenAnAlreadyLinkedSave()
        {
            var a = new Account { Linked = "guest-A", GoogleOwner = "another-player" };
            Assert.Throws<InvalidOperationException>(() => Run(a).GetAwaiter().GetResult());
            Assert.That(a.PlayerId, Is.EqualTo("guest-A"));
            Assert.That(a.Calls, Is.EqualTo(new[]{"restore","verify"}));
        }
        [Test] public void ExpiredGuestSessionRetainsOwnerOnFailure()
        {
            var a = new Account { FailRestore = true };
            Assert.Throws<InvalidOperationException>(() => Run(a).GetAwaiter().GetResult());
            Assert.That(m_Owner, Is.EqualTo("guest-A"));
            Assert.That(a.Calls, Is.EqualTo(new[]{"restore"}));
        }
        [Test] public void LostSessionCanRecoverOnlyTheSameLinkedAccount()
        {
            m_Owner = "guest-A";
            var a = new Account { HasSession = false, PlayerId = null, GoogleOwner = m_Owner };
            Assert.That(Run(a).GetAwaiter().GetResult(), Is.EqualTo(m_Owner));
            Assert.That(a.Calls, Is.EqualTo(new[]{"verify","sign-in"}));
            Assert.That(m_Codes, Is.EqualTo(2));
        }
        [Test] public void LocalProgressAloneCannotBeAssignedToAnotherAccount()
        {
            var a = new Account { HasSession = false, PlayerId = null };
            Assert.Throws<InvalidOperationException>(() => Run(a).GetAwaiter().GetResult());
            Assert.That(a.Calls, Is.Empty);
        }
        [TestCase(null, "new-player")]
        [TestCase("returning-player", "returning-player")]
        public void CleanInstallSignsInWithGoogle(string google, string expected)
        {
            var a = new Account { HasSession = false, PlayerId = null, GoogleOwner = google };
            Assert.That(Run(a, false).GetAwaiter().GetResult(), Is.EqualTo(expected));
            Assert.That(a.Calls, Is.EqualTo(new[]{"sign-in/create"}));
        }
    }
}
#endif
