# ps4sdk ps4KernelDlSym Symbol Resolver

The ps4sdk ps4KernelDlSym Symbol Resolver is a windows forms application (requires .NET 4.0), which has been made to fit my needs for ps4 compiled payloads using [Hitodamas open source ps4 sdk](https://github.com/seb5594/ps4sdk) .
This tool has been made to generate easily an offsettable to all symbols, where are getting called. In the current state, it is impossible to resolve stacked function calls. If i feel to, i will update this repo....
You are permitted to use/edit this source code, as long you are referencing to this project.

# Requirements (for usage)
- .NET 4.0
- ps4sdk compiled payload(s)
- brain

# How does it work?
This application uses the [open source disassembler library Capstone.NET](https://github.com/9ee1/Capstone.NET), to analyze function calls from x86_64 assemblies compiled binarys (payload/s), for further development reasons.
It resolves all absolute calls to ps4KernelDlSym and it outputs a list of all symbol name(s), which are needed to execute the payload!
The resulting output will be displayed as a raw list (default) or in a c-styled array (my preferred way).

# ToDo (coming soon?)
- Resolve stacked symbol names (might be added, when i feel to!)
