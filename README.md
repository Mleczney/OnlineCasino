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

Projekt implementuje **čtyřvrstvou architekturu**:

- **Presentation Layer** - Controllers, Views, Areas
- **Application Layer** - Services, DTOs, Interfaces, Validation
- **Infrastructure Layer** - EF Core, DbContext, Repositories
- **Domain Layer** - Entity models

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
- SQL Server

### Kroky
```bash
# 1. Klonovat repozitář
git clone https://github.com/Mleczney/OnlineCasino.git

# 2. Přejít do složky projektu
cd OnlineCasino/OnlineCasino

# 3. Obnovit balíčky
dotnet restore

# 4. Aktualizovat databázi
dotnet ef database update

# 5. Spustit aplikaci
dotnet run
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
