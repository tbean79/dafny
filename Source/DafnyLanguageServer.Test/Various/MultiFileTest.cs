﻿using Microsoft.Dafny.LanguageServer.IntegrationTest.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Dafny.LanguageServer.IntegrationTest.Various {
  [TestClass]
  public class MultiFileTest : DafnyLanguageServerTestBase {
    private static readonly string TestFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Various", "TestFiles", "testFile.dfy");

    private ILanguageClient _client;

    [TestInitialize]
    public async Task SetUp() {
      _client = await InitializeClient();
    }

    // https://github.com/dafny-lang/language-server-csharp/issues/40
    [TestMethod]
    public async Task ImplicitlyIncludingTheSameModuleTwiceDoesNotResultInDuplicateError() {
      var source = @"
include ""multi1.dfy""
include ""multi2.dfy""

method Test() {
  assert true;
}";
      var documentItem = CreateTestDocument(source, TestFilePath);
      await _client.OpenDocumentAndWaitAsync(documentItem, CancellationToken);
      Assert.IsTrue(Documents.TryGetDocument(documentItem, out var document));
      Assert.IsTrue(!document.Errors.HasErrors);
    }

    // https://github.com/dafny-lang/language-server-csharp/issues/40
    [TestMethod]
    public async Task ImplicitlyIncludingTheSameModuleTwiceDoesNotOverrideActualError() {
      var source = @"
include ""multi1.dfy""
include ""multi2.dfy""

method Test() {
  assert false;
}";
      var documentItem = CreateTestDocument(source, TestFilePath);
      await _client.OpenDocumentAndWaitAsync(documentItem, CancellationToken);
      Assert.IsTrue(Documents.TryGetDocument(documentItem, out var document));
      Assert.AreEqual(1, document.Errors.ErrorCount);
    }
  }
}
