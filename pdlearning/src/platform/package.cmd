@ECHO OFF
SET RepoRoot=%~dp0..\..\
%RepoRoot%build.cmd -projects %RepoRoot%src\platform\Platform\**\*.csproj %*
