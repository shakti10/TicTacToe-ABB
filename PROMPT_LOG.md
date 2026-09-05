# AI Prompt Log

This file records the prompts, AI-generated guidance, human decisions, and verification performed while building the Tic Tac Toe solution.

The log exists to support the AI-assisted development expectations in `requiremnt.md` and to keep the implementation traceable to the stated requirements.

## Working Rules

- Treat `requiremnt.md` as the primary source of truth.
- Do not add features that are not required unless they are explicitly documented as a decision.
- Prefer the smallest implementation that satisfies the requirements.
- Do not invent behavior when the requirement is unclear; record the assumption first.
- Backend game state, validation, game status, move history, and scoreboard are authoritative.
- Record meaningful prompts and decisions as the solution evolves.
- Review generated code manually and verify behavior with tests or a focused check.

## Confirmed Decisions

- Use an Angular + TypeScript frontend and a .NET Web API backend.
- Use in-memory storage because persistence is not required.
- Keep game rules and state transitions testable in backend code.
- Support Two Player Mode and Play Against Computer mode.
- In Computer Mode, the human player is X and the computer is O.
- Use the required computer move priority: win, block, center, corner, then any available cell.
- Use Undo Option A: disable undo after a game is completed. This keeps the scoreboard final and avoids score reversal logic.
- Keep Reset Game separate from Reset Scoreboard.
- Keep the frontend responsible for presentation and API calls, not authoritative game rules.

## Prompt P001 - Requirement Understanding

Date: 2026-09-05

### Prompt

> Read the Tic Tac Toe requirement document and create a concise understanding of the required solution. Identify the required frontend, backend, API behavior, game rules, computer move priority, undo behavior, scoreboard behavior, testing expectations, assumptions, and prohibited over-engineering. Stay strictly within the requirement and do not invent features.

### AI-Generated Result

- Converted the document into an implementation understanding.
- Identified the minimum REST API surface.
- Identified backend state ownership as a central requirement.
- Identified in-memory storage as sufficient.
- Identified Undo Option A as the simplest permitted scoreboard strategy.

### Human Decisions

- Use in-memory state.
- Disable undo after a completed game.
- Keep the architecture small and testable.
- Treat any additional behavior as an explicit assumption rather than an implied requirement.

### Manual Review

- Compared the decisions against the requirements in `requiremnt.md`.
- Confirmed that the decisions do not add authentication, persistence, advanced AI, or unrelated application features.

## Prompt P002 - Prompt Tracking Setup

Date: 2026-09-05

### Prompt

> Create a prompt-tracking file for the Tic Tac Toe solution. The file must record each meaningful AI prompt, the generated result, human decisions, manual changes, reviewed areas, assumptions, trade-offs, and verification. Keep it aligned with `requiremnt.md` and avoid over-engineering.

### AI-Generated Result

- Created this `PROMPT_LOG.md` file.
- Added working rules, confirmed decisions, the initial requirement-understanding record, and this reusable structure.

### Human Decisions

- Continue adding entries to this file throughout implementation.
- Record exact prompts where practical rather than only summaries.
- Keep generated suggestions separate from decisions accepted into the solution.

### Manual Review

- Confirm that this file covers the AI-assisted development questions from the requirement.
- Confirm that later entries can document code generation, manual changes, testing, assumptions, and trade-offs.

## Entry Template

Copy this section for each meaningful prompt used during implementation.

```markdown
## Prompt P000 - Short Purpose

Date: YYYY-MM-DD

### Prompt

> Exact prompt used

### AI-Generated Result

- Summary of the generated analysis or code.

### Accepted

- What was accepted into the solution.

### Changed Manually

- What was modified, rejected, or implemented differently.

### Reviewed

- Files, behavior, tests, or requirements checked manually.

### Assumptions

- Any behavior not explicitly defined by the requirement.

### Trade-offs

- Important simplifications or design choices.

### Verification

- Test or focused check performed and its result.
```

## Pending Prompt Areas

These are planned prompt topics, not completed work:

- Completed: convert the requirement into a small backend specification.
- Completed: define the domain model and state transitions.
- Completed: implement and test core game rules.
- Completed: implement the REST API.
- Completed: implement the Angular frontend and API integration.
- Completed: add the required tests.
- Completed: review the complete solution against every acceptance criterion.
- Completed: write the README and document assumptions, limitations, and AI workflow.

## Approved Five-Prompt Execution Sequence

The following five prompts are the approved implementation sequence. They are recorded exactly as agreed and will be followed one at a time.

## Prompt P001 - Requirement Analysis

Status: Completed

### Prompt

> Read `requiremnt.md` and convert it into a concise functional specification, API contract, implementation checklist, assumptions list, and test checklist. Do not add features that are not required.

## Prompt P002 - Backend Design

Status: Completed

### Prompt

> Design the smallest .NET Web API structure that satisfies the Tic Tac Toe requirements. Use in-memory state, keep game rules in testable application code, and make the backend the source of truth. Do not introduce authentication, persistence, CQRS, event sourcing, or unnecessary abstractions.

## Prompt P003 - Game Rules

Status: Completed

### Prompt

> Implement and test the core Tic Tac Toe rules: valid moves, turn switching, row/column/diagonal wins, draws, completed-game rejection, two-player undo, computer-mode pair undo, and the specified computer move priority.

## Prompt P004 - Frontend Integration

Status: Completed

### Prompt

> Build the minimal Angular UI required by the specification. It must call the REST API, render the board, current turn, mode, status, winning cells, move history, scoreboard, reset controls, and validation errors. Do not duplicate game rules in the frontend.

## Prompt P005 - Verification

Status: Completed

### Prompt

> Review the implementation against every acceptance criterion in `requiremnt.md`. Report missing behavior, incorrect assumptions, test gaps, and unnecessary complexity. Do not propose unrelated enhancements.

## Prompt P003 - Backend Specification

Date: 2026-09-05

### Prompt

> Convert `requiremnt.md` into the smallest backend specification for the Tic Tac Toe application. Define the core types, REST API contract, validation rules, state transitions, win and draw detection, scoreboard behavior, undo behavior, computer move priority, assumptions, and out-of-scope items. Keep the backend authoritative and do not add features that are not required.

### AI-Generated Result

- Created `BACKEND_SPECIFICATION.md`.
- Defined the required game, move, player, status, mode, and scoreboard data.
- Defined the required game and scoreboard endpoints.
- Defined valid and invalid move behavior.
- Defined two-player undo and computer-mode pair undo.
- Defined Option A: undo is rejected after a completed game.
- Defined the required computer move priority.

### Accepted

- Use the backend as the source of truth.
- Use in-memory storage.
- Return the complete game state after game commands.
- Apply the automatic computer move before returning the response.
- Reject invalid commands without changing game state.

### Changed Manually

- None yet. The specification remains a design artifact and has not generated application code.

### Reviewed

- Checked the specification against the game board, player turn, win, draw, reset, move history, undo, scoreboard, computer mode, backend, frontend, clarification, and testing sections of `requiremnt.md`.
- Confirmed that persistence, authentication, online multiplayer, difficulty settings, and other unrelated features are explicitly out of scope.

### Assumptions

- The scoreboard is shared by games during the lifetime of the running backend process.
- Restarting the backend clears in-memory games and scoreboard values.
- Computer fallback choices use a stable row/column order.
- If computer mode contains only the human's first move, undo removes that single move; normal computer turns remove the human/computer pair.
- The exact HTTP success status for create and reset may be selected consistently during implementation.

### Trade-offs

- The API uses row and column values from 1 through 3 because they match the requirement's move-history examples.
- Undo after completion is disabled instead of implementing scoreboard reversal, using the permitted Option A.
- The automatic computer move is performed synchronously inside the move command so the frontend receives one authoritative state.

### Verification

- Manually reviewed `BACKEND_SPECIFICATION.md` after creation.
- Confirmed that it contains no unrequested product features or backend infrastructure.

## Prompt P004 - Frontend Integration Result

Date: 2026-09-05

### AI-Generated Result

- Created a standalone Angular application under `Frontend`.
- Connected the UI to the existing API at `http://localhost:5000/api`.
- Added game creation, mode selection, board moves, reset game, undo, scoreboard reset, and error handling.
- Rendered the backend-provided board, current player, status, winning cells, move history, and scoreboard.
- Added responsive styling for laptop and smaller screens without adding unrelated features.

### Accepted

- The frontend sends commands to the backend and renders returned game state.
- Computer mode is selected when creating a new backend game.
- Undo is disabled when the game is completed, matching the approved Option A decision.

### Changed Manually

- Replaced the Angular starter screen with the required game interface.
- Used the actual API response shape (`board.grid`, `moveHistory`, and `scoreboard`) verified from the backend DTOs.
- Increased the Angular component-style budget from 4 KB to 6 KB because the responsive game stylesheet exceeded the starter budget.

### Reviewed

- Reviewed `GamesController`, `ScoreboardController`, `Dtos`, `Program`, and `launchSettings` before wiring the UI.
- Confirmed API requests use the backend's routes and camelCase JSON shape.
- Confirmed the frontend build succeeds with `npm run build`.

### Assumptions

- The backend is started at `http://localhost:5000` using its existing launch profile.
- The API allows cross-origin requests as configured by its existing CORS policy.
- Starting the frontend creates a new game session for the selected mode.

### Trade-offs

- No frontend game-rule engine was added because the backend is authoritative.
- No additional state-management library was introduced because one game session fits the component state.
- No API client generation was added because the contract is small and already verified through the controller DTOs.

### Verification

- `npm run build` passes from `Frontend`.
- The first build attempt failed only because it ran from the wrong directory; the corrected build passed.
- `POST /api/games` returned a valid Two Player game state from the running API.
- Computer mode returned both the human X move and the automatic O center move after an opening corner.
- `GET http://localhost:4200/` returned HTTP 200 from the Angular development server.
- The existing backend core test suite passed: 10 tests passed, 0 failed.

## Prompt P005 - Verification Result

Date: 2026-09-05

### AI-Generated Result

- Reviewed the implementation against the functional requirements, backend requirements, frontend requirements, clarification on backend ownership, testing expectations, README expectations, and acceptance criteria.
- Reviewed the supplied UI screenshot and confirmed that the board, current status, winning-cell highlight, scoreboard, mode controls, and move history are rendered. Reset and undo controls are present below the board action area.
- Identified a stale generated Angular test that still expected the placeholder `Frontend` title and `Hello, Frontend` content.
- Identified that the required root README was missing.

### Accepted

- Replaced the stale Angular starter test with focused component smoke tests.
- Added the root `README.md` with setup, API, tests, AI workflow, design decisions, assumptions, limitations, and future improvements.
- Kept the existing UI and backend scope unchanged because the required behavior is already implemented.

### Changed Manually

- Removed the placeholder assertions from `Frontend/src/app/app.component.spec.ts`.
- Added the requirement-focused README at the repository root.
- Updated this prompt log so all five approved prompts have truthful statuses.

### Reviewed

- Confirmed the backend owns state, validation, status, move history, computer moves, and scoreboard.
- Confirmed the frontend renders the backend response instead of calculating authoritative results.
- Confirmed the screenshot shows a completed-game message and highlighted winning cells.
- Confirmed reset game, undo, mode selection, scoreboard reset, and history controls exist in the template.
- Confirmed no persistence, authentication, online multiplayer, or unrelated features were added.

### Assumptions

- The UI may place secondary actions below the board rather than in the first viewport on smaller laptop heights; they remain available in the game view.
- The current Angular smoke tests validate component creation and the default mode; full browser interaction coverage remains a future improvement.

### Trade-offs

- Kept the single-component Angular implementation because the application has one game view and the backend contract is small.
- Kept the fixed local API URL because local execution is the stated target and no environment configuration was required.

### Verification

- `npm run build` passes from `Frontend`.
- Backend core tests pass: 10 passed, 0 failed.
- Angular ChromeHeadless tests pass: 2 passed, 0 failed.
- Live `POST /api/games` and Computer Mode move checks passed against the running API.
- Live `GET http://localhost:4200/` returned HTTP 200.
- ChromeHeadless reported a shutdown warning after the successful run, but all tests completed successfully.
