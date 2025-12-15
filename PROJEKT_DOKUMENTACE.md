# 🎰 Online Casino - Projektová dokumentace

## Základní informace o projektu

**Název projektu:** Online Casino  
**Technologie:** ASP.NET Core MVC 9.0  
**Databáze:** SQL Server s Entity Framework Core (Code-First)  
**Autor:** Mleczney  

## Popis projektu

Online Casino je webová aplikace umožňující uživatelům hrát jednoduché kasinové hry, spravovat svůj účet a balance. Administrátoři mohou spravovat všechny aspekty systému včetně hráčů, her, sázek a transakcí.

---

## 1. Splnění obecných podmínek projektu

### ✅ 1.1 Webová aplikace ASP.NET Core MVC 9.0
- Projekt je vytvořen v ASP.NET Core MVC verze 9.0
- Používá standardní MVC pattern s Controllers, Views a Models

### ✅ 1.2 Vícevrstvá architektura
Projekt implementuje čtyři vrstvy:

#### **Presentation Layer** (`Controllers/`, `Views/`, `Areas/`)
- MVC Controllers pro obsluhu HTTP requestů
- Razor Views pro zobrazení UI
- Admin Area pro správu systému

#### **Application Layer** (`Application/`)
- **Services** (`Application/Services/`) - Obsahuje business logiku
- **Interfaces** (`Application/Interfaces/`) - Definuje kontrakty pro služby
- **DTOs** (`Application/DTOs/`) - Data Transfer Objects pro přenos dat mezi vrstvami
- **Validation** (`Application/Validation/`) - Vlastní validační atributy

#### **Infrastructure Layer** (`Infrastructure/`)
- **Data** (`Infrastructure/Data/`) - DbContext, migrace, seed data
- **Repositories** (implicitně přes EF Core)

#### **Domain Layer** (`Domain/`)
- **Entities** (`Domain/Entities/`) - Doménové entity (Player, Game, Bet, Transaction, GameSession)

### ✅ 1.3 Služby (Services)
Veškerá funkcionalita je implementována pomocí služeb:
- `IPlayerService` / `PlayerService` - Správa hráčů
- `IGameService` / `GameService` - Správa her
- `IBetService` / `BetService` - Správa sázek
- `ITransactionService` / `TransactionService` - Správa transakcí
- `IGameSessionService` / `GameSessionService` - Správa herních relací

**Kontrollery neobsahují business logiku** - vše je delegováno na služby.

### ✅ 1.4 Oddělení vrstev
- **Presentation vrstva** NIKDY přímo nepoužívá Infrastructure vrstvu (pouze v Program.cs pro konfiguraci)
- Controllers používají pouze Application vrstvu (Services)
- Není žádný SQL kód v controllerech či views - vše přes EF Core

---

## 2. Entity a databáze

### ✅ 2.1 Minimálně 5 entit (nepočítají se EF Core a Identity entity)
Projekt obsahuje **5 hlavních entit**:

1. **Player** (`Domain/Entities/Player.cs`)
   - Id, Username, Email, PasswordHash, Balance, CreatedAt
   - Vztahy: Bets, Transactions, GameSessions

2. **Game** (`Domain/Entities/Game.cs`)
   - Id, Name, Description, MinBet, MaxBet, IsActive
   - Vztahy: Bets, GameSessions

3. **Bet** (`Domain/Entities/Bet.cs`)
   - Id, PlayerId, GameId, Amount, WinAmount, IsWin, CreatedAt, GameSessionId
   - Vztahy: Player, Game, GameSession

4. **Transaction** (`Domain/Entities/Transaction.cs`)
   - Id, PlayerId, Type (enum), Amount, BalanceBefore, BalanceAfter, Description, CreatedAt
   - Vztahy: Player

5. **GameSession** (`Domain/Entities/GameSession.cs`)
   - Id, PlayerId, GameId, StartedAt, EndedAt, InitialBalance, FinalBalance, TotalBets, TotalWagered, TotalWon
   - Vztahy: Player, Game, Bets

### ✅ 2.2 Cizí klíče
Všechny entity jsou propojeny pomocí cizích klíčů:
- `Bet.PlayerId` → `Player.Id`
- `Bet.GameId` → `Game.Id`
- `Bet.GameSessionId` → `GameSession.Id`
- `Transaction.PlayerId` → `Player.Id`
- `GameSession.PlayerId` → `Player.Id`
- `GameSession.GameId` → `Game.Id`

### ✅ 2.3 Code-First s migracemi
- Databáze vytvořena pomocí Code-First approach
- Migrace: `Migrations/20251215143311_InitialCreateWithMultiLayer.cs`
- Konfigurace entit v `Infrastructure/Data/CasinoContext.cs`

---

## 3. Admin Area

### ✅ 3.1 Struktura Admin Area
```
Areas/
  Admin/
    Controllers/
      - HomeController.cs
      - PlayersController.cs
      - GamesController.cs
      - BetsController.cs
      - TransactionsController.cs
      - GameSessionsController.cs
    Views/
      - Home/Index.cshtml
      - Players/ (Index, Create, Edit, Delete)
      - Games/ (Index, Create, Edit, Delete)
```

### ✅ 3.2 CRUD operace pro všechny entity
Admin může spravovat:
- **Players** - Create, Read, Update, Delete (včetně balance, ale ne hash hesla)
- **Games** - Create, Read, Update, Delete, Activate/Deactivate
- **Bets** - Read, Delete
- **Transactions** - Read, Delete
- **GameSessions** - Read, Delete

### ✅ 3.3 Editace položek
Implementována editace pro:
- Players (Username, Email, Balance)
- Games (Name, Description, MinBet, MaxBet, IsActive)

Admin nemůže měnit `PasswordHash` - to je správné z bezpečnostního hlediska.

---

## 4. Validace

### ✅ 4.1 Serverová validace
Všechny entity a DTOs mají DataAnnotations validaci:
```csharp
[Required(ErrorMessage = "Username je povinný")]
[StringLength(50, MinimumLength = 3, ErrorMessage = "Username musí mít 3-50 znaků")]
public string Username { get; set; }

[EmailAddress(ErrorMessage = "Neplatný email")]
public string Email { get; set; }

[Range(1, 100000, ErrorMessage = "Částka musí být mezi 1 a 100000")]
public decimal Amount { get; set; }
```

### ✅ 4.2 Klientská validace
Views obsahují:
```html
@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```
Což aktivuje jQuery Unobtrusive Validation.

### ✅ 4.3 Vlastní validační atribut
**`MinimumBalanceAttribute`** (`Application/Validation/MinimumBalanceAttribute.cs`)

```csharp
public class MinimumBalanceAttribute : ValidationAttribute
{
    private readonly decimal _minimumBalance;
    
    public MinimumBalanceAttribute(double minimumBalance)
    {
        _minimumBalance = (decimal)minimumBalance;
        ErrorMessage = $"Balance musí být minimálně {_minimumBalance} Kč";
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Validační logika
    }
}
```

Použití:
```csharp
[MinimumBalance(0)]
public decimal Balance { get; set; }
```

---

## 5. Autentizace a autorizace

### ✅ 5.1 ASP.NET Core Identity
- Implementováno v `Program.cs`
- Používá `IdentityUser` a `IdentityRole`
- Uživatelé jsou uloženi v Identity tabulkách
- Hesla jsou hashována pomocí Identity + BCrypt pro Player entity

### ✅ 5.2 Role
Systém má **3 role**:

1. **Admin**
   - Plný přístup ke všem funkcím
   - Může spravovat uživatele, role, všechny entity
   - Přístup do Admin Area
   
2. **Manager**
   - Může spravovat Games, Bets, GameSessions
   - Nemůže měnit kritické věci (uživatele, role)
   - Omezený přístup do Admin Area
   
3. **Player**
   - Může hrát hry, vkládat kredit, zobrazit své sázky
   - Nemá přístup do Admin Area

### ✅ 5.3 Autorizační atributy
```csharp
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PlayersController : Controller { }

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class GamesController : Controller { }

[Authorize]
public IActionResult Profile() { }
```

### ✅ 5.4 Seed data
`Infrastructure/Data/SeedData.cs` vytváří:
- Role: Admin, Manager, Player
- Výchozí admin účet (username: `admin`, heslo: `Admin123`)
- Výchozí manager účet (username: `manager`, heslo: `Manager123`)

---

## 6. Responzivní design

### ✅ Bootstrap 5
- Všechny views používají Bootstrap 5
- Responzivní grid system (`row`, `col-md-*`)
- Responzivní komponenty (cards, buttons, forms)
- Mobilní menu v navigaci

Příklad:
```html
<div class="row">
    <div class="col-md-6">
        <!-- Content -->
    </div>
    <div class="col-md-6">
        <!-- Content -->
    </div>
</div>
```

---

## 7. Pokrytí tématu

### ✅ Funkcionalita Online Casina
Projekt pokrývá základní funkcionalitu online casina:

**Pro hráče:**
- Registrace a přihlášení
- Vklad kreditu (simulovaný)
- Hraní her (dice game)
- Zobrazení historie sázek
- Zobrazení profilu a statistik

**Pro admina:**
- Správa všech hráčů
- Správa her
- Sledování všech sázek
- Sledování transakcí
- Sledování herních relací

**Pro managera:**
- Správa her
- Sledování sázek
- Sledování herních relací

---

## 8. Struktura projektu

```
OnlineCasino/
├── Application/           # Application Layer
│   ├── DTOs/
│   │   ├── PlayerDto.cs
│   │   ├── GameDto.cs
│   │   ├── BetDto.cs
│   │   └── ...
│   ├── Interfaces/
│   │   ├── IPlayerService.cs
│   │   ├── IGameService.cs
│   │   └── ...
│   ├── Services/
│   │   ├── PlayerService.cs
│   │   ├── GameService.cs
│   │   └── ...
│   └── Validation/
│       └── MinimumBalanceAttribute.cs
│
├── Domain/                # Domain Layer
│   └── Entities/
│       ├── Player.cs
│       ├── Game.cs
│       ├── Bet.cs
│       ├── Transaction.cs
│       └── GameSession.cs
│
├── Infrastructure/        # Infrastructure Layer
│   └── Data/
│       ├── CasinoContext.cs
│       └── SeedData.cs
│
├── Areas/                 # Presentation Layer - Admin Area
│   └── Admin/
│       ├── Controllers/
│       └── Views/
│
├── Controllers/           # Presentation Layer - Main Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── BetsController.cs
│   ├── GamesController.cs
│   └── PlayersController.cs
│
├── Views/                 # Presentation Layer - Views
│   ├── Home/
│   ├── Account/
│   ├── Bets/
│   └── ...
│
├── Migrations/            # EF Core Migrations
└── Program.cs             # Application Entry Point
```

---

## 9. Jak spustit projekt

### 9.1 Předpoklady
- .NET 9.0 SDK
- SQL Server (LocalDB nebo jiná instance)

### 9.2 Kroky
1. Klonovat repozitář
2. Nastavit connection string v `appsettings.json`
3. Spustit migrace: `dotnet ef database update`
4. Spustit aplikaci: `dotnet run`
5. Přihlásit se jako admin (username: `admin`, heslo: `Admin123`)

### 9.3 Testovací účty
- **Admin**: username: `admin`, heslo: `Admin123`
- **Manager**: username: `manager`, heslo: `Manager123`
- **Player**: Registrovat nového uživatele

---

## 10. Závěr

Projekt splňuje všechny požadavky:

✅ ASP.NET Core MVC 9.0  
✅ Vícevrstvá architektura (Presentation, Application, Infrastructure, Domain)  
✅ Všechna funkcionalita přes Services  
✅ Oddělení vrstev (Presentation nepoužívá Infrastructure)  
✅ Code-First s migracemi  
✅ Minimálně 5 entit + ViewModels/DTOs  
✅ Cizí klíče mezi entitami  
✅ Admin Area s CRUD pro všechny entity  
✅ Editace položek  
✅ Serverová i klientská validace  
✅ Vlastní validační atribut (MinimumBalanceAttribute)  
✅ ASP.NET Core Identity s rolemi (Admin, Manager, Player)  
✅ Autorizační atributy  
✅ Bootstrap pro responzivní design  
✅ Dostatečné pokrytí tématu Online Casino  

---

## 11. Možná rozšíření

- Více typů her (blackjack, roulette, poker)
- Real-time notifikace (SignalR)
- Leaderboard (žebříček hráčů)
- Bonusy a promoce
- Historie výběrů a vkladů
- Email notifikace
- 2FA autentizace
- Chat podpora
