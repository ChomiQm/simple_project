# simple_project_task

## Opis

Projekt "simple_project_task" to aplikacja webowa służąca do zarządzania dokumentami. Umożliwia użytkownikom przeglądanie, dodawanie i zarządzanie dokumentami po zalogowaniu. Aplikacja została stworzona przy użyciu C# 12 i .NET 8.0, z wykorzystaniem Microsoft SQL Server oraz Identity Framework dla zarządzania uwierzytelnieniem i autoryzacją użytkowników.

## Technologie

- **C# 12**
- **.NET 8.0**
- **Microsoft SQL Server**
- **Identity Framework**
- **SSMS (SQL Server Management Studio)**
- **VueJS**
- **Javascript**
- **Vite**

## Wymagania

Użytkownicy muszą zarejestrować się i zalogować, aby przeglądać dokumenty. Aby dodać dokumenty, użytkownicy muszą uzupełnić swoje dane osobowe.

## Konfiguracja

### Backend

Zmodyfikuj plik `appsettings.json` według swojej konfiguracji bazy danych:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "conn_string_db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```


Frontend:  
Aby zainstalować niezbędne pakiety, wykonaj następujące polecenie w katalogu projektu frontend: npm install  
Backend:  
W większości przypadków pakiety NuGet powinny zainstalować się automatycznie, ale na wszelki: dotnet restore -> dotnet run  
