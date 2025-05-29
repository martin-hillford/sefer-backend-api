dotnet clean
find . -type d -name "obj" -exec rm -rf {} + -o -name "bin" -exec rm -rf {} +
