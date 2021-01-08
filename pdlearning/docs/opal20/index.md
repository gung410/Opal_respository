---
uid: Home
---

# OPAL2.0 Documentation

## Quick Start Notes

1. First, ensure that you are using an administrative shell - you can also install as a non-admin by checking out Non-Administrative Installation.

2. Install the chocolatey.

    ```PowerShell
    @powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin
    ```

3. Install docfx.

    ```PowerShell
    choco install docfx -y
    ```

4. docfx -s (Open terminal at the folder include docfx.json file.)
