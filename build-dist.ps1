# Compila CotasParedes para Revit 2024 (net48), 2025 e 2026 (net8.0-windows) e monta dist\{ano}\
$root = $PSScriptRoot
$proj = Join-Path $root 'CotasParedes'
$dist = Join-Path $root 'dist'
if (Test-Path $dist) { Remove-Item $dist -Recurse -Force }

foreach ($year in 2024, 2025, 2026) {
    Remove-Item (Join-Path $proj 'bin'), (Join-Path $proj 'obj') -Recurse -Force -ErrorAction SilentlyContinue
    dotnet build (Join-Path $proj 'CotasParedes.csproj') -c Release -p:RevitYear=$year
    if ($LASTEXITCODE -ne 0) { throw "Falha ao compilar Revit $year" }

    $out = Get-ChildItem (Join-Path $proj 'bin\Release') -Directory | Select-Object -First 1
    $target = Join-Path $dist "$year\CotasParedes"
    New-Item -ItemType Directory $target -Force | Out-Null
    Copy-Item (Join-Path $proj 'CotasParedes.addin') (Join-Path $dist $year)
    foreach ($f in 'CotasParedes.dll', 'CotasParedes.pdb', 'BimBlasterStyles.xaml', 'icon16.png', 'icon32.png') {
        Copy-Item (Join-Path $out.FullName $f) $target
    }
}
Remove-Item (Join-Path $proj 'bin'), (Join-Path $proj 'obj') -Recurse -Force -ErrorAction SilentlyContinue
