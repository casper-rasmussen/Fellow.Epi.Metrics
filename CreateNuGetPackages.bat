SETLOCAL

set "module=Fellow.Epi.Metrics"
set output="../ModulePackages/%module%"

rmdir /s /q %output%
mkdir %output%

nuget pack "Fellow.Epi.Metrics\Fellow.Epi.Metrics.csproj" -Build -OutputDirectory %output% -Prop Configuration=Release;Platform=AnyCPU