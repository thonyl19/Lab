@echo 
setlocal enabledelayedexpansion

set "REPLACEMENT_FILE=keyword_replacements.txt"
set "TARGET_FILELIST=filelist.txt"
@echo 處理開始
REM === 處理所有被 staged 的檔案 ===
for /f "delims=" %%F in ('git diff --cached --name-only --diff-filter=ACM') do (
    set "FILE=%%F"

    if exist "!FILE!" (

        REM === 關鍵字置換，防止重複註解 ===
        for /f "tokens=1,2 delims==" %%A in (%REPLACEMENT_FILE%) do (
            set "FIND=%%A"
            set "REPLACE=%%B"

            powershell -Command ^
            "$content = Get-Content -Raw '!FILE!'; ^
            $pattern = '^\s*(?!\/\/)\s*\b{0}\b' -f [Regex]::Escape('!FIND!'); ^
            $updated = ($content -split \"`n\") -replace $pattern, '!REPLACE!'; ^
            [System.IO.File]::WriteAllLines('!FILE!', $updated)"
        )

        REM === 特定檔案處理（支援 *.js 等） ===
        for /f "delims=" %%P in (%TARGET_FILELIST%) do (
            for %%G in (%%P) do (
                for /f "delims=" %%W in ('dir /b /s %%G 2^>nul') do (
					@echo %TARGET_FILELIST%
                    REM 比對 full path
                    if /i "%%~fW"=="%CD%\!FILE!" (
                        findstr /C:"// AUTO-INJECTED FILE HEADER" "!FILE!" >nul
                        if errorlevel 1 (
                            powershell -Command ^
                            "$old = Get-Content '!FILE!'; ^
                             $new = '// AUTO-INJECTED FILE HEADER'; ^
                             $new, $old | Set-Content '!FILE!'"
                        )
                    )
                )
            )
        )

        REM === 重新加入 git stage ===
        git add "!FILE!"
    )
)

exit 0
