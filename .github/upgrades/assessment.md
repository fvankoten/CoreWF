# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [CoreWf.EtwTracking\System.Activities.EtwTracking.csproj](#corewfetwtrackingsystemactivitiesetwtrackingcsproj)
  - [Perf\CoreWf.Benchmarks\CoreWf.Benchmarks.csproj](#perfcorewfbenchmarkscorewfbenchmarkscsproj)
  - [Perf\Perf.AssemblyReference.Benchmarks\Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj)
  - [System.Xaml\System.Xaml.csproj](#systemxamlsystemxamlcsproj)
  - [Test\CustomTestObjects\CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)
  - [Test\ImperativeTestCases\ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)
  - [Test\System.Xaml.TestCases\System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)
  - [Test\TestCases.Activities\TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)
  - [Test\TestCases.Runtime\TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)
  - [Test\TestCases.Workflows\TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)
  - [Test\TestCases.Xaml\TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)
  - [Test\TestConsole\TestConsole.csproj](#testtestconsoletestconsolecsproj)
  - [Test\TestObjects\TestObjects.csproj](#testtestobjectstestobjectscsproj)
  - [Test\WorkflowApplicationTestExtensions\WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj)
  - [UiPath.Workflow.Runtime\UiPath.Workflow.Runtime.csproj](#uipathworkflowruntimeuipathworkflowruntimecsproj)
  - [UiPath.Workflow\UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj)
  - [VisualBasic\Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj](#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 17 | All require upgrade |
| Total NuGet Packages | 69 | 13 need upgrade |
| Total Code Files | 1151 |  |
| Total Code Files with Incidents | 658 |  |
| Total Lines of Code | 236896 |  |
| Total Number of Issues | 29443 |  |
| Estimated LOC to modify | 29402+ | at least 12,4% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [CoreWf.EtwTracking\System.Activities.EtwTracking.csproj](#corewfetwtrackingsystemactivitiesetwtrackingcsproj) | net8.0-windows | 🟡 Medium | 1 | 422 | 422+ | Wpf, Sdk Style = True |
| [Perf\CoreWf.Benchmarks\CoreWf.Benchmarks.csproj](#perfcorewfbenchmarkscorewfbenchmarkscsproj) | net8.0 | 🟡 Medium | 0 | 107 | 107+ | DotNetCoreApp, Sdk Style = True |
| [Perf\Perf.AssemblyReference.Benchmarks\Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | net8.0;net8.0-windows | 🟢 Low | 12 | 13 | 13+ | DotNetCoreApp, Sdk Style = True |
| [System.Xaml\System.Xaml.csproj](#systemxamlsystemxamlcsproj) | net8.0 | 🟢 Low | 0 | 95 | 95+ | ClassLibrary, Sdk Style = True |
| [Test\CustomTestObjects\CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj) | net8.0;net8.0-windows | 🟢 Low | 1 | 0 |  | DotNetCoreApp, Sdk Style = True |
| [Test\ImperativeTestCases\ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 165 | 165+ | DotNetCoreApp, Sdk Style = True |
| [Test\System.Xaml.TestCases\System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj) | net8.0;net8.0-windows | 🟢 Low | 1 | 17 | 17+ | DotNetCoreApp, Sdk Style = True |
| [Test\TestCases.Activities\TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 1104 | 1104+ | DotNetCoreApp, Sdk Style = True |
| [Test\TestCases.Runtime\TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 474 | 474+ | DotNetCoreApp, Sdk Style = True |
| [Test\TestCases.Workflows\TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 1306 | 1306+ | DotNetCoreApp, Sdk Style = True |
| [Test\TestCases.Xaml\TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj) | net8.0;net8.0-windows | 🟢 Low | 1 | 8 | 8+ | DotNetCoreApp, Sdk Style = True |
| [Test\TestConsole\TestConsole.csproj](#testtestconsoletestconsolecsproj) | net8.0;net8.0-windows | 🟢 Low | 1 | 0 |  | DotNetCoreApp, Sdk Style = True |
| [Test\TestObjects\TestObjects.csproj](#testtestobjectstestobjectscsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 3064 | 3064+ | DotNetCoreApp, Sdk Style = True |
| [Test\WorkflowApplicationTestExtensions\WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 133 | 133+ | DotNetCoreApp, Sdk Style = True |
| [UiPath.Workflow.Runtime\UiPath.Workflow.Runtime.csproj](#uipathworkflowruntimeuipathworkflowruntimecsproj) | net8.0;net8.0-windows | 🟡 Medium | 0 | 19811 | 19811+ | ClassLibrary, Sdk Style = True |
| [UiPath.Workflow\UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj) | net8.0;net8.0-windows | 🟡 Medium | 1 | 2683 | 2683+ | ClassLibrary, Sdk Style = True |
| [VisualBasic\Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj](#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj) | net8.0 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 56 | 81,2% |
| ⚠️ Incompatible | 3 | 4,3% |
| 🔄 Upgrade Recommended | 10 | 14,5% |
| ***Total NuGet Packages*** | ***69*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 26973 | High - Require code changes |
| 🟡 Source Incompatible | 2313 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 116 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 187909 |  |
| ***Total APIs Analyzed*** | ***217311*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| AgileObjects.ReadableExpressions | 4.1.3 |  | [TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj) | ✅Compatible |
| AutoMapper | 9.0.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Azure.Identity | 1.14.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ⚠️Das NuGet-Paket ist veraltet |
| Azure.Messaging.ServiceBus | 7.20.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Azure.Storage.Blobs | 12.24.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| BenchmarkDotNet | 0.13.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| BenchmarkDotNet | 0.13.12 |  | [CoreWf.Benchmarks.csproj](#perfcorewfbenchmarkscorewfbenchmarkscsproj) | ✅Compatible |
| Bogus | 35.6.3 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| ClosedXML | 0.105.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| CsvHelper | 33.1.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Dapper | 2.1.66 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| EPPlus | 8.0.6 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| FluentValidation | 6.4.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Google.Apis | 1.70.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Google.Apis.Auth | 1.70.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Google.Cloud.Storage.V1 | 4.13.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Hangfire.Core | 1.8.20 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| HtmlAgilityPack | 1.12.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Humanizer | 2.14.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| MailKit | 4.12.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| MediatR | 12.5.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore.Authentication | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore.Authorization | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.AspNetCore.Http | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore.Mvc | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore.Routing | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.AspNetCore.StaticFiles | 2.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.Azure.Cosmos | 3.52.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.Azure.Functions.Extensions | 1.1.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.Azure.ServiceBus | 5.2.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ⚠️Das NuGet-Paket ist veraltet |
| Microsoft.Azure.Storage.Blob | 11.2.3 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ⚠️Das NuGet-Paket ist veraltet |
| Microsoft.CodeAnalysis.CSharp.Features | 4.11.0 |  | [TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj) | ✅Compatible |
| Microsoft.CodeAnalysis.CSharp.Scripting | 4.11.0 |  | [UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj) | ✅Compatible |
| Microsoft.CodeAnalysis.Scripting.Common | 4.11.0 |  | [Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj](#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj) | ✅Compatible |
| Microsoft.CodeAnalysis.VisualBasic | 4.11.0 |  | [Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj](#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj)<br/>[UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj) | ✅Compatible |
| Microsoft.CodeAnalysis.VisualBasic.Features | 4.11.0 |  | [TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj) | ✅Compatible |
| Microsoft.CodeAnalysis.Workspaces.Common | 4.11.0 |  | [TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.Extensions.Configuration.Json | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.Extensions.DependencyInjection | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.Extensions.Logging | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.Extensions.Options | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Microsoft.IdentityModel.Protocols | 8.12.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.IdentityModel.Tokens | 8.12.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Microsoft.NET.Test.Sdk | 17.0.0 |  | [CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)<br/>[ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)<br/>[System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)<br/>[TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)<br/>[TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)<br/>[TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)<br/>[TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)<br/>[TestConsole.csproj](#testtestconsoletestconsolecsproj)<br/>[TestObjects.csproj](#testtestobjectstestobjectscsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | ✅Compatible |
| Microsoft.SourceLink.GitHub | 8.0.0 |  | [UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj)<br/>[UiPath.Workflow.Runtime.csproj](#uipathworkflowruntimeuipathworkflowruntimecsproj) | ✅Compatible |
| MongoDB.Driver | 3.4.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Newtonsoft.Json | 13.0.3 | 13.0.4 | [CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)<br/>[ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)<br/>[Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj)<br/>[System.Activities.EtwTracking.csproj](#corewfetwtrackingsystemactivitiesetwtrackingcsproj)<br/>[System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)<br/>[TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)<br/>[TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)<br/>[TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)<br/>[TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)<br/>[TestConsole.csproj](#testtestconsoletestconsolecsproj)<br/>[TestObjects.csproj](#testtestobjectstestobjectscsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| Nito.AsyncEx.Tasks | 5.1.2 |  | [UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | ✅Compatible |
| NLog | 6.0.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| NodaTime | 3.2.2 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Npgsql | 9.0.3 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| NUnit | 4.1.0 |  | [System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj) | ✅Compatible |
| NUnit3TestAdapter | 4.5.0 |  | [System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj) | ✅Compatible |
| Polly | 8.6.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| ReflectionMagic | 5.0.1 |  | [UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj) | ✅Compatible |
| RestSharp | 112.1.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Serilog | 4.3.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Serilog.Sinks.Console | 6.0.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| Shouldly | 4.0.3 |  | [CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)<br/>[ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)<br/>[System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)<br/>[TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)<br/>[TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)<br/>[TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)<br/>[TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)<br/>[TestConsole.csproj](#testtestconsoletestconsolecsproj)<br/>[TestObjects.csproj](#testtestobjectstestobjectscsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | ✅Compatible |
| SixLabors.ImageSharp | 3.1.11 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| System.CodeDom | 8.0.0 | 10.0.2 | [UiPath.Workflow.csproj](#uipathworkflowuipathworkflowcsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| System.Data.SqlClient | 4.9.0 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| System.Drawing.Common | 6.0.0 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| System.IO.Pipelines | 9.0.6 | 10.0.2 | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | Ein NuGet-Paketupgrade wird empfohlen |
| System.Reactive | 6.0.1 |  | [Perf.AssemblyReference.Benchmarks.csproj](#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj) | ✅Compatible |
| xunit | 2.4.1 |  | [CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)<br/>[ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)<br/>[System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)<br/>[TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)<br/>[TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)<br/>[TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)<br/>[TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)<br/>[TestConsole.csproj](#testtestconsoletestconsolecsproj)<br/>[TestObjects.csproj](#testtestobjectstestobjectscsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | ✅Compatible |
| xunit.runner.visualstudio | 2.4.3 |  | [CustomTestObjects.csproj](#testcustomtestobjectscustomtestobjectscsproj)<br/>[ImperativeTestCases.csproj](#testimperativetestcasesimperativetestcasescsproj)<br/>[System.Xaml.TestCases.csproj](#testsystemxamltestcasessystemxamltestcasescsproj)<br/>[TestCases.Activities.csproj](#testtestcasesactivitiestestcasesactivitiescsproj)<br/>[TestCases.Runtime.csproj](#testtestcasesruntimetestcasesruntimecsproj)<br/>[TestCases.Workflows.csproj](#testtestcasesworkflowstestcasesworkflowscsproj)<br/>[TestCases.Xaml.csproj](#testtestcasesxamltestcasesxamlcsproj)<br/>[TestConsole.csproj](#testtestconsoletestconsolecsproj)<br/>[TestObjects.csproj](#testtestobjectstestobjectscsproj)<br/>[WorkflowApplicationTestExtensions.csproj](#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 26973 | 91,7% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |
| CodeDom & Dynamic Code Generation | 2164 | 7,4% | Runtime code generation, compilation, and scripting APIs including CodeDom and JScript that have limited support in .NET Core/.NET. These were used for dynamic code generation but are largely obsolete. Consider Roslyn APIs for code generation or alternative scripting solutions. |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Activities.Activity | 2644 | 9,0% | Binary Incompatible |
| T:System.Activities.ArgumentDirection | 991 | 3,4% | Binary Incompatible |
| T:System.Activities.ActivityInstance | 975 | 3,3% | Binary Incompatible |
| T:System.Activities.ActivityDelegate | 646 | 2,2% | Binary Incompatible |
| T:System.Activities.ActivityInstanceState | 529 | 1,8% | Binary Incompatible |
| F:System.Activities.ArgumentDirection.In | 371 | 1,3% | Binary Incompatible |
| T:System.Activities.Tracking.ActivityInfo | 370 | 1,3% | Binary Incompatible |
| M:System.Activities.RequiredArgumentAttribute.#ctor | 333 | 1,1% | Binary Incompatible |
| T:System.Activities.RequiredArgumentAttribute | 332 | 1,1% | Binary Incompatible |
| P:System.Activities.Activity.DisplayName | 318 | 1,1% | Binary Incompatible |
| T:System.Activities.RuntimeDelegateArgument | 304 | 1,0% | Binary Incompatible |
| T:System.Activities.WorkflowApplication | 299 | 1,0% | Binary Incompatible |
| M:System.Activities.RuntimeDelegateArgument.#ctor(System.String,System.Type,System.Activities.ArgumentDirection,System.Activities.DelegateArgument) | 292 | 1,0% | Binary Incompatible |
| T:System.Activities.Bookmark | 290 | 1,0% | Binary Incompatible |
| T:System.Activities.RuntimeArgument | 283 | 1,0% | Binary Incompatible |
| T:System.Activities.NativeActivityContext | 275 | 0,9% | Binary Incompatible |
| T:System.Activities.WorkflowIdentity | 266 | 0,9% | Binary Incompatible |
| T:System.Activities.LocationReferenceEnvironment | 198 | 0,7% | Binary Incompatible |
| T:System.Activities.Argument | 187 | 0,6% | Binary Incompatible |
| P:System.Activities.ActivityInstance.Activity | 184 | 0,6% | Binary Incompatible |
| T:System.Activities.BookmarkResumptionResult | 182 | 0,6% | Binary Incompatible |
| T:System.Activities.Location | 172 | 0,6% | Binary Incompatible |
| T:System.CodeDom.MemberAttributes | 161 | 0,5% | Source Incompatible |
| T:System.Activities.ActivityWithResult | 154 | 0,5% | Binary Incompatible |
| P:System.Activities.Variable.Name | 152 | 0,5% | Binary Incompatible |
| M:System.Activities.NativeActivity.#ctor | 148 | 0,5% | Binary Incompatible |
| T:System.Activities.Statements.Sequence | 141 | 0,5% | Binary Incompatible |
| T:System.Activities.CodeActivityMetadata | 138 | 0,5% | Binary Incompatible |
| T:System.Activities.Variable | 132 | 0,4% | Binary Incompatible |
| T:System.Activities.BookmarkScope | 130 | 0,4% | Binary Incompatible |
| P:System.Activities.Statements.Sequence.Activities | 127 | 0,4% | Binary Incompatible |
| T:System.Activities.LocationReference | 123 | 0,4% | Binary Incompatible |
| M:System.Activities.Statements.Sequence.#ctor | 111 | 0,4% | Binary Incompatible |
| T:System.Activities.Statements.FlowNode | 108 | 0,4% | Binary Incompatible |
| T:System.Activities.Statements.WriteLine | 107 | 0,4% | Binary Incompatible |
| T:System.Activities.Validation.ValidationError | 102 | 0,3% | Binary Incompatible |
| T:System.Activities.InArgument | 97 | 0,3% | Binary Incompatible |
| P:System.Activities.ActivityDelegate.Handler | 97 | 0,3% | Binary Incompatible |
| T:System.Activities.Validation.ActivityValidationServices | 95 | 0,3% | Binary Incompatible |
| M:System.Activities.Statements.WriteLine.#ctor | 93 | 0,3% | Binary Incompatible |
| P:System.Activities.Statements.WriteLine.Text | 91 | 0,3% | Binary Incompatible |
| T:System.Uri | 90 | 0,3% | Behavioral Change |
| T:System.Activities.CodeActivityContext | 89 | 0,3% | Binary Incompatible |
| T:System.Activities.NativeActivityMetadata | 88 | 0,3% | Binary Incompatible |
| T:System.Activities.PersistableIdleAction | 88 | 0,3% | Binary Incompatible |
| T:System.Activities.ActivityContext | 88 | 0,3% | Binary Incompatible |
| T:System.CodeDom.CodeTypeReference | 88 | 0,3% | Source Incompatible |
| P:System.Activities.Validation.ValidationResults.Errors | 88 | 0,3% | Binary Incompatible |
| T:System.Activities.Hosting.WorkflowInstanceExtensionManager | 87 | 0,3% | Binary Incompatible |
| T:System.Activities.Expressions.ExpressionServices | 86 | 0,3% | Binary Incompatible |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P2["<b>📦&nbsp;System.Activities.EtwTracking.csproj</b><br/><small>net8.0-windows</small>"]
    P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P4["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P7["<b>📦&nbsp;ImperativeTestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
    P9["<b>📦&nbsp;System.Xaml.TestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P10["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P12["<b>📦&nbsp;Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj</b><br/><small>net8.0</small>"]
    P13["<b>📦&nbsp;TestConsole.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P14["<b>📦&nbsp;CoreWf.Benchmarks.csproj</b><br/><small>net8.0</small>"]
    P15["<b>📦&nbsp;CustomTestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P17["<b>📦&nbsp;Perf.AssemblyReference.Benchmarks.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
    P1 --> P8
    P2 --> P1
    P3 --> P8
    P3 --> P1
    P4 --> P8
    P4 --> P16
    P4 --> P3
    P4 --> P1
    P5 --> P8
    P5 --> P16
    P5 --> P3
    P5 --> P1
    P6 --> P8
    P6 --> P16
    P6 --> P11
    P6 --> P15
    P6 --> P1
    P7 --> P8
    P7 --> P1
    P9 --> P8
    P9 --> P1
    P10 --> P8
    P10 --> P3
    P10 --> P11
    P10 --> P1
    P11 --> P1
    P11 --> P8
    P11 --> P12
    P12 --> P8
    P13 --> P8
    P13 --> P5
    P13 --> P1
    P14 --> P8
    P14 --> P11
    P14 --> P1
    P15 --> P8
    P15 --> P1
    P16 --> P8
    P16 --> P1
    P17 --> P8
    P17 --> P11
    P17 --> P1
    click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    click P2 "#corewfetwtrackingsystemactivitiesetwtrackingcsproj"
    click P3 "#testtestobjectstestobjectscsproj"
    click P4 "#testtestcasesactivitiestestcasesactivitiescsproj"
    click P5 "#testtestcasesruntimetestcasesruntimecsproj"
    click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
    click P7 "#testimperativetestcasesimperativetestcasescsproj"
    click P8 "#systemxamlsystemxamlcsproj"
    click P9 "#testsystemxamltestcasessystemxamltestcasescsproj"
    click P10 "#testtestcasesxamltestcasesxamlcsproj"
    click P11 "#uipathworkflowuipathworkflowcsproj"
    click P12 "#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj"
    click P13 "#testtestconsoletestconsolecsproj"
    click P14 "#perfcorewfbenchmarkscorewfbenchmarkscsproj"
    click P15 "#testcustomtestobjectscustomtestobjectscsproj"
    click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
    click P17 "#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"

```

## Project Details

<a id="corewfetwtrackingsystemactivitiesetwtrackingcsproj"></a>
### CoreWf.EtwTracking\System.Activities.EtwTracking.csproj

#### Project Info

- **Current Target Framework:** net8.0-windows
- **Proposed Target Framework:** net10.0-windows
- **SDK-style**: True
- **Project Kind:** Wpf
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 5
- **Number of Files with Incidents**: 2
- **Lines of Code**: 913
- **Estimated LOC to modify**: 422+ (at least 46,2% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["System.Activities.EtwTracking.csproj"]
        MAIN["<b>📦&nbsp;System.Activities.EtwTracking.csproj</b><br/><small>net8.0-windows</small>"]
        click MAIN "#corewfetwtrackingsystemactivitiesetwtrackingcsproj"
    end
    subgraph downstream["Dependencies (1"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 422 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1491 |  |
| ***Total APIs Analyzed*** | ***1913*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 422 | 100,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="perfcorewfbenchmarkscorewfbenchmarkscsproj"></a>
### Perf\CoreWf.Benchmarks\CoreWf.Benchmarks.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 3
- **Dependants**: 0
- **Number of Files**: 5
- **Number of Files with Incidents**: 4
- **Lines of Code**: 319
- **Estimated LOC to modify**: 107+ (at least 33,5% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["CoreWf.Benchmarks.csproj"]
        MAIN["<b>📦&nbsp;CoreWf.Benchmarks.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#perfcorewfbenchmarkscorewfbenchmarkscsproj"
    end
    subgraph downstream["Dependencies (3"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P11
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 107 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 278 |  |
| ***Total APIs Analyzed*** | ***385*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 107 | 100,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"></a>
### Perf\Perf.AssemblyReference.Benchmarks\Perf.AssemblyReference.Benchmarks.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 3
- **Dependants**: 0
- **Number of Files**: 2
- **Number of Files with Incidents**: 2
- **Lines of Code**: 195
- **Estimated LOC to modify**: 13+ (at least 6,7% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["Perf.AssemblyReference.Benchmarks.csproj"]
        MAIN["<b>📦&nbsp;Perf.AssemblyReference.Benchmarks.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"
    end
    subgraph downstream["Dependencies (3"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P11
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 5 | High - Require code changes |
| 🟡 Source Incompatible | 6 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 2 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 186 |  |
| ***Total APIs Analyzed*** | ***199*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 5 | 38,5% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="systemxamlsystemxamlcsproj"></a>
### System.Xaml\System.Xaml.csproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 15
- **Number of Files**: 189
- **Number of Files with Incidents**: 21
- **Lines of Code**: 49105
- **Estimated LOC to modify**: 95+ (at least 0,2% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (15)"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P4["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P7["<b>📦&nbsp;ImperativeTestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P9["<b>📦&nbsp;System.Xaml.TestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P10["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P12["<b>📦&nbsp;Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj</b><br/><small>net8.0</small>"]
        P13["<b>📦&nbsp;TestConsole.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P14["<b>📦&nbsp;CoreWf.Benchmarks.csproj</b><br/><small>net8.0</small>"]
        P15["<b>📦&nbsp;CustomTestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P17["<b>📦&nbsp;Perf.AssemblyReference.Benchmarks.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
        click P3 "#testtestobjectstestobjectscsproj"
        click P4 "#testtestcasesactivitiestestcasesactivitiescsproj"
        click P5 "#testtestcasesruntimetestcasesruntimecsproj"
        click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
        click P7 "#testimperativetestcasesimperativetestcasescsproj"
        click P9 "#testsystemxamltestcasessystemxamltestcasescsproj"
        click P10 "#testtestcasesxamltestcasesxamlcsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P12 "#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj"
        click P13 "#testtestconsoletestconsolecsproj"
        click P14 "#perfcorewfbenchmarkscorewfbenchmarkscsproj"
        click P15 "#testcustomtestobjectscustomtestobjectscsproj"
        click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
        click P17 "#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"
    end
    subgraph current["System.Xaml.csproj"]
        MAIN["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        click MAIN "#systemxamlsystemxamlcsproj"
    end
    P1 --> MAIN
    P3 --> MAIN
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    P7 --> MAIN
    P9 --> MAIN
    P10 --> MAIN
    P11 --> MAIN
    P12 --> MAIN
    P13 --> MAIN
    P14 --> MAIN
    P15 --> MAIN
    P16 --> MAIN
    P17 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 6 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 89 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 22843 |  |
| ***Total APIs Analyzed*** | ***22938*** |  |

<a id="testcustomtestobjectscustomtestobjectscsproj"></a>
### Test\CustomTestObjects\CustomTestObjects.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 1
- **Number of Files**: 4
- **Number of Files with Incidents**: 1
- **Lines of Code**: 6
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
    end
    subgraph current["CustomTestObjects.csproj"]
        MAIN["<b>📦&nbsp;CustomTestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testcustomtestobjectscustomtestobjectscsproj"
    end
    subgraph downstream["Dependencies (2"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    P6 --> MAIN
    MAIN --> P8
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 5 |  |
| ***Total APIs Analyzed*** | ***5*** |  |

<a id="testimperativetestcasesimperativetestcasescsproj"></a>
### Test\ImperativeTestCases\ImperativeTestCases.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 0
- **Number of Files**: 6
- **Number of Files with Incidents**: 4
- **Lines of Code**: 248
- **Estimated LOC to modify**: 165+ (at least 66,5% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["ImperativeTestCases.csproj"]
        MAIN["<b>📦&nbsp;ImperativeTestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testimperativetestcasesimperativetestcasescsproj"
    end
    subgraph downstream["Dependencies (2"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 165 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 212 |  |
| ***Total APIs Analyzed*** | ***377*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 165 | 100,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testsystemxamltestcasessystemxamltestcasescsproj"></a>
### Test\System.Xaml.TestCases\System.Xaml.TestCases.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 0
- **Number of Files**: 53
- **Number of Files with Incidents**: 6
- **Lines of Code**: 20641
- **Estimated LOC to modify**: 17+ (at least 0,1% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["System.Xaml.TestCases.csproj"]
        MAIN["<b>📦&nbsp;System.Xaml.TestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testsystemxamltestcasessystemxamltestcasescsproj"
    end
    subgraph downstream["Dependencies (2"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 3 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 14 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 25691 |  |
| ***Total APIs Analyzed*** | ***25708*** |  |

<a id="testtestcasesactivitiestestcasesactivitiescsproj"></a>
### Test\TestCases.Activities\TestCases.Activities.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 4
- **Dependants**: 0
- **Number of Files**: 80
- **Number of Files with Incidents**: 56
- **Lines of Code**: 28168
- **Estimated LOC to modify**: 1104+ (at least 3,9% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["TestCases.Activities.csproj"]
        MAIN["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestcasesactivitiestestcasesactivitiescsproj"
    end
    subgraph downstream["Dependencies (4"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
        click P3 "#testtestobjectstestobjectscsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P16
    MAIN --> P3
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1102 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 2 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 15363 |  |
| ***Total APIs Analyzed*** | ***16467*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 1102 | 99,8% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testtestcasesruntimetestcasesruntimecsproj"></a>
### Test\TestCases.Runtime\TestCases.Runtime.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 4
- **Dependants**: 1
- **Number of Files**: 17
- **Number of Files with Incidents**: 13
- **Lines of Code**: 2504
- **Estimated LOC to modify**: 474+ (at least 18,9% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P13["<b>📦&nbsp;TestConsole.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P13 "#testtestconsoletestconsolecsproj"
    end
    subgraph current["TestCases.Runtime.csproj"]
        MAIN["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestcasesruntimetestcasesruntimecsproj"
    end
    subgraph downstream["Dependencies (4"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
        click P3 "#testtestobjectstestobjectscsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    P13 --> MAIN
    MAIN --> P8
    MAIN --> P16
    MAIN --> P3
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 461 | High - Require code changes |
| 🟡 Source Incompatible | 13 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2120 |  |
| ***Total APIs Analyzed*** | ***2594*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 461 | 97,3% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testtestcasesworkflowstestcasesworkflowscsproj"></a>
### Test\TestCases.Workflows\TestCases.Workflows.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 5
- **Dependants**: 0
- **Number of Files**: 41
- **Number of Files with Incidents**: 21
- **Lines of Code**: 4672
- **Estimated LOC to modify**: 1306+ (at least 28,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["TestCases.Workflows.csproj"]
        MAIN["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestcasesworkflowstestcasesworkflowscsproj"
    end
    subgraph downstream["Dependencies (5"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P15["<b>📦&nbsp;CustomTestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P15 "#testcustomtestobjectscustomtestobjectscsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P16
    MAIN --> P11
    MAIN --> P15
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1253 | High - Require code changes |
| 🟡 Source Incompatible | 53 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 4516 |  |
| ***Total APIs Analyzed*** | ***5822*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| CodeDom & Dynamic Code Generation | 51 | 3,9% | Runtime code generation, compilation, and scripting APIs including CodeDom and JScript that have limited support in .NET Core/.NET. These were used for dynamic code generation but are largely obsolete. Consider Roslyn APIs for code generation or alternative scripting solutions. |
| Windows Workflow Foundation | 1253 | 95,9% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testtestcasesxamltestcasesxamlcsproj"></a>
### Test\TestCases.Xaml\TestCases.Xaml.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 4
- **Dependants**: 0
- **Number of Files**: 10
- **Number of Files with Incidents**: 3
- **Lines of Code**: 1521
- **Estimated LOC to modify**: 8+ (at least 0,5% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["TestCases.Xaml.csproj"]
        MAIN["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestcasesxamltestcasesxamlcsproj"
    end
    subgraph downstream["Dependencies (4"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P3 "#testtestobjectstestobjectscsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P3
    MAIN --> P11
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 2 | High - Require code changes |
| 🟡 Source Incompatible | 1 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 5 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1126 |  |
| ***Total APIs Analyzed*** | ***1134*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 2 | 25,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testtestconsoletestconsolecsproj"></a>
### Test\TestConsole\TestConsole.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 3
- **Dependants**: 0
- **Number of Files**: 4
- **Number of Files with Incidents**: 1
- **Lines of Code**: 36
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["TestConsole.csproj"]
        MAIN["<b>📦&nbsp;TestConsole.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestconsoletestconsolecsproj"
    end
    subgraph downstream["Dependencies (3"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P5 "#testtestcasesruntimetestcasesruntimecsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    MAIN --> P8
    MAIN --> P5
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 11 |  |
| ***Total APIs Analyzed*** | ***11*** |  |

<a id="testtestobjectstestobjectscsproj"></a>
### Test\TestObjects\TestObjects.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 3
- **Number of Files**: 229
- **Number of Files with Incidents**: 143
- **Lines of Code**: 32656
- **Estimated LOC to modify**: 3064+ (at least 9,4% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P4["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P10["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P4 "#testtestcasesactivitiestestcasesactivitiescsproj"
        click P5 "#testtestcasesruntimetestcasesruntimecsproj"
        click P10 "#testtestcasesxamltestcasesxamlcsproj"
    end
    subgraph current["TestObjects.csproj"]
        MAIN["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testtestobjectstestobjectscsproj"
    end
    subgraph downstream["Dependencies (2"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    P4 --> MAIN
    P5 --> MAIN
    P10 --> MAIN
    MAIN --> P8
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 2973 | High - Require code changes |
| 🟡 Source Incompatible | 91 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 18631 |  |
| ***Total APIs Analyzed*** | ***21695*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 2973 | 97,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"></a>
### Test\WorkflowApplicationTestExtensions\WorkflowApplicationTestExtensions.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 2
- **Dependants**: 3
- **Number of Files**: 12
- **Number of Files with Incidents**: 7
- **Lines of Code**: 624
- **Estimated LOC to modify**: 133+ (at least 21,3% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P4["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P4 "#testtestcasesactivitiestestcasesactivitiescsproj"
        click P5 "#testtestcasesruntimetestcasesruntimecsproj"
        click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
    end
    subgraph current["WorkflowApplicationTestExtensions.csproj"]
        MAIN["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
    end
    subgraph downstream["Dependencies (2"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    MAIN --> P8
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 132 | High - Require code changes |
| 🟡 Source Incompatible | 1 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 563 |  |
| ***Total APIs Analyzed*** | ***696*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 132 | 99,2% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="uipathworkflowruntimeuipathworkflowruntimecsproj"></a>
### UiPath.Workflow.Runtime\UiPath.Workflow.Runtime.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 14
- **Number of Files**: 487
- **Number of Files with Incidents**: 334
- **Lines of Code**: 78382
- **Estimated LOC to modify**: 19811+ (at least 25,3% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (14)"]
        P2["<b>📦&nbsp;System.Activities.EtwTracking.csproj</b><br/><small>net8.0-windows</small>"]
        P3["<b>📦&nbsp;TestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P4["<b>📦&nbsp;TestCases.Activities.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P5["<b>📦&nbsp;TestCases.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P7["<b>📦&nbsp;ImperativeTestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P9["<b>📦&nbsp;System.Xaml.TestCases.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P10["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P13["<b>📦&nbsp;TestConsole.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P14["<b>📦&nbsp;CoreWf.Benchmarks.csproj</b><br/><small>net8.0</small>"]
        P15["<b>📦&nbsp;CustomTestObjects.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P16["<b>📦&nbsp;WorkflowApplicationTestExtensions.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P17["<b>📦&nbsp;Perf.AssemblyReference.Benchmarks.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P2 "#corewfetwtrackingsystemactivitiesetwtrackingcsproj"
        click P3 "#testtestobjectstestobjectscsproj"
        click P4 "#testtestcasesactivitiestestcasesactivitiescsproj"
        click P5 "#testtestcasesruntimetestcasesruntimecsproj"
        click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
        click P7 "#testimperativetestcasesimperativetestcasescsproj"
        click P9 "#testsystemxamltestcasessystemxamltestcasescsproj"
        click P10 "#testtestcasesxamltestcasesxamlcsproj"
        click P11 "#uipathworkflowuipathworkflowcsproj"
        click P13 "#testtestconsoletestconsolecsproj"
        click P14 "#perfcorewfbenchmarkscorewfbenchmarkscsproj"
        click P15 "#testcustomtestobjectscustomtestobjectscsproj"
        click P16 "#testworkflowapplicationtestextensionsworkflowapplicationtestextensionscsproj"
        click P17 "#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"
    end
    subgraph current["UiPath.Workflow.Runtime.csproj"]
        MAIN["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#uipathworkflowruntimeuipathworkflowruntimecsproj"
    end
    subgraph downstream["Dependencies (1"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
    end
    P2 --> MAIN
    P3 --> MAIN
    P4 --> MAIN
    P5 --> MAIN
    P6 --> MAIN
    P7 --> MAIN
    P9 --> MAIN
    P10 --> MAIN
    P11 --> MAIN
    P13 --> MAIN
    P14 --> MAIN
    P15 --> MAIN
    P16 --> MAIN
    P17 --> MAIN
    MAIN --> P8

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 19788 | High - Require code changes |
| 🟡 Source Incompatible | 23 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 83989 |  |
| ***Total APIs Analyzed*** | ***103800*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Windows Workflow Foundation | 19788 | 99,9% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="uipathworkflowuipathworkflowcsproj"></a>
### UiPath.Workflow\UiPath.Workflow.csproj

#### Project Info

- **Current Target Framework:** net8.0;net8.0-windows
- **Proposed Target Framework:** net8.0;net8.0-windows;net10.0;net10.0--windows
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 3
- **Dependants**: 4
- **Number of Files**: 90
- **Number of Files with Incidents**: 39
- **Lines of Code**: 16227
- **Estimated LOC to modify**: 2683+ (at least 16,5% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (4)"]
        P6["<b>📦&nbsp;TestCases.Workflows.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P10["<b>📦&nbsp;TestCases.Xaml.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P14["<b>📦&nbsp;CoreWf.Benchmarks.csproj</b><br/><small>net8.0</small>"]
        P17["<b>📦&nbsp;Perf.AssemblyReference.Benchmarks.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P6 "#testtestcasesworkflowstestcasesworkflowscsproj"
        click P10 "#testtestcasesxamltestcasesxamlcsproj"
        click P14 "#perfcorewfbenchmarkscorewfbenchmarkscsproj"
        click P17 "#perfperfassemblyreferencebenchmarksperfassemblyreferencebenchmarkscsproj"
    end
    subgraph current["UiPath.Workflow.csproj"]
        MAIN["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click MAIN "#uipathworkflowuipathworkflowcsproj"
    end
    subgraph downstream["Dependencies (3"]
        P1["<b>📦&nbsp;UiPath.Workflow.Runtime.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        P12["<b>📦&nbsp;Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj</b><br/><small>net8.0</small>"]
        click P1 "#uipathworkflowruntimeuipathworkflowruntimecsproj"
        click P8 "#systemxamlsystemxamlcsproj"
        click P12 "#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj"
    end
    P6 --> MAIN
    P10 --> MAIN
    P14 --> MAIN
    P17 --> MAIN
    MAIN --> P1
    MAIN --> P8
    MAIN --> P12

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 563 | High - Require code changes |
| 🟡 Source Incompatible | 2116 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 4 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 10271 |  |
| ***Total APIs Analyzed*** | ***12954*** |  |

#### Project Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| CodeDom & Dynamic Code Generation | 2113 | 78,8% | Runtime code generation, compilation, and scripting APIs including CodeDom and JScript that have limited support in .NET Core/.NET. These were used for dynamic code generation but are largely obsolete. Consider Roslyn APIs for code generation or alternative scripting solutions. |
| Windows Workflow Foundation | 563 | 21,0% | Windows Workflow Foundation APIs for workflow management that are not available in .NET Core/.NET. WF provided declarative workflow capabilities but was complex and underused. Consider alternative workflow engines like UiPath CoreWF (community port), Elsa Workflows, or manual implementation. |

<a id="visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj"></a>
### VisualBasic\Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj

#### Project Info

- **Current Target Framework:** net8.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 1
- **Dependants**: 1
- **Number of Files**: 11
- **Number of Files with Incidents**: 1
- **Lines of Code**: 679
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P11["<b>📦&nbsp;UiPath.Workflow.csproj</b><br/><small>net8.0;net8.0-windows</small>"]
        click P11 "#uipathworkflowuipathworkflowcsproj"
    end
    subgraph current["Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj"]
        MAIN["<b>📦&nbsp;Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj</b><br/><small>net8.0</small>"]
        click MAIN "#visualbasicmicrosoftcodeanalysisvisualbasicscriptingvbproj"
    end
    subgraph downstream["Dependencies (1"]
        P8["<b>📦&nbsp;System.Xaml.csproj</b><br/><small>net8.0</small>"]
        click P8 "#systemxamlsystemxamlcsproj"
    end
    P11 --> MAIN
    MAIN --> P8

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 613 |  |
| ***Total APIs Analyzed*** | ***613*** |  |

