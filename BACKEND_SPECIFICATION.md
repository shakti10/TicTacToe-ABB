# Backend Specification

This specification is derived from `requiremnt.md`. It defines the smallest .NET Web API backend needed for the Tic Tac Toe application.

## Scope

The backend is the source of truth for:

- Current game state
- Move validation
- Turn order
- Win and draw detection
- Move history
- Undo behavior
- Session scoreboard
- Computer moves

Storage is in memory. No database, authentication, users, accounts, persistence, or real-time communication is required.

## Core Types

### GameMode

- `TwoPlayer`
- `Computer`

### Player

- `X`
- `O`

### GameStatus

- `InProgress`
- `Won`
- `Draw`

### GameState

```text
GameId: unique identifier
Board: 3 x 3 collection of X, O, or empty cells
CurrentPlayer: X or O
GameMode: TwoPlayer or Computer
Status: InProgress, Won, or Draw
Winner: X, O, or empty
WinningCells: zero or more board positions
MoveHistory: ordered list of moves
Scoreboard: X wins, O wins, draws
```

### Move

```text
MoveNumber: 1-based sequence number
Player: X or O
Row: 1 through 3
Column: 1 through 3
```

### Scoreboard

```text
XWins: non-negative count
OWins: non-negative count
Draws: non-negative count
```

## API Contract

The exact endpoint names may vary according to the requirement, but the implementation will use the following routes.

### Create Game

```text
POST /api/games
```

Creates a new game session.

Request:

```json
{
  "mode": "TwoPlayer"
}
```

Response: `200 OK` or `201 Created` with the complete `GameState`.

Rules:

- The board is empty.
- `CurrentPlayer` is `X`.
- `Status` is `InProgress`.
- Move history is empty.
- The existing session scoreboard is unchanged.

### Get Game

```text
GET /api/games/{gameId}
```

Returns the complete current `GameState`.

Unknown game IDs return `404 Not Found`.

### Submit Move

```text
POST /api/games/{gameId}/moves
```

Request:

```json
{
  "player": "X",
  "row": 1,
  "column": 1
}
```

Response: `200 OK` with the updated `GameState`.

The backend rejects the request when:

- The game ID is unknown.
- The player is not the current player.
- The row or column is outside 1 through 3.
- The selected cell is occupied.
- The game is already won or drawn.
- The request attempts to submit an O move directly in Computer Mode.

A rejected move must not change the board, move history, current player, status, or scoreboard.

After a valid move:

1. Place the player mark.
2. Append the move to history.
3. Detect a win.
4. Detect a draw if the board is full and there is no win.
5. Update the scoreboard once if the game completed.
6. Otherwise switch the current player.

In Computer Mode, after a valid human X move that leaves the game in progress, the backend immediately selects and applies the computer O move using the required priority. The response contains the resulting state after the computer move.

### Undo Move

```text
POST /api/games/{gameId}/undo
```

Response: `200 OK` with the updated `GameState`.

Rules:

- Undo is rejected when there are no moves.
- Undo is rejected after a win or draw because this implementation uses the permitted Option A.
- In Two Player Mode, remove only the most recent move.
- In Computer Mode, remove the latest computer move and the preceding human move as one pair. If only one move exists, remove that move.
- Restore the board, current player, status, winner, winning cells, and move history.
- The scoreboard is unchanged because completed games cannot be undone.

### Reset Game

```text
POST /api/games/{gameId}/reset
```

Response: `200 OK` with the reset `GameState`.

Rules:

- Clear the board.
- Clear move history.
- Set current player to X.
- Set status to InProgress.
- Clear winner and winning cells.
- Preserve the session scoreboard.

### Get Scoreboard

```text
GET /api/scoreboard
```

Returns the current session scoreboard.

### Reset Scoreboard

```text
POST /api/scoreboard/reset
```

Resets X wins, O wins, and draws to zero. The current game state is not changed.

## Rule Processing

### Win Detection

After every valid move, evaluate the eight possible lines:

- Three rows
- Three columns
- Two diagonals

The first completed line for the player is returned as `WinningCells`. A won game cannot accept further moves.

### Draw Detection

If all nine cells are occupied and no winning line exists, set status to `Draw`, clear `Winner`, and increment the draw count once.

### Scoreboard Update

The scoreboard is updated only during the transition from `InProgress` to `Won` or `Draw`. Repeated requests against a completed game are rejected, so the same game cannot increment the scoreboard more than once.

### Computer Move Priority

For O's turn, inspect available cells in this order:

1. Select a move that gives O a win.
2. Select a move that prevents X from winning on its next move.
3. Select the center cell.
4. Select the first available corner using a documented stable order.
5. Select the first remaining available cell using a documented stable order.

The computer must never select an occupied cell or move after the game completes.

## Error Behavior

Use a consistent client error response with an HTTP `400 Bad Request` status for invalid move and undo requests. Use `404 Not Found` for unknown game IDs. The response should contain a short human-readable message. Exact error JSON shape is an implementation detail and must be documented in the README once implemented.

## State Ownership

The frontend sends commands and renders the returned state. It may disable controls for usability, but the backend independently validates every command and remains authoritative.

## Explicit Assumptions

- A game can be created in either supported mode through the create-game request.
- The scoreboard is shared by games within the running backend process.
- Restarting the backend clears all games and scoreboard values because storage is in memory.
- A stable row/column position is used for move history and computer fallback ordering.
- The backend applies the automatic computer move before returning the move response.
- Undo is disabled after completion, as allowed by the requirement's Option A.
- The exact HTTP success status for create/reset may be chosen consistently during implementation.

## Out of Scope

- Persistent storage
- Authentication and authorization
- Multiple backend instances
- Online multiplayer
- Difficulty settings
- Random computer behavior
- User registration or profiles
- Chat, notifications, or deployment configuration
