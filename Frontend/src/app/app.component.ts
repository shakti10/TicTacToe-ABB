import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

type GameMode = 'TwoPlayer' | 'Computer';
type Player = 'X' | 'O';

interface Cell {
  row: number;
  column: number;
}

interface Move {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

interface GameState {
  gameId: string;
  mode: GameMode;
  currentPlayer: Player;
  status: 'InProgress' | 'Won' | 'Draw';
  winner: Player | null;
  winningCells: Cell[];
  moveHistory: Move[];
  board: { grid: (Player | null)[][] };
  scoreboard: Scoreboard;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api';

  game: GameState | null = null;
  selectedMode: GameMode = 'TwoPlayer';
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.createGame();
  }

  createGame(): void {
    this.runRequest(
      this.http.post<GameState>(`${this.apiUrl}/games`, { mode: this.selectedMode }),
      (game) => this.game = game
    );
  }

  selectMode(mode: GameMode): void {
    if (this.loading || this.selectedMode === mode) {
      return;
    }

    this.selectedMode = mode;
    this.createGame();
  }

  play(row: number, column: number): void {
    if (!this.game || this.loading || this.game.status !== 'InProgress') {
      return;
    }

    this.runRequest(
      this.http.post<GameState>(`${this.apiUrl}/games/${this.game.gameId}/moves`, {
        player: this.game.currentPlayer,
        row,
        column
      }),
      (game) => this.game = game
    );
  }

  undo(): void {
    if (!this.game) {
      return;
    }

    this.runRequest(
      this.http.post<GameState>(`${this.apiUrl}/games/${this.game.gameId}/undo`, {}),
      (game) => this.game = game
    );
  }

  resetGame(): void {
    if (!this.game) {
      return;
    }

    this.runRequest(
      this.http.post<GameState>(`${this.apiUrl}/games/${this.game.gameId}/reset`, {}),
      (game) => this.game = game
    );
  }

  resetScoreboard(): void {
    this.runRequest(
      this.http.post<Scoreboard>(`${this.apiUrl}/scoreboard/reset`, {}),
      (scoreboard) => {
        if (this.game) {
          this.game = { ...this.game, scoreboard };
        }
      }
    );
  }

  cellValue(row: number, column: number): Player | null {
    return this.game?.board.grid[row - 1]?.[column - 1] ?? null;
  }

  isWinningCell(row: number, column: number): boolean {
    return this.game?.winningCells.some((cell) => cell.row === row && cell.column === column) ?? false;
  }

  isDisabled(row: number, column: number): boolean {
    return this.loading || this.game?.status !== 'InProgress' || this.cellValue(row, column) !== null;
  }

  statusLabel(): string {
    if (!this.game) {
      return 'Connecting to game server';
    }
    if (this.game.status === 'Won') {
      return `Player ${this.game.winner} wins`;
    }
    if (this.game.status === 'Draw') {
      return 'A hard-fought draw';
    }
    return `${this.game.currentPlayer}'s turn`;
  }

  modeLabel(): string {
    return this.game?.mode === 'Computer' ? 'Play against computer' : 'Two players';
  }

  private runRequest<T>(request: Observable<T>, onSuccess: (value: T) => void): void {
    this.loading = true;
    this.errorMessage = '';
    request.subscribe({
      next: (value) => onSuccess(value),
      error: (error: HttpErrorResponse) => {
        this.errorMessage = error.error?.message ?? 'The game server could not complete that action.';
        this.loading = false;
      },
      complete: () => this.loading = false
    });
  }
}
