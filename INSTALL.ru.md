# Установка GodhomeQoL

GodhomeQoL — мод для [Hollow Knight Modding API](https://github.com/hk-modding/api). Ему нужны три библиотечных мода: **Satchel**, **Osmi** и **Vasi**.

> **Про форк.** Lumafly (менеджер модов) ставит *оригинальный* GodhomeQoL из [NightFuryoOo/GodhomeQoL](https://github.com/NightFuryoOo/GodhomeQoL). Сборки из этого репозитория ставятся вручную, как описано ниже.

## 1. Сделайте бэкап сейвов (рекомендуется)

Сейвы и настройки всех модов лежат в одной папке:

```
%USERPROFILE%\AppData\LocalLow\Team Cherry\Hollow Knight
```

Команда PowerShell, которая копирует её на рабочий стол в папку с датой:

```powershell
Copy-Item "$env:USERPROFILE\AppData\LocalLow\Team Cherry\Hollow Knight" "$([Environment]::GetFolderPath('Desktop'))\HK_backup_$(Get-Date -Format yyyy-MM-dd_HH-mm)" -Recurse
```

## 2. Установите Modding API и зависимости

Проще всего так: установите [Lumafly](https://themulhima.github.io/Lumafly/), дайте ему поставить Modding API, затем установите из его списка модов **Satchel**, **Osmi** и **Vasi**. Проверьте, что все три *включены*: выключенные моды Lumafly переносит в `Mods\Disabled\`, и игра их не загружает.

## 3. Возьмите DLL мода

Любой из вариантов:

- **Релиз:** скачайте `GodhomeQoL.zip` со страницы *Releases* репозитория.
- **Последняя сборка любой ветки:** вкладка *Actions* → workflow *Build* → откройте нужный запуск → скачайте артефакт `GodhomeQoL` (нужно войти в GitHub).
- **Собрать самому:** см. [Сборка из исходников](#сборка-из-исходников).

## 4. Скопируйте её в игру

Положите `GodhomeQoL.dll` сюда (путь Steam по умолчанию):

```
C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\GodhomeQoL\GodhomeQoL.dll
```

```powershell
$mods = "C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
New-Item -ItemType Directory -Path "$mods\GodhomeQoL" -Force | Out-Null
Copy-Item ".\GodhomeQoL.dll" "$mods\GodhomeQoL\" -Force
```

Пока заменяете файл, игра должна быть закрыта. Если PowerShell пишет *Access denied*, запустите его от имени администратора.

## 5. Проверьте, что мод загрузился

- В списке модов в правом верхнем углу главного меню есть `GodhomeQoL`.
- В игре по **F3** открывается быстрое меню.
- Ошибки пишутся в `ModLog.txt` в папке из шага 1.

**По умолчанию всё выключено.** Пока вы не включите что-нибудь в меню F3, игра не меняется. В группах *Quality of Life*, *Boss Animation Skipping* и *Menu Animation Skipping* сначала включите саму группу (верхняя строка), потом нужные пункты.

При обновлении со старой версии все настройки GodhomeQoL один раз сбрасываются в «выключено» (в логе будет строка `Reset all modules to disabled (defaults migration)`).

## Удаление

Удалите папку `Managed\Mods\GodhomeQoL\` и файл `GodhomeQoL.GlobalSettings.json` из папки из шага 1.
Учтите: *Unlock All Modes* записывает открытие режимов Steel Soul / Godseeker в данные самой игры, и удаление мода это не откатывает.

## Сборка из исходников

Проект собирается под .NET Framework 4.7.2 и компилируется против DLL игры, поэтому ему нужна папка `Managed` с модами (Modding API + Satchel, Osmi, Vasi).

### Через Docker (на Windows не нужно ставить ничего, кроме Docker Desktop)

Запускайте из папки репозитория (той, где лежит `GodhomeQoL.csproj`). Папка игры подключается только для чтения; результат появится в `bin\Release\net472\GodhomeQoL.dll` и `out\export\GodhomeQoL\GodhomeQoL.zip`.

```powershell
docker run --rm `
  -v "${PWD}:/src" `
  -v "C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed:/hk:ro" `
  -w /src mcr.microsoft.com/dotnet/sdk:8.0 `
  dotnet build GodhomeQoL.csproj -c Release `
    -p:HollowKnightRefs=/hk/ `
    -p:ManagedModsDir=/src/out/managed/ `
    -p:ExportDir=/src/out/export
```

Если вы в Windows PowerShell 5.1 передаёте скрипт в `bash -c`, не используйте внутри него двойные кавычки: PowerShell их вырезает.

### С локальным .NET SDK

```powershell
dotnet build GodhomeQoL.csproj -c Release -p:HollowKnightRefs="<путь к hollow_knight_Data\Managed>\"
```

Без дополнительных параметров сборка ещё и копирует DLL в `Managed\Mods\GodhomeQoL\`, а экспорт пишет в `C:\Users\User\Documents\HKModsExport` (см. target `CopyMod` в `.csproj`).

### Через GitHub Actions

`.github/workflows/build.yml` собирает мод на каждый push и pull request; push тега вида `v1.0.1.5` ещё и публикует GitHub Release. DLL игры скачивает action [setup-hk](https://github.com/BadMagic100/setup-hk), а зависимости-моды берутся из `ModDependencies.txt`.
