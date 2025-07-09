# Contributing to Zenix

Thank you for your interest in contributing to the Zenix MSX emulator project! We welcome contributions from developers interested in emulation, .NET, clean architecture, and retro computing.

## 🚀 Quick Start

1. **Fork the repository** and create a feature branch
2. **Set up the development environment:**
   ```bash
   git clone https://github.com/yourusername/zenix.git
   cd zenix
   dotnet restore
   dotnet build
   ```
3. **Run tests to verify setup:**
   ```bash
   dotnet test tests/Zenix.Tests.csproj
   ```
4. **Try the demos:**
   ```bash
   dotnet run --project src -- cycles
   dotnet run --project src -- interrupts
   dotnet run --project src -- config
   ```

## 📋 Development Guidelines

### Build Requirements

**⚠️ CRITICAL: The Zenix project enforces strict static code analysis and style rules. All warnings are treated as build errors.**

#### **Mandatory Rules (Build Fails if Violated)**

- **TreatWarningsAsErrors**: `true` - No warnings are allowed
- **EnforceCodeStyleInBuild**: `true` - Style violations cause build failures

#### **Key Enforced Rules (IDE Error Level)**

| Rule | Description | Example Fix |
|------|-------------|-------------|
| **IDE0130** | Namespace must match folder structure | `src/Core/Interrupt/` → `namespace Zenix.Core.Interrupt;` |
| **IDE0300** | Use collection expressions | `new byte[] { 1, 2 }` → `[1, 2]` |
| **IDE0161** | Use file-scoped namespaces | `namespace Foo { ... }` → `namespace Foo;` |
| **IDE0065** | Using directives outside namespace | Move `using` statements to top of file |
| **IDE0005** | Remove unnecessary usings | Delete unused `using` statements |
| **IDE0011** | Add braces to control statements | `if (x) return;` → `if (x) { return; }` |

#### **Configuration Files**

Two files enforce these rules:

1. **`Directory.Build.props`** (MSBuild level):
   ```xml
   <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
   <IDE0130>error</IDE0130>
   <IDE0300>error</IDE0300>
   <!-- ... other rules ... -->
   ```

2. **`.editorconfig`** (Editor level):
   ```ini
   [*.cs]
   dotnet_diagnostic.IDE0130.severity = error
   dotnet_diagnostic.IDE0300.severity = error
   csharp_prefer_braces = true:error
   ```

#### **Development Workflow**

1. **Before any commit**: Run `dotnet build` - it MUST pass with zero warnings
2. **All pull requests**: Automatically rejected if build fails with violations
3. **IDE setup**: Configure your editor to show these rules as errors
4. **Common fixes**: See [coding-guidelines.md](docs/coding-guidelines.md#how-to-fix-common-violations)

### Code Standards
- **Follow the [coding guidelines](docs/coding-guidelines.md)** - comprehensive standards for C#, naming, and architecture
- **IDE0130 compliance**: Namespaces must match folder structure exactly (enforced and verified)
- **Use immutable records** for configuration and data structures
- **Apply functional programming principles** where appropriate
- **One class per file** with file names matching class names
- **File-scoped namespaces** for all new code
- **Collection expressions**: Use `[...]` instead of `new Type[] { ... }` (IDE0300)
- **Comprehensive unit tests** for all new functionality

### Architecture Principles
- **Clean Architecture**: Separate concerns with clear boundaries
- **Dependency Injection**: Use constructor injection for dependencies
- **Domain-Driven Design**: Model real-world MSX concepts accurately
- **Test-Driven Development**: Write tests first when possible

### Namespace Structure
All code must follow the established namespace structure:
```
Zenix.Core                     # CPU and memory components
Zenix.Core.Interrupt          # Interrupt system
Zenix.App                     # Application layer
Zenix.App.Configuration       # Configuration system
Zenix.CLI                     # Command-line interface
Zenix.Infrastructure          # Infrastructure services
Zenix.Demos                   # Demonstration programs
Zenix.Tests.Core.Tests        # Unit tests
```

## 🧪 Testing

- **Run all tests** before submitting: `dotnet test`
- **Write unit tests** for new functionality
- **Follow the Arrange-Act-Assert pattern**
- **Use meaningful test names** that describe the scenario
- **Maintain test coverage** for critical components

### Build Configuration

The project uses several configuration files to enforce quality and consistency:

- **`Directory.Build.props`**: MSBuild properties shared across all projects
  - Enforces treat warnings as errors
  - Sets common package versions for test projects
  - Configures code analysis and documentation generation
  
- **`.editorconfig`**: Code style rules for VS Code and Visual Studio
  - Enforces consistent formatting
  - Configures naming conventions
  - Sets indentation and line ending preferences

## 📝 Documentation

- **Update relevant documentation** for any changes
- **Follow Markdown best practices**
- **Keep line widths under 100 characters**
- **Use American English spelling**
- **Include code examples** where helpful

## 🔄 Pull Request Process

1. **Create a feature branch** from `main`
2. **Make your changes** following the guidelines above
3. **Update documentation** and tests as needed
4. **Verify all tests pass** and code builds cleanly
5. **Submit a pull request** with:
   - Clear description of changes
   - Reference to any related issues
   - Screenshots/demos if applicable

## 💡 Contribution Ideas

### Core Emulation
- Z80 CPU instruction implementations (most core instructions complete)
- VDP (Video Display Processor) emulation
- PSG (Programmable Sound Generator) support  
- Memory bank switching and MSX slot system
- Cartridge and disk image support

### Infrastructure
- WebAssembly frontend (Blazor WASM)
- Performance optimizations and profiling
- Real-time debugging and tracing tools
- Save state and replay functionality
- Configuration improvements

### Documentation
- API documentation
- Emulation accuracy guides
- Performance analysis
- Historical MSX research

## 🤝 Code of Conduct

We follow the [Contributor Covenant](https://www.contributor-covenant.org/) Code of Conduct. Please be respectful and considerate in all interactions.

## 📞 Getting Help

- **Issues**: Report bugs or request features
- **Discussions**: Ask questions or share ideas
- **Documentation**: Check [docs/](docs/) for detailed guides

Thank you for helping make Zenix better! 🌟

