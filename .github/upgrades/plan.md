# .NET 8 to .NET 10 Upgrade Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

## Executive Summary

### Scenario Description
.NET 8 to .NET 10 upgrade for CoreWF solution - migrating Windows Workflow Foundation components and related test infrastructure from .NET 8 to .NET 10.0 LTS.

### Scope
- **Total Projects**: 17 projects across the CoreWF solution
- **Current State**: All projects targeting .NET 8.0 (with .NET 8.0-windows variants)
- **Target State**: All projects targeting .NET 10.0 (with .NET 10.0-windows variants)
- **Total Issues**: 29,443 compatibility issues (26,990 mandatory, 2,450 potential, 3 optional)
- **Affected Files**: 658 source files requiring attention

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in single operation.

**Rationale**: 
- 17 projects (medium solution size, within All-At-Once threshold)
- All currently on .NET 8.0 (homogeneous starting point)
- Clear 4-level dependency structure with foundation library (System.Xaml) at base
- Assessment shows compatible package versions available for .NET 10.0

### Complexity Assessment
**Classification**: Medium Complexity

**Discovered Metrics**:
- Projects: 17 (medium scale)
- Dependency Depth: 4 levels (manageable hierarchy)
- High-Risk Projects: 2 (UiPath.Workflow.Runtime: 19,812 issues, TestObjects: 3,066 issues)
- Major Technologies: Windows Workflow Foundation (26,973 issues), CodeDom & Dynamic Code Generation (2,164 issues)

**Critical Issues**:
- Massive Windows Workflow Foundation API changes (26,973 compatibility issues)
- CodeDom & Dynamic Code Generation compatibility (2,164 issues) 
- Binary and source incompatibilities across core workflow runtime components

**Iteration Strategy**: Comprehensive planning with focus on foundation-first migration order and technology migration strategies for WF and CodeDom components.

## Migration Strategy

### Approach Selection and Justification

**Selected Strategy: All-At-Once Migration**

**Justification based on All-At-Once criteria:**
- **Solution Size**: 17 projects (within All-At-Once threshold of <30 projects)
- **Current State**: All projects homogeneously on .NET 8.0/.NET 8.0-windows
- **Dependency Structure**: Clean 4-level hierarchy without circular dependencies
- **Package Compatibility**: Assessment shows all required packages have .NET 10.0 compatible versions

**All-At-Once Strategy Benefits for CoreWF:**
- **Fastest completion**: Single coordinated operation eliminates multi-targeting complexity
- **Unified testing surface**: All projects benefit from .NET 10.0 improvements simultaneously  
- **Clean dependency resolution**: No intermediate states with mixed framework versions
- **Simplified coordination**: All developers adapt to .NET 10.0 at once

### Dependency-Based Ordering Rationale

**All-At-Once Ordering Principles Applied:**
While all projects upgrade simultaneously, the validation and testing sequence respects dependency order:

1. **Foundation Validation First**: System.Xaml (affects all 16 downstream projects)
2. **Core Runtime Validation**: UiPath.Workflow.Runtime (highest issue concentration - 67% of mandatory issues)
3. **Extension Layer Validation**: UiPath.Workflow, test support libraries
4. **Application Layer Validation**: Test projects, benchmarks, console apps

**Atomic Operation Structure:**
- All TargetFramework properties updated simultaneously across all 17 projects
- All package references updated simultaneously using package update matrix
- Single `dotnet restore` operation for entire solution
- Single compilation pass to identify all breaking changes at once
- Coordinated fix application across all affected components

### Execution Decision: Single Coordinated Batch

**No Intermediate States:**
- All projects remain on .NET 8.0 until atomic upgrade completes
- No multi-targeting during migration process
- No partial solution states where some projects are .NET 10.0 and others .NET 8.0

**Parallel Execution Where Possible:**
- Project file updates can be performed in parallel
- Package updates applied simultaneously across solution
- Compilation error fixes grouped by technology (Windows Workflow Foundation, CodeDom) rather than by project

**Risk Management for All-At-Once:**
- Comprehensive breaking changes catalog prepared in advance
- All package versions validated for .NET 10.0 compatibility before execution
- Complete test suite execution after atomic upgrade to validate entire solution

## Detailed Dependency Analysis

### Dependency Graph Summary
The solution follows a clean 4-level dependency hierarchy with `System.Xaml` as the foundation library:

```
Level 0: System.Xaml (Foundation)
    ?? Level 1: Microsoft.CodeAnalysis.VisualBasic.Scripting, UiPath.Workflow.Runtime
        ?? Level 2: CustomTestObjects, ImperativeTestCases, System.Activities.EtwTracking, 
                    System.Xaml.TestCases, TestObjects, UiPath.Workflow, WorkflowApplicationTestExtensions
            ?? Level 3: CoreWf.Benchmarks, Perf.AssemblyReference.Benchmarks, TestCases.Activities,
                        TestCases.Runtime, TestCases.Workflows, TestCases.Xaml
                ?? Level 4: TestConsole
```

### Project Groupings by Migration Phase

#### Foundation Phase (Level 0)
- **System.Xaml.csproj** - Core XAML processing library (96 issues, 1 mandatory)

#### Core Runtime Phase (Level 1) 
- **UiPath.Workflow.Runtime.csproj** - Main workflow runtime (19,812 issues, 19,789 mandatory)
- **Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj** - VB scripting support (1 issue, 1 mandatory)

#### Extension & Test Support Phase (Level 2)
- **UiPath.Workflow.csproj** - High-level workflow API (2,685 issues, 564 mandatory)
- **System.Activities.EtwTracking.csproj** - ETW tracking (424 issues, 423 mandatory)
- **TestObjects.csproj** - Test infrastructure (3,066 issues, 2,974 mandatory) 
- **WorkflowApplicationTestExtensions.csproj** - Test extensions (135 issues, 133 mandatory)
- **CustomTestObjects.csproj** - Custom test objects (2 issues, 1 mandatory)
- **ImperativeTestCases.csproj** - Imperative test cases (167 issues, 166 mandatory)
- **System.Xaml.TestCases.csproj** - XAML test cases (19 issues, 1 mandatory)

#### Application & Advanced Testing Phase (Level 3)
- **TestCases.Activities.csproj** - Activity test cases (1,106 issues, 1,103 mandatory)
- **TestCases.Runtime.csproj** - Runtime test cases (476 issues, 462 mandatory)
- **TestCases.Workflows.csproj** - Workflow test cases (1,308 issues, 1,254 mandatory)
- **TestCases.Xaml.csproj** - XAML test cases (10 issues, 3 mandatory)
- **CoreWf.Benchmarks.csproj** - Performance benchmarks (108 issues, 108 mandatory)
- **Perf.AssemblyReference.Benchmarks.csproj** - Assembly reference benchmarks (26 issues, 6 mandatory)

#### Console Applications Phase (Level 4)
- **TestConsole.csproj** - Test console application (2 issues, 1 mandatory)

### Critical Path Identification
**Primary Critical Path**: System.Xaml ? UiPath.Workflow.Runtime ? UiPath.Workflow ? TestCases.* ? TestConsole

**Risk Concentration**: 
- UiPath.Workflow.Runtime contains 67% of all mandatory issues (19,789 of 26,990)
- Foundation library System.Xaml has minimal issues but affects all 16 downstream projects
- TestObjects affects 3 downstream test projects and contains significant mandatory issues (2,974)

### All-At-Once Execution Strategy
Given the All-At-Once strategy, all projects will be updated simultaneously within phases:
1. **Phase 0**: Prerequisites (SDK, global.json validation)
2. **Phase 1**: Atomic upgrade of all 17 projects (TargetFramework + packages)
3. **Phase 2**: Fix compilation errors from Windows Workflow Foundation and CodeDom changes
4. **Phase 3**: Test validation across all test projects

## Project-by-Project Plans

### System.Xaml (Level 0 - Foundation)
**Current State**: .NET 8.0 ClassLibrary, 96 issues (1 mandatory), Foundation library
**Target State**: .NET 10.0 ClassLibrary, Core XAML processing for entire solution
**Migration Steps**: [Details to be filled]

### Microsoft.CodeAnalysis.VisualBasic.Scripting (Level 1 - Core Runtime)
**Current State**: .NET 8.0 ClassLibrary, 1 issue (1 mandatory), VB scripting support
**Target State**: .NET 10.0 ClassLibrary, Enhanced VB scripting capabilities
**Migration Steps**: [Details to be filled]

### UiPath.Workflow.Runtime (Level 1 - Core Runtime) 
**Current State**: .NET 8.0/.NET 8.0-windows ClassLibrary, 19,812 issues (19,789 mandatory), Main workflow runtime
**Target State**: .NET 10.0/.NET 10.0-windows ClassLibrary, Core workflow execution engine
**Migration Steps**: [Details to be filled]

### CustomTestObjects (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 2 issues (1 mandatory), Custom test objects
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced test object support
**Migration Steps**: [Details to be filled]

### ImperativeTestCases (Level 2 - Extension & Test Support) 
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 167 issues (166 mandatory), Imperative test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Updated imperative testing patterns
**Migration Steps**: [Details to be filled]

### System.Activities.EtwTracking (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0-windows Wpf, 424 issues (423 mandatory), ETW tracking support
**Target State**: .NET 10.0-windows Wpf, Enhanced ETW tracking capabilities
**Migration Steps**: [Details to be filled]

### System.Xaml.TestCases (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 19 issues (1 mandatory), XAML test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced XAML testing
**Migration Steps**: [Details to be filled]

### TestObjects (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 3,066 issues (2,974 mandatory), Test infrastructure
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Modernized test infrastructure
**Migration Steps**: [Details to be filled]

### UiPath.Workflow (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0/.NET 8.0-windows ClassLibrary, 2,685 issues (564 mandatory), High-level workflow API
**Target State**: .NET 10.0/.NET 10.0-windows ClassLibrary, Enhanced workflow API surface
**Migration Steps**: [Details to be filled]

### WorkflowApplicationTestExtensions (Level 2 - Extension & Test Support)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 135 issues (133 mandatory), Test extensions
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced test extensions
**Migration Steps**: [Details to be filled]

### CoreWf.Benchmarks (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0 DotNetCoreApp, 108 issues (108 mandatory), Performance benchmarks
**Target State**: .NET 10.0 DotNetCoreApp, .NET 10.0 performance benchmarking
**Migration Steps**: [Details to be filled]

### Perf.AssemblyReference.Benchmarks (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 26 issues (6 mandatory), Assembly reference benchmarks
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced assembly reference benchmarking
**Migration Steps**: [Details to be filled]

### TestCases.Activities (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 1,106 issues (1,103 mandatory), Activity test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Comprehensive activity testing
**Migration Steps**: [Details to be filled]

### TestCases.Runtime (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 476 issues (462 mandatory), Runtime test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced runtime testing
**Migration Steps**: [Details to be filled]

### TestCases.Workflows (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 1,308 issues (1,254 mandatory), Workflow test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Comprehensive workflow testing
**Migration Steps**: [Details to be filled]

### TestCases.Xaml (Level 3 - Application & Advanced Testing)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 10 issues (3 mandatory), XAML test cases
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, Enhanced XAML testing
**Migration Steps**: [Details to be filled]

### TestConsole (Level 4 - Console Applications)
**Current State**: .NET 8.0/.NET 8.0-windows DotNetCoreApp, 2 issues (1 mandatory), Test console application
**Target State**: .NET 10.0/.NET 10.0-windows DotNetCoreApp, .NET 10.0 console testing
**Migration Steps**: [Details to be filled]

## Risk Management

### High-Risk Changes Assessment

| Project | Risk Level | Issue Count | Primary Risks |
|---------|------------|-------------|---------------|
| UiPath.Workflow.Runtime | **Critical** | 19,812 (19,789 mandatory) | Massive binary incompatibilities, core workflow engine |
| TestObjects | **High** | 3,066 (2,974 mandatory) | Test infrastructure foundation, affects 3 downstream projects |
| UiPath.Workflow | **High** | 2,685 (564 mandatory) | High-level API changes, behavioral changes |
| TestCases.Workflows | **Medium** | 1,308 (1,254 mandatory) | Workflow test validation |
| TestCases.Activities | **Medium** | 1,106 (1,103 mandatory) | Activity test validation |
| System.Activities.EtwTracking | **Medium** | 424 (423 mandatory) | ETW tracking compatibility |

### Breaking Changes Concentration

**Binary Incompatibilities (Api.0001)**: 26,973 occurrences
- Affects existing binaries, requires recompilation
- Concentrated heavily in UiPath.Workflow.Runtime (core workflow engine)
- Impacts all projects due to dependency on workflow runtime components

**Source Incompatibilities (Api.0002)**: 2,313 occurrences  
- Requires code changes to compile successfully
- Obsolete APIs removal, method signature changes
- Spread across workflow and XAML processing components

**Behavioral Changes (Api.0003)**: 116 occurrences
- Runtime behavior changes without recompilation requirement
- Potential for subtle bugs, requires thorough testing
- Concentrated in core workflow and XAML processing

### All-At-Once Strategy Risk Factors

**Advantages for Risk Management:**
- Single comprehensive testing phase validates all interactions simultaneously
- No risk of version mismatches during intermediate states  
- All breaking changes identified and fixed in coordinated fashion

**Risk Mitigation Requirements:**
- Comprehensive breaking changes catalog preparation before execution
- Complete test suite validation after atomic upgrade
- Rollback strategy for entire solution if critical issues discovered

### Contingency Plans

**High-Risk Project Failures:**
- **UiPath.Workflow.Runtime failure**: Implement staged compilation with project-specific breaking changes catalog
- **TestObjects failure**: May require manual test infrastructure modernization before dependent test projects
- **Cross-cutting API failures**: Prepare alternative APIs and compatibility shims where possible

**Package Update Failures:**
- All identified package updates have .NET 10.0 compatible versions available
- Obsolete packages (Azure.Identity, Microsoft.Azure.ServiceBus, Microsoft.Azure.Storage.Blob) have migration paths documented

**Performance Regression:**
- Benchmark projects (CoreWf.Benchmarks, Perf.AssemblyReference.Benchmarks) will validate .NET 10.0 performance improvements
- .NET 10.0 LTS provides performance improvements that should benefit workflow execution

## Testing & Validation Strategy

[To be filled]

## Complexity & Effort Assessment

### Per-Project Complexity Assessment

| Project | Complexity | Dependencies | Risk Level | Issue Density |
|---------|------------|--------------|-------------|---------------|
| **System.Xaml** | Low | 0 dependencies | Low | 96 issues, foundation impact |
| **Microsoft.CodeAnalysis.VisualBasic.Scripting** | Low | 1 dependency | Low | 1 issue, minimal changes |
| **UiPath.Workflow.Runtime** | **Critical** | 1 dependency | Critical | 19,812 issues, core engine |
| **CustomTestObjects** | Low | 2 dependencies | Low | 2 issues, simple test objects |
| **ImperativeTestCases** | Low | 2 dependencies | Medium | 167 issues, test patterns |
| **System.Activities.EtwTracking** | Medium | 1 dependency | Medium | 424 issues, ETW integration |
| **System.Xaml.TestCases** | Low | 2 dependencies | Low | 19 issues, XAML testing |
| **TestObjects** | **High** | 2 dependencies | High | 3,066 issues, affects 3 projects |
| **UiPath.Workflow** | **High** | 3 dependencies | High | 2,685 issues, API surface |
| **WorkflowApplicationTestExtensions** | Low | 2 dependencies | Medium | 135 issues, test extensions |
| **CoreWf.Benchmarks** | Medium | 3 dependencies | Medium | 108 issues, performance validation |
| **Perf.AssemblyReference.Benchmarks** | Low | 3 dependencies | Low | 26 issues, simple benchmarks |
| **TestCases.Activities** | Medium | 4 dependencies | Medium | 1,106 issues, activity validation |
| **TestCases.Runtime** | Medium | 4 dependencies | Medium | 476 issues, runtime validation |
| **TestCases.Workflows** | Medium | 5 dependencies | Medium | 1,308 issues, workflow validation |
| **TestCases.Xaml** | Low | 4 dependencies | Low | 10 issues, XAML validation |
| **TestConsole** | Low | 3 dependencies | Low | 2 issues, console application |

### All-At-Once Complexity Considerations

**Foundation Layer (Low Overall Complexity):**
- System.Xaml: Minimal issues, enables all downstream projects
- Microsoft.CodeAnalysis.VisualBasic.Scripting: Single issue, straightforward update

**Core Runtime Layer (Critical Complexity):**
- UiPath.Workflow.Runtime: Contains 67% of all mandatory issues, requires extensive breaking change management
- Complexity concentrated in workflow engine APIs, binary compatibility issues

**Extension Layer (Mixed Complexity):**
- UiPath.Workflow: High API surface changes, behavioral modifications
- TestObjects: High issue count, affects downstream test projects  
- Other projects: Low to medium complexity, manageable issue counts

**Application Layer (Medium Complexity):**
- Test projects: Moderate complexity, validation requirements
- Benchmark projects: Performance validation, compatibility verification

### Resource Requirements

**Technical Skills Required:**
- **Workflow Foundation Expertise**: Critical for UiPath.Workflow.Runtime and UiPath.Workflow
- **.NET Breaking Changes Knowledge**: Understanding of .NET 8?10 API evolution
- **Test Infrastructure Modernization**: Updates to test patterns and frameworks
- **Performance Analysis**: Benchmark validation and .NET 10.0 performance characteristics

**All-At-Once Execution Capacity:**
- **Parallel Project Updates**: Project files and package references can be updated simultaneously
- **Coordinated Compilation**: Single solution build identifies all breaking changes at once
- **Unified Testing**: All test projects validate simultaneously after fixes applied
- **Single Integration Point**: No complex multi-version dependency management required

## Source Control Strategy

[To be filled]

## Success Criteria

[To be filled]