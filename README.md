# GomokuPlus — IFN584 Assignment 1

**Student Name:** Susmitha Radhakrishnan Nair
**Student ID:** _n12342467
**Unit:** IFN584 Object-Oriented Design and Development
**Recording File:** `n12342467.cast`


## Feature Implementation Claim Table


| Requirement | Status | asciinema Timestamp (mm:ss) | Notes |

| **Human vs. human (HvH) mode** | Working| 02:15| |Showed the game board, alternating player turns, legal move  execution, and handling of incorrect inputs|
| **Human vs. computer (HvC) mode** | Working | 05:56 | Showed the human player's turn followed by the computer automatically making a legal move|
| **Save / load state persistence** | Working| 07:56| |Saved an active game, exited the program, reopened the saved game, and continued from the previous state|
| **Heavy stone** | Working | 08:30 | |Showed how a Heavy Stone is placed and how the number of Heavy Stones remaining is updated|
| **Eraser stone** | Working | 08:32 | |Showed an opponent's Ordinary Stone being removed and confirmed that a Heavy Stone cannot be erased|
| **Automated testing script mode** | Working | 10.04 | Ran the required command-line test sequence and displayed the resulting final board|

## Assessment 3 development baseline

The first framework slice keeps the Assignment 1 command-line behavior compatible while introducing reusable seams under `Gomokuplus/Framework`:

- `IGameRules` separates variant validation, move application, and help text from the session loop.
- `GomokuPlusRules` adapts the existing Ordinary, Heavy, and Eraser rules without changing the script syntax.
- `HistoryManager` stores deep-copied snapshots and supports `U` (undo) and `R` (redo). A new move after undo clears the redo branch.
- Save files now include undo/redo history in addition to the current board, turn, and stone inventories.
- `IAiStrategy` separates computer play from the player/session loop. `Easy` selects a reproducible legal move; `Hard` first searches for an immediate win, then blocks an immediate opponent win.
- `GameVariantRegistry` is the central variant discovery point. Standard Gomoku, GomokuPlus, and GomokuFog use the Gomoku session path. Standard Reversi, Anti-Reversi, and Corner Reversi use the dedicated 8x8 Reversi script path until the shared session snapshot grows pass/score-aware state.
- `GameLogic/Reversi` contains an 8x8 rules core with initial position setup, legal captures, pass validation, terminal detection, scoring, Standard Reversi objectives, Anti-Reversi's lowest-disc objective, Corner Reversi corner prioritisation, and a script runner.

Build and test the current slice with:

```text
dotnet build Gomokuplus/GomokuPlus.csproj
dotnet test Gomokuplus.Tests/GomokuPlus.Tests.csproj
dotnet run --project Gomokuplus/GomokuPlus.csproj -- "O1:1,O1:2,O1:3,O1:4,O1:5"
dotnet run --project Gomokuplus/GomokuPlus.csproj -- --list-variants
dotnet run --project Gomokuplus/GomokuPlus.csproj -- --variant "Standard Gomoku" --script "O1:1,O1:2,O1:3"
dotnet run --project Gomokuplus/GomokuPlus.csproj -- --variant "Standard Reversi" --script "4:6,3:6,2:6"
```

The remaining Assessment 3 variants and framework adapters should be added behind these contracts rather than extending `GomokuGame` with more variant-specific conditionals.
