import { HttpClientTestingModule, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent, HttpClientTestingModule],
      providers: [provideHttpClientTesting()]
    }).compileComponents();
  });

  it('creates the game screen', () => {
    const fixture = TestBed.createComponent(AppComponent);

    expect(fixture.componentInstance).toBeTruthy();
  });

  it('starts in two-player mode', () => {
    const fixture = TestBed.createComponent(AppComponent);

    expect(fixture.componentInstance.selectedMode).toBe('TwoPlayer');
  });
});
