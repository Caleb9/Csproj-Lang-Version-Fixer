# What

Simple utility for batch processing of legacy style of C# *.csproj
files. Makes it possible to set C# language version (`LangVersion`
property) and target framework version (`TargetFrameworkVersion`
property).

Execute with `--help` switch to see available options.

# Why

Because I needed to set `LangVersion` and `TargetFrameworkVersion`
conveniently for all projects in a large solution.

# Usage example

### Display help

```
$ ./CsprojLangVersionFixer/bin/Debug/CsprojFixer.exe --help
```

### Using GNU find to generate paths to all .csproj files in a directory:

```
$ find path_to_project -iname '*.csproj' \
  -exec CsprojLangVersionFixer/bin/Debug/CsprojFixer.exe -l latest -f v4.7.2 {} \+
```

# TODO

* Right now it allows setting the properties to unconstrained string
  values. This should be limited to only allow valid values.
* Set up AppVeyor to make compiled version available.
* More thorough testing.
* General documentation and cleanup.
* .NET Core .csproj format support
