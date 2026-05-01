# AGENTS.md — Unity Project Conventions

## Project Overview

- **Unity 2022.3.57f1c2**, C#, Windows
- **JKFrame** custom framework (UI, Pool, Event, Audio, Save, Localization, Scene, StateMachine)
- **VContainer** (DI), **Sirenix Odin Inspector**, **TextMeshPro**, **Unity Addressables**
- Architecture: Feature-based DDD (Domain / Application / Infrastructure / DI per feature)
- No existing test infrastructure

---

## Development Flow

1. **先写计划，再写代码** — Before implementing or modifying any feature, output in this order:
   - **Plan**: 1–2 implementation approaches
   - **Change List**: which files need to be modified / created
2. **任务完成后不要检查文件是否存在语法错误** — Unity Editor auto-detects and flags syntax errors on save. Manually checking is a waste of time.
3. **不要手动修改预制体 YAML** — 涉及 Unity 编辑器操作（挂脚本、拖引用、标记 Addressable 等），告知用户，由用户在 Unity Editor 中完成。
4. Follow all naming conventions, code style, and project patterns below.

---

## Build / Lint / Test Commands

- No test framework or CI configured yet.
- To add tests: use Unity Test Framework (NUnit) under `Assets/Tests/`.
- Use `Demo.sln` in IDE for compilation checks.
- Unity auto-compiles `Assembly-CSharp.dll` on script save.

---

## Code Style

### Naming Conventions

| Category | Convention | Example |
|---|---|---|
| Private fields | `_camelCase` prefix | `_image`, `_registry`, `_instance` |
| Serialized private fields | `_camelCase` prefix | `[SerializeField] private GameObject _roomPrefab;` |
| Public fields | `PascalCase` | `GridWidth`, `DataBinaryPath` |
| Public properties | `PascalCase` | `Ins`, `Registry`, `GridX { get; set; }` |
| Methods | `PascalCase` | `Initialize()`, `Generate()`, `OnClick()` |
| Local variables / params | `camelCase` | `direction`, `fromPos`, `input` |
| Interfaces | `I` prefix | `IDungeonService`, `IPanelHandler` |
| Enums / members | `PascalCase` | `RoomType.Normal` |
| Namespaces | `PascalCase` dot-separated | `Features.Dungeon.Domain` |
| Static singletons | property `Ins` | (not `Instance`) |

### Braces & Indentation

- **Allman style** (braces on new line) for all blocks: classes, methods, properties, control flow, namespaces
- Indentation: **Tabs only** (no spaces)
- Single-statement `if` / `for` may omit braces, but prefer braces for clarity

### Using Statements

Order: `System.*` → `UnityEngine.*` / `TMPro` → VContainer → project namespaces → `JKFrame`

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using VContainer.Unity;
using Features.Dungeon.Domain;
using JKFrame;
```

- Keep usings **outside** namespace declarations
- Do NOT use file-scoped namespaces

### Types & Nullability

- Nullable reference types: **DO NOT** use `#nullable enable` (project doesn't enable it)
- Use `[Serializable]` for inspector-configurable plain C# classes
- Use `[SerializeField] private` for inspector-exposed fields (never make public just for inspector)

### Attributes & DI

| Purpose | Attribute |
|---|---|
| VContainer injection | `[Inject] public void Construct(IFoo service)` |
| JKFrame UI registration | `[UIWindowDataAttribute(typeof(MyWin), true, "UI/Path", layer)]` |
| Odin Inspector | `[LabelText]`, `[FolderPath]`, `[Required]`, `[BoxGroup]` |
| Binary data table | `[BinaryTable]` on static container class |
| Editor test method | `[ContextMenu("ActionName")]` |
| Mandatory components | `[RequireComponent(typeof(RectTransform))]` |

### Error Handling & Logging

- Use `Debug.Log`, `Debug.LogWarning`, `Debug.LogError` for all logging
- Defensive null checks with early return or null-conditional operator:
  ```csharp
  if (_image == null) _image = gameObject.AddComponent<Image>();
  window?.AppendCommand(message);
  ```
- `try-catch` only for reflection / IO operations; log with `Debug.LogError`
- Do NOT use Result / Option monads or custom error types
- Explicit null comparisons; avoid `is` pattern matching for null checks

### Comments & Documentation

- XML `/// <summary>` docs on all **public** classes, methods, properties
- **Chinese (中文)** for implementation notes, inline comments, and TODOs
- No trailing comments on code lines

---

## Project Structure

```
Assets/Scripts/
├── Core/Infrastructure/       Singletons, utils, JKFrame framework
├── Features/{FeatureName}/
│   ├── Domain/                Pure logic, models, interfaces (no Unity deps)
│   ├── Application/           Use cases, service interfaces
│   ├── Infrastructure/        MonoBehaviour implementations
│   └── DI/                    VContainer LifetimeScope
├── Main/                      Cross-cutting systems (GM console, etc.)
├── Presentation/UI/           UI components, debug tools
└── Services/                  Binary data, Excel tooling, factories
```

---

## Key Anti-Patterns to Avoid

- Public `camelCase` fields (inconsistent; use `PascalCase`)
- File-scoped namespaces (project uses brace-delimited `namespace { }`)
- Making fields public just for inspector access (use `[SerializeField] private`)
- Using obsolete `BinaryFormatter` for new code
- `#nullable enable` (not used in project)

---

## Dependencies

| Package | Usage |
|---|---|
| JKFrame | UI, object pool, event bus, audio, save/load, localization, scene mgmt, state machine |
| VContainer | Constructor / method injection via `[Inject]` |
| Odin Inspector | Custom inspector UI attributes |
| TextMeshPro | All UI text via `TMP_Text` / `TMP_InputField` |
| Unity Addressables | Asset management |
