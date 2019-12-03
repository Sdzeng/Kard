@echo 开始编译  
@dotnet build  --framework netcoreapp2.1 --runtime centos.7-x64  --configuration Release --version-suffix 1.0.0
@cd ../
@set kardPublish="Kard.Publish" 
@if not exist %kardPublish% ( md %kardPublish%) else (rd /s /Q %kardPublish% && md %kardPublish%)
@echo 开始发布  
@dotnet publish --framework netcoreapp2.1 --runtime centos.7-x64 --output %kardPublish% --configuration Release --version-suffix 1.0.0 --self-contained false
@pause