<div align="center">

# PSP Library

**See what every PSP ISO is, before you launch it.**

A small Windows app that reads your PSP game folder and shows each game's cover, title, catalog ID, region and firmware, so you stop guessing from file names like `cvn-tokd.hako`.

[![Stars](https://img.shields.io/github/stars/dvgamerr-app/psp-library-win?style=for-the-badge&color=0a7ea4)](https://github.com/dvgamerr-app/psp-library-win/stargazers)
[![Release](https://img.shields.io/github/v/release/dvgamerr-app/psp-library-win?style=for-the-badge&color=0a7ea4)](https://github.com/dvgamerr-app/psp-library-win/releases/latest)
![Platform](https://img.shields.io/badge/platform-Windows-0a7ea4?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.0-0a7ea4?style=for-the-badge)

[Website](http://dvgamerr.app/psp-library-win/) · [Quick start](#quick-start) · [Changelog](docs/changelog.md) · [ภาษาไทย](README.th.md)

<img src="docs/screenshots/library.jpg" alt="PSP Library listing 27 games with catalog ID, region and firmware" width="560">

</div>

**If this saves you from opening a folder of mystery ISOs, please give it a ⭐. It helps other PSP collectors find it.**

## Why

- **Know every ISO at a glance.** Title, cover art, catalog ID, version, firmware, parental level and region are read straight from the game's own `PARAM.SFO`, `ICON0.PNG` and `PIC1.PNG`.
- **Your ISOs stay untouched.** Games are opened read-only; extracted metadata is cached in a `PSPStore` folder next to the app.
- **No install.** Download a zip, extract, run `PSPlibrary.exe`.
- **Totals for the whole collection.** Game count, total size and an English / Japan / Other region split in the status bar.

## Features

| | Feature | What it does |
|---|---|---|
| 📚 | **Library list** | Catalog ID, file name, region and firmware for every ISO in your folder, sortable. |
| 🖼️ | **Cover and background** | Shows the game's icon and `PIC1` background pulled from inside the ISO. |
| 🔎 | **Game details** | Title, version, firmware, parental level, type (UMD / CD-DVD / Blu-ray), size and created date. |
| 🪟 | **Quick viewer** | Open a single `.iso` with PSP Library to see its details window without scanning a folder. |
| 📊 | **Collection summary** | Total games, total size and region counts. |
| 💾 | **Remembers your layout** | Window position, size and column widths are saved between runs. |

## Quick start

1. Download `psp-library-v1.1-release-x64.zip` (or `x86`) from the [latest release](https://github.com/dvgamerr-app/psp-library-win/releases/latest).
2. Extract it anywhere and run `PSPlibrary.exe`. It needs Windows with .NET Framework 4.0 or newer.
3. On first start, choose the folder that holds your PSP games.
4. Wait for the scan. Pick a game in the list to see its details.

To check a single game, run `PSPlibrary.exe "C:\path\to\game.iso"` or use **Open with** on an `.iso` file.

> [!WARNING]
> **The scan reorganises your game folder.** Any `.iso`, `.cso` or `.pbp` file sitting directly in the folder you choose is moved into a sub-folder named after the file. Point the app at a folder you are happy to have tidied, or back it up first. This behaviour is tracked in [docs/issue.md](docs/issue.md).

## Current limits

- Details are read from `.iso` files only. `.cso` and `.pbp` files are not parsed yet.
- Editor Mode (editing description, genre and so on) is not enabled in the current build.
- Windows only.

## Screenshots

<details>
<summary>More</summary>

<img src="docs/screenshots/viewer.jpg" alt="Single game viewer showing Hot Shots Golf: Open Tee" width="560">

<img src="docs/screenshots/problem.jpg" alt="A folder of ISOs with unhelpful names" width="560">

</details>

## Build from source

1. Open `PSPlibrary.sln` in Visual Studio.
2. Restore NuGet packages (`DiscUtils` 0.11.0.2).
3. Build **Release** for **x64** or **x86**.

Stack: C#, WPF, .NET Framework 4.0, [DiscUtils](https://github.com/DiscUtils/DiscUtils) for reading ISO 9660.

## Documentation

- [Changelog](docs/changelog.md)
- [Known issues from the code review](docs/issue.md)

## Support the project

- ⭐ [Star the repo](https://github.com/dvgamerr-app/psp-library-win)
- 🐛 [Report a bug or request a feature](https://github.com/dvgamerr-app/psp-library-win/issues)

PSP Library is a fan-made tool and is not affiliated with Sony Interactive Entertainment. It does not include or download any game.
