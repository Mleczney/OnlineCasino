# 🎰 Online Casino

Webová aplikace pro online casino vytvořená v ASP.NET Core MVC 9.0 s vícevrstvou architekturou.

## 🎯 Funkce

- 🎰 Hraní kasinových her (dice game)
- 👤 Registrace a přihlášení uživatelů
- 💰 Správa kreditu (vklady, výběry)
- 📊 Sledování statistik a historie sázek
- 🔐 Role-based přístup (Admin, Manager, Player)
- ⚙️ Admin panel pro správu systému

## 🏗️ Architektura

Projekt implementuje **čtyřvrstvou architekturu** jako **samostatné Class Library projekty**:

```
📦 OnlineCasino.sln
├── 🎯 OnlineCasino (Web Application)
│   └── Presentation Layer - Controllers, Views, Areas
├── 📚 OnlineCasino.Application (Class Library)
│   └── Application Layer - DTOs, Interfaces, Validation
├── 🔧 OnlineCasino.Infrastructure (Class Library)
│   └── Infrastructure Layer - EF Core, DbContext, Services
└── 📋 OnlineCasino.Domain (Class Library)
    └── Domain Layer - Entity models
```

### Závislosti mezi projekty:
```
OnlineCasino (Web) 
    ↓ references
    ├─→ OnlineCasino.Infrastructure
    ├─→ OnlineCasino.Application
    └─→ OnlineCasino.Domain

OnlineCasino.Infrastructure
    ↓ references
    ├─→ OnlineCasino.Application
    └─→ OnlineCasino.Domain

OnlineCasino.Application
    ↓ references
    └─→ OnlineCasino.Domain

OnlineCasino.Domain
    └─→ (žádné závislosti)
```

## 🚀 Technologie

- ASP.NET Core MVC 9.0
- Entity Framework Core 9.0
- SQL Server
- ASP.NET Core Identity
- Bootstrap 5
- BCrypt.Net pro hashování hesel

## 📦 Entity

1. **Player** - Hráči systému
2. **Game** - Dostupné hry
3. **Bet** - Sázky hráčů
4. **Transaction** - Transakce (vklady, výběry)
5. **GameSession** - Herní relace

## 🔒 Role

- **Admin** - Plný přístup ke všem funkcím
- **Manager** - Správa her, sázek, relací
- **Player** - Hraní her, správa vlastního účtu

## 🏃 Spuštění projektu

### Předpoklady
- .NET 9.0 SDK
- SQL Server (LocalDB je součástí Visual Studio nebo SQL Server Express)

### Kroky
```bash
# 1. Klonovat repozitář
git clone https://github.com/Mleczney/OnlineCasino.git

# 2. Přejít do složky projektu
cd OnlineCasino

# 3. Obnovit balíčky pro celé solution (všechny 4 projekty)
dotnet restore

# 4. Spustit aplikaci (z web projektu)
cd OnlineCasino
dotnet run
# POZNÁMKA: Databáze a migrace se vytvoří automaticky při prvním spuštění
```

**Důležité:** Databáze a všechny tabulky se nyní vytváří **automaticky** při prvním spuštění aplikace. Není potřeba ručně spouštět `dotnet ef database update`.

### 🔧 Řešení problémů

#### Chyba: "Invalid object name 'AspNetUsers'" nebo "There is already an object named 'AspNetRoles' in the database"

Tyto chyby mohou nastat při problémech s databází:

**Řešení 1:** Smazat existující databázi a nechat aplikaci vytvořit novou
```bash
cd OnlineCasino
dotnet ef database drop --force
dotnet run
# Databáze se automaticky vytvoří při spuštění
```

**Řešení 2:** Restartovat aplikaci
Pokud aplikace běží, zkuste ji zastavit a spustit znovu. Migrace se aplikují automaticky při startu.

#### Resetování databáze

Pro kompletní reset databáze a dat:

```bash
# 1. Smazat databázi
cd OnlineCasino
dotnet ef database drop --force

# 2. Spustit aplikaci
dotnet run
# Databáze, migrace a testovací účty se vytvoří automaticky
```

## 👤 Testovací účty

Po spuštění aplikace můžete použít:

- **Admin**: username: `admin`, heslo: `Admin123`
- **Manager**: username: `manager`, heslo: `Manager123`
- **Player**: Vytvořte si vlastní účet registrací

## 📚 Dokumentace

Kompletní dokumentaci projektu najdete v souboru [PROJEKT_DOKUMENTACE.md](PROJEKT_DOKUMENTACE.md)

## 📝 Licence

MIT License

## 👨‍💻 Autor

Mleczney
