# CoreWF .NET 10.0 Upgrade Tasks

## Overview

This document tracks the upgrade of the CoreWF solution from .NET 8.0 to .NET 10.0. All 17 projects will be upgraded simultaneously in a single atomic operation, followed by comprehensive testing and a single final commit.

**Progress**: 1/3 tasks complete (33%) ![0%](https://progress-bar.xyz/33)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-01-29 12:37)*
**References**: Plan §Migration Strategy, Plan §Detailed Dependency Analysis

- [✓] (1) Verify required .NET 10.0 SDK is installed per Plan §Migration Strategy
- [✓] (2) .NET 10.0 SDK is present (**Verify**)
- [✓] (3) If present, update `global.json` to require .NET 10.0 SDK per Plan §Migration Strategy
- [✓] (4) `global.json` (if present) references .NET 10.0 SDK (**Verify**)

---

### [▶] TASK-002: Atomic framework and package upgrade
**References**: Plan §Project-by-Project Plans, Plan §Detailed Dependency Analysis, Plan §Risk Management

- [✓] (1) Update `TargetFramework` in all 17 project files to .NET 10.0 per Plan §Project-by-Project Plans
- [✓] (2) All project files updated to .NET 10.0 (**Verify**)
- [✓] (3) Update all package references to .NET 10.0 compatible versions per Plan §Detailed Dependency Analysis
- [✓] (4) All package references updated (**Verify**)
- [▶] (5) Restore all dependencies for the solution
- [ ] (6) All dependencies restored successfully (**Verify**)
- [ ] (7) Build the entire solution and fix all compilation errors per Plan §Risk Management (focus: Windows Workflow Foundation, CodeDom, and breaking changes)
- [ ] (8) Solution builds with 0 errors (**Verify**)

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy, Plan §Project-by-Project Plans

- [ ] (1) Run all test projects in the solution per Plan §Project-by-Project Plans
- [ ] (2) Fix any test failures (reference Plan §Risk Management for common issues)
- [ ] (3) Re-run all tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)
- [ ] (5) Commit all upgrade changes with message: "TASK-003: Complete .NET 10.0 upgrade for CoreWF solution"

---





