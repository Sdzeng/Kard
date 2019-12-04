@set kardPublish="Kard.Publish" 
@set kardWeb="Kard.Web" 
@cd %kardWeb%
@echo %cd% 开始编译  
@dotnet build  --framework netcoreapp2.1 --runtime centos.7-x64  --configuration Release --version-suffix 1.0.0


@cd ../
@if not exist %kardPublish% ( md %kardPublish%) else (rd /s /Q %kardPublish% && md %kardPublish%)
@echo %cd% 开始发布  
@dotnet publish --framework netcoreapp2.1 --runtime centos.7-x64 --output %cd%/%kardPublish% --configuration Release --version-suffix 1.0.0 --self-contained false
@pause