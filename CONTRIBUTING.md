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

### Code Standards
- **Follow the [coding guidelines](docs/coding-guidelines.md)** - comprehensive standards for C#, naming, and architecture
- **IDE0130 compliance**: Namespaces must match folder structure exactly (enforced and verified)
- **Use immutable records** for configuration and data structures
- **Apply functional programming principles** where appropriate
- **One class per file** with file names matching class names
- **File-scoped namespaces** for all new code
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

